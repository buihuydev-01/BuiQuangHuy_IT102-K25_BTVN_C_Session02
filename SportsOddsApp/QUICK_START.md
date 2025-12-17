# Quick Start Guide - Hướng dẫn nhanh

## 1. Cài đặt Requirements

### Windows:
1. Tải và cài đặt **.NET 6.0 SDK** từ: https://dotnet.microsoft.com/download
2. (Tùy chọn) Cài đặt **Visual Studio 2022** Community Edition

### Kiểm tra cài đặt:
```bash
dotnet --version
# Should output: 6.0.x or higher
```

## 2. Build Project

### Sử dụng Visual Studio:
1. Double-click file `SportsOddsApp.sln`
2. Nhấn **F5** hoặc click **Start**

### Sử dụng Command Line:
```bash
cd SportsOddsApp
dotnet restore
dotnet build
dotnet run
```

## 3. Sử dụng ứng dụng

### Lần đầu chạy:
1. Ứng dụng sẽ tự động tải dữ liệu
2. Nếu báo lỗi → Cần cập nhật cookies (xem bước 4)

### Các tính năng:
- **🔄 Làm mới**: Tải lại dữ liệu ngay
- **▶ Tự động**: Bật tự động cập nhật (10s/lần)
- **⏸ Dừng**: Tắt tự động cập nhật

### Bảng dữ liệu:
- **Thời Gian**: Giờ thi đấu
- **Giải đấu**: Tên giải
- **Trận đấu**: Đội nhà vs Đội khách
- **Cược chấp - Tài/Xỉu**: Tỷ lệ cược chấp
- **1X2**: Thắng/Hòa/Thua
- **Hiệp 1**: Tỷ lệ hiệp 1

## 4. Cập nhật Cookies (nếu cần)

### Khi nào cần cập nhật?
- Lỗi kết nối
- Không tải được dữ liệu
- Response trống

### Cách cập nhật:
1. Mở browser, vào trang web sports
2. Nhấn **F12** → Tab **Network**
3. Tìm request `today-data.aspx`
4. Copy header `Cookie:`
5. Dán vào file `Services/OddsApiService.cs` tại method `SetupHeaders()`

Chi tiết: Xem file `HOW_TO_UPDATE_COOKIES.md`

## 5. Troubleshooting

### Lỗi: "Could not load file or assembly..."
```bash
dotnet clean
dotnet restore
dotnet build
```

### Lỗi: "The name 'InitializeComponent' does not exist"
- Rebuild solution trong Visual Studio
- Hoặc: `dotnet build --no-incremental`

### Lỗi: "Connection timeout"
- Kiểm tra Internet
- Kiểm tra cookies
- Kiểm tra URL còn đúng không

### Lỗi: "No matches found"
- Website có thể thay đổi cấu trúc
- Cần cập nhật parser
- Kiểm tra response trong browser DevTools

## 6. Build EXE file

### Build single-file executable:
```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```

File .exe sẽ ở:
```
bin/Release/net6.0-windows/win-x64/publish/SportsOddsApp.exe
```

### Build framework-dependent (nhỏ hơn):
```bash
dotnet publish -c Release -r win-x64 --self-contained false
```

## 7. Tùy chỉnh

### Thay đổi thời gian refresh:
File: `ViewModels/MainViewModel.cs`
```csharp
Interval = TimeSpan.FromSeconds(10) // Đổi 10 thành giá trị khác
```

### Thay đổi màu sắc:
File: `MainWindow.xaml`
Tìm các `Background="..."` và `Foreground="..."`

### Filter giải đấu:
Thêm logic filter trong `MainViewModel.cs`:
```csharp
Matches = Matches.Where(m => 
    m.LeagueName.Contains("e-Football")
).ToList();
```

## 8. Next Steps

- [ ] Thêm filter theo giải
- [ ] Thêm search box
- [ ] Export to Excel
- [ ] Thêm notifications
- [ ] Lưu favorites
- [ ] Chart hiển thị odds changes

## 9. FAQ

**Q: Có cần đăng nhập không?**
A: Không, chỉ cần cookies hợp lệ

**Q: Có thể chạy 24/7 không?**
A: Có, nhưng cookies sẽ hết hạn sau ~24h

**Q: Có thể theo dõi nhiều giải cùng lúc không?**
A: Có, ứng dụng hiển thị tất cả giải có trong API response

**Q: Dữ liệu có chính xác không?**
A: Dữ liệu lấy trực tiếp từ API, độ chính xác phụ thuộc vào nguồn

**Q: Có thể custom UI không?**
A: Có, chỉnh sửa file `MainWindow.xaml`

## 10. Support

Nếu gặp vấn đề:
1. Check logs trong console
2. Check Network tab trong DevTools
3. Verify cookies còn hiệu lực
4. Check API response format

## Screenshots

### Main Window
```
┌─────────────────────────────────────────────────┐
│  🔄 Làm mới  ▶ Tự động  ⏸ Dừng                 │
├─────────────────────────────────────────────────┤
│ Thời Gian │ Giải đấu │ Trận đấu │ Tỷ lệ ...    │
├─────────────────────────────────────────────────┤
│  15:15    │ e-F24    │ Team A   │ 0.82  ...    │
│           │          │ Team B   │ 0.92  ...    │
├─────────────────────────────────────────────────┤
│  15:30    │ e-F24    │ Team C   │ 0.75  ...    │
│           │          │ Team D   │ 0.97  ...    │
└─────────────────────────────────────────────────┘
```

Happy coding! 🚀
