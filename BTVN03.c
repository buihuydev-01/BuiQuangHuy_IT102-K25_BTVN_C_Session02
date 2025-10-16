#include <stdio.h>

int main(void) {
    // Khai báo và khởi tạo hai biến số nguyên
    int a = 10;
    int b = 3;

    // Lưu kết quả vào các biến khác
    int tong = a + b;
    int hieu = a - b;
    int tich = a * b;
    double thuong;

    // Kiểm tra chia cho 0 trước khi tính thương
    if (b != 0) {
        thuong = (double)a / b;
    } else {
        thuong = 0.0; // hoặc xử lý lỗi tuỳ ý
    }

    // In kết quả
    printf("a = %d, b = %d\n", a, b);
    printf("Tổng = %d\n", tong);
    printf("Hiệu = %d\n", hieu);
    printf("Tích = %d\n", tich);
    printf("Thương = %.6f\n", thuong);

    return 0;
}