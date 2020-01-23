using Xunit;
using FluentAssertions;
using CSharpx;
using PickAll;

public class WordifySpecs
{
    [Fact]
    public void Should_extract_words_from_a_page()
    {
        var page = WaffleBuilder.GeneratePage(paragraphs: 3);

        var sut = new Wordify(new WordifySettings());
        var words = sut.ExtractWords(page);

        words.Should().NotBeEmpty()
            .And.OnlyContain(word => word.IsAlphanumeric());
    }

    [Fact]
    public void Should_extract_words_from_a_page_excluding_noise()
    {
        var page = WaffleBuilder.GeneratePage(paragraphs: 3);

        var sut = new Wordify(new WordifySettings{NoiseLength = 2});
        var words = sut.ExtractWords(page);

        words.Should().NotBeEmpty()
            .And.OnlyContain(word => word.IsAlphanumeric())
            .And.OnlyContain(word => word.Length > 2);
    }        
}