using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using SportsOddsApp.Models;
using SportsOddsApp.Services;

namespace SportsOddsApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private OddsApiService _apiService;
        private readonly DispatcherTimer _refreshTimer;
        private bool _isLoading;
        private string _statusMessage;
        private DateTime _lastUpdate;
        private string _cookieString;

        public ObservableCollection<Match> Matches { get; set; }
        public ObservableCollection<MatchRow> MatchRows { get; set; }

        public string CookieString
        {
            get => _cookieString;
            set
            {
                _cookieString = value;
                OnPropertyChanged(nameof(CookieString));
                // Recreate API service with new cookie
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _apiService = new OddsApiService(value);
                    StatusMessage = "Cookie đã cập nhật";
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged(nameof(StatusMessage));
            }
        }

        public string LastUpdateText
        {
            get => $"Cập nhật lần cuối: {_lastUpdate:HH:mm:ss}";
        }

        public ICommand RefreshCommand { get; }
        public ICommand StartAutoRefreshCommand { get; }
        public ICommand StopAutoRefreshCommand { get; }

        public MainViewModel()
        {
            // Load cookie from config or use default
            _cookieString = LoadSavedCookie();
            _apiService = new OddsApiService(_cookieString);
            Matches = new ObservableCollection<Match>();
            MatchRows = new ObservableCollection<MatchRow>();
            
            RefreshCommand = new RelayCommand(async () => await LoadMatchesAsync());
            StartAutoRefreshCommand = new RelayCommand(StartAutoRefresh);
            StopAutoRefreshCommand = new RelayCommand(StopAutoRefresh);

            _refreshTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1) // Refresh every 1 second
            };
            _refreshTimer.Tick += async (s, e) => await LoadMatchesAsync();

            // Initial load
            Task.Run(async () => await LoadMatchesAsync());
        }

        private string LoadSavedCookie()
        {
            // Try to load from appsettings.json or return default
            try
            {
                var configPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
                if (System.IO.File.Exists(configPath))
                {
                    var json = System.IO.File.ReadAllText(configPath);
                    // Simple parse - you can use Newtonsoft.Json for better parsing
                    var match = System.Text.RegularExpressions.Regex.Match(json, @"""FullCookieString""\s*:\s*""([^""]*)""");
                    if (match.Success && !string.IsNullOrWhiteSpace(match.Groups[1].Value))
                    {
                        return match.Groups[1].Value;
                    }
                }
            }
            catch { }

            // Return default cookie
            return "ASP.NET_SessionId=thj4vns4z2mckcjz5rzaopqc; _hjSession_1325134=eyJpZCI6ImIzMDMzNTZhLTYxYWEtNDM2Ny04ZTVlLTQyYTM1OTUzYzMxMSIsImMiOjE3NjU5NTQ4MDkwNDUsInMiOjAsInIiOjAsInNiIjowLCJzciI6MCwic2UiOjAsImZzIjoxLCJzcCI6MH0=; fullScreenAds=true";
        }

        private async Task LoadMatchesAsync()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Đang tải dữ liệu...";

                List<Models.Match> matches = await _apiService.GetTodayMatchesAsync();

                // Update on UI thread - KHÔNG RESET dữ liệu
                System.Windows.Application.Current?.Dispatcher.Invoke(() =>
                {
                    // Update existing matches or add new ones
                    foreach (var match in matches)
                    {
                        var existingMatch = Matches.FirstOrDefault(m => m.MatchId == match.MatchId);
                        if (existingMatch != null)
                        {
                            // Update existing match (không thêm dòng mới)
                            UpdateMatch(existingMatch, match);
                        }
                        else
                        {
                            // Add new match only if not exists
                            Matches.Add(match);
                        }
                    }

                    // KHÔNG XÓA matches cũ - giữ lại để theo dõi
                    // Sort by time
                    var sorted = Matches.OrderBy(m => m.MatchTime).ToList();
                    Matches.Clear();
                    foreach (var match in sorted)
                    {
                        Matches.Add(match);
                    }
                    
                    // Convert to MatchRows (mỗi mức kèo = 1 row)
                    RebuildMatchRows();
                });

                _lastUpdate = DateTime.Now;
                OnPropertyChanged(nameof(LastUpdateText));
                StatusMessage = $"Đã tải {matches.Count} trận đấu";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Lỗi: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void RebuildMatchRows()
        {
            MatchRows.Clear();
            
            foreach (var match in Matches)
            {
                // Determine how many odds levels exist
                int maxLevels = Math.Max(
                    Math.Max(match.HdpH1Levels.Count, match.OuH1Levels.Count),
                    Math.Max(match.Odds1X2H1Levels.Count, 
                        Math.Max(match.HdpLevels.Count, 
                            Math.Max(match.OuLevels.Count, match.Odds1X2Levels.Count)))
                );
                
                // If no levels, create at least 1 row with legacy data
                if (maxLevels == 0)
                {
                    MatchRows.Add(CreateMainRow(match));
                    continue;
                }
                
                // Create main row + sub-rows for each level
                for (int i = 0; i < maxLevels; i++)
                {
                    var row = new MatchRow
                    {
                        MatchId = match.MatchId,
                        IsMainRow = (i == 0),
                        Time = (i == 0) ? match.Time : "",
                        LeagueName = (i == 0) ? match.LeagueName : "",
                        MatchDisplay = (i == 0) ? match.MatchDisplay : "",
                        Score = (i == 0) ? match.ScoreDisplay : "",
                        Status = (i == 0) ? match.StatusText : ""
                    };
                    
                    // Fill odds from collections
                    if (i < match.HdpLevels.Count)
                    {
                        row.HdpLine = match.HdpLevels[i].Line;
                        row.HdpHome = match.HdpLevels[i].HomeOdds;
                        row.HdpAway = match.HdpLevels[i].AwayOdds;
                        row.Amount = match.HdpLevels[i].Amount;
                    }
                    
                    if (i < match.OuLevels.Count)
                    {
                        row.OuLine = match.OuLevels[i].Line;
                        row.OuHome = match.OuLevels[i].HomeOdds;
                        row.OuAway = match.OuLevels[i].AwayOdds;
                    }
                    
                    if (i < match.Odds1X2Levels.Count)
                    {
                        row.Odds1 = match.Odds1X2Levels[i].Odds1;
                        row.OddsX = match.Odds1X2Levels[i].OddsX;
                        row.Odds2 = match.Odds1X2Levels[i].Odds2;
                    }
                    
                    // H1 Odds
                    if (i < match.HdpH1Levels.Count)
                    {
                        row.HdpH1Line = match.HdpH1Levels[i].Line;
                        row.HdpH1Home = match.HdpH1Levels[i].HomeOdds;
                        row.HdpH1Away = match.HdpH1Levels[i].AwayOdds;
                    }
                    
                    if (i < match.OuH1Levels.Count)
                    {
                        row.OuH1Line = match.OuH1Levels[i].Line;
                        row.OuH1Home = match.OuH1Levels[i].HomeOdds;
                        row.OuH1Away = match.OuH1Levels[i].AwayOdds;
                    }
                    
                    if (i < match.Odds1X2H1Levels.Count)
                    {
                        row.Odds1H1 = match.Odds1X2H1Levels[i].Odds1;
                        row.OddsXH1 = match.Odds1X2H1Levels[i].OddsX;
                        row.Odds2H1 = match.Odds1X2H1Levels[i].Odds2;
                    }
                    
                    MatchRows.Add(row);
                }
            }
        }
        
        private MatchRow CreateMainRow(Models.Match match)
        {
            return new MatchRow
            {
                MatchId = match.MatchId,
                IsMainRow = true,
                Time = match.Time,
                LeagueName = match.LeagueName,
                MatchDisplay = match.MatchDisplay,
                Score = match.ScoreDisplay,
                Status = match.StatusText,
                
                HdpLine = match.HdpH1Line,
                HdpHome = match.HdpHome,
                HdpAway = match.HdpAway,
                
                OuLine = match.OuHome,
                OuHome = match.OuHome,
                OuAway = match.OuAway,
                
                Odds1 = match.Odds1,
                OddsX = match.OddsX,
                Odds2 = match.Odds2,
                
                HdpH1Line = match.HdpH1Line,
                HdpH1Home = match.HdpH1Home,
                HdpH1Away = match.HdpH1Away,
                
                OuH1Line = match.OuH1Line,
                OuH1Home = match.OuH1Home,
                OuH1Away = match.OuH1Away,
                
                Odds1H1 = match.Odds1H1,
                OddsXH1 = match.OddsXH1,
                Odds2H1 = match.Odds2H1
            };
        }
        
        private void UpdateMatch(Models.Match existing, Models.Match updated)
        {
            existing.Time = updated.Time;
            existing.HomeTeam = updated.HomeTeam;
            existing.AwayTeam = updated.AwayTeam;
            existing.Score = updated.Score;  // Cập nhật tỷ số
            existing.HdpHome = updated.HdpHome;
            existing.HdpAway = updated.HdpAway;
            existing.OuHome = updated.OuHome;
            existing.OuAway = updated.OuAway;
            existing.Odds1 = updated.Odds1;
            existing.OddsX = updated.OddsX;
            existing.Odds2 = updated.Odds2;
            existing.Status = updated.Status;
            
            // Hiệp 1
            existing.HdpH1Home = updated.HdpH1Home;
            existing.HdpH1Away = updated.HdpH1Away;
            existing.HdpH1Line = updated.HdpH1Line;
            existing.OuH1Home = updated.OuH1Home;
            existing.OuH1Away = updated.OuH1Away;
            existing.OuH1Line = updated.OuH1Line;
            existing.Odds1H1 = updated.Odds1H1;
            existing.OddsXH1 = updated.OddsXH1;
            existing.Odds2H1 = updated.Odds2H1;
            
            // Trigger property changed cho MatchDisplay và ScoreDisplay
            existing.OnPropertyChanged(nameof(existing.MatchDisplay));
            existing.OnPropertyChanged(nameof(existing.ScoreDisplay));
        }

        private void StartAutoRefresh()
        {
            _refreshTimer.Start();
            StatusMessage = "Đã bật tự động cập nhật (1 giây/lần)";
        }

        private void StopAutoRefresh()
        {
            _refreshTimer.Stop();
            StatusMessage = "Đã tắt tự động cập nhật";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Simple RelayCommand implementation
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object parameter) => _execute();
    }
}
