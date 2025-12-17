# Hướng dẫn cập nhật Cookies

Cookies có thể hết hạn sau một thời gian. Khi ứng dụng báo lỗi kết nối, bạn cần cập nhật cookies mới.

## Bước 1: Mở DevTools trên Browser

1. Mở trang web: `https://sports.wwyyuuvv22.com`
2. Đăng nhập vào tài khoản (nếu cần)
3. Nhấn **F12** để mở DevTools
4. Chọn tab **Network** (Mạng)

## Bước 2: Tìm request API

1. Trong tab Network, tìm request đến:
   ```
   today-data.aspx?od-param=...
   ```

2. Click vào request đó

3. Tìm phần **Request Headers** ở bên phải

## Bước 3: Copy Cookies

1. Tìm dòng `Cookie:` trong Request Headers

2. Copy toàn bộ giá trị của Cookie, ví dụ:
   ```
   ASP.NET_SessionId=abc123xyz; _hjSession_1325134=...; fullScreenAds=true; states=...
   ```

## Bước 4: Cập nhật vào Code

### Cách 1: Cập nhật trực tiếp trong code

Mở file: `Services/OddsApiService.cs`

Tìm method `SetupHeaders()`:

```csharp
private void SetupHeaders()
{
    _httpClient.DefaultRequestHeaders.Clear();
    _httpClient.DefaultRequestHeaders.Add("accept", "*/*");
    _httpClient.DefaultRequestHeaders.Add("accept-language", "vi-VN,vi;q=0.9,...");
    
    // CẬP NHẬT Ở ĐÂY - Thay thế cookie cũ bằng cookie mới
    _httpClient.DefaultRequestHeaders.Add("Cookie", "PASTE_COOKIE_MỚI_VÀO_ĐÂY");
    
    // ... các headers khác
}
```

### Cách 2: Cập nhật qua config file

Mở file: `appsettings.json`

Cập nhật giá trị cookies:

```json
{
  "Cookies": {
    "SessionId": "GIÁ_TRỊ_MỚI",
    "FullCookieString": "TOÀN_BỘ_COOKIE_STRING"
  }
}
```

## Bước 5: Rebuild và chạy lại

```bash
dotnet build
dotnet run
```

## Lưu ý quan trọng

⚠️ **Cookies thường hết hạn sau:**
- Vài giờ nếu không hoạt động
- 24 giờ trong hầu hết trường hợp
- Khi đăng xuất khỏi website

⚠️ **Không chia sẻ cookies của bạn** - Cookies chứa thông tin đăng nhập

⚠️ **Cách kiểm tra cookies còn hiệu lực:**
1. Mở Postman hoặc curl
2. Gửi request với cookies mới
3. Kiểm tra response có hợp lệ không

## Ví dụ kiểm tra bằng curl

```bash
curl "https://sports.wwyyuuvv22.com/web-root/restricted/odds-display/today-data.aspx?od-param=3,1,1,1,1,2,1,2,0&fi=1&v=13474&dl=0" \
  -H "Cookie: YOUR_COOKIE_HERE" \
  -H "User-Agent: Mozilla/5.0..."
```

Nếu response trả về dữ liệu JavaScript hợp lệ → Cookies còn hiệu lực ✅

Nếu response trả về lỗi hoặc redirect → Cookies hết hạn ❌

## Tự động hóa (Advanced)

Để tự động lấy cookies, bạn có thể:

1. Sử dụng **Selenium WebDriver** để tự động đăng nhập
2. Sử dụng **Puppeteer/Playwright** để lấy cookies
3. Lưu cookies vào file và đọc từ file

Ví dụ với Selenium:

```csharp
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

var driver = new ChromeDriver();
driver.Navigate().GoToUrl("https://sports.wwyyuuvv22.com");

// Login process...

// Get cookies
var cookies = driver.Manage().Cookies.AllCookies;
string cookieString = string.Join("; ", 
    cookies.Select(c => $"{c.Name}={c.Value}"));

Console.WriteLine(cookieString);
```

## Troubleshooting

### Lỗi: "Unauthorized" hoặc "403 Forbidden"
→ Cookies đã hết hạn, cần lấy cookies mới

### Lỗi: "Invalid response format"
→ Website có thể đã thay đổi cấu trúc, cần cập nhật parser

### Lỗi: "Connection timeout"
→ Kiểm tra kết nối Internet hoặc URL có thay đổi không

## Liên hệ

Nếu có vấn đề, hãy:
1. Kiểm tra lại các bước trên
2. Xem logs trong console
3. Kiểm tra Network tab trong DevTools
