#include <stdio.h>

int main(void) {
    // Khai báo và khởi tạo 2 biến int và 1 biến float
    int a = 5;
    int b = -3;
    float c = 4.7;

    // Tính tổng cả 3 số lưu vào float
    float sumFloat = a + b + c;

    // Tính tổng phần nguyên của 3 số (ép phần nguyên của float bằng (int))
    int sumInt = a + b + (int)c;

    // In kết quả
    printf("a = %d\n", a);
    printf("b = %d\n", b);
    printf("c = %.2f\n\n", c);
    printf("Tổng (float) = %.2f\n", sumFloat);
    printf("Tổng phần nguyên (int) = %d\n", sumInt);

    return 0;
}