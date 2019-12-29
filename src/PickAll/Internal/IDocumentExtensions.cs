using System.Collections.Generic;
using AngleSharp.Dom;

namespace PickAll.Internal
{
    static class IDocumentExtensions
    {
        public static IEnumerable<IElement> SelectElements(this IDocument document,
            params string[] elements)
        {
            for (var i = 0; i < elements.Length; i++) {
                foreach (var element in document.QuerySelectorAll(elements[i])) {
                    yield return element;
                }
            }
        }
    }
}