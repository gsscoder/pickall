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
            public string ProfileID;

            public string ImageUrl;

            public override string ToString()
            {
                var builder = new StringBuilder();
                if (!string.IsNullOrEmpty(ProfileID)) {
                    builder.Append("{ProfileID: ");
                    builder.Append(ProfileID);
                }
                if (builder.Length > 0) {
                    builder.Append(", ");
                }
                if (!string.IsNullOrEmpty(ImageUrl)) {
                    builder.Append("ImageUrl: ");
                    builder.Append(ImageUrl);
                }
                if (builder.Length > 0) {
                    builder.Append("}");
                }
                return builder.ToString();
            }
        }

        private static readonly string _baseUrl = "https://m.facebook.com";
        private static readonly Regex _profileId = new Regex(@"(?<=/).+?(?=\?)", RegexOptions.Compiled);
        private readonly Options _options;

        public Facebook(Options options) : base()  
        {
            _options = options;
        }

        public Facebook() : this(new Options { RetrieveImageLink = false })
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
                    Data data = null;
                    Action initData = () => { if (data == null) data = new Data(); };
                    if (_options.RetrieveImageLink)
                    {
                        initData();
                        data.ImageUrl = table.QuerySelector<IHtmlImageElement>("td.bo.bp img")
                            .Attributes["src"].Value;
                    }
                    if (_options.RetrieveProfileID)
                    {
                        initData();
                        data.ProfileID = GetProfileID(link);
                    }
                    results.Add(CreateResult(index, $"{_baseUrl}{link}", description, data));
                    index++;
                }
                return results;
            }
        }

        private static string GetProfileID(string url)
        {
            var match = _profileId.Match(url);
            return match.Groups.Count == 1 ? match.Groups[0].Value : url;
        }
    }
}