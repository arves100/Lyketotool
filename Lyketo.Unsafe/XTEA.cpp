#include <stdint.h>
#include <memory.h>
#include "Memory.hpp"

// From: https://en.wikipedia.org/wiki/XTEA

/* take 64 bits of data in v[0] and v[1] and 128 bits of key[0] - key[3] */

#define NUM_ROUNDS 32

extern "C"
{

static void xteaencode(uint32_t v[2], uint32_t const key[4]) {
    unsigned int i;
    uint32_t v0=v[0], v1=v[1], sum=0, delta=0x9E3779B9;
    for (i=0; i < NUM_ROUNDS; i++) {
        v0 += (((v1 << 4) ^ (v1 >> 5)) + v1) ^ (sum + key[sum & 3]);
        sum += delta;
        v1 += (((v0 << 4) ^ (v0 >> 5)) + v0) ^ (sum + key[(sum>>11) & 3]);
    }
    v[0]=v0; v[1]=v1;
}

static void xteadecode(uint32_t v[2], uint32_t const key[4]) {
    unsigned int i;
    uint32_t v0=v[0], v1=v[1], delta=0x9E3779B9, sum=delta*NUM_ROUNDS;
    for (i=0; i < NUM_ROUNDS; i++) {
        v1 -= (((v0 << 4) ^ (v0 >> 5)) + v0) ^ (sum + key[(sum>>11) & 3]);
        sum -= delta;
        v0 -= (((v1 << 4) ^ (v1 >> 5)) + v1) ^ (sum + key[sum & 3]);
    }
    v[0]=v0; v[1]=v1;
}

bool l_xtea_crypt(const uint32_t* data, size_t len, const uint32_t* key)
{
    if (!data || !key || len < 1)
        return false;

    if ((len % 2) != 0)
        l_alloc_memory(len + 1);
    else
        l_alloc_memory(len);

    memcpy_s(l_get_buffer(), l_get_buffer_size(), data, len);

    uint32_t* ptr = reinterpret_cast<uint32_t*>(l_get_buffer());

    for (size_t i = 0; i < l_get_buffer_size(); i += 8, ptr += 2)
    {
        xteaencode(ptr, key);
    }

    return true;
}

bool l_xtea_decrypt(const uint32_t* data, size_t len, const uint32_t* key)
{
    if (!data || !key || len < 2 || (len % 2) != 0)
        return false;

    l_alloc_memory(len);

    memcpy_s(l_get_buffer(), l_get_buffer_size(), data, len);

    uint32_t* ptr = reinterpret_cast<uint32_t*>(l_get_buffer());

    for (size_t i = 0; i < len; ptr += 2, i += 8)
    {
        xteadecode(ptr, key);
    }

    return true;
}

} // EXTERN_C
