using System.Text;
using System.Linq;
using Xunit;
using FluentAssertions;
using PickAll;

public class FetchedDocumentExtensionsSpecs
{
    [Fact]
    public void Should_extract_single_tag()
    {
        var body = WaffleBuilder.GenerateParagraph();
        var html = WaffleBuilder.GeneratePageAsString(body);

        var sut = new FetchedDocument(Encoding.UTF8.GetBytes(html));

        var content = sut.ElementSelector("body");
        
        content.Trim().Should().NotBeNull()
            .And.Be(body.Trim());
    }

    [Fact]
    public void Should_extract_multiple_tags()
    {
        var body = WaffleBuilder.GenerateParagraph(samples: 3, small: true);
        var html = WaffleBuilder.GeneratePageAsString(body);

        var sut = new FetchedDocument(Encoding.UTF8.GetBytes(html));

        var contents = sut.ElementSelectorAll("p");

        contents.Should().NotBeNullOrEmpty()
            .And.HaveCount(3);

    }
}