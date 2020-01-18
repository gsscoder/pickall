using System.Linq;
using Xunit;
using FluentAssertions;
using CSharpx;

public class IDocumentExtensionsTests
{
    [Fact]
    public void Should_extract_sanitized_text_from_a_page()
    {
        var page = WaffleBuilder.GeneratePage(paragraphs: 3);

        var words = from word in page.TextSelectorAll(
            includeTitle: true, sanitizeText: true).FlattenOnce()
                    where word.Trim().Length > 0
                    select word;

        words.Should().NotBeEmpty()
            .And.OnlyContain(word => word.IsAlphanumeric());
    }
}