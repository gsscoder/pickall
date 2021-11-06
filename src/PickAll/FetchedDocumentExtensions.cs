using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace PickAll
{
    public static class FetchedDocumentExtensions
    {
        public static string ElementSelector(this IFetchedDocument document, string tag)
        {
            Guard.AgainstNull(nameof(document), document);

            return document.ElementSelectorAll(tag).SingleOrDefault() ?? string.Empty;
        }

        public static IEnumerable<string> ElementSelectorAll(this IFetchedDocument document, string tag)
        {
            Guard.AgainstNull(nameof(document), document);
            Guard.AgainstNull(nameof(tag), tag);
            Guard.AgainstEmptyWhiteSpace(nameof(tag), tag);

            var getElement = new Regex($@"(?<=<{tag}>)(.|\n)*?(?=<\/{tag}>)",
                RegexOptions.Compiled | RegexOptions.Multiline);
            var matches = getElement.Matches(document.Text());
            var contents = new List<string>();
            foreach (Match match in matches) {
                contents.Add(match.Value);
            }
            return contents;
        }
    }
}
