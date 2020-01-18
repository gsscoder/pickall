using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Dom;
using CSharpx;

#if DEBUG
public
#endif
static class IDocumentExtensions
{
    public static IEnumerable<string> TextSelectorAll(
        this IDocument document, bool includeTitle, bool sanitizeText)
    {
        Func<string, string> nullSanitize = text => text;
        var sanitize = sanitizeText
            ? text => text.Sanitize(normalizeWhiteSpace: true)
            : nullSanitize;
        var result = new List<string>();
        if (includeTitle) {
            result.AddRange(document.Title.Split());
        }
        result.AddRange(TextFromDocument());
        return result.Distinct();

        IEnumerable<string> TextFromDocument()
        {
            var elements =  SelectElements(
                "div", "p", "h1", "h2","h3", "h4", "h5", "h6");
            foreach (var element in elements) {
                yield return sanitize(element.Text());
            }
        }

        IEnumerable<IElement> SelectElements(params string[] elements)
        {
            for (var i = 0; i < elements.Length; i++) {
                foreach (var element in document.QuerySelectorAll(elements[i])) {
                    yield return element;
                }
            }
        }
    }
}