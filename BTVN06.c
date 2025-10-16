#include <stdio.h>

int main(void) {
    /* Khai báo và khởi tạo */
    int a = 5, b = 3, c = 2, d = 1;

    /* Tính A = a * b - 2*c + 3*(a - d) và lưu kết quả */
    int A = a * b - 2 * c + 3 * (a - d);

    /* In kết quả */
    printf("a=%d b=%d c=%d d=%d\n", a, b, c, d);
    printf("A = %d\n", A);

    return 0;
}