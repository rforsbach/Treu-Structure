#include "StdAfx.h"
#include "Rcm.h"

namespace Canguro {
	namespace Analysis {
		namespace Solver {

array<int>^ Rcm::DoRcm(cli::array<System::Collections::Generic::LinkedList<int>^, 1> ^adjList)
{
	int size = adjList->Length;
	
	// Colors: WHITE - 0, GREY - 1, BLACK - 2	
	char *color = new char[size];
	ZeroMemory(color, size * sizeof(char));
	int realSize = -1;

	Queue<int>^ queue = gcnew Queue<int>();	
	List<int>^ nums = gcnew List<int>();
	nums->Clear();

	int u;

	int first = 0;
	color[first] = 1;
	queue->Enqueue(first);

	while (queue->Count != 0)
	{
		u = queue->Dequeue();
		LinkedList<int>^ etmp = adjList[u];
		
		for each(int idtmp in etmp)
		{
			// Recorrer vecinos que no hayan sido recorridos (color=0)
			if (!color[idtmp])
			{
				color[idtmp] = 1;
				queue->Enqueue(idtmp);
			}
		}
		color[u] = 2;
		nums->Add(u);
	}
	
	delete[] color;

	// Get renumbering
	int numsCount = nums->Count;
	if (numsCount != size)
		return nullptr;

	array<int>^ renumbering = gcnew array<int>(size);	
	for (int i=0;i<numsCount;i++)
		renumbering[nums[numsCount - i - 1]] = i;

	return renumbering;
}

		}
	}
}