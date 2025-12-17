using System.ComponentModel;

namespace SportsOddsApp.Models
{
    /// <summary>
    /// Represents a single odds level (e.g., 500, 1000, 2000)
    /// </summary>
    public class OddsLevel : INotifyPropertyChanged
    {
        private string _line;
        private string _homeOdds;
        private string _awayOdds;
        private string _amount;

        /// <summary>
        /// Handicap line or O/U line (e.g., "0.50", "-0.25", "2.5")
        /// </summary>
        public string Line
        {
            get => _line;
            set { _line = value; OnPropertyChanged(nameof(Line)); }
        }

        /// <summary>
        /// Home odds value
        /// </summary>
        public string HomeOdds
        {
            get => _homeOdds;
            set { _homeOdds = value; OnPropertyChanged(nameof(HomeOdds)); }
        }

        /// <summary>
        /// Away odds value
        /// </summary>
        public string AwayOdds
        {
            get => _awayOdds;
            set { _awayOdds = value; OnPropertyChanged(nameof(AwayOdds)); }
        }

        /// <summary>
        /// Betting amount (e.g., "500", "1000", "2000")
        /// </summary>
        public string Amount
        {
            get => _amount;
            set { _amount = value; OnPropertyChanged(nameof(Amount)); }
        }

        /// <summary>
        /// Display format: "Line (Home/Away)"
        /// Example: "0.50 (0.85/0.95)"
        /// </summary>
        public string Display
        {
            get
            {
                if (!string.IsNullOrEmpty(Line))
                {
                    return $"{Line} ({HomeOdds}/{AwayOdds})";
                }
                return $"{HomeOdds}/{AwayOdds}";
            }
        }

        /// <summary>
        /// Color code based on odds value (for highlighting)
        /// </summary>
        public string OddsColor
        {
            get
            {
                // Parse home odds and determine color
                if (double.TryParse(HomeOdds, out double val))
                {
                    if (val > 0.9) return "#27AE60"; // Green for high odds
                    if (val < -0.9) return "#E74C3C"; // Red for negative odds
                }
                return "#34495E"; // Default gray
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Represents 1X2 odds (3 values)
    /// </summary>
    public class Odds1X2 : INotifyPropertyChanged
    {
        private string _odds1;
        private string _oddsX;
        private string _odds2;
        private string _amount;

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

        public string Amount
        {
            get => _amount;
            set { _amount = value; OnPropertyChanged(nameof(Amount)); }
        }

        public string Display
        {
            get => $"{Odds1} / {OddsX} / {Odds2}";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
