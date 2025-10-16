#include <stdio.h>
#include <math.h>
int main(void) {
    int a = 3;
    int b = 4;
    int c = 5;

    double value1 = pow(a,2) + sqrt(pow(b,2) + 4*a*c) / 2 * a;
    double value2 = (pow(b,3) / pow(c,2));
    double value3 = sqrt(abs(a-b));
    double S = value1 - value2 + value3;
    printf("S = %.2f\n", S);
    return 0;
}