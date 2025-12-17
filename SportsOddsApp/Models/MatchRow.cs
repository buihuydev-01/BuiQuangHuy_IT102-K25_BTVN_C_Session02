using System.ComponentModel;

namespace SportsOddsApp.Models
{
    /// <summary>
    /// Represents a single row in DataGrid (can be main match or odds level)
    /// </summary>
    public class MatchRow : INotifyPropertyChanged
    {
        private string _time;
        private string _leagueName;
        private string _matchDisplay;
        private string _score;
        private bool _isMainRow;
        
        // Match info (for main row)
        public int MatchId { get; set; }
        public string Time
        {
            get => _time;
            set { _time = value; OnPropertyChanged(nameof(Time)); }
        }
        
        public string LeagueName
        {
            get => _leagueName;
            set { _leagueName = value; OnPropertyChanged(nameof(LeagueName)); }
        }
        
        public string MatchDisplay
        {
            get => _matchDisplay;
            set { _matchDisplay = value; OnPropertyChanged(nameof(MatchDisplay)); }
        }
        
        public string Score
        {
            get => _score;
            set { _score = value; OnPropertyChanged(nameof(Score)); }
        }
        
        // Is this the main row or a sub-row (kèo khác)?
        public bool IsMainRow
        {
            get => _isMainRow;
            set { _isMainRow = value; OnPropertyChanged(nameof(IsMainRow)); }
        }
        
        // Display prefix for sub-rows
        public string RowPrefix => IsMainRow ? "" : "└ Kèo khác";
        
        // Odds data (for all rows)
        public string HdpLine { get; set; }
        public string HdpHome { get; set; }
        public string HdpAway { get; set; }
        
        public string OuLine { get; set; }
        public string OuHome { get; set; }
        public string OuAway { get; set; }
        
        public string Odds1 { get; set; }
        public string OddsX { get; set; }
        public string Odds2 { get; set; }
        
        // H1 Odds
        public string HdpH1Line { get; set; }
        public string HdpH1Home { get; set; }
        public string HdpH1Away { get; set; }
        
        public string OuH1Line { get; set; }
        public string OuH1Home { get; set; }
        public string OuH1Away { get; set; }
        
        public string Odds1H1 { get; set; }
        public string OddsXH1 { get; set; }
        public string Odds2H1 { get; set; }
        
        // Amount/Level indicator
        public string Amount { get; set; }
        
        // Status
        public string Status { get; set; }
        
        // Formatting helpers
        public string HdpDisplay => string.IsNullOrEmpty(HdpHome) ? "-" : $"{HdpHome} | {HdpAway}";
        public string OuDisplay => string.IsNullOrEmpty(OuHome) ? "-" : $"{OuHome} | {OuAway}";
        public string X12Display => string.IsNullOrEmpty(Odds1) ? "-" : $"{Odds1} | {OddsX} | {Odds2}";
        
        public string HdpH1Display => string.IsNullOrEmpty(HdpH1Home) ? "-" : $"{HdpH1Home} | {HdpH1Away}";
        public string OuH1Display => string.IsNullOrEmpty(OuH1Home) ? "-" : $"{OuH1Home} | {OuH1Away}";
        public string X12H1Display => string.IsNullOrEmpty(Odds1H1) ? "-" : $"{Odds1H1} | {OddsXH1} | {Odds2H1}";
        
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
