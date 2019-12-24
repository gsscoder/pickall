using Xunit;
using FluentAssertions;
using PickAll.PostProcessors;

namespace PickAll.Tests.Unit
{
    public class ImproveTests
    {
        [Fact]
        public async void Should_exclude_non_alphanumeric_words()
        {
            var context = new SearchContext();
            await context.SearchAsync("query");

            var sut = new Improve(new ImproveSettings {
                WordCount = 2});
            sut.Context = context;

            var fakeResults = new ResultInfo[] {
                ResultInfoHelper.OnlyDescription("hello from tests"),
                ResultInfoHelper.OnlyDescription("@ll ok"),
                ResultInfoHelper.OnlyDescription(".this excluded")
            };

            sut.FoldDescriptions(fakeResults).Should().NotBeEmpty()
                .And.HaveCount(2)
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

            var fakeResults = new ResultInfo[] {
                ResultInfoHelper.OnlyDescription(
                    "this is a fake result " + "hello".Repeat(10) + "word".Repeat(3) +
                    "massive".Repeat(50) + "repetition".Repeat(50)),
                ResultInfoHelper.OnlyDescription(
                    "this too " + "ok".Repeat(8) + "something".Repeat(10) + 
                    "massive".Repeat(50) + "repetition".Repeat(50)),
                ResultInfoHelper.OnlyDescription(
                    "this also " + "hello".Repeat(5) + "something".Repeat(20) +
                    "massive".Repeat(50) + "repetition".Repeat(50))
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

            var fakeResults = new ResultInfo[] {
                ResultInfoHelper.OnlyDescription(
                    "this is " + "a".Repeat(100) + " fake result " +
                    "catch".Repeat(50) + " " + "word".Repeat(50) +
                    "massive".Repeat(100) + "repetition".Repeat(100)),
                ResultInfoHelper.OnlyDescription(
                    "this too " + "a".Repeat(100) + " description with noise " +
                    "massive".Repeat(100) + "repetition".Repeat(100)),
                ResultInfoHelper.OnlyDescription(
                    "the".Repeat(100) + " should be removed " +
                    "massive".Repeat(100) + "repetition".Repeat(100))
            };

            sut.FoldDescriptions(fakeResults).Should().NotBeEmpty()
                .And.HaveCount(2)
                .And.BeEquivalentTo("catch", "word");
        }
    }
}