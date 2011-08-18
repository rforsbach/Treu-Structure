// SparseMatrix.h: interface for the SparseMatrix class.
//
//////////////////////////////////////////////////////////////////////

#pragma once

namespace Canguro {
	namespace Analysis {
		namespace Solver {

			public ref class DenseMatrix
			{
			public:
				void cutRows (int newRows);
				void Jacobi (DenseMatrix^ w2, DenseMatrix^ V) { Jacobi(w2, V, 0.00000001, 100); }
				void Jacobi (DenseMatrix^ w2, DenseMatrix^ V, double tolerance, int maxit);
				double* getRowPtr (int row);
				void setRowCol (int rowcol) { setRowCol(rowcol, 0.0, 1.0); }
				void setRowCol (int rowcol, double val) { setRowCol(rowcol, val, 1.0); }
				void setRowCol (int rowcol, double val, double diag);
				DenseMatrix^ operator =(DenseMatrix^ msrc);
				void setSize (int rows, int cols);
				DenseMatrix();
				virtual System::String^ ToString() override;
				DenseMatrix (DenseMatrix^ msrc);
				DenseMatrix^ operator* (DenseMatrix^ m2);
				void operator+=(DenseMatrix^ m2);
				void setTranspose(bool trans);
				int getRows();
				int getCols();
				double& item (int row, int col);
				DenseMatrix(int rows, int cols);
				~DenseMatrix() { this->!DenseMatrix(); }
				!DenseMatrix();

				property double default[int, int]
				{
					double get (int row, int col)
					{
						return item(row, col);
					}

					void set (int row, int col, double value)
					{
						item(row, col) = value;
					}
				}

				double DotProductRow(int rowLeft, DenseMatrix^ matRight, int rowRight);
				double DotProductCol(int colLeft, DenseMatrix^ matRight, int colRight);

			protected:
				void init();
				bool transpose;
				int rows, cols;
				double *m;
			};

		}
	}
}