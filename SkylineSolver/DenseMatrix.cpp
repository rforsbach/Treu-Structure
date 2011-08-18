// SparseMatrix.cpp: implementation of the SparseMatrix class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "DenseMatrix.h"

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

namespace Canguro {
	namespace Analysis {
		namespace Solver {

DenseMatrix::DenseMatrix(int rows, int cols)
{
	init();
	setSize(rows, cols);
}

DenseMatrix::!DenseMatrix()
{
	delete[] m;
}

double& DenseMatrix::item (int row, int col)
{
	if (transpose)
		return m[col*cols+row];
	else
		return m[row*cols+col];
}

int DenseMatrix::getCols()
{
	if (transpose)
		return rows;
	else
		return cols;
}

int DenseMatrix::getRows()
{
	if (transpose)
		return cols;
	else
		return rows;
}

void DenseMatrix::setTranspose(bool trans)
{
	transpose = trans;
}

DenseMatrix^ DenseMatrix::operator *(DenseMatrix^ m2)
{
	DenseMatrix^ res = gcnew DenseMatrix();

	if (getCols() == m2->getRows())
	{
		res->setSize(getRows(), m2->getCols());

		int i, j, k;
		double sum;

		for (i=0; i<res->rows; i++)
		for (j=0; j<res->cols; j++)
		{
			for (k=0, sum=0; k<getCols(); k++)
				sum += item(i, k) * m2->item(k, j);
			res->item(i, j) = sum;
		}
	}

	return res;
}

DenseMatrix::DenseMatrix(DenseMatrix^ msrc)
{
	cols = msrc->cols;
	rows = msrc->rows;
	transpose = msrc->transpose;

	int size = rows*cols;
	m = new double[size];
	CopyMemory(m, msrc->m, size * sizeof(double));
}

DenseMatrix::DenseMatrix()
{
	init();
}

System::String^ DenseMatrix::ToString()
{
	int i, j, k;
	System::Text::StringBuilder^ sb = gcnew System::Text::StringBuilder();
	char buf[50];

	for (i=0, k=0; i<rows; i++)
	{
		for (j=0; j<cols; j++, k++)
		{
			sb->Append(m[k].ToString());
			sb->Append("\t");
		}
		sb->Append("\n\r");
	}

	return sb->ToString();
}

void DenseMatrix::setSize(int rows, int cols)
{
	if ((this->rows != rows) || (this->cols != cols))
	{
		this->rows = rows;
		this->cols = cols;
		if (m) delete[] m;
		m = new double[rows*cols];
	}
	ZeroMemory(m, rows*cols*sizeof(double));
}

DenseMatrix^ DenseMatrix::operator =(DenseMatrix^ msrc)
{
	int oldSize = cols*rows;
	cols = msrc->cols;
	rows = msrc->rows;
	transpose = msrc->transpose;

	int size = rows*cols;
	if (size != oldSize)
	{
		if (m) delete[] m;
		m = new double[size];
	}
	CopyMemory(m, msrc->m, size * sizeof(double));

	return this;
}

void DenseMatrix::init()
{
	cols = 0;
	rows = 0;
	transpose = false;
	m = NULL;
}

void DenseMatrix::setRowCol(int rowcol, double val, double diag)
{
	int i;

	if (cols == rows)
	{
		for (i=0; i<cols; i++)
			if (i != rowcol)
				item(i, rowcol) = item(rowcol, i) = val;
			else
				item(i, i) = diag;
	}
}

double* DenseMatrix::getRowPtr(int row)
{
	if (transpose)
		return NULL;

	return &m[row*cols];
}

void DenseMatrix::operator+=(DenseMatrix^ m2)
{
	if ((getCols() == m2->getCols()) && (getRows() == m2->getRows()))
	{
		for (int i=0; i<rows*cols; i++)
			m[i] += m2->m[i];
	}
}


void DenseMatrix::Jacobi(DenseMatrix ^w2, DenseMatrix ^V, double tolerance, int maxit)
{
	 if (rows == cols)	// debe ser una matriz cuadrada
	 {
		 int neq = rows;
		 int i, j, k;
		 double sum = 0.0, ssum, amax, aa, si, co, tol, tt;
		 V->setSize(rows, rows);

		 tol = fabs(tolerance);

		 for (i=0; i<neq; i++)
		 {
			 for (j=0; j<neq; j++)
			 {
				 if (tolerance > 0.0)
					 V->item(i, j) = 0.0;
				 sum += fabs(item(i, j));
			 }
			 if (tolerance > 0.0)
				 V->item(i, i) = 1.0;
		 }

		 if (neq == 0) return;
		 if (sum <= 0.0) return;
		 sum /= (double)(neq*neq);

		 do 
		 {
			 ssum = 0.0;
			 amax = 0.0;

			 for (j=1; j<neq; j++)
			 for (i=0; i<j; i++)
			 {
				 aa = fabs(item(i, j));
				 if (aa > amax) amax = aa;
				 ssum += aa;
				 if (aa>=0.1*amax)
				 {
					 aa = atan2(2.0*item(i, j), item(i, i)-item(j, j)) / 2.0;
					 si = sin(aa);
					 co = cos(aa);

					 for (k=0; k<neq; k++)
					 {
						 tt = item(k, i);
						 item(k, i) = co*tt + si*item(k, j);
						 item(k, j) = -si*tt + co*item(k, j);
						 tt = V->item(k, i);
						 V->item(k, i) = co*tt + si*V->item(k, j);
						 V->item(k, j) = -si*tt + co*V->item(k, j);
					 }

					 item(i, i) = co*item(i, i) + si*item(j, i);
					 item(j, j) = -si*item(i, j) + co*item(j, j);
					 item(i, j) = 0.0;

					 for (k=0; k<neq; k++)
					 {
						 item(i, k) = item(k, i);
						 item(j, k) = item(k, j);
					 }
				 } // If
			 }	// For j, i
		 } while (((fabs(ssum)/sum) > tol) && --maxit);

		 // Copiar eigenvalores a w2
		 w2->setSize(1, neq);
		 for (i=0; i<neq; i++)
			 w2->item(0, i) = item(i, i);
	 }	// If
}


void DenseMatrix::cutRows(int newRows)
{
	rows = (newRows<rows)?newRows:rows;
}

double DenseMatrix::DotProductRow(int rowLeft, DenseMatrix^ matRight, int rowRight)
{
	double dot = 0.0;
	if (matRight->getCols() != getCols())
		throw gcnew System::InvalidOperationException("Cannot apply a dot product to vectors of different size");

	for (int i=0; i<getCols(); i++)
		dot += this[rowLeft, i] * matRight[rowRight, i];

	return dot;
}

double DenseMatrix::DotProductCol(int colLeft, DenseMatrix^ matRight, int colRight)
{
	double dot = 0.0;
	if (matRight->getRows() != getRows())
		throw gcnew System::InvalidOperationException("Cannot apply a dot product to vectors of different size");

	for (int i=0; i<getRows(); i++)
		dot += this[i, colLeft] * matRight[i, colRight];

	return dot;
}


		}
	}
}