#include "Memory.hpp"

#include <memory.h>

static Memory* g_pGlobalMemory = nullptr;

Memory::Memory() : m_pbMemoryBuffer(nullptr), m_zMemoryBufferLength(0)
{
}

Memory::Memory(size_t zSize) : Memory()
{
	Alloc(zSize);
}

Memory::Memory(const uint8_t* pbSrc, size_t zSize) : Memory()
{
	CopyBuffer(pbSrc, zSize);
}

Memory::Memory(Memory* memory) : Memory(memory->GetBuffer(), memory->GetBufferSize())
{

}


Memory::~Memory()
{
	Free();
}

void Memory::Free()
{
	if (m_pbMemoryBuffer)
	{
		delete[] m_pbMemoryBuffer;
	}

	m_zMemoryBufferLength = 0;
	m_pbMemoryBuffer = nullptr;
}

void Memory::Alloc(size_t zSize)
{
	Free();
	m_pbMemoryBuffer = new uint8_t[zSize];
	memset(m_pbMemoryBuffer, 0, zSize);
	m_zMemoryBufferLength = zSize;
}

size_t Memory::GetBufferSize()
{
	return m_zMemoryBufferLength;
}

uint8_t* Memory::GetBuffer()
{
	return m_pbMemoryBuffer;
}

void Memory::CopyBufferTo(uint8_t* pbDest, size_t zLen)
{
	memcpy_s(pbDest, zLen, m_pbMemoryBuffer, zLen);
}

void Memory::CopyBuffer(const uint8_t* pbSrc, size_t zLen)
{
	Alloc(zLen);
	memcpy_s(m_pbMemoryBuffer, zLen, pbSrc, zLen);
}

Memory* GetGlobalMemory()
{
	if (!g_pGlobalMemory)
		g_pGlobalMemory = new Memory();

	return g_pGlobalMemory;
}

void FreeGlobalMemory()
{
	delete g_pGlobalMemory;
	g_pGlobalMemory = nullptr;
}
