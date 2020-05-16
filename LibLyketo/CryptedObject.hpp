/*
	File: CryptedObject.h
	Purpouse: Open source CryptedObject implementation
*/
#pragma once

#include "CompressAlgorithms.hpp"
#include <memory>
#include <map>
#include <vector>

class CryptedObject
{
public:
	CryptedObject();
	~CryptedObject();

	bool Decrypt(const uint8_t* pbInput, size_t nLength, const uint32_t adwKeys[]);
	bool Encrypt(const uint8_t* pbInput, size_t nLength, const uint32_t adwKeys[]);
	
	void ForceAlgorithm(uint32_t dwFourcc);
	void AddAlgorithm(uint32_t dwFourcc, std::unique_ptr<ICompressAlgorithm> upcAlgorithm);

	const uint8_t* GetBuffer() { return m_vBuffer.data(); }
	size_t GetSize() { return m_vBuffer.size(); }

private:
	uint32_t m_dwFourCC;
	uint32_t m_dwAfterCryptLength;
	uint32_t m_dwAfterCompressLength;
	uint32_t m_dwRealLength;

	std::vector<uint8_t> m_vBuffer;
	std::map<uint32_t, std::unique_ptr<ICompressAlgorithm>> m_mAlgorithms;

	uint32_t m_dwForcedAlgorithm;
};
