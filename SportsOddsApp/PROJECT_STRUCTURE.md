# Project Structure - Cấu trúc dự án

## Tổng quan

Ứng dụng WPF C# theo mô hình **MVVM (Model-View-ViewModel)** để theo dõi tỷ lệ cược bóng đá.

```
SportsOddsApp/
│
├── Models/                      # Lớp dữ liệu
│   └── Match.cs                 # Model cho trận đấu
│
├── Services/                    # Business logic & API
│   ├── OddsApiService.cs        # Service call API
│   └── JavaScriptDataParser.cs  # Parser dữ liệu JS
│
├── ViewModels/                  # MVVM ViewModels
│   └── MainViewModel.cs         # ViewModel chính
│
├── Views/                       # UI (XAML)
│   ├── MainWindow.xaml          # Giao diện chính
│   └── MainWindow.xaml.cs       # Code-behind
│
├── App.xaml                     # Application config
├── App.xaml.cs                  # Application startup
│
├── appsettings.json             # Cấu hình
├── SportsOddsApp.csproj         # Project file
├── SportsOddsApp.sln            # Solution file
│
└── Docs/                        # Documentation
    ├── README.md
    ├── QUICK_START.md
    ├── HOW_TO_UPDATE_COOKIES.md
    └── PROJECT_STRUCTURE.md
```

## Chi tiết các thành phần

### 1. Models/

#### Match.cs
```csharp
public class Match : INotifyPropertyChanged
{
    // Properties
    - MatchId, LeagueId, LeagueName
    - HomeTeam, AwayTeam
    - MatchTime, Time
    - HdpHome, HdpAway      // Handicap odds
    - OuHome, OuAway        // Over/Under odds
    - Odds1, OddsX, Odds2   // 1X2 odds
    - Status, StatusText
    
    // Methods
    - OnPropertyChanged()   // INotifyPropertyChanged
}
```

**Vai trò**: Đại diện cho một trận đấu với đầy đủ thông tin và tỷ lệ cược.

**Data Binding**: Implements `INotifyPropertyChanged` để tự động cập nhật UI khi dữ liệu thay đổi.

---

### 2. Services/

#### OddsApiService.cs
```csharp
public class OddsApiService
{
    // Fields
    - HttpClient _httpClient
    - Const BaseUrl, ApiUrl
    
    // Methods
    - SetupHeaders()              // Setup HTTP headers & cookies
    - GetTodayMatchesAsync()      // Main API call
    - ParseMatches(jsContent)     // Parse JS response
}
```

**Vai trò**: 
- Gọi API để lấy dữ liệu
- Parse response JavaScript
- Trả về danh sách `Match` objects

**HTTP Request**:
```
GET https://sports.wwyyuuvv22.com/.../today-data.aspx
Headers:
  - Cookie: [Session cookies]
  - User-Agent: Chrome/143
  - Accept: */*
```

#### JavaScriptDataParser.cs
```csharp
public class JavaScriptDataParser
{
    // Classes
    - ParsedData { Leagues, Matches, Odds }
    - RawMatch { MatchId, Teams, Time, ... }
    - RawOdds { Type, Handicap, Odds, ... }
    
    // Methods
    - Parse(jsContent)           // Main parser
    - ParseLeagues(data)         // Extract leagues
    - ParseMatches(data)         // Extract matches
    - ParseOdds(data)            // Extract odds
    - CleanText(text)            // Clean Unicode
    - ConvertToMatches()         // Convert to Match objects
}
```

**Vai trò**: 
- Parse JavaScript response phức tạp
- Extract leagues, matches, odds
- Clean Unicode characters
- Map odds to matches

**Data Flow**:
```
JavaScript String 
  → ParsedData { leagues, matches, odds }
    → List<Match>
```

---

### 3. ViewModels/

#### MainViewModel.cs
```csharp
public class MainViewModel : INotifyPropertyChanged
{
    // Properties
    - ObservableCollection<Match> Matches
    - bool IsLoading
    - string StatusMessage
    - string LastUpdateText
    
    // Commands
    - ICommand RefreshCommand
    - ICommand StartAutoRefreshCommand
    - ICommand StopAutoRefreshCommand
    
    // Fields
    - OddsApiService _apiService
    - DispatcherTimer _refreshTimer
    
    // Methods
    - LoadMatchesAsync()         // Load data from API
    - UpdateMatch()              // Update existing match
    - StartAutoRefresh()         // Start timer
    - StopAutoRefresh()          // Stop timer
}
```

**Vai trò**:
- Quản lý state của ứng dụng
- Điều khiển auto-refresh
- Expose commands cho UI
- Update ObservableCollection

**MVVM Pattern**:
```
View (XAML) 
  ← Data Binding → 
ViewModel (MainViewModel)
  ← Uses → 
Model (Match) & Service (OddsApiService)
```

#### RelayCommand.cs
```csharp
public class RelayCommand : ICommand
{
    - Action _execute
    - Func<bool> _canExecute
    
    - Execute(parameter)
    - CanExecute(parameter)
}
```

**Vai trò**: Simple ICommand implementation cho XAML button bindings.

---

### 4. Views/

#### MainWindow.xaml
```xml
<Window>
  <Grid>
    <!-- Toolbar -->
    <Border> <!-- Buttons, Status --> </Border>
    
    <!-- DataGrid -->
    <DataGrid ItemsSource="{Binding Matches}">
      <DataGrid.Columns>
        - Time
        - League
        - Match (Home/Away)
        - Handicap Odds
        - 1X2 Odds
        - O/U Odds
        - Status
      </DataGrid.Columns>
    </DataGrid>
    
    <!-- Status Bar -->
    <Border> <!-- Count, Progress --> </Border>
  </Grid>
</Window>
```

**Vai trò**: 
- Define UI layout
- Data binding to ViewModel
- Styling và colors

**Key Bindings**:
```xml
ItemsSource="{Binding Matches}"
Command="{Binding RefreshCommand}"
Text="{Binding StatusMessage}"
```

#### MainWindow.xaml.cs
```csharp
public partial class MainWindow : Window
{
    - InitializeComponent()
}

// Converters
- OddsColorConverter         // Convert odds to colors
- BooleanToVisibilityConverter
```

**Vai trò**:
- Code-behind (minimal in MVVM)
- Value converters for binding

---

### 5. Application Files

#### App.xaml & App.xaml.cs
```csharp
public partial class App : Application
{
    - OnStartup()           // Enable TLS 1.2
}
```

**Vai trò**:
- Application entry point
- Global resources
- TLS configuration

#### SportsOddsApp.csproj
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <TargetFramework>net6.0-windows</TargetFramework>
  <UseWPF>true</UseWPF>
  <PackageReference Include="Newtonsoft.Json" />
</Project>
```

**Vai trò**: Project configuration, NuGet packages

---

## Data Flow Diagram

```
┌─────────────────────────────────────────────────┐
│  1. User clicks "Refresh" button               │
└─────────────────┬───────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────┐
│  2. MainViewModel.RefreshCommand                │
│     → LoadMatchesAsync()                        │
└─────────────────┬───────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────┐
│  3. OddsApiService.GetTodayMatchesAsync()       │
│     → HTTP GET to API                           │
│     → Response: JavaScript string               │
└─────────────────┬───────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────┐
│  4. JavaScriptDataParser.Parse()                │
│     → Extract leagues, matches, odds            │
│     → Clean Unicode text                        │
│     → ConvertToMatches()                        │
└─────────────────┬───────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────┐
│  5. Return List<Match>                          │
└─────────────────┬───────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────┐
│  6. MainViewModel updates                       │
│     ObservableCollection<Match>                 │
└─────────────────┬───────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────┐
│  7. UI automatically updates                    │
│     (via INotifyPropertyChanged)                │
└─────────────────────────────────────────────────┘
```

## Threading Model

```
UI Thread (Main Thread)
  ├── MainWindow
  ├── MainViewModel
  └── ObservableCollection updates
  
Background Thread (Task.Run)
  ├── HTTP Request (async)
  ├── Data parsing
  └── Dispatcher.Invoke → Update UI
  
Timer Thread (DispatcherTimer)
  └── Auto-refresh every 10s
```

## Key Design Patterns

### 1. MVVM (Model-View-ViewModel)
- **Model**: `Match.cs`
- **View**: `MainWindow.xaml`
- **ViewModel**: `MainViewModel.cs`

### 2. Service Layer
- `OddsApiService` - API communication
- `JavaScriptDataParser` - Data parsing

### 3. Command Pattern
- `RelayCommand` for button actions

### 4. Observer Pattern
- `INotifyPropertyChanged` for data binding
- `ObservableCollection` for list updates

### 5. Dependency Injection (Simple)
- ViewModel creates services
- Services injected via constructor

## Extension Points

### Thêm tính năng mới:

1. **Filter theo giải**:
   - Add property `SelectedLeagues` in ViewModel
   - Filter Matches before displaying

2. **Export Excel**:
   - Add new command `ExportCommand`
   - Use EPPlus/ClosedXML library

3. **Notifications**:
   - Add `NotificationService`
   - Compare odds changes
   - Show Windows toast

4. **Chart**:
   - Add `OddsHistoryService`
   - Store historical data
   - Use LiveCharts library

5. **Database**:
   - Add `DatabaseService`
   - Use Entity Framework Core
   - Store historical matches

## Performance Considerations

1. **ObservableCollection Updates**:
   - Update existing items instead of Clear/Add
   - Reduces UI flicker

2. **Async/Await**:
   - All API calls are async
   - Don't block UI thread

3. **Timer Interval**:
   - 10 seconds default
   - Adjustable based on needs

4. **Data Caching**:
   - Can add caching layer
   - Reduce API calls

## Security Notes

⚠️ **Cookies in Code**:
- Currently hardcoded
- Should be encrypted or in config
- User should update them

⚠️ **HTTPS**:
- TLS 1.2 enabled
- Certificate validation active

⚠️ **API Keys**:
- No API key required currently
- Only session cookies

## Testing Strategy

### Unit Tests:
- `JavaScriptDataParser` tests
- `Match` model tests
- Command tests

### Integration Tests:
- `OddsApiService` with mock HTTP
- ViewModel with mock service

### UI Tests:
- Manual testing (WPF automation complex)
- Screenshot comparison

## Build & Deployment

### Debug Build:
```bash
dotnet build -c Debug
```

### Release Build:
```bash
dotnet build -c Release
```

### Publish:
```bash
dotnet publish -c Release -r win-x64 --self-contained
```

### Output:
```
bin/Release/net6.0-windows/win-x64/publish/
  ├── SportsOddsApp.exe
  ├── SportsOddsApp.dll
  └── ... (dependencies)
```

## Maintenance

### Regular Updates:
1. Check cookies expiration
2. Verify API endpoint
3. Update parser if format changes
4. Test on new .NET versions

### Monitoring:
1. Log errors to file
2. Track API response times
3. Monitor memory usage

## Contributing

### Code Style:
- C# naming conventions
- 4 spaces indentation
- XML comments for public APIs

### Git Workflow:
```
main
  ├── feature/add-filter
  ├── feature/add-export
  └── bugfix/parse-error
```

---

## Summary

Đây là ứng dụng WPF well-structured với:
- ✅ Clean MVVM architecture
- ✅ Async/await for responsiveness
- ✅ Data binding for reactive UI
- ✅ Modular services
- ✅ Easy to extend

Perfect for learning WPF và real-time data monitoring! 🚀
