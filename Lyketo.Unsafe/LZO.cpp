#include "lzokay/lzokay.hpp"
#include "Memory.hpp"

extern "C"
{

static size_t LzoOutputLength = 0;

size_t l_lzo_get_size()
{
	return LzoOutputLength;
}

int32_t l_lzo_compress(const uint8_t* input, size_t input_length)
{
	if (!input || input_length < 1)
		return -5; // Invalid arguments

	auto estimatedSize = lzokay::compress_worst_size(input_length);

	l_alloc_memory(estimatedSize);

	return static_cast<int32_t>(lzokay::compress(input, input_length, l_get_buffer(), estimatedSize, LzoOutputLength));
}

int32_t l_lzo_decompress(const uint8_t* input, size_t input_length, size_t output_length)
{
	if (!input || input_length < 1)
		return -5;

	l_alloc_memory(output_length);

	return static_cast<int32_t>(lzokay::decompress(input, input_length, l_get_buffer(), output_length, LzoOutputLength));
}

} // EXTERN_C
