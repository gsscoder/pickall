using System.Linq;
using CSharpx;
using Xunit;
using FluentAssertions;
using PickAll.PostProcessors;
using PickAll.Tests.Fakes;

namespace PickAll.Tests.Unit
{
    public class ImproveTests
    {
        [Fact]
        public async void Should_exclude_non_alphanumeric_words()
        {
            var context = new SearchContext();
            await context.SearchAsync("query");

            var titles = WaffleHelper.Titles(3);

            var sut = new Improve(new ImproveSettings {
                WordCount = (ushort)titles.ToWords().Count()});
            sut.Context = context;

            var first = titles.First()
                .ApplyToWord(titles.First().RandomWordIndex(), word => word.Mangle())
                .ApplyToWord(titles.First().RandomWordIndex(word => word.IsAlphanumeric()), word => word.Mangle());
            var second = titles.ElementAt(1)
                .ApplyToWord(titles.ElementAt(1).RandomWordIndex(), word => word.Mangle());

            var fakeResults = new ResultInfo[] {
                ResultInfoHelper.OnlyDescription(first),
                ResultInfoHelper.OnlyDescription(second),
                ResultInfoHelper.OnlyDescription(titles.ElementAt(2))
            };

            sut.FoldDescriptions(fakeResults).Should().NotBeEmpty()
                .And.OnlyContain(word => word.IsAlphanumeric());
        }

        [Fact]
        public async void Should_fold_descriptions_excluding_query()
        {
            var context = new SearchContext();
            await context.SearchAsync("massive repetition");

            var sut = new Improve(new ImproveSettings {
                WordCount = 2});
            sut.Context = context;

            var titles = WaffleHelper.Titles(3, title => title
                    .BetweenWords("massive".Repeat(50))
                    .BetweenWords("something".Repeat(25))
                    .BetweenWords("repetition".Repeat(50))
                    .BetweenWords("hello".Repeat(25)));

            var fakeResults = new ResultInfo[] {
                ResultInfoHelper.OnlyDescription(titles.First()),
                ResultInfoHelper.OnlyDescription(titles.ElementAt(1)),
                ResultInfoHelper.OnlyDescription(titles.ElementAt(2))
            };

            sut.FoldDescriptions(fakeResults).Should().NotBeEmpty()
                .And.HaveCount(2)
                .And.BeEquivalentTo("something", "hello");
        }

        [Fact]
        public async void Should_fold_descriptions_excluding_query_and_noise()
        {
            var context = new SearchContext();
            await context.SearchAsync("massive repetition");

            var sut = new Improve(new ImproveSettings {
                WordCount = 2,
                NoiseLength = 3});
            sut.Context = context;

            var titles = WaffleHelper.Titles(3, title => title
                    .BetweenWords("massive".Repeat(50))
                    .BetweenWords("catch".Repeat(25))
                    .BetweenWords("a".Repeat(30))
                    .BetweenWords("repetition".Repeat(50))
                    .BetweenWords("word".Repeat(25))
                    .BetweenWords("of").Repeat(30));

            var fakeResults = new ResultInfo[] {
                ResultInfoHelper.OnlyDescription(titles.First()),
                ResultInfoHelper.OnlyDescription(titles.ElementAt(1)),
                ResultInfoHelper.OnlyDescription(titles.ElementAt(2))
            };

            sut.FoldDescriptions(fakeResults).Should().NotBeEmpty()
                .And.HaveCount(2)
                .And.BeEquivalentTo("catch", "word");
        }
    }
}