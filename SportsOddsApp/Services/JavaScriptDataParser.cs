using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SportsOddsApp.Models;

// Alias to avoid conflict
using RegexMatch = System.Text.RegularExpressions.Match;

namespace SportsOddsApp.Services
{
    /// <summary>
    /// Advanced parser for JavaScript odds data
    /// </summary>
    public class JavaScriptDataParser
    {
        public class ParsedData
        {
            public Dictionary<int, string> Leagues { get; set; } = new();
            public List<RawMatch> Matches { get; set; } = new();
            public List<RawOdds> Odds { get; set; } = new();
            public Dictionary<int, int> MatchLiveIdToMatchId { get; set; } = new(); // Mapping matchLiveId → matchId
        }

        public class RawMatch
        {
            public int MatchId { get; set; }
            public int LeagueId { get; set; }
            public string HomeTeam { get; set; }
            public string AwayTeam { get; set; }
            public DateTime MatchTime { get; set; }
            public int Status { get; set; }
            public int HomeScore { get; set; }
            public int AwayScore { get; set; }
        }

        public class RawOdds
        {
            public int MatchLiveId { get; set; }
            public int Type { get; set; }
            public int SubType { get; set; }
            public double Handicap { get; set; }
            public double HomeOdds { get; set; }
            public double AwayOdds { get; set; }
        }

        public ParsedData Parse(string jsContent)
        {
            var result = new ParsedData();

            try
            {
                // Extract the main data array from $M('odds-display').onUpdate(3,[...]);
                var mainMatch = Regex.Match(jsContent, 
                    @"\$M\('odds-display'\)\.onUpdate\(3,\[(.*?)\]\);", 
                    RegexOptions.Singleline);

                if (!mainMatch.Success)
                {
                    Console.WriteLine("Could not find main data pattern");
                    return result;
                }

                string data = mainMatch.Groups[1].Value;

                // Parse leagues
                result.Leagues = ParseLeagues(data);

                // Parse matches
                result.Matches = ParseMatches(data);

                // Parse match mapping (matchLiveId → matchId)
                result.MatchLiveIdToMatchId = ParseMatchMapping(data);

                // Parse odds
                result.Odds = ParseOdds(data);

                Console.WriteLine($"Parsed: {result.Leagues.Count} leagues, {result.Matches.Count} matches, {result.Odds.Count} odds");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Parse error: {ex.Message}");
            }

            return result;
        }

        private Dictionary<int, string> ParseLeagues(string data)
        {
            var leagues = new Dictionary<int, string>();

            try
            {
                // Pattern: [[leagueId,'name','','']...]
                var leaguePattern = @"\[(\d+),'([^']*?)','[^']*','[^']*'\]";
                var matches = Regex.Matches(data, leaguePattern);

                foreach (RegexMatch match in matches)
                {
                    if (int.TryParse(match.Groups[1].Value, out int leagueId))
                    {
                        string name = CleanText(match.Groups[2].Value);
                        if (!string.IsNullOrEmpty(name) && name.Length > 3)
                        {
                            leagues[leagueId] = name;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"League parse error: {ex.Message}");
            }

            return leagues;
        }

        private List<RawMatch> ParseMatches(string data)
        {
            var matches = new List<RawMatch>();

            try
            {
                // Pattern: [matchId,type,leagueId,'homeTeam','awayTeam','code',mode,'datetime',status,...]
                var matchPattern = @"\[(\d+),\d+,(\d+),'([^']*?)','([^']*?)','[^']*',\d+,'(\d{2}/\d{2}/\d{4}\s+\d{2}:\d{2})',(\d+)";
                var matchResults = Regex.Matches(data, matchPattern);

                foreach (RegexMatch match in matchResults)
                {
                    try
                    {
                        var rawMatch = new RawMatch
                        {
                            MatchId = int.Parse(match.Groups[1].Value),
                            LeagueId = int.Parse(match.Groups[2].Value),
                            HomeTeam = CleanText(match.Groups[3].Value),
                            AwayTeam = CleanText(match.Groups[4].Value),
                            Status = 0,
                            HomeScore = 0,
                            AwayScore = 0
                        };

                        // Parse datetime: "12/17/2025 15:15"
                        string dateTimeStr = match.Groups[5].Value;
                        if (DateTime.TryParseExact(dateTimeStr, "MM/dd/yyyy HH:mm",
                            System.Globalization.CultureInfo.InvariantCulture,
                            System.Globalization.DateTimeStyles.None,
                            out DateTime matchTime))
                        {
                            rawMatch.MatchTime = matchTime;
                        }

                        // Parse status
                        if (match.Groups[6].Success)
                        {
                            int.TryParse(match.Groups[6].Value, out int status);
                            rawMatch.Status = status;
                        }

                        // Only add if teams are valid
                        if (!string.IsNullOrEmpty(rawMatch.HomeTeam) && 
                            !string.IsNullOrEmpty(rawMatch.AwayTeam))
                        {
                            matches.Add(rawMatch);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Match item parse error: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Matches parse error: {ex.Message}");
            }

            return matches;
        }

        private Dictionary<int, int> ParseMatchMapping(string data)
        {
            var mapping = new Dictionary<int, int>();

            try
            {
                // Pattern: [matchLiveId,matchId,x,x,x,x]
                // Example: [[113397115,9183877,0,0,0,19],...]
                var mappingPattern = @"\[(\d+),(\d+),\d+,\d+,\d+,\d+\]";
                var matches = Regex.Matches(data, mappingPattern);

                foreach (RegexMatch match in matches)
                {
                    try
                    {
                        int matchLiveId = int.Parse(match.Groups[1].Value);
                        int matchId = int.Parse(match.Groups[2].Value);
                        
                        if (!mapping.ContainsKey(matchLiveId))
                        {
                            mapping[matchLiveId] = matchId;
                        }
                    }
                    catch { }
                }

                Console.WriteLine($"Parsed {mapping.Count} match mappings");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Mapping parse error: {ex.Message}");
            }

            return mapping;
        }

        private List<RawOdds> ParseOdds(string data)
        {
            var oddsList = new List<RawOdds>();

            try
            {
                // Pattern: [oddsId,[matchLiveId,type,subtype,amount,handicap],[homeOdds,awayOdds]]
                var oddsPattern = @"\[\d+,\[(\d+),(\d+),(\d+),[^,]*,([^\]]*)\],\[([^,]*),([^\]]*)\]\]";
                var matches = Regex.Matches(data, oddsPattern);

                foreach (RegexMatch match in matches)
                {
                    try
                    {
                        var odds = new RawOdds
                        {
                            MatchLiveId = int.Parse(match.Groups[1].Value),
                            Type = int.Parse(match.Groups[2].Value),
                            SubType = int.Parse(match.Groups[3].Value)
                        };

                        // Parse handicap
                        string handicapStr = match.Groups[4].Value.Trim();
                        double handicapValue = 0;
                        double.TryParse(handicapStr, 
                            System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture,
                            out handicapValue);
                        odds.Handicap = handicapValue;

                        // Parse odds values
                        string homeStr = match.Groups[5].Value.Trim();
                        string awayStr = match.Groups[6].Value.Trim();

                        double homeValue = 0;
                        double.TryParse(homeStr,
                            System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture,
                            out homeValue);
                        odds.HomeOdds = homeValue;

                        double awayValue = 0;
                        double.TryParse(awayStr,
                            System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture,
                            out awayValue);
                        odds.AwayOdds = awayValue;

                        oddsList.Add(odds);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Odds item parse error: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Odds parse error: {ex.Message}");
            }

            return oddsList;
        }

        private string CleanText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            // Remove zero-width characters (Unicode \u200C)
            text = Regex.Replace(text, @"\u200C", "");
            text = Regex.Replace(text, @"[\u200B-\u200D\uFEFF]", "");

            // Decode \xXX patterns
            text = Regex.Replace(text, @"\\x([0-9A-Fa-f]{2})", m =>
            {
                try
                {
                    int value = Convert.ToInt32(m.Groups[1].Value, 16);
                    return ((char)value).ToString();
                }
                catch
                {
                    return m.Value;
                }
            });

            // Decode \uXXXX patterns
            text = Regex.Replace(text, @"\\u([0-9A-Fa-f]{4})", m =>
            {
                try
                {
                    int value = Convert.ToInt32(m.Groups[1].Value, 16);
                    return ((char)value).ToString();
                }
                catch
                {
                    return m.Value;
                }
            });

            return text.Trim();
        }

        /// <summary>
        /// Convert parsed data to Match objects
        /// </summary>
        public List<Match> ConvertToMatches(ParsedData parsedData, string rawData = "")
        {
            var matches = new List<Match>();

            // Create match objects
            foreach (var rawMatch in parsedData.Matches)
            {
                var match = new Match
                {
                    MatchId = rawMatch.MatchId,
                    LeagueId = rawMatch.LeagueId,
                    LeagueName = parsedData.Leagues.ContainsKey(rawMatch.LeagueId) 
                        ? parsedData.Leagues[rawMatch.LeagueId] 
                        : "Unknown",
                    HomeTeam = rawMatch.HomeTeam,
                    AwayTeam = rawMatch.AwayTeam,
                    MatchTime = rawMatch.MatchTime,
                    Time = rawMatch.MatchTime.ToString("HH:mm"),
                    Status = rawMatch.Status,
                    Score = (rawMatch.HomeScore > 0 || rawMatch.AwayScore > 0) 
                        ? $"{rawMatch.HomeScore} - {rawMatch.AwayScore}" 
                        : ""
                };

                matches.Add(match);
            }

            // Apply odds to matches
            ApplyOddsToMatches(matches, parsedData.Odds, parsedData.MatchLiveIdToMatchId, rawData);

            return matches;
        }

        private void ApplyOddsToMatches(List<Match> matches, List<RawOdds> oddsList, Dictionary<int, int> liveIdToMatchId, string rawData)
        {
            // Reverse mapping: matchId → matchLiveId
            var matchIdToLiveId = liveIdToMatchId.ToDictionary(x => x.Value, x => x.Key);

            foreach (var match in matches)
            {
                // Find matchLiveId for this match
                if (!matchIdToLiveId.TryGetValue(match.MatchId, out int matchLiveId))
                {
                    // Nếu không tìm thấy mapping, thử tìm gần đúng
                    continue;
                }

                // Get all odds for this match
                var matchOdds = oddsList.Where(o => o.MatchLiveId == matchLiveId).ToList();

                if (matchOdds.Any())
                {
                    // ========== FULL TIME (Cả trận) ==========
                    
                    // Type 1 = Handicap Full Time (lấy odds chính - thường là mức 1500 hoặc 2000)
                    var hdpOdds = matchOdds
                        .Where(o => o.Type == 1)
                        .OrderByDescending(o => Math.Abs(o.HomeOdds) + Math.Abs(o.AwayOdds))
                        .FirstOrDefault();
                    
                    if (hdpOdds != null)
                    {
                        match.HdpHome = FormatOdds(hdpOdds.HomeOdds);
                        match.HdpAway = FormatOdds(hdpOdds.AwayOdds);
                    }

                    // Type 3 = Over/Under Full Time
                    var ouOdds = matchOdds
                        .Where(o => o.Type == 3)
                        .OrderByDescending(o => Math.Abs(o.HomeOdds) + Math.Abs(o.AwayOdds))
                        .FirstOrDefault();
                    
                    if (ouOdds != null)
                    {
                        match.OuHome = FormatOdds(ouOdds.HomeOdds);
                        match.OuAway = FormatOdds(ouOdds.AwayOdds);
                    }

                    // Type 5 = 1X2 Full Time (có 3 giá trị trong 1 array)
                    var x12Odds = matchOdds.FirstOrDefault(o => o.Type == 5);
                    if (x12Odds != null)
                    {
                        // Parse from the odds data - cần parse lại từ raw data
                        var x12Values = Parse1X2Odds(matchLiveId, rawData, type: 5);
                        if (x12Values.Count >= 3)
                        {
                            match.Odds1 = x12Values[0];
                            match.OddsX = x12Values[1];
                            match.Odds2 = x12Values[2];
                        }
                    }

                    // ========== HALF 1 (Hiệp 1) ==========
                    
                    // Type 7 = Handicap Half 1
                    var hdpH1Odds = matchOdds
                        .Where(o => o.Type == 7)
                        .OrderByDescending(o => Math.Abs(o.HomeOdds) + Math.Abs(o.AwayOdds))
                        .FirstOrDefault();
                    
                    if (hdpH1Odds != null)
                    {
                        match.HdpH1Home = FormatOdds(hdpH1Odds.HomeOdds);
                        match.HdpH1Away = FormatOdds(hdpH1Odds.AwayOdds);
                        match.HdpH1Line = FormatHandicap(hdpH1Odds.Handicap);
                    }

                    // Type 9 = Over/Under Half 1
                    var ouH1Odds = matchOdds
                        .Where(o => o.Type == 9)
                        .OrderByDescending(o => Math.Abs(o.HomeOdds) + Math.Abs(o.AwayOdds))
                        .FirstOrDefault();
                    
                    if (ouH1Odds != null)
                    {
                        match.OuH1Home = FormatOdds(ouH1Odds.HomeOdds);
                        match.OuH1Away = FormatOdds(ouH1Odds.AwayOdds);
                        match.OuH1Line = FormatHandicap(ouH1Odds.Handicap);
                    }

                    // Type 8 = 1X2 Half 1
                    var x12H1Values = Parse1X2Odds(matchLiveId, rawData, type: 8);
                    if (x12H1Values.Count >= 3)
                    {
                        match.Odds1H1 = x12H1Values[0];
                        match.OddsXH1 = x12H1Values[1];
                        match.Odds2H1 = x12H1Values[2];
                    }
                }
            }
        }

        private List<string> Parse1X2Odds(int matchLiveId, string data, int type = 5)
        {
            var result = new List<string>();
            
            try
            {
                // Pattern: [oddsId,[matchLiveId,type,subtype,amount,0],[value1,value2,value3]]
                var pattern = $@"\[\d+,\[{matchLiveId},{type},\d+,[^,]*,0\],\[([^\]]+)\]\]";
                var match = Regex.Match(data, pattern);
                
                if (match.Success)
                {
                    var values = match.Groups[1].Value.Split(',');
                    foreach (var val in values)
                    {
                        if (double.TryParse(val.Trim(), 
                            System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture,
                            out double oddsVal))
                        {
                            result.Add(FormatOdds(oddsVal));
                        }
                    }
                }
            }
            catch { }

            return result;
        }

        private string FormatHandicap(double handicap)
        {
            if (Math.Abs(handicap) < 0.01)
                return "0";

            return handicap > 0 ? $"+{handicap:F2}" : $"{handicap:F2}";
        }

        private string FormatOdds(double odds)
        {
            if (Math.Abs(odds) < 0.01)
                return "-";

            return odds > 0 ? $"+{odds:F2}" : $"{odds:F2}";
        }
    }
}
