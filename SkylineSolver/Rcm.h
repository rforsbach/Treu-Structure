#pragma once

using namespace System::Collections::Generic;

namespace Canguro {
	namespace Analysis {
		namespace Solver {

			public ref class Rcm
			{
			public:	
				static array<int>^ DoRcm(array<LinkedList<int>^, 1>^ adjList);
			};

		}
	}
}