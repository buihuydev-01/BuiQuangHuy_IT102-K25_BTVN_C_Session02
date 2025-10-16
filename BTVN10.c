#include <stdio.h>
#include <math.h>

int main(void) {
    int a = 1, b = 2, c = 3;
  
    double value1 = sqrt(pow(a,2) + pow(b,2)) / (c + 1);

    double value2 = (a * b) / c;

    double value3 = sqrt(abs(a-b)+ pow(c,2));

    double S = value1 + value2 - value3;

    printf("S = %.2f\n", S); 
    return 0;
}