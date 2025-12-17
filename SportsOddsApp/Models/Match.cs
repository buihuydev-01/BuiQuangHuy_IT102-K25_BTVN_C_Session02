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

        // Handicap (Cược chấp)
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

        // Over/Under (Tài/Xỉu)
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

        // 1X2 Odds
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

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
