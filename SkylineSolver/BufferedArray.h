
#pragma once
#pragma unmanaged

///*
#define BLOCK_SIZE 0x200000
#define BLOCK_BITS 21
#define MASK_BITS 0x1FFFFF
//*/
/*
#define BLOCK_SIZE 0x40000
#define BLOCK_BITS 18
#define MASK_BITS 0x3FFFF
*/

#define MEMORY_USE 0.5
#define HEAVY_ERROR 5

class BufferedArray  
{
public:
	void setReadOnly (bool rw=true);
	virtual void init (int size);

	inline double& operator[] (int i)
	{	
		register int block = (i >> BLOCK_BITS);
		if (!blockPts[block]) insertBlock(block);
		return blockPts[block][i & MASK_BITS]; 
	};

	BufferedArray();
	virtual ~BufferedArray();

protected:
	bool readOnly;
	int lastBlock;
	int firstBlock;
	virtual void insertBlock (int newIndex);
	int ramBlocks;
	CString* blockFiles;
	virtual void loadBlock (int newIndex, double *ramAddress);
	virtual void storeBlock (int index, bool saveOnly = false);
	double ** blockPts;
	double *data;
	int numBlocks;
	int size;
};
