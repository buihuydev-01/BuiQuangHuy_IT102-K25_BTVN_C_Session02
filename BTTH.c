#include <stdio.h>

int main(void) {
    int a = 2;
    int b = 3;
    int c = 4;

    double result = (double)(a + b) * c;
    printf("a = %d, b = %d, c = %d\n", a, b, c);
    printf("Kết quả = %g\n", result);

    return 0;
}