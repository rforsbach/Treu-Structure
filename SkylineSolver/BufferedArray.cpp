// BufferedArray.cpp: implementation of the BufferedArray class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "BufferedArray.h"

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

BufferedArray::BufferedArray()
{
	size = 0;
	blockFiles = NULL;
	blockPts = NULL;
	data = NULL;
	firstBlock = lastBlock = -1;
	numBlocks = 0;
	ramBlocks = 0;
	readOnly = false;
}

BufferedArray::~BufferedArray()
{
	if (data) delete[] data;
	if (blockPts) delete[] blockPts;
	if (blockFiles)
	{
		for (int i=0; i<numBlocks; i++)
			if (!blockFiles[i].IsEmpty())
				_wremove(blockFiles[i]);
		delete[] blockFiles;
	}
}

void BufferedArray::storeBlock(int index, bool saveOnly)
{
	if (index < 0 || index >= numBlocks) return;
	if (!blockPts[index]) return;

	if (!readOnly) 
	{
		if (blockFiles[index].IsEmpty())
		{
			CString dir;
			GetTempPathW(255, dir.GetBuffer(255));
			dir.ReleaseBuffer();
			
			//CString dir = ((CSGL1App*)AfxGetApp())->getProgramDir();
			wchar_t *path = _wtempnam(dir + L"\\tmp", L"kblock");
			if (path)
				blockFiles[index] = path;
			else
				throw HEAVY_ERROR;
		}

		FILE *fp;
		_wfopen_s(&fp, blockFiles[index], L"wb");
		fwrite(blockPts[index], sizeof(double) * BLOCK_SIZE, 1, fp);
		if (ferror(fp))
			int ppp=1;
		fflush(fp);
		fclose(fp);
	}

	if (!saveOnly) blockPts[index] = NULL;
}

void BufferedArray::loadBlock(int newIndex, double *ramAddress)
{
	if (newIndex < 0 || newIndex >= numBlocks) return;
	if (blockPts[newIndex]) return;

	if (ramAddress < data || (ramAddress + BLOCK_SIZE) > (data + ramBlocks*BLOCK_SIZE))
		int ppp=1;

	blockPts[newIndex] = ramAddress;
	
	if (blockFiles[newIndex].IsEmpty())
	{
		ZeroMemory(ramAddress, BLOCK_SIZE * sizeof(double));
	}
	else
	{
		FILE *fp;
		_wfopen_s(&fp, blockFiles[newIndex], L"rb");
		fread(ramAddress, sizeof(double) * BLOCK_SIZE, 1, fp);
		if (ferror(fp))
			int ppp=1;
		fclose(fp);
	}
}

void BufferedArray::insertBlock(int newIndex)
{
	int i;
	double *expiredAddress;

	if (blockPts[newIndex]) return;

	if (newIndex < firstBlock)
	{
		if (firstBlock - newIndex < ramBlocks)
			i = firstBlock-1;
		else
			i = newIndex + ramBlocks - 1;

		for (; i >= newIndex; i--)
		{
			expiredAddress = blockPts[lastBlock];
			storeBlock(lastBlock);
			loadBlock(i, expiredAddress);
			lastBlock--;
		}
		firstBlock = newIndex;
		lastBlock = newIndex + ramBlocks - 1;
	}
	else if (newIndex > lastBlock)
	{
		if (newIndex - lastBlock < ramBlocks)
			i = lastBlock+1;
		else
			i = newIndex - ramBlocks + 1;

		for (; i <= newIndex; i++)
		{
			expiredAddress = blockPts[firstBlock];
			storeBlock(firstBlock);
			loadBlock(i, expiredAddress);
			firstBlock++;
		}
		firstBlock = newIndex - ramBlocks + 1;
		lastBlock = newIndex;
	}
	else
		throw HEAVY_ERROR;
}

void BufferedArray::init(int size)
{
	int neededMem, blockMem, i;

	this->size = size;
	int memSize = size*sizeof(double);
	numBlocks = (int)(size / BLOCK_SIZE) + 1;
	blockMem = BLOCK_SIZE * sizeof(double);

	// Checar memoria!!!!!!
	int totalMem = 0x20000000;
	//int totalMem = 0x800000;

	totalMem = (int)(totalMem * MEMORY_USE);
	neededMem = numBlocks * blockMem;
	ramBlocks = (totalMem < neededMem) ? totalMem : neededMem;
	ramBlocks = (int)(ramBlocks / blockMem);
	
	data = new double[ramBlocks * BLOCK_SIZE];
	ZeroMemory(data, ramBlocks * BLOCK_SIZE * sizeof(double));
	blockPts = new double*[numBlocks];
	ZeroMemory(blockPts, numBlocks*sizeof(double*));
	for (i=0; i<ramBlocks; i++)
		blockPts[i] = data + i*BLOCK_SIZE;
	blockFiles = new CString[numBlocks];
	firstBlock = 0;
	lastBlock = ramBlocks-1;
	readOnly = false;
}


void BufferedArray::setReadOnly(bool rw)
{
	int i;
	if (ramBlocks != numBlocks)
		for (i=0; i<numBlocks; i++)
			storeBlock(i, true);

	readOnly = rw;
}
