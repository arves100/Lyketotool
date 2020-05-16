#pragma once

#include <stdint.h>

class Memory
{
public:
	Memory();
	Memory(size_t zSize);
	Memory(const uint8_t* pbSrc, size_t zSize);
	Memory(Memory* memory);

	~Memory();

	void Alloc(size_t zSize);
	void Free();
	size_t GetBufferSize();
	uint8_t* GetBuffer();
	void CopyBuffer(const uint8_t* pbSrc, size_t zLen);
	void CopyBufferTo(uint8_t* pbDest, size_t zLen);

private:
	uint8_t* m_pbMemoryBuffer;
	size_t m_zMemoryBufferLength;
};

Memory* GetGlobalMemory();
void FreeGlobalMemory();
