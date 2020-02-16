using System.Linq;
using Xunit;
using FluentAssertions;
using CSharpx;
using PickAll;

public class ImproveSpecs
{
    [Fact]
    public async void Should_exclude_non_alphanumeric_words()
    {
        var context = new SearchContext();
        await context.SearchAsync("query");

        var titles = WaffleBuilder.GenerateTitle(3);

        var sut = new Improve(new ImproveSettings
            {
                WordCount = (ushort)titles.FlattenOnce().Count()
            });
        sut.Context = context;

        var first = titles.First()
            .ApplyAt(titles.First().ChoiceOfIndex(), word => word.Mangle())
            .ApplyAt(titles.First().ChoiceOfIndex(word => word.IsAlphanumeric()), word => word.Mangle());
        var second = titles.ElementAt(1)
            .ApplyAt(titles.ElementAt(1).ChoiceOfIndex(), word => word.Mangle());

        var results = new ResultInfo[] {
            ResultInfoHelper.OnlyDescription(first),
            ResultInfoHelper.OnlyDescription(second),
            ResultInfoHelper.OnlyDescription(titles.ElementAt(2))
        };

        sut.FoldDescriptions(results).Should().NotBeNullOrEmpty()
            .And.OnlyContain(word => word.IsAlphanumeric());
    }

    [Fact]
    public async void Should_fold_descriptions_excluding_query()
    {
        var context = new SearchContext()
            .With<Improve>(new ImproveSettings
                {
                    WordCount = 2
                });
        await context.SearchAsync("massive repetition");

        var sut = (Improve)context.Services.First();
        sut.Context = context;

        var titles = WaffleBuilder.GenerateTitle(3, title => title
                .Intersperse("massive".Replicate(50))
                .Intersperse("something".Replicate(25))
                .Intersperse("repetition".Replicate(50))
                .Intersperse("hello".Replicate(25)));

        var results = new ResultInfo[] {
            ResultInfoHelper.OnlyDescription(titles.First()),
            ResultInfoHelper.OnlyDescription(titles.ElementAt(1)),
            ResultInfoHelper.OnlyDescription(titles.ElementAt(2))
        };

        sut.FoldDescriptions(results).Should().NotBeNullOrEmpty()
            .And.HaveCount(2)
            .And.BeEquivalentTo("something", "hello");
    }

    [Fact]
    public async void Should_fold_descriptions_excluding_query_and_noise()
    {
        var context = new SearchContext()
            .With<Improve>(new ImproveSettings
            {
                WordCount = 2,
                NoiseLength = 3
            });
        await context.SearchAsync("massive repetition");

        var sut = (Improve)context.Services.First();
        sut.Context = context;

        var titles = WaffleBuilder.GenerateTitle(3, title => title
                .Intersperse("massive".Replicate(50))
                .Intersperse("catch".Replicate(25))
                .Intersperse("a".Replicate(30))
                .Intersperse("repetition".Replicate(50))
                .Intersperse("word".Replicate(25))
                .Intersperse("of").Replicate(30));

        var results = new ResultInfo[] {
            ResultInfoHelper.OnlyDescription(titles.First()),
            ResultInfoHelper.OnlyDescription(titles.ElementAt(1)),
            ResultInfoHelper.OnlyDescription(titles.ElementAt(2))
        };

        sut.FoldDescriptions(results).Should().NotBeNullOrEmpty()
            .And.HaveCount(2)
            .And.BeEquivalentTo("catch", "word");
    }
}