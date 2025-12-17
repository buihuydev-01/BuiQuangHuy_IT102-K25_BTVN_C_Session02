# Hướng dẫn tải Project về máy

## 🎯 Cách 1: Download ZIP (Khuyến nghị)

File ZIP đã được tạo sẵn: `/workspace/SportsOddsApp.zip` (33KB)

### Trong Cursor:
1. Mở Explorer (Ctrl+Shift+E)
2. Tìm file `SportsOddsApp.zip` ở thư mục `/workspace/`
3. Right-click → **Download**
4. Giải nén file ZIP trên máy của bạn

### Trong VS Code Remote:
1. Click vào Explorer
2. Right-click vào `SportsOddsApp.zip`
3. Chọn **Download**

---

## 📋 Cách 2: Clone qua Git (nếu có repo)

```bash
# Nếu bạn đã push lên GitHub
git clone https://github.com/your-username/SportsOddsApp.git
cd SportsOddsApp
```

---

## 🗂️ Cách 3: Copy từng file thủ công

Nếu không download được ZIP, bạn có thể tạo thủ công:

### Bước 1: Tạo cấu trúc thư mục

```bash
mkdir SportsOddsApp
cd SportsOddsApp
mkdir Models Services ViewModels
```

### Bước 2: Copy các file sau

#### Root Files:
- `SportsOddsApp.csproj`
- `SportsOddsApp.sln`
- `App.xaml`
- `App.xaml.cs`
- `MainWindow.xaml`
- `MainWindow.xaml.cs`
- `appsettings.json`
- `.gitignore`

#### Documentation:
- `README.md`
- `QUICK_START.md`
- `HOW_TO_UPDATE_COOKIES.md`
- `PROJECT_STRUCTURE.md`
- `CHANGELOG.md`
- `SUMMARY.md`

#### Models/:
- `Models/Match.cs`

#### Services/:
- `Services/OddsApiService.cs`
- `Services/JavaScriptDataParser.cs`

#### ViewModels/:
- `ViewModels/MainViewModel.cs`

### Bước 3: Copy nội dung

Mở từng file trong workspace, copy nội dung và paste vào file tương ứng trên máy bạn.

---

## 🚀 Cách 4: Sử dụng Terminal/Command Line

### Từ remote workspace:

```bash
# Compress
cd /workspace
tar -czf SportsOddsApp.tar.gz SportsOddsApp/

# Hoặc ZIP
zip -r SportsOddsApp.zip SportsOddsApp/ -x "*/bin/*" "*/obj/*"
```

Sau đó download file nén về máy.

---

## ✅ Sau khi download xong

### Kiểm tra cấu trúc:

```
SportsOddsApp/
├── Models/
│   └── Match.cs
├── Services/
│   ├── OddsApiService.cs
│   └── JavaScriptDataParser.cs
├── ViewModels/
│   └── MainViewModel.cs
├── MainWindow.xaml
├── MainWindow.xaml.cs
├── App.xaml
├── App.xaml.cs
├── SportsOddsApp.csproj
├── SportsOddsApp.sln
└── README.md
```

### Build & Run:

```bash
cd SportsOddsApp
dotnet restore
dotnet build
dotnet run
```

Hoặc mở `SportsOddsApp.sln` trong **Visual Studio 2022**.

---

## 🐛 Troubleshooting

### Lỗi: "Cannot download file"
- Thử cách 3 (copy thủ công)
- Hoặc dùng terminal command

### Lỗi: "File corrupted"
- Download lại file ZIP
- Kiểm tra kích thước file (phải ~33KB)

### Lỗi: "Permission denied"
- Check quyền truy cập folder
- Run terminal với admin rights

---

## 📞 Support

Nếu gặp vấn đề download, hãy:
1. Thử các cách khác nhau ở trên
2. Check terminal output for errors
3. Verify workspace connection

---

**✨ Chúc bạn download thành công!**
