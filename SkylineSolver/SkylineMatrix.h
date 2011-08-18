
#pragma once

#include "DenseMatrix.h"
#include "BufferedArray.h"
#include "BufferedArrayOffset.h"

#pragma managed

using namespace Canguro::Analysis::Solver;

class SkylineMatrix
{
public:
	DenseMatrix^ operator* (DenseMatrix^ m);
	bool calc (DenseMatrix^ solution, DenseMatrix^ rhs);
	System::String^ ToString(bool x = false);
	void eliminateEq(int nEq);
	bool solve();
	void setItem (int nEq1, int nEq2, double value);
	double getItem (int nEq1, int nEq2);
	int getNEquations() { return nEqns; }
	SkylineMatrix(array<int>^ profile);
	~SkylineMatrix();

private:
	void elslv(int neqns, int *xenv, double *diag, BufferedArrayOffset &rhs);
	bool solved;
	//double& item (int nEq1, int nEq2);
	void euslv (double *rhs);
	int *xenv;
	void elslv (int neqns, int *xenv, double *diag, double *rhs);
	double *diagonal;
	BufferedArray env;
	int nEqns;
};
