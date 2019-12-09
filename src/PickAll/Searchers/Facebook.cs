using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using AngleSharp;
using AngleSharp.Html.Dom;
using AngleSharp.Dom;

namespace PickAll.Searchers
{
    /// <summary>
    /// <see cref="Searcher"> that searches on Google search engine.
    /// </summary>
    public class Facebook : Searcher
    {
        public class Options
        {
            public bool RetrieveProfileID;

            public bool RetrieveImageLink;
        }

        public class Data
        {
            public Data(string profileID, string imageUrl)
            {
                ProfileID = profileID;
                ImageUrl = imageUrl;
            }

            public string ProfileID { get; private set; }

            public string ImageUrl { get; private set; }

            public override string ToString()
            {
                var builder = new StringBuilder();
                if (!string.IsNullOrEmpty(ProfileID)) {
                    builder.Append("ProfileID: ");
                    builder.Append(ProfileID);
                }
                if (!string.IsNullOrEmpty(ImageUrl)) {
                    if (builder.Length > 0) {
                        builder.Append(", ");
                    }
                    builder.Append("ImageUrl: ");
                    builder.Append(ImageUrl);
                }
                if (builder.Length > 0) {
                    builder.Insert(0, '{');
                    builder.Append('}');
                }
                return builder.ToString();
            }
        }

        private static readonly string _baseUrl = "https://m.facebook.com";
        private static readonly Regex[] _profileId = {
            new Regex(@"(?<=\?id=).+?(?=$|\?|&)", RegexOptions.Compiled),
            new Regex(@"(?<=/).*?(?=$|/|\?|&)", RegexOptions.Compiled)};
        private readonly Options _options;

        public Facebook(Options options) : base()  
        {
            _options = options;
        }

        public Facebook() : this(new Options())
        {
        }

        public override async Task<IEnumerable<ResultInfo>> SearchAsync(string query)
        {
            var formatted = string.Join("+", query.Split());
            using (var document = await Context.OpenAsync($"{_baseUrl}/public/{formatted}")) {
                // Results are list of tables
                var tables = document.QuerySelectorAll<IHtmlTableElement>(
                    "div#BrowseResultsContainer table");
                var results = new List<ResultInfo>();
                ushort index = 0;
                foreach (var table in tables) {
                    var link =table.QuerySelector<IHtmlAnchorElement>("td.bt.bu a")
                        .Attributes["href"].Value;
                    var description = table.QuerySelector<IHtmlDivElement>("div.bw").Text();
                    String profileID = null;
                    String smallImageUrl = null;
                    if (_options.RetrieveProfileID) {
                        profileID = GetProfileID(link);
                    }
                    if (_options.RetrieveImageLink) {         
                        smallImageUrl = table.QuerySelector<IHtmlImageElement>("td.bo.bp img")
                            .Attributes["src"].Value;
                    }
                    results.Add(CreateResult(index, $"{_baseUrl}{link}", description,
                        new Data(profileID, smallImageUrl)));
                    index++;
                }
                return results;
            }
        }

        private static string GetProfileID(string url)
        {
            foreach (var regEx in _profileId) {
                var match = regEx.Match(url);
                if (match.Length > 0) {
                    return match.Value;
                }
            }
            return string.Empty;
        }
    }
}