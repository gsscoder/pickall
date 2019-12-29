using System;
using System.Collections.Generic;
using System.Linq;
using WaffleGenerator;
using AngleSharp;
using AngleSharp.Dom;

namespace PickAll.Tests.Fakes
{
    static class ResultInfoGenerator
    {
        public static IEnumerable<ResultInfo> Generate(string originator, ushort samples)
        {
            for (ushort index = 0; index <= samples - 1; index++) {
                yield return new ResultInfo(
                    originator, index, WaffleHelper.Link(), WaffleEngine.Title(), null);
            } 
        }

        public static IEnumerable<ResultInfo> GenerateUnique(string originator, ushort samples)
        {
            var generated = new List<ResultInfo>();
            for (ushort index = 0; index <= samples - 1; index++) {
                var url = WaffleHelper.Link();
                var description = WaffleEngine.Title();
                var searched = from @this in generated
                               where @this.Url == url || @this.Description == description
                               select @this;
                if (searched.Count() == 0) {
                    var result = new ResultInfo(
                        originator, index, url, description, null);
                    generated.Add(result);
                }
                else {
                    index--;
                }
            }
            return generated;
        }
    }

    static class WaffleHelper
    {
        public static IEnumerable<string> Titles(int times, Func<string, string> modifier = null)
        {
            Func<string, string> _nullModifier = @string => @string;
            var _modifier = modifier ?? _nullModifier;

            for (var i = 0; i < times; i++) {
                var title = WaffleEngine.Title();
                yield return _modifier(title);
            }
        }

        public static string Link()
        {
            return new UrlEngine().Build(false, new Random().Next(0, 3));
        }

        public static IDocument Page(int paragraphs = 1)
        {
            var context = BrowsingContext.New(Configuration.Default.WithDefaultLoader());
            return context.OpenAsync(request => request.Content(
                    WaffleEngine.Html(
                        paragraphs: paragraphs,
                        includeHeading: true,
                        includeHeadAndBody: true))).GetAwaiter().GetResult();
        } 
    }
}