using System.Linq;
using Xunit;
using FluentAssertions;
using AngleSharp.Dom;
using PickAll.Internal;
using PickAll.Tests.Fakes;
using CSharpx;

namespace PickAll.Tests.Unit
{
    public class IDocumentExtensionsTests
    {
        [Fact]
        public void Should_extract_text_from_a_page()
        {
            IDocument page = WaffleBuilder.GeneratePage(paragraphs: 3);

            var words = from word in page.TextSelectorAll(includeTitle: true).FlattenOnce()
                        where word.Length > 0
                        select word;

            words.Should().NotBeEmpty()
                .And.OnlyContain(word => word.IsAlphanumeric());
        }
    }
}