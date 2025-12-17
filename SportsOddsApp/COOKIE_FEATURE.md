# 🍪 Tính năng nhập Cookie mới

## ✨ Đã thêm vào phiên bản này

### 1. Ô nhập Cookie trong UI

Bây giờ bạn có thể **nhập Cookie trực tiếp** trong ứng dụng mà không cần sửa code!

```
┌─────────────────────────────────────────────────┐
│  🔄 Làm mới   ▶ Tự động   ⏸ Dừng                │
│                                                  │
│  🍪 Cookie/Session: [________________] ❓ Hướng dẫn│
└─────────────────────────────────────────────────┘
```

### 2. Các tính năng

✅ **Nhập Cookie trực tiếp** - Paste vào ô và nhấn Enter
✅ **Tự động áp dụng** - Không cần restart app
✅ **Load từ config** - Tự động load cookie từ `appsettings.json`
✅ **Nút trợ giúp** - Click "❓ Hướng dẫn" để xem cách lấy cookie
✅ **Validation** - Tự động kiểm tra cookie hợp lệ

---

## 📖 Hướng dẫn sử dụng

### Cách 1: Nhập trực tiếp trong UI (Khuyến nghị ⭐)

1. **Mở app** và tìm ô "🍪 Cookie/Session:" ở toolbar

2. **Lấy cookie từ browser:**
   - Mở browser (Chrome/Edge)
   - Vào trang web sports
   - Nhấn **F12** (mở DevTools)
   - Tab **Network**
   - Refresh trang (F5)
   - Tìm request `today-data.aspx`
   - Click vào → **Request Headers** → **Cookie:**
   - Copy toàn bộ giá trị

3. **Paste vào ô Cookie** trong app

4. **Nhấn Enter** hoặc click nút **"🔄 Làm mới"**

5. ✅ Done! App sẽ dùng cookie mới

---

### Cách 2: Lưu vào config file

Nếu bạn muốn cookie tự động load mỗi lần mở app:

1. Mở file **`appsettings.json`**

2. Tìm section `"Cookies"`

3. Cập nhật `"FullCookieString"`:

```json
{
  "Cookies": {
    "FullCookieString": "PASTE_COOKIE_CỦA_BẠN_VÀO_ĐÂY",
    "Note": "..."
  }
}
```

4. Save file

5. Restart app → Cookie tự động load

---

## 🔍 Chi tiết kỹ thuật

### Luồng hoạt động

```
User nhập cookie vào TextBox
        ↓
CookieString property changed
        ↓
ViewModel tạo lại OddsApiService với cookie mới
        ↓
HttpClient headers được cập nhật
        ↓
Request tiếp theo dùng cookie mới
```

### Code changes

#### 1. MainViewModel.cs
```csharp
public string CookieString
{
    get => _cookieString;
    set
    {
        _cookieString = value;
        OnPropertyChanged(nameof(CookieString));
        if (!string.IsNullOrWhiteSpace(value))
        {
            _apiService = new OddsApiService(value);
            StatusMessage = "Cookie đã cập nhật";
        }
    }
}
```

#### 2. OddsApiService.cs
```csharp
public OddsApiService(string cookieString)
{
    _cookieString = cookieString ?? GetDefaultCookie();
    // Setup headers với cookie mới
}
```

#### 3. MainWindow.xaml
```xml
<TextBox Text="{Binding CookieString, UpdateSourceTrigger=PropertyChanged}"
         ToolTip="Paste cookie từ browser..."/>
```

---

## 💡 Tips & Tricks

### Tip 1: Copy cookie nhanh
```
F12 → Network → Right-click request → Copy → Copy as cURL
→ Extract phần -H "Cookie: ..." từ command
```

### Tip 2: Test cookie
Sau khi paste cookie mới:
1. Click "🔄 Làm mới"
2. Xem Status message
3. Nếu thành công → Thấy trận đấu load
4. Nếu lỗi → Cookie không hợp lệ

### Tip 3: Lưu cookie backup
- Copy cookie hiện tại vào notepad
- Khi cookie hết hạn, có backup để tham khảo format

---

## 🐛 Troubleshooting

### Lỗi: "Cookie đã cập nhật" nhưng vẫn không load dữ liệu

**Nguyên nhân:** Cookie có thể đúng format nhưng đã hết hạn

**Fix:**
1. Lấy cookie MỚI từ browser (refresh trang trước)
2. Paste lại
3. Click "Làm mới"

### Lỗi: "Invalid cookie format"

**Nguyên nhân:** Cookie bị thiếu hoặc sai format

**Fix:**
1. Check cookie phải có dạng: `key1=value1; key2=value2; ...`
2. Không có ký tự xuống dòng
3. Copy lại từ DevTools

### Lỗi: "Connection refused"

**Nguyên nhân:** URL hoặc network issue

**Fix:**
1. Check Internet connection
2. Verify URL trong `OddsApiService.cs`
3. Check firewall/antivirus

---

## 📊 So sánh với phiên bản cũ

| Tính năng | Trước | Bây giờ |
|-----------|-------|---------|
| Update cookie | Sửa code | Nhập UI ✅ |
| Restart app | Cần | Không cần ✅ |
| Config file | Không | Có ✅ |
| Validation | Không | Có ✅ |
| Help guide | Không | Có ✅ |

---

## 🔮 Tính năng tương lai

Có thể thêm:
- [ ] Save/Load multiple cookies
- [ ] Cookie history
- [ ] Auto-refresh cookie
- [ ] Encrypted cookie storage
- [ ] Import from browser directly
- [ ] Cookie expiration warning

---

## 📝 Example Cookie

**Format đúng:**
```
ASP.NET_SessionId=abc123xyz; _hjSession_1325134=eyJpZCI6...; fullScreenAds=true; states=:1:1:...
```

**Format SAI:**
```
Cookie: ASP.NET_SessionId=...  ❌ (có "Cookie:" ở đầu)
ASP.NET_SessionId=...
_hjSession_1325134=...         ❌ (có xuống dòng)
```

---

## ✅ Checklist sử dụng

Khi cookie hết hạn:
- [ ] Mở browser và vào trang web
- [ ] F12 → Network tab
- [ ] Refresh trang (F5)
- [ ] Tìm request `today-data.aspx`
- [ ] Copy Cookie từ Request Headers
- [ ] Paste vào ô Cookie trong app
- [ ] Nhấn Enter hoặc "Làm mới"
- [ ] Check app load được dữ liệu

---

**Enjoy! 🎉**

Version: 1.1.0 with Cookie Input Feature
Last Updated: Dec 17, 2025
