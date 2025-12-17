using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SportsOddsApp.Models;
using System.Linq;
using System.Globalization;

// Alias to avoid conflict between Models.Match and Regex.Match
using RegexMatch = System.Text.RegularExpressions.Match;

namespace SportsOddsApp.Services
{
    public class OddsApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _cookieString;
        private const string BaseUrl = "https://sports.wwyyuuvv22.com";
        private const string ApiUrl = "/web-root/restricted/odds-display/today-data.aspx";

        public OddsApiService() : this(null)
        {
        }

        public OddsApiService(string cookieString)
        {
            _cookieString = cookieString ?? GetDefaultCookie();
            
            var handler = new HttpClientHandler
            {
                UseCookies = true
            };

            _httpClient = new HttpClient(handler);
            SetupHeaders();
        }

        private string GetDefaultCookie()
        {
            return "ASP.NET_SessionId=thj4vns4z2mckcjz5rzaopqc; _hjSession_1325134=eyJpZCI6ImIzMDMzNTZhLTYxYWEtNDM2Ny04ZTVlLTQyYTM1OTUzYzMxMSIsImMiOjE3NjU5NTQ4MDkwNDUsInMiOjAsInIiOjAsInNiIjowLCJzciI6MCwic2UiOjAsImZzIjoxLCJzcCI6MH0=; _hjSessionUser_1325134=eyJpZCI6ImVjMWE4OTkwLTZhMDAtNTNjYi05MmM2LWEwMmE1Njg1YjNiNyIsImNyZWF0ZWQiOjE3NjU5NTQ4MDkwNDUsImV4aXN0aW5nIjp0cnVlfQ==; fullScreenAds=true; states=:1:1:::1:1:::::::::1765954828164:1765954828206:1765954828206:1765954828208:1765954828208";
        }

        private void SetupHeaders()
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("accept", "*/*");
            _httpClient.DefaultRequestHeaders.Add("accept-language", "vi-VN,vi;q=0.9,fr-FR;q=0.8,fr;q=0.7,en-US;q=0.6,en;q=0.5");
            _httpClient.DefaultRequestHeaders.Add("Cookie", _cookieString);
            _httpClient.DefaultRequestHeaders.Add("priority", "u=1, i");
            _httpClient.DefaultRequestHeaders.Add("referer", "https://sports.wwyyuuvv22.com/web-root/restricted/default.aspx?loginname=e58bcafe29635564953cc8958b63389a&lang=VI_VN&oddstyle=MY&theme=sbo&oddsmode=double&jd=jd&u=10028yy_v86h1831617sbd&in=0");
            _httpClient.DefaultRequestHeaders.Add("sec-ch-ua", "\"Google Chrome\";v=\"143\", \"Chromium\";v=\"143\", \"Not A(Brand\";v=\"24\"");
            _httpClient.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
            _httpClient.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
            _httpClient.DefaultRequestHeaders.Add("sec-fetch-dest", "empty");
            _httpClient.DefaultRequestHeaders.Add("sec-fetch-mode", "cors");
            _httpClient.DefaultRequestHeaders.Add("sec-fetch-site", "same-origin");
            _httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/143.0.0.0 Safari/537.36");
        }

        public async Task<List<Match>> GetTodayMatchesAsync()
        {
            try
            {
                string url = $"{BaseUrl}{ApiUrl}?od-param=3,1,1,1,1,2,1,2,0&fi=1&v=13474&dl=0";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();
                return ParseMatches(content);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching data: {ex.Message}");
                return new List<Match>();
            }
        }

        private List<Match> ParseMatches(string jsContent)
        {
            try
            {
                var parser = new JavaScriptDataParser();
                var parsedData = parser.Parse(jsContent);
                var matches = parser.ConvertToMatches(parsedData, jsContent);
                return matches.OrderBy(m => m.MatchTime).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing data: {ex.Message}");
                return new List<Match>();
            }
        }
    }
}
