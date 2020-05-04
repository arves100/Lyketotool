#include "Memory.hpp"

#include <memory.h>

static uint8_t* MemoryBuffer = nullptr;
static size_t MemoryBufferSize = 0;

void l_free_memory()
{
	if (MemoryBuffer)
	{
		delete[] MemoryBuffer;
	}

	MemoryBufferSize = 0;
	MemoryBuffer = nullptr;
}

void l_alloc_memory(size_t size)
{
	l_free_memory();
	MemoryBuffer = new uint8_t[size];
	memset(MemoryBuffer, 0, size);
	MemoryBufferSize = size;
}

size_t l_get_buffer_size()
{
	return MemoryBufferSize;
}

uint8_t* l_get_buffer()
{
	return MemoryBuffer;
}

void l_copy_buffer(uint8_t* dst, size_t len)
{
	memcpy_s(dst, len, MemoryBuffer, len);
}
