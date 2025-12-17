# ⚽ CẬP NHẬT: HIỂN THỊ THÔNG SỐ HIỆP 1 (HALF 1)

## ✨ Tính năng mới v1.2

Đã thêm **thông số Hiệp 1 (H1)** vào bảng hiển thị!

---

## 📊 CẤU TRÚC BẢNG MỚI

### Format giống bảng thứ 2 trong ảnh:

```
┌─────────────────────────────────────────────────────────────────────────────────────┐
│ Thời │ Giải đấu │ Trận đấu  │ Chấp FT │ T/X FT │ 1X2 FT │ Chấp H1 │ Odds   │ T/X H1 │
│ Gian │          │           │         │        │        │         │ Chấp H1│        │
├─────────────────────────────────────────────────────────────────────────────────────┤
│15:30 │ e-F24    │ Argentina │ 0.75    │ 0.97   │ 1.33   │ 0.25    │ 0.90   │ 1.00   │
│      │          │ Finland   │ 0.97    │ 0.84   │ 5.60   │ 0.68    │ 0.83   │ 0.81   │
└─────────────────────────────────────────────────────────────────────────────────────┘
```

---

## 🎯 CÁC CỘT MỚI

### Full Time (Cả trận):
- **Chấp FT**: Cược chấp Full Time
- **T/X FT**: Tài/Xỉu Full Time  
- **1X2 FT**: Win/Draw/Lose Full Time

### Half 1 (Hiệp 1):
- **Chấp H1**: Tỷ lệ chấp Hiệp 1 (vd: 0.25, -0.5)
- **Odds Chấp H1**: Odds của cược chấp H1 (Home | Away)
- **T/X H1**: Line Tài/Xỉu Hiệp 1 (vd: 1.0, 1.5)
- **Odds T/X H1**: Odds Tài/Xỉu H1 (Over | Under)
- **1X2 H1**: Win/Draw/Lose Hiệp 1

---

## 🔧 THAY ĐỔI KỸ THUẬT

### 1. Model (Match.cs)

Thêm properties cho Hiệp 1:

```csharp
// Handicap Half 1
public string HdpH1Home { get; set; }
public string HdpH1Away { get; set; }
public string HdpH1Line { get; set; }  // Tỷ lệ chấp

// Over/Under Half 1
public string OuH1Home { get; set; }
public string OuH1Away { get; set; }
public string OuH1Line { get; set; }   // Line

// 1X2 Half 1
public string Odds1H1 { get; set; }
public string OddsXH1 { get; set; }
public string Odds2H1 { get; set; }
```

### 2. Parser (JavaScriptDataParser.cs)

Parse odds Hiệp 1 từ API:

```csharp
// Type 7 = Handicap Half 1
// Type 9 = Over/Under Half 1
// Type 8 = 1X2 Half 1
```

### 3. UI (MainWindow.xaml)

Thêm 5 cột mới cho Hiệp 1:
- Chấp H1
- Odds Chấp H1
- T/X H1
- Odds T/X H1
- 1X2 H1

---

## 📥 DOWNLOAD & CÀI ĐẶT

### File mới: **SportsOddsApp_H1_Feature.zip** (43KB)

### Bước 1: Download
```
Trong Cursor:
Ctrl+Shift+E → Tìm "SportsOddsApp_H1_Feature.zip"
Right-click → Download
```

### Bước 2: Giải nén
```
C:\Projects\SportsOddsApp\
```

### Bước 3: Build
```
Mở Visual Studio 2022
Open → SportsOddsApp.sln
Build → Rebuild Solution (Ctrl+Shift+B)
```

### Bước 4: Run
```
F5 hoặc Start
```

---

## 🎨 GIAO DIỆN MỚI

### Toolbar (không đổi):
```
┌─────────────────────────────────────────────┐
│  🔄 Làm mới   ▶ Tự động   ⏸ Dừng           │
│  🍪 Cookie: [__________] ❓ Hướng dẫn      │
└─────────────────────────────────────────────┘
```

### Bảng dữ liệu (MỚI):
```
┌────────────────────────────────────────────────────────────────┐
│ Thời Gian │ Giải đấu │ Trận đấu │ Chấp FT │ T/X FT │ 1X2 FT  │
├────────────────────────────────────────────────────────────────┤
│           │          │          │ Chấp H1 │ Odds Chấp H1       │
│           │          │          │ T/X H1  │ Odds T/X H1        │
│           │          │          │ 1X2 H1  │                    │
└────────────────────────────────────────────────────────────────┘
```

---

## 🎯 CÁCH SỬ DỤNG

### Xem thông số Hiệp 1:

1. **Mở app** và load dữ liệu (tự động hoặc click "🔄 Làm mới")

2. **Scroll ngang** để xem các cột:
   - Ban đầu: Thời Gian, Giải đấu, Trận đấu
   - Giữa: Chấp FT, T/X FT, 1X2 FT
   - **Mới**: Chấp H1, Odds Chấp H1, T/X H1, Odds T/X H1, 1X2 H1

3. **Đọc thông số**:
   - **Chấp H1**: Tỷ lệ chấp (vd: +0.25 = chấp trên 0.25)
   - **Odds Chấp H1**: Home | Away (vd: 0.90 | 0.83)
   - **T/X H1**: Line (vd: 1.0 = Tài/Xỉu 1 bàn)
   - **Odds T/X H1**: Over | Under

4. **So sánh**:
   - Full Time (FT) = Cả trận
   - Half 1 (H1) = Chỉ Hiệp 1

---

## 📊 VÍ DỤ DỮ LIỆU

### Trận: Argentina vs Finland (15:30)

| Loại | Chấp | Odds Home | Odds Away | Line/Handicap |
|------|------|-----------|-----------|---------------|
| **Chấp FT** | - | 0.75 | 0.97 | - |
| **T/X FT** | - | 0.97 | 0.84 | - |
| **1X2 FT** | - | 1.33 | 5.60 | 4.43 |
| **Chấp H1** | +0.25 | 0.90 | 0.68 | - |
| **T/X H1** | 1.0 | 0.81 | 0.91 | - |
| **1X2 H1** | - | 2.47 | 2.07 | 3.78 |

---

## 💡 TIPS & TRICKS

### Tip 1: Filter chỉ xem Hiệp 1
Có thể thêm filter để chỉ hiển thị trận có odds Hiệp 1:
```csharp
// Trong ViewModel
Matches = Matches.Where(m => 
    !string.IsNullOrEmpty(m.HdpH1Line) || 
    !string.IsNullOrEmpty(m.OuH1Line)
).ToList();
```

### Tip 2: Highlight odds thay đổi
Khi odds Hiệp 1 thay đổi, highlight màu vàng:
```xml
<TextBlock Foreground="{Binding IsH1OddsChanged, Converter={...}}"/>
```

### Tip 3: Export Hiệp 1
Export chỉ data Hiệp 1 ra Excel để phân tích

---

## 🔄 SO SÁNH PHIÊN BẢN

| Tính năng | v1.1 | v1.2 (Hiện tại) |
|-----------|------|-----------------|
| Odds Full Time | ✅ | ✅ |
| Odds Hiệp 1 | ❌ | ✅ Mới! |
| Odds Hiệp 2 | ❌ | ❌ (Coming) |
| Cột hiển thị | 9 | 14 |
| Cookie input | ✅ | ✅ |

---

## 🐛 TROUBLESHOOTING

### Không thấy data Hiệp 1?

**Nguyên nhân:** Trận đấu chưa có odds Hiệp 1

**Giải pháp:**
- Chỉ trận sắp diễn ra mới có odds H1
- Trận đã đá không có odds H1
- Check thời gian trận (< 30 phút trước giờ bóng lăn)

### Odds Hiệp 1 hiển thị "-"?

**Nguyên nhân:** Parser chưa map được matchLiveId

**Giải pháp:**
- Đợi auto-refresh (10s)
- Click "🔄 Làm mới"
- Check logs trong Output window

### Bảng quá rộng?

**Giải pháp:**
- Scroll ngang để xem các cột
- Hoặc giảm width các cột trong XAML
- Hoặc ẩn các cột không cần thiết

---

## 🎉 TÍNH NĂNG SẮP TỚI

### v1.3 (Coming soon):
- [ ] Odds Hiệp 2
- [ ] Corner (phạt góc)
- [ ] Cards (thẻ vàng/đỏ)
- [ ] Chart so sánh odds
- [ ] Alert khi odds thay đổi lớn

---

## 📚 API DATA FORMAT

### Odds Types trong API:

```javascript
Type 1 = Handicap Full Time
Type 3 = Over/Under Full Time  
Type 5 = 1X2 Full Time
Type 7 = Handicap Half 1
Type 8 = 1X2 Half 1
Type 9 = Over/Under Half 1
```

### Response format:
```javascript
[oddsId,[matchLiveId,type,subtype,amount,handicap],[homeOdds,awayOdds]]
```

---

## ✅ CHECKLIST

- [ ] Download SportsOddsApp_H1_Feature.zip
- [ ] Giải nén và build (0 errors)
- [ ] Run app (F5)
- [ ] Load dữ liệu (auto hoặc manual refresh)
- [ ] Scroll ngang xem các cột Hiệp 1
- [ ] Test với cookie mới (nếu cần)
- [ ] Verify data Hiệp 1 hiển thị đúng

---

## 🚀 PUSH LÊN GITHUB

Nếu muốn push version mới:

```bash
cd C:\Projects\SportsOddsApp
git add .
git commit -m "feat: Add Half 1 (H1) odds display

- Added H1 handicap, over/under, 1X2 columns
- Parse Type 7, 8, 9 from API
- Update UI with H1 sections
- Format similar to table 2 in reference image"

git push origin main
```

---

## 📖 TÀI LIỆU

- **README.md** - Overview
- **QUICK_START.md** - Installation
- **COOKIE_FEATURE.md** - Cookie input guide
- **H1_FEATURE_UPDATE.md** - This file! (Hiệp 1 guide)

---

## 💬 FEEDBACK

Nếu có vấn đề hoặc đề xuất:
1. Check console logs (Output window)
2. Verify API response format
3. Test với trận khác
4. Report issue với screenshot

---

**Version**: 1.2.0  
**Date**: December 17, 2025  
**Status**: ✅ Ready with Half 1 Features  

🎉 **Enjoy watching Hiệp 1 odds!** 🎉
