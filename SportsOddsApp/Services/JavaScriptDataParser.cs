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
                    
                    // Type 1 = Handicap Full Time - LẤY TẤT CẢ CÁC MỨC
                    var hdpOddsList = matchOdds
                        .Where(o => o.Type == 1)
                        .OrderByDescending(o => o.Amount) // Sắp xếp theo mức cược
                        .ToList();
                    
                    foreach (var odds in hdpOddsList)
                    {
                        match.HdpLevels.Add(new OddsLevel
                        {
                            Line = FormatHandicap(odds.Handicap),
                            HomeOdds = FormatOdds(odds.HomeOdds),
                            AwayOdds = FormatOdds(odds.AwayOdds),
                            Amount = odds.Amount.ToString("0")
                        });
                    }
                    
                    // Giữ backwards compatibility - lấy mức cao nhất cho old properties
                    var bestHdp = hdpOddsList.FirstOrDefault();
                    if (bestHdp != null)
                    {
                        match.HdpHome = FormatOdds(bestHdp.HomeOdds);
                        match.HdpAway = FormatOdds(bestHdp.AwayOdds);
                    }

                    // Type 3 = Over/Under Full Time - LẤY TẤT CẢ CÁC MỨC
                    var ouOddsList = matchOdds
                        .Where(o => o.Type == 3)
                        .OrderByDescending(o => o.Amount)
                        .ToList();
                    
                    foreach (var odds in ouOddsList)
                    {
                        match.OuLevels.Add(new OddsLevel
                        {
                            Line = FormatHandicap(odds.Handicap),
                            HomeOdds = FormatOdds(odds.HomeOdds),
                            AwayOdds = FormatOdds(odds.AwayOdds),
                            Amount = odds.Amount.ToString("0")
                        });
                    }
                    
                    var bestOu = ouOddsList.FirstOrDefault();
                    if (bestOu != null)
                    {
                        match.OuHome = FormatOdds(bestOu.HomeOdds);
                        match.OuAway = FormatOdds(bestOu.AwayOdds);
                    }

                    // Type 5 = 1X2 Full Time - Parse tất cả các mức
                    ParseAll1X2Odds(matchLiveId, rawData, 5, match.Odds1X2Levels);
                    
                    // Backwards compatibility
                    var best1X2 = match.Odds1X2Levels.FirstOrDefault();
                    if (best1X2 != null)
                    {
                        match.Odds1 = best1X2.Odds1;
                        match.OddsX = best1X2.OddsX;
                        match.Odds2 = best1X2.Odds2;
                    }

                    // ========== HALF 1 (Hiệp 1) ==========
                    
                    // Type 7 = Handicap Half 1 - LẤY TẤT CẢ CÁC MỨC
                    var hdpH1OddsList = matchOdds
                        .Where(o => o.Type == 7)
                        .OrderByDescending(o => o.Amount)
                        .ToList();
                    
                    foreach (var odds in hdpH1OddsList)
                    {
                        match.HdpH1Levels.Add(new OddsLevel
                        {
                            Line = FormatHandicap(odds.Handicap),
                            HomeOdds = FormatOdds(odds.HomeOdds),
                            AwayOdds = FormatOdds(odds.AwayOdds),
                            Amount = odds.Amount.ToString("0")
                        });
                    }
                    
                    var bestHdpH1 = hdpH1OddsList.FirstOrDefault();
                    if (bestHdpH1 != null)
                    {
                        match.HdpH1Home = FormatOdds(bestHdpH1.HomeOdds);
                        match.HdpH1Away = FormatOdds(bestHdpH1.AwayOdds);
                        match.HdpH1Line = FormatHandicap(bestHdpH1.Handicap);
                    }

                    // Type 9 = Over/Under Half 1 - LẤY TẤT CẢ CÁC MỨC
                    var ouH1OddsList = matchOdds
                        .Where(o => o.Type == 9)
                        .OrderByDescending(o => o.Amount)
                        .ToList();
                    
                    foreach (var odds in ouH1OddsList)
                    {
                        match.OuH1Levels.Add(new OddsLevel
                        {
                            Line = FormatHandicap(odds.Handicap),
                            HomeOdds = FormatOdds(odds.HomeOdds),
                            AwayOdds = FormatOdds(odds.AwayOdds),
                            Amount = odds.Amount.ToString("0")
                        });
                    }
                    
                    var bestOuH1 = ouH1OddsList.FirstOrDefault();
                    if (bestOuH1 != null)
                    {
                        match.OuH1Home = FormatOdds(bestOuH1.HomeOdds);
                        match.OuH1Away = FormatOdds(bestOuH1.AwayOdds);
                        match.OuH1Line = FormatHandicap(bestOuH1.Handicap);
                    }

                    // Type 8 = 1X2 Half 1 - Parse tất cả các mức
                    ParseAll1X2Odds(matchLiveId, rawData, 8, match.Odds1X2H1Levels);
                    
                    var best1X2H1 = match.Odds1X2H1Levels.FirstOrDefault();
                    if (best1X2H1 != null)
                    {
                        match.Odds1H1 = best1X2H1.Odds1;
                        match.OddsXH1 = best1X2H1.OddsX;
                        match.Odds2H1 = best1X2H1.Odds2;
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

        /// <summary>
        /// Parse ALL 1X2 odds levels for a match
        /// </summary>
        private void ParseAll1X2Odds(int matchLiveId, string data, int type, System.Collections.ObjectModel.ObservableCollection<Odds1X2> collection)
        {
            try
            {
                // Pattern: [oddsId,[matchLiveId,type,subtype,amount,0],[val1,val2,val3]]
                var pattern = $@"\[(\d+),\[{matchLiveId},{type},\d+,([^,]*),0\],\[([^\]]+)\]\]";
                var matches = Regex.Matches(data, pattern);
                
                foreach (RegexMatch match in matches)
                {
                    try
                    {
                        var amount = match.Groups[2].Value;
                        var valuesStr = match.Groups[3].Value;
                        var values = valuesStr.Split(',');
                        
                        if (values.Length >= 3)
                        {
                            double val1 = 0, val2 = 0, val3 = 0;
                            
                            double.TryParse(values[0].Trim(), 
                                System.Globalization.NumberStyles.Any,
                                System.Globalization.CultureInfo.InvariantCulture,
                                out val1);
                            double.TryParse(values[1].Trim(), 
                                System.Globalization.NumberStyles.Any,
                                System.Globalization.CultureInfo.InvariantCulture,
                                out val2);
                            double.TryParse(values[2].Trim(), 
                                System.Globalization.NumberStyles.Any,
                                System.Globalization.CultureInfo.InvariantCulture,
                                out val3);
                            
                            collection.Add(new Odds1X2
                            {
                                Odds1 = FormatOdds(val1),
                                OddsX = FormatOdds(val2),
                                Odds2 = FormatOdds(val3),
                                Amount = amount
                            });
                        }
                    }
                    catch { }
                }
            }
            catch { }
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
