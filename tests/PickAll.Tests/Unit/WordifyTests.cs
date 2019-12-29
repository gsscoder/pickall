using Xunit;
using FluentAssertions;
using AngleSharp.Dom;
using PickAll.PostProcessors;
using PickAll.Tests.Fakes;

namespace PickAll.Tests.Unit
{
    public class WordifyTests
    {
        [Fact]
        public void Should_extract_words_from_a_page()
        {
            IDocument page = WaffleBuilder.GeneratePage(paragraphs: 3);

            var sut = new Wordify(new WordifySettings());
            var words = sut.ExtractText(page);

            words.Should().NotBeEmpty()
                .And.OnlyContain(word => word.IsAlphanumeric());
        }

        [Fact]
        public void Should_extract_words_from_a_page_excluding_noise()
        {
            IDocument page = WaffleBuilder.GeneratePage(paragraphs: 3);

            var sut = new Wordify(new WordifySettings{NoiseLength = 2});
            var words = sut.ExtractText(page);

            words.Should().NotBeEmpty()
                .And.OnlyContain(word => word.IsAlphanumeric())
                .And.OnlyContain(word => word.Length > 2);
        }        
    }
}