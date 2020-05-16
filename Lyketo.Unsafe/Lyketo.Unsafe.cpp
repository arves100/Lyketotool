#include "Lyketo.Unsafe.h"

#pragma comment(lib, "user32.lib")

namespace LyketoUnsafe
{
    CryptedObject::CryptedObject() : m_pObject(new ::CryptedObject())
    {
    }

    CryptedObject::~CryptedObject()
    {
        if (m_pObject)
            delete m_pObject;

        m_pObject = nullptr;
    }

    array<System::Byte>^ CryptedObject::Decrypt(array<System::Byte>^ input, array<System::UInt32>^ keys)
    {
        if (input->Length < 1 || keys->Length != 4)
            return nullptr;

        pin_ptr<System::Byte> nativeInput = &input[0];
        pin_ptr<System::UInt32> nativeKeys = &keys[0];

        if (!m_pObject->Decrypt(nativeInput, input->Length, nativeKeys))
            return nullptr;

        int arraySize = static_cast<int>(m_pObject->GetSize());

        array<System::Byte>^ destinationArray = gcnew array<System::Byte>(arraySize);

        const uint8_t* sourceArray = m_pObject->GetBuffer();
        pin_ptr<System::Byte> nativeDest = &destinationArray[0];

        memcpy_s(nativeDest, arraySize, sourceArray, arraySize);

        return destinationArray;
    }
    
    array<System::Byte>^ CryptedObject::Encrypt(array<System::Byte>^ input, array<System::UInt32>^ keys)
    {
        if (input->Length < 1 || keys->Length != 4)
            return nullptr;

        pin_ptr<System::Byte> nativeInput = &input[0];
        pin_ptr<System::UInt32> nativeKeys = &keys[0];

        if (!m_pObject->Encrypt(nativeInput, input->Length, nativeKeys))
            return nullptr;

        int arraySize = static_cast<int>(m_pObject->GetSize());

        array<System::Byte>^ destinationArray = gcnew array<System::Byte>(arraySize);

        const uint8_t* sourceArray = m_pObject->GetBuffer();
        pin_ptr<System::Byte> nativeDest = &destinationArray[0];

        memcpy_s(nativeDest, arraySize, sourceArray, arraySize);

        return destinationArray;
    }

    void CryptedObject::ForceAlgorithm(System::UInt32 fourcc)
    {
        m_pObject->ForceAlgorithm(fourcc);
    }
}
