#include <stdio.h>

int main() {
    short s = 10;                  // short: so nguyen nho, thuong 2 byte
    int i = 100;                   // int: so nguyen, thuong 4 byte (co the khac tren mot so he thong)
    long l = 1000;                // long: so nguyen lon hon int, 4/8 byte tuong ung voi he thong
    long long ll = 100000;       // long long: so nguyen rat lon, it nhat 8 byte
    unsigned short us = 20;        // unsigned short: so nguyen khong dau nho, thuong 2 byte
    unsigned int ui = 200;        // unsigned int: so nguyen khong dau, thuong 4 byte
    unsigned long ul = 300;      // unsigned long: so nguyen khong dau lon, 4/8 byte
    unsigned long long ull = 400u; // unsigned long long: so nguyen khong dau rat lon, it nhat 8 byte
    char c = 'A';                  // char: ky tu, 1 byte

    /* In ra de tranh canh bao bien khong su dung */
    printf("s = %hd\n", s);
    printf("i = %d\n", i);
    printf("l = %ld\n", l);
    printf("ll = %lld\n", ll);
    printf("us = %hu\n", us);
    printf("ui = %u\n", ui);
    printf("ul = %lu\n", ul);
    printf("ull = %llu\n", ull);
    printf("c = %c\n", c);

    return 0;
}