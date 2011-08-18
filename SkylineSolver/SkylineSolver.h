// SkylineSolver.h

#pragma once

#include "SkylineMatrix.h"

namespace Canguro {
	namespace Analysis {
		namespace Solver {

			public ref class SkylineSolver
			{
			public:
				DenseMatrix^ operator* (DenseMatrix^ m);
				bool Calc (DenseMatrix^ solution, DenseMatrix^ rhs);
				bool solve();
				void setItem (int nEq1, int nEq2, double value);
				double getItem (int nEq1, int nEq2);
				SkylineSolver(array<int>^ profile);
				~SkylineSolver() { this->!SkylineSolver(); }
				!SkylineSolver();
				
				virtual System::String^ ToString() override
				{
					return matrix->ToString();
				}

				property double default[int, int] 
				{
					double get(int row, int col)
					{
						return getItem(row, col); 
					}
					void set (int row, int col, double value)
					{
						setItem(row, col, value);
					}
				}

				property int NEquations
				{
					int get ()
					{
						return matrix->getNEquations();
					}
				}

			private:
				SkylineMatrix* matrix;
			};
		
		}
	}
}
