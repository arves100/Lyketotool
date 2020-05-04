#pragma once

#include <stdint.h>

void l_alloc_memory(size_t size);
void l_free_memory();
size_t l_get_buffer_size();
uint8_t* l_get_buffer();
