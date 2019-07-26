using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace LyricTestBot.Services
{
    public class LyricParser : ILyricParser
    {
        public string Artist { get; set; }
        
        public string SongTitle { get; set; }
        
        public LyricParser(string artist, string title)
        {
            Artist = artist;
            SongTitle = title;
        }

        public async Task<string> GetLyricsAsync()
        {
            var parsedDoc = await ParseSettingsAsync();

            if (parsedDoc == null)
                return null;

            var result = parsedDoc
                .QuerySelectorAll("div")
                .Where(item => item.ClassName == "col-xs-12 col-lg-8 text-center");

            var sb = new StringBuilder();

            foreach (var item in result)
                sb.Append(item.TextContent);
            
            var resultStr = sb.ToString();

            try
            {
                var indexOfSteam = resultStr.IndexOf("if  (", StringComparison.Ordinal);
                resultStr = resultStr
                    .Remove(indexOfSteam)
                    .TrimStart()
                    .TrimEnd();

                resultStr = resultStr
                    .Substring(resultStr.LastIndexOf("\n\n\n", StringComparison.Ordinal))
                    .TrimStart();
            }
            catch (IndexOutOfRangeException e)
            {
                if (e.Source == null)
                    throw;
                Console.WriteLine($"Exception source: {e.Source}");
                Console.WriteLine($"{e.GetType().Name} is outside the bounds of the array");
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine($"Exception message: {e.Message}");
            }

            return resultStr;
        }
        

        private async Task<IHtmlDocument> ParseSettingsAsync()
        {
            var client = new HttpClient();
            var domParser = new HtmlParser();

            var response = await client.GetAsync($"https://www.azlyrics.com/lyrics/{Artist}/{SongTitle}.html");
            string source;

            if (response != null && response.StatusCode == HttpStatusCode.OK)
                source = await response.Content.ReadAsStringAsync();
            else
                return null;

            var document = await domParser.ParseDocumentAsync(source);

            return document;
        }
    }
}