using System.Diagnostics.Contracts;
using System.Linq;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace PickAll
{
    static class IHtmlElementExtensions
    {
        public static string FirstChildText(this IHtmlElement element,
            params string[] selectorsGroup)
        {
            Contract.Ensures(selectorsGroup != null && selectorsGroup.Length > 0);

            foreach (var selectors in selectorsGroup) {
                var selected = element.QuerySelector(selectors);
                if (selected != null) {
                    if (selected.ChildNodes.Count() > 0) {
                        return selected.FirstChild.Text();
                    }
                }
            }
            return string.Empty;
        }
    }
}