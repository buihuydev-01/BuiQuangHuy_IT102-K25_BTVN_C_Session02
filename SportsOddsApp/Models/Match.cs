using System;
using System.ComponentModel;

namespace SportsOddsApp.Models
{
    public class Match : INotifyPropertyChanged
    {
        private string _time;
        private string _homeTeam;
        private string _awayTeam;
        private string _score;
        private string _hdpHome;
        private string _hdpAway;
        private string _ouHome;
        private string _ouAway;
        private string _odds1;
        private string _oddsX;
        private string _odds2;
        private int _status;

        public int MatchId { get; set; }
        public int LeagueId { get; set; }
        public string LeagueName { get; set; }
        public DateTime MatchTime { get; set; }

        public string Time
        {
            get => _time;
            set { _time = value; OnPropertyChanged(nameof(Time)); }
        }

        public string HomeTeam
        {
            get => _homeTeam;
            set { _homeTeam = value; OnPropertyChanged(nameof(HomeTeam)); }
        }

        public string AwayTeam
        {
            get => _awayTeam;
            set { _awayTeam = value; OnPropertyChanged(nameof(AwayTeam)); }
        }

        public string Score
        {
            get => _score;
            set { _score = value; OnPropertyChanged(nameof(Score)); }
        }

        // Display format: "Team1 - Team2"
        public string MatchDisplay
        {
            get => $"{HomeTeam} - {AwayTeam}";
        }

        // Score display: "0 - 0" or "-"
        public string ScoreDisplay
        {
            get => string.IsNullOrEmpty(Score) ? "-" : Score;
        }

        // Handicap Full Time (Cược chấp Cả trận)
        public string HdpHome
        {
            get => _hdpHome;
            set { _hdpHome = value; OnPropertyChanged(nameof(HdpHome)); }
        }

        public string HdpAway
        {
            get => _hdpAway;
            set { _hdpAway = value; OnPropertyChanged(nameof(HdpAway)); }
        }

        // Over/Under Full Time (Tài/Xỉu Cả trận)
        public string OuHome
        {
            get => _ouHome;
            set { _ouHome = value; OnPropertyChanged(nameof(OuHome)); }
        }

        public string OuAway
        {
            get => _ouAway;
            set { _ouAway = value; OnPropertyChanged(nameof(OuAway)); }
        }

        // 1X2 Odds Full Time
        public string Odds1
        {
            get => _odds1;
            set { _odds1 = value; OnPropertyChanged(nameof(Odds1)); }
        }

        public string OddsX
        {
            get => _oddsX;
            set { _oddsX = value; OnPropertyChanged(nameof(OddsX)); }
        }

        public string Odds2
        {
            get => _odds2;
            set { _odds2 = value; OnPropertyChanged(nameof(Odds2)); }
        }

        // ============== HIỆP 1 (HALF 1) ==============
        
        // Handicap Half 1 (Cược chấp Hiệp 1)
        private string _hdpH1Home;
        private string _hdpH1Away;
        private string _hdpH1Line;  // Tỷ lệ chấp (vd: 0.25, -0.5)

        public string HdpH1Home
        {
            get => _hdpH1Home;
            set { _hdpH1Home = value; OnPropertyChanged(nameof(HdpH1Home)); }
        }

        public string HdpH1Away
        {
            get => _hdpH1Away;
            set { _hdpH1Away = value; OnPropertyChanged(nameof(HdpH1Away)); }
        }

        public string HdpH1Line
        {
            get => _hdpH1Line;
            set { _hdpH1Line = value; OnPropertyChanged(nameof(HdpH1Line)); }
        }

        // Over/Under Half 1 (Tài/Xỉu Hiệp 1)
        private string _ouH1Home;
        private string _ouH1Away;
        private string _ouH1Line;  // Line (vd: 1.5, 2.0)

        public string OuH1Home
        {
            get => _ouH1Home;
            set { _ouH1Home = value; OnPropertyChanged(nameof(OuH1Home)); }
        }

        public string OuH1Away
        {
            get => _ouH1Away;
            set { _ouH1Away = value; OnPropertyChanged(nameof(OuH1Away)); }
        }

        public string OuH1Line
        {
            get => _ouH1Line;
            set { _ouH1Line = value; OnPropertyChanged(nameof(OuH1Line)); }
        }

        // 1X2 Half 1 (Hiệp 1)
        private string _odds1H1;
        private string _oddsXH1;
        private string _odds2H1;

        public string Odds1H1
        {
            get => _odds1H1;
            set { _odds1H1 = value; OnPropertyChanged(nameof(Odds1H1)); }
        }

        public string OddsXH1
        {
            get => _oddsXH1;
            set { _oddsXH1 = value; OnPropertyChanged(nameof(OddsXH1)); }
        }

        public string Odds2H1
        {
            get => _odds2H1;
            set { _odds2H1 = value; OnPropertyChanged(nameof(Odds2H1)); }
        }

        public int Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(nameof(Status)); }
        }

        public string StatusText
        {
            get
            {
                return Status switch
                {
                    0 => "Chưa bắt đầu",
                    1 => "Đang diễn ra",
                    2 => "Hiệp 1",
                    3 => "Hiệp 2",
                    8 => "Kết thúc",
                    _ => ""
                };
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
