// BufferedArrayOffset.cpp: implementation of the BufferedArrayOffset class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "BufferedArrayOffset.h"
#include "BufferedArray.h"

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

BufferedArrayOffset::BufferedArrayOffset(BufferedArray *ba, int offset)
{
	this->offset = offset;
	this->ba = ba;
}

BufferedArrayOffset::~BufferedArrayOffset()
{

}

double& BufferedArrayOffset::operator[](int i)
{	
	return (*ba)[i+offset];
}
