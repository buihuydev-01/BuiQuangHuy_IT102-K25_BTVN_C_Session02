# Sports Odds Monitor - Ứng dụng theo dõi tỷ lệ cược

Ứng dụng WPF C# để theo dõi tỷ lệ cược bóng đá theo thời gian thực.

## Tính năng

✅ **Hiển thị danh sách trận đấu** theo thời gian thực
✅ **Cập nhật tự động** mỗi 10 giây
✅ **Hiển thị tỷ lệ cược**: Cược chấp, Tài/Xỉu, 1X2, Lẻ/Chẵn
✅ **Giao diện đẹp** với màu sắc dễ nhìn
✅ **Sắp xếp** theo thời gian trận đấu
✅ **Trạng thái trực tiếp** của từng trận

## Cấu trúc dự án

```
SportsOddsApp/
├── Models/
│   └── Match.cs                 # Model dữ liệu trận đấu
├── Services/
│   └── OddsApiService.cs        # Service gọi API và parse dữ liệu
├── ViewModels/
│   └── MainViewModel.cs         # ViewModel (MVVM pattern)
├── MainWindow.xaml              # Giao diện chính
├── MainWindow.xaml.cs           # Code-behind
├── App.xaml                     # Application config
├── App.xaml.cs                  # Application code-behind
└── SportsOddsApp.csproj         # Project file
```

## Yêu cầu

- **.NET 6.0 SDK** hoặc cao hơn
- **Windows OS** (WPF chỉ chạy trên Windows)
- **Visual Studio 2022** hoặc **Visual Studio Code**

## Cách build và chạy

### Sử dụng Visual Studio 2022:

1. Mở file `SportsOddsApp.sln` trong Visual Studio
2. Nhấn **F5** hoặc click **Start** để chạy

### Sử dụng Command Line:

```bash
# Di chuyển vào thư mục project
cd SportsOddsApp

# Restore dependencies
dotnet restore

# Build project
dotnet build

# Chạy ứng dụng
dotnet run
```

### Build file .exe:

```bash
# Build Release version
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true

# File .exe sẽ nằm trong: bin/Release/net6.0-windows/win-x64/publish/
```

## Hướng dẫn sử dụng

### 1. Khởi động ứng dụng
- Ứng dụng sẽ tự động tải dữ liệu khi khởi động

### 2. Các nút chức năng

- **🔄 Làm mới**: Tải lại dữ liệu ngay lập tức
- **▶ Tự động**: Bật chế độ tự động cập nhật mỗi 10 giây
- **⏸ Dừng**: Tắt chế độ tự động cập nhật

### 3. Bảng hiển thị

Bảng hiển thị các thông tin:

| Cột | Mô tả |
|-----|-------|
| **Thời Gian** | Giờ thi đấu (HH:mm) |
| **Giải đấu** | Tên giải đấu |
| **Trận đấu** | Đội nhà vs Đội khách |
| **Cược chấp - Tài/Xỉu** | Tỷ lệ cược chấp |
| **1X2** | Tỷ lệ thắng/hòa/thua |
| **Lẻ/Chẵn** | Tỷ lệ lẻ/chẵn |
| **Hiệp 1 - Tài/Xỉu** | Tỷ lệ tài/xỉu hiệp 1 |
| **Hiệp 1 - 1X2** | Tỷ lệ 1X2 hiệp 1 |
| **Trạng thái** | Trạng thái trận đấu |

### 4. Màu sắc

- 🔴 **Đỏ**: Đội nhà / Tỷ lệ dương
- 🔵 **Xanh**: Đội khách / Tỷ lệ âm
- 🟢 **Xanh lá**: Trạng thái đang diễn ra

## API Endpoint

Ứng dụng gọi API từ:
```
https://sports.wwyyuuvv22.com/web-root/restricted/odds-display/today-data.aspx
```

### Parameters:
- `od-param`: 3,1,1,1,1,2,1,2,0
- `fi`: 1 (Filter)
- `v`: Version number
- `dl`: 0

### Response format:
JavaScript code với format:
```javascript
$M('odds-display').onUpdate(3,[data_array]);
```

## Cải tiến có thể thêm

🔹 **Filter theo giải đấu**: Lọc chỉ xem các giải quan tâm
🔹 **Sắp xếp theo cột**: Click vào header để sắp xếp
🔹 **Export dữ liệu**: Xuất ra Excel/CSV
🔹 **Thông báo**: Alert khi tỷ lệ thay đổi lớn
🔹 **Lưu cấu hình**: Lưu settings người dùng
🔹 **Chart**: Biểu đồ thay đổi tỷ lệ theo thời gian
🔹 **Âm thanh**: Cảnh báo bằng âm thanh
🔹 **Dark mode**: Chế độ tối cho ban đêm

## Xử lý lỗi

Nếu gặp lỗi kết nối:
1. Kiểm tra kết nối Internet
2. Kiểm tra Cookie và Session trong `OddsApiService.cs`
3. URL có thể thay đổi, cần cập nhật `BaseUrl`

## Lưu ý quan trọng

⚠️ **Cookies và Session**: Cookies trong code có thể hết hạn. Cần lấy cookies mới từ browser:
1. Mở DevTools (F12) trên trang web
2. Vào tab Network
3. Tìm request đến API
4. Copy cookies và cập nhật trong `OddsApiService.cs`

⚠️ **Legal**: Ứng dụng chỉ mục đích học tập và nghiên cứu. Không sử dụng cho mục đích thương mại.

## License

MIT License - Tự do sử dụng và chỉnh sửa.

## Tác giả

Created with ❤️ for educational purposes.
