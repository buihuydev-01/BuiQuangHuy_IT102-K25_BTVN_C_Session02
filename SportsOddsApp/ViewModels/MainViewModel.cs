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
            
            RefreshCommand = new RelayCommand(async () => await LoadMatchesAsync());
            StartAutoRefreshCommand = new RelayCommand(StartAutoRefresh);
            StopAutoRefreshCommand = new RelayCommand(StopAutoRefresh);

            _refreshTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(10) // Refresh every 10 seconds
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

                var matches = await _apiService.GetTodayMatchesAsync();

                // Update on UI thread
                System.Windows.Application.Current?.Dispatcher.Invoke(() =>
                {
                    // Update existing matches or add new ones
                    foreach (var match in matches)
                    {
                        var existingMatch = Matches.FirstOrDefault(m => m.MatchId == match.MatchId);
                        if (existingMatch != null)
                        {
                            // Update existing match
                            UpdateMatch(existingMatch, match);
                        }
                        else
                        {
                            // Add new match
                            Matches.Add(match);
                        }
                    }

                    // Remove matches that are no longer in the data
                    var matchIds = matches.Select(m => m.MatchId).ToHashSet();
                    var toRemove = Matches.Where(m => !matchIds.Contains(m.MatchId)).ToList();
                    foreach (var match in toRemove)
                    {
                        Matches.Remove(match);
                    }

                    // Sort by time
                    var sorted = Matches.OrderBy(m => m.MatchTime).ToList();
                    Matches.Clear();
                    foreach (var match in sorted)
                    {
                        Matches.Add(match);
                    }
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

        private void UpdateMatch(Match existing, Match updated)
        {
            existing.Time = updated.Time;
            existing.HomeTeam = updated.HomeTeam;
            existing.AwayTeam = updated.AwayTeam;
            existing.Score = updated.Score;
            existing.HdpHome = updated.HdpHome;
            existing.HdpAway = updated.HdpAway;
            existing.OuHome = updated.OuHome;
            existing.OuAway = updated.OuAway;
            existing.Odds1 = updated.Odds1;
            existing.OddsX = updated.OddsX;
            existing.Odds2 = updated.Odds2;
            existing.Status = updated.Status;
        }

        private void StartAutoRefresh()
        {
            _refreshTimer.Start();
            StatusMessage = "Đã bật tự động cập nhật (10 giây/lần)";
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
