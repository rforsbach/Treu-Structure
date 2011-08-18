#include "stdafx.h"
#include "SkylineMatrix.h"

SkylineMatrix::SkylineMatrix(array<int>^ profile)
{
	int i;
	solved = false;

	int size = profile->Length;
	nEqns = size;
		
	// Crear estructura de datos
	diagonal = new double[nEqns+1];
	ZeroMemory(diagonal, (nEqns+1) * sizeof(double));

	// Copiar el profile
	xenv = new int[nEqns+2];

	xenv[0] = 1;
	xenv[1] = 1;
	for (i=2; i<=(nEqns+1);i++)
	{
		xenv[i] = xenv[i-1] + profile[i-2];
	}
		
	//env = new BufferedArray();
	env.init(xenv[nEqns+1]);
}

bool SkylineMatrix::calc(DenseMatrix^ solution, DenseMatrix^ rhs)
{
	int i;
	double *row;

	if (!solved) return false;

	solution->setSize(rhs->getRows(), rhs->getCols());

	for (i=0; i<rhs->getRows(); i++)
	{
		row = solution->getRowPtr(i);
		CopyMemory( row, rhs->getRowPtr(i), rhs->getCols()*sizeof(double) );
		elslv( nEqns, xenv, diagonal, row - 1);
		euslv( row - 1 );
	}
	
	return true;
}

/*
Realiza la operación DenseMatrix * SkylineMatrix
*/ 
DenseMatrix^ SkylineMatrix::operator *(DenseMatrix^ m)
{
	DenseMatrix^ res = gcnew DenseMatrix();

	int i, j, k, iband, col;
	double *rowres, *rowm;

	res->setSize(m->getRows(), nEqns);

	for (k=0; k<m->getRows(); k++)
	{
		rowres = res->getRowPtr(k) - 1;
		rowm   = m->getRowPtr(k) - 1;

		for (i=1; i<=nEqns; i++)
		{
			iband = xenv[i+1] - xenv[i];

			if (iband >= i) 
				iband = i - 1;

			// Sumar diagonal
			rowres[i] += diagonal[i] * rowm[i];

			// Sumar lo demás
			for (j=xenv[i], col=i-iband; j<xenv[i+1]; j++, col++)
			{
				if (!solved)	// Porque ya no sería simétrica (el otro lado son 0's)
					rowres[i]   += env[j] * rowm[col];
				rowres[col] += env[j] * rowm[i];
			}
		}
	}

	return res;
}

System::String^ SkylineMatrix::ToString(bool x)
{
	int i, j;
	System::Text::StringBuilder ^ sb = gcnew System::Text::StringBuilder();

	char buf[20];

	sb->Append("[\n\r");
	for (i=0; i<nEqns; i++)
	{
		for (j=0; j<nEqns; j++)
			if (getItem(i, j)) 
			{
				if (x)
					sb->Append("X");
				else
				{
					sb->Append(System::String::Format("{0}", getItem(i, j)));
					sb->Append("\t");
				}
			}
			else 
			{
				if (x)
					sb->Append(" . ");
				else
					sb->Append("0\t");
			}
		
		if (x)
			sb->Append("\n\r");
		else
			sb->Append(";\n\r");
	}
	sb->Append("]");

	return sb->ToString();
}

#pragma unmanaged
SkylineMatrix::~SkylineMatrix()
{
	delete[] diagonal;
	delete[] xenv;
	//delete env;
}

double SkylineMatrix::getItem(int nEq1, int nEq2)
{
	nEq1++; nEq2++;
	if (nEq1 == nEq2)
		return diagonal[nEq1];
	else
		if (nEq1 > nEq2)
		{
			if ((nEq1-nEq2) > (xenv[nEq1+1] - xenv[nEq1])) return 0;
			return env[xenv[nEq1+1] - (nEq1 - nEq2)];
		}
		else
		{
			if ((nEq2-nEq1) > (xenv[nEq2+1] - xenv[nEq2])) return 0;
			return env[xenv[nEq2+1] - (nEq2 - nEq1)];
		}
}

void SkylineMatrix::setItem(int nEq1, int nEq2, double value)
{
	solved = false;
	nEq1++; nEq2++;
	if (nEq1 == nEq2)
		diagonal[nEq1] = value;
	else
	{
		if (nEq1 < nEq2)
		{
			int tmp = nEq1;
			nEq1 = nEq2;
			nEq2 = tmp;
		}
		if ((nEq1-nEq2) > (xenv[nEq1+1] - xenv[nEq1])) return;
		env[xenv[nEq1+1] - (nEq1 - nEq2)] = value;
		//env[xenv[nEq1+1] - (nEq1 - nEq2)] = env[xenv[nEq1+1] - (nEq1 - nEq2)] + value;
	}
}

bool SkylineMatrix::solve()
{
	int i, j, ixenv, iband, ifirst, jstop;
	double temp, s;
	
	if (diagonal[1] <= 0) return false;
	diagonal[1] = sqrt(diagonal[1]);

	for (i=2; i<=nEqns; i++)
	{
		ixenv = xenv[i];
		iband = xenv[i+1] - ixenv;
		temp = diagonal[i];

		if (iband)
		{
			ifirst = i-iband;
			// *******************************************************************
			//elslv (iband, &xenv[ifirst-1], &diagonal[ifirst-1], &env[ixenv-1]);
			// *******************************************************************
			elslv (iband, &xenv[ifirst-1], &diagonal[ifirst-1], BufferedArrayOffset(&env, ixenv-1));
			jstop = xenv[i+1]-1;

			for (j=ixenv; j<=jstop; j++)
			{
				s = env[j];
				temp -= s*s;
			}
		}

		if (temp <= 0) 
			return false;
		diagonal[i] = sqrt(temp);
	}

	solved = true;
	env.setReadOnly();
	return true;
}

void SkylineMatrix::elslv(int neqns, int *xenv, double *diag, double *rhs)
{
	int i, k, kstrt, kstop, iband, l, ifirst = 0, last = 0;
	double s;

	while (!rhs[++ifirst]) {
		if (ifirst >= neqns)
			return;
	}

	for (i=ifirst; i<=neqns; i++)
	{
		iband = xenv[i+1] - xenv[i];
		if (iband >= i) iband = i-1;

		s = rhs[i];
		l = i - iband;
		rhs[i] = 0.0;

		if (iband && last >= l)
		{
			kstrt = xenv[i+1] - iband;
			kstop = xenv[i+1] - 1;
			for (k=kstrt; k<=kstop; k++)
				s -= env[k]*rhs[l++];
		}

		if (s)
		{
			rhs[i] = s / diag[i];
			last = i;
		}
	}
}

void SkylineMatrix::euslv(double *rhs)
{
	int i, k, iband, kstrt, kstop, L;
	double s;

	i = nEqns+1;
	
	while (--i>0)
	{
		if (rhs[i])
		{		
			s = rhs[i] / diagonal[i];
			rhs[i] = s;

			iband = xenv[i+1] - xenv[i];
			if (iband >= i) 
				iband = i - 1;
			if (iband)
			{
				kstrt = i - iband;
				kstop = i - 1;
				L = xenv[i+1] - iband;

				for (k=kstrt; k<=kstop; k++)
					rhs[k] -= s * env[L++];
			}
		}
	}
}

void SkylineMatrix::elslv(int neqns, int *xenv, double *diag, BufferedArrayOffset &rhs)
{
	int i, k, kstrt, kstop, iband, l, ifirst = 0, last = 0;
	double s, tmp, tmp2;

	while (!rhs[++ifirst]) {
		if (ifirst >= neqns)
			return;
	}

	for (i=ifirst; i<=neqns; i++)
	{
		iband = xenv[i+1] - xenv[i];
		if (iband >= i) iband = i-1;

		s = rhs[i];
		l = i - iband;
		rhs[i] = 0.0;

		if (iband && last >= l)
		{
			kstrt = xenv[i+1] - iband;
			kstop = xenv[i+1] - 1;
			for (k=kstrt; k<=kstop; k++)
				s -= (tmp = env[k]) * (tmp2 = rhs[l++]);
		}

		if (s)
		{
			rhs[i] = s / diag[i];
			last = i;
		}
	}
}
