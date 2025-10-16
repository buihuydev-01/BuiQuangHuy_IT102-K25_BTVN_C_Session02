#include <stdio.h>

int main(void) {
    int number = 12345;
    int result = 0;

    int original = number;

    while (number > 0) {
        int digit = number % 10;     
        result = result * 10 + digit; 
        number = number / 10;         
    }

    printf("Original: %d\n", original);
    printf("Reversed: %d\n", result);

    return 0;
}