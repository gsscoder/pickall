using System;
using System.Linq;
using Xunit;
using FluentAssertions;
using CSharpx;
using PickAll;

public partial class SearchContextSpecs
{
    [Fact]
    public void Can_initialize_SearchContext_using_types()
    {
        var sut = new SearchContext(
            typeof(Google),
            typeof(Yahoo),
            typeof(Uniqueness),
            typeof(Order));

        sut.Services.Should().NotBeNullOrEmpty()
            .And.HaveCount(4)
            .And.SatisfyRespectively(
                item => item.Should().BeOfType<Google>(),
                item => item.Should().BeOfType<Yahoo>(),
                item => item.Should().BeOfType<Uniqueness>(),
                item => item.Should().BeOfType<Order>());
    }

    [Fact]
    public void Initializing_SearchContext_with_wrong_types_throws_NotSupportedException()
    {
        Action action = () => new SearchContext(
            typeof(Google),
            typeof(string),
            typeof(Uniqueness),
            typeof(int));

        action.Should().ThrowExactly<NotSupportedException>()
            .WithMessage("All services must inherit from T.");
    }

    [Fact]
    public async void When_none_searcher_is_set_Search_returns_an_empty_collection()
    {
        var sut = new SearchContext();

        var results = await sut.SearchAsync("query");

        results.Should().BeEmpty();
    }

    [Fact]
    public async void When_two_searchers_are_set_Search_returns_a_merged_collection()
    {
        var sut = new SearchContext()
            .With<ArbitrarySearcher>(new ArbitrarySearcherSettings { Samples = 8 })
            .With<ArbitrarySearcher>(new ArbitrarySearcherSettings { Samples = 12 });
        var results = await sut.SearchAsync("query");

        results.Should().NotBeNullOrEmpty()
            .And.HaveCount(20);
    }

    [Fact]
    public async void Search_invokes_services_by_addition_order()
    {
        var sut = new SearchContext()
            .With<Marker>(
                new MarkerSettings { Stamp = "stamp0" })
            .With<ArbitrarySearcher>(new ArbitrarySearcherSettings { Samples = 5 })
            .With<Marker>(new MarkerSettings { Stamp = "stamp1" })
            .With<Marker>(new MarkerSettings { Stamp = "stamp2" });
        var results = await sut.SearchAsync("search");

        results.First().Description.Should().StartWith("stamp2|stamp1|");
    }

    [Fact]
    public async void Removed_searcher_does_not_produce_results()
    {
        var sut = new SearchContext()
            .With<ArbitrarySearcher>(new ArbitrarySearcherSettings { Samples = 8 })
            .With<ArbitrarySearcher>(new ArbitrarySearcherSettings { Samples = 10 })
            .Without<ArbitrarySearcher>();
        var results = await sut.SearchAsync("search");

        results.Should().NotBeNullOrEmpty()
            .And.HaveCount(10);
    }

    [Fact]
    public async void Removed_post_processor_does_not_take_effect()
    {
        var sut = new SearchContext()
            .With<ArbitrarySearcher>(new ArbitrarySearcherSettings { Samples = 5 })
            .With<Marker>(new MarkerSettings { Stamp = "stamp" })
            .Without<Marker>();
        var results = await sut.SearchAsync("query");

        results.Should().NotBeNullOrEmpty()
            .And.OnlyContain(x => !x.Description.StartsWith("stamp"));
    }

    [Fact]
    public async void Context_is_set_in_services()
    {
        var sut = new SearchContext()
            .With<ArbitrarySearcher>(new ArbitrarySearcherSettings { Samples = 1 })
            .With<Uniqueness>();

        await sut.SearchAsync("query");

        sut.Services.Should().NotBeNullOrEmpty()
            .And.HaveCount(2)
            .And.SatisfyRespectively(
                item => ((Searcher)item).Context.Should().NotBeNull(),
                item => ((PostProcessor)item).Context.Should().NotBeNull());
    }

    [Fact]
    public async void Should_limit_results_if_maximumResults_is_set()
    {
        var sut = new SearchContext(maximumResults: 10)
            .With<ArbitrarySearcher>(new ArbitrarySearcherSettings { Samples = 20 });

        var results = await sut.SearchAsync("query");

        results.Should().NotBeNullOrEmpty()
            .And.HaveCount(10);
    }

    [Fact]
    public async void A_searcher_should_limit_results_directly()
    {
        var sut = new SearchContext(maximumResults: 10)
            .With<ArbitrarySearcher>(
                new ArbitrarySearcherSettings { Samples = 20, AtLeast = Maybe.Just<ushort>(15) });
        sut.EnforceMaximumResults = false;

        var results = await sut.SearchAsync("query");

        results.Should().NotBeNullOrEmpty()
            .And.HaveCount(10);
    }

    [Fact]
    public async void Maximum_results_should_be_partitioned_per_searcher()
    {
        var sut = new SearchContext(maximumResults: 10)
            .With<ArbitrarySearcher>(new ArbitrarySearcherSettings { Samples = 20 })
            .With<ArbitrarySearcher>(new ArbitrarySearcherSettings { Samples = 20 })
            .With<ArbitrarySearcher>(new ArbitrarySearcherSettings { Samples = 20 })
            .With<Order>()
            .With<Uniqueness>();
        sut.EnforceMaximumResults = false;

        await sut.SearchAsync("query");

        sut.Services.Take(3).Cast<Searcher>().Should()
            .SatisfyRespectively(
                item => item.Runtime.MaximumResults.Should().Be(4),
                item => item.Runtime.MaximumResults.Should().Be(3),
                item => item.Runtime.MaximumResults.Should().Be(3));
    }
}
