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
            IDocument page = WaffleHelper.Page(paragraphs: 3);

            var sut = new Wordify(new WordifySettings());
            var words = sut.TextFromDocument(page);

            words.Should().NotBeEmpty()
                .And.OnlyContain(word => word.IsAlphanumeric());
        }
    }
}