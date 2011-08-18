// This is the main DLL file.

#include "stdafx.h"

#include "SkylineSolver.h"

namespace Canguro {
	namespace Analysis {
		namespace Solver {
			
SkylineSolver::SkylineSolver(array<int>^ profile)
{
	matrix = new SkylineMatrix(profile);
}

SkylineSolver::!SkylineSolver()
{
	delete matrix;
}

double SkylineSolver::getItem(int nEq1, int nEq2)
{
	return matrix->getItem(nEq1, nEq2);
}

void SkylineSolver::setItem(int nEq1, int nEq2, double value)
{
	matrix->setItem(nEq1, nEq2, value);
}

bool SkylineSolver::solve()
{
	return matrix->solve();
}

bool SkylineSolver::Calc(DenseMatrix^ solution, DenseMatrix^ rhs)
{
	return matrix->calc(solution, rhs);
}

/*
Realiza la operación DenseMatrix * SkylineMatrix
*/ 
DenseMatrix^ SkylineSolver::operator *(DenseMatrix^ m)
{
	return (*matrix) * m;
}

		}
	}
}