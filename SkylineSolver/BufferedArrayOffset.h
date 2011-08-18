
#pragma once
#pragma unmanaged

class BufferedArray;

class BufferedArrayOffset  
{
public:
	BufferedArrayOffset(BufferedArray *ba, int offset);
	virtual ~BufferedArrayOffset();

	virtual double& operator[] (int i);
protected:
	BufferedArray* ba;
	int offset;
};
