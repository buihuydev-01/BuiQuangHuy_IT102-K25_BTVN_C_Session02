#include <stdio.h>
#include <math.h>

int main(void) {
    int a = 2;
    int b = 3;
    int c = 1;

    long long a_cubed = (long long)a * a * a;
    long long b_squared = (long long)b * b;

    double under_sqrt = (double)(a + b - c);
    if (under_sqrt < 0.0) {
        printf("Lỗi: biểu thức trong căn âm: a + b - c = %.0f\n", under_sqrt);
        return 1;
    }

    double A = (double)a_cubed + (double)b_squared + 2.0 * c + sqrt(under_sqrt);
    printf("a=%d, b=%d, c=%d\n", a, b, c);
    printf("A = a^3 + b^2 + 2c + sqrt(a + b - c) = %.6f\n", A);

    return 0;
}
