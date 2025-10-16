#include <stdio.h>

int main(void) {
    int number = 12345;

    /* Tách từng chữ số bằng % và / */
    int d1 = number % 10;
    int d2 = (number / 10) % 10;
    int d3 = (number / 100) % 10;
    int d4 = (number / 1000) % 10;
    int d5 = (number / 10000) % 10;

    /* Tính tổng các chữ số */
    int sum = d1 + d2 + d3 + d4 + d5;

    /* In kết quả */
    printf("Number: %d\n", number);
    printf("Digits (from most to least significant): %d %d %d %d %d\n", d5, d4, d3, d2, d1);
    printf("Sum of digits: %d\n", sum);

    return 0;
}