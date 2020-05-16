/*
	Project: LibLyketo
	File: CryptedObject.cpp
	Purpose: Defines
*/
#include "CryptedObject.hpp"
#include "BitConvert.hpp"
#include "xtea.hpp"

CryptedObject::CryptedObject() : m_dwFourCC(0), m_dwAfterCryptLength(0), m_dwAfterCompressLength(0), m_dwRealLength(0), m_dwForcedAlgorithm(0)
{
	// Standard file formats
	m_mAlgorithms[BitConvert::FromByteArray("MCOZ")] = std::make_unique<CompressAlgorithmLzo1x>();
}

CryptedObject::~CryptedObject()
{

}

bool CryptedObject::Decrypt(const uint8_t* pbInput, size_t nLength, const uint32_t adwKeys[])
{
	if (!pbInput || !adwKeys || nLength < 20)
		return false;

	m_dwFourCC = BitConvert::FromByteArray(pbInput);

	auto it = m_mAlgorithms.find(m_dwFourCC);

	if (it == m_mAlgorithms.end())
		return false;

	m_dwAfterCryptLength = BitConvert::FromByteArray(pbInput + 4);
	m_dwAfterCompressLength = BitConvert::FromByteArray(pbInput + 8);
	m_dwRealLength = BitConvert::FromByteArray(pbInput + 12);
	std::vector<uint8_t> data;

	if (m_dwRealLength < 1)
	{
		return false;
	}

	// 1. Decrypt the data
	if (m_dwAfterCryptLength > 0)
	{
		if ((nLength - 20) != m_dwAfterCryptLength)
		{
			return false;
		}
		
		data.resize(m_dwAfterCompressLength + 20); // +19 -.-''
		data.reserve(m_dwAfterCompressLength + 20);

		XTEA::Decrypt(pbInput + 16, data.data(), m_dwAfterCryptLength, adwKeys, 32);

		if (BitConvert::FromByteArray(data.data()) != m_dwFourCC) // Verify decryptation
		{
			return false;
		}
	}

	// 2. Decompress the data
	if (m_dwAfterCompressLength > 0)
	{
		const uint8_t* inputData;

		if (m_dwAfterCryptLength < 1) // Data is not encrypted
		{
			if ((nLength - 16) != m_dwAfterCompressLength)
				return false;

			data.resize(m_dwAfterCompressLength);
			data.reserve(m_dwAfterCompressLength);
			memcpy_s(data.data(), m_dwAfterCompressLength, pbInput + 16, m_dwAfterCompressLength);

			inputData = data.data();
		}
		else
		{
			inputData = data.data() + 4;
		}

		m_vBuffer.resize(m_dwRealLength);
		m_vBuffer.reserve(m_dwRealLength);

		size_t nRealLength = m_dwRealLength;
		if (!it->second->Decrypt(inputData, m_vBuffer.data(), m_dwAfterCompressLength, &nRealLength))
		{
			return false;
		}

		if (nRealLength != m_dwRealLength)
		{
			return false;
		}
	}
	else
	{
		if (m_dwAfterCompressLength > 0)
		{
			return true;
		}

		// Data is not compressed at all

		size_t nRealDataLenCalculated = nLength - 16;

		if (nRealDataLenCalculated != m_dwRealLength)
			return false;

		m_vBuffer.resize(nRealDataLenCalculated);
		m_vBuffer.reserve(nRealDataLenCalculated);
		memcpy_s(m_vBuffer.data(), nRealDataLenCalculated, pbInput + 16, nRealDataLenCalculated);
	}

	return true;
}

bool CryptedObject::Encrypt(const uint8_t* pbInput, size_t nLength, const uint32_t adwKeys[])
{
	if (!pbInput || !adwKeys || nLength < 1)
		return false;

	auto it = m_mAlgorithms.find(m_dwForcedAlgorithm);
	if (it == m_mAlgorithms.end())
	{
		it = m_mAlgorithms.begin(); // Pick the first one if an invalid was specified
	}

	m_dwRealLength = static_cast<uint32_t>(nLength);
	m_dwFourCC = it->first;

	size_t nCompressedSize = it->second->GetWrostSize(nLength);

	std::vector<uint8_t> data;
	data.reserve(nCompressedSize);
	data.resize(nCompressedSize);

	// 1. Compress the data
	if (!it->second->Encrypt(pbInput, data.data(), nLength, &nCompressedSize))
	{
		return false;
	}

	m_dwAfterCompressLength = static_cast<uint32_t>(nCompressedSize);

	// 2. Append encrypt FourCC
	data.resize(data.size() + 4);
	data.reserve(data.size() + 4);

	uint8_t tmp[4];
	BitConvert::ToByteArray(m_dwFourCC, tmp);
	data.insert(data.begin(), tmp[3]);
	data.insert(data.begin(), tmp[2]);
	data.insert(data.begin(), tmp[1]);
	data.insert(data.begin(), tmp[0]);

	// 3. Encrypt data
	m_dwAfterCryptLength = m_dwAfterCompressLength + 20;
	m_vBuffer.reserve(m_dwAfterCryptLength);
	m_vBuffer.resize(m_dwAfterCryptLength);

	XTEA::Encrypt(data.data(), m_vBuffer.data(), m_dwAfterCryptLength, adwKeys, 32);

	// 4. Store header
	m_vBuffer.reserve(m_dwAfterCryptLength + 16);
	m_vBuffer.resize(m_dwAfterCryptLength + 16);

	BitConvert::ToByteArray(m_dwRealLength, tmp);
	data.insert(data.begin(), tmp[3]);
	data.insert(data.begin(), tmp[2]);
	data.insert(data.begin(), tmp[1]);
	data.insert(data.begin(), tmp[0]);

	BitConvert::ToByteArray(m_dwAfterCompressLength, tmp);
	data.insert(data.begin(), tmp[3]);
	data.insert(data.begin(), tmp[2]);
	data.insert(data.begin(), tmp[1]);
	data.insert(data.begin(), tmp[0]);

	BitConvert::ToByteArray(m_dwAfterCryptLength, tmp);
	data.insert(data.begin(), tmp[3]);
	data.insert(data.begin(), tmp[2]);
	data.insert(data.begin(), tmp[1]);
	data.insert(data.begin(), tmp[0]);

	BitConvert::ToByteArray(m_dwFourCC, tmp);
	data.insert(data.begin(), tmp[3]);
	data.insert(data.begin(), tmp[2]);
	data.insert(data.begin(), tmp[1]);
	data.insert(data.begin(), tmp[0]);

	return true; 
}

void CryptedObject::AddAlgorithm(uint32_t dwFourcc, std::unique_ptr<ICompressAlgorithm> upcAlgorithm)
{
	m_mAlgorithms[dwFourcc] = std::unique_ptr<ICompressAlgorithm>(std::move(upcAlgorithm));
}

void CryptedObject::ForceAlgorithm(uint32_t dwFourcc)
{
	if (m_mAlgorithms.find(dwFourcc) != m_mAlgorithms.end())
		m_dwForcedAlgorithm = dwFourcc;
}
