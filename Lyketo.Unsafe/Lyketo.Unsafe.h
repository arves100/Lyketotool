#pragma once

#include <memory>

#include "../LibLyketo/CryptedObject.hpp"

namespace LyketoUnsafe
{
	public ref class CryptedObject
	{
    public:
        CryptedObject();
        ~CryptedObject();

        array<System::Byte>^ Decrypt(array<System::Byte>^ input, array<System::UInt32>^ keys);
        array<System::Byte>^ Encrypt(array<System::Byte>^ input, array<System::UInt32>^ keys);

        void ForceAlgorithm(System::UInt32 fourcc);

    private:
        ::CryptedObject* m_pObject;
	};
}
