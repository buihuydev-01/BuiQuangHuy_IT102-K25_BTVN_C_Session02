#include <stdio.h>

int main(void) {
    /* Khai báo và khởi tạo */
    int length = 8; /* chiều dài */
    int width  = 5; /* chiều rộng */

    /* Tính chu vi và diện tích */
    int perimeter = 2 * (length + width);
    int area = length * width;

    /* Hiển thị kết quả */
    printf("Length = %d\n", length);
    printf("Width  = %d\n", width);
    printf("Perimeter = %d\n", perimeter);
    printf("Area      = %d\n", area);

    return 0;
}