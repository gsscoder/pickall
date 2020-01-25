using System;
using System.Linq;
using Xunit;
using FluentAssertions;
using PickAll;

public class SearchContextExtensionsSpecs
{
    [Fact]
    public void Can_add_service_by_name()
    {
        var sut = new SearchContext()
            .With("DuckDuckGo")
            .With("Uniqueness");

        sut.Services.Should().NotBeEmpty()
            .And.HaveCount(2)
            .And.SatisfyRespectively(
                item => item.Should().BeOfType<DuckDuckGo>(),
                item => item.Should().BeOfType<Uniqueness>());
    }

    [Fact]
    public void Can_add_service_by_name_ignoring_case()
    {
        var sut = new SearchContext()
            .With("DUCKDUCKgo")
            .With("uniQueness");

        sut.Services.Should().NotBeEmpty()
            .And.HaveCount(2)
            .And.SatisfyRespectively(
                item => item.Should().BeOfType<DuckDuckGo>(),
                item => item.Should().BeOfType<Uniqueness>());
    }

    [Fact]
    public void Can_remove_service_by_name_ignoring_case()
    {
        var sut = new SearchContext()
            .With<DuckDuckGo>()
            .With<Uniqueness>()
            .Without("DUCKDUCKgo")
            .Without("uniQueness");

        sut.Services.Should().BeEmpty();
    }

    [Fact]
    public void Can_add_post_processor_service_with_parameters_by_name()
    {
        var sut = new SearchContext()
            .With("FuzzyMatch", new FuzzyMatchSettings { Text = "nothing", MaximumDistance = 10 });

        sut.Services.Should().NotBeEmpty()
            .And.ContainSingle()
            .And.ContainItemsAssignableTo<FuzzyMatch>();
    }

    [Fact]
    public void Can_add_service_with_generic_or_non_generic_With_method()
    {
        var sut = new SearchContext()
            .With<Google>()
            .With("DuckDuckGo")
            .With<Uniqueness>()
            .With("Order");

        sut.Services.Should().NotBeEmpty()
            .And.HaveCount(4)
            .And.SatisfyRespectively(
                item => item.Should().BeOfType<Google>(),
                item => item.Should().BeOfType<DuckDuckGo>(),
                item => item.Should().BeOfType<Uniqueness>(),
                item => item.Should().BeOfType<Order>());
    }

    [Fact]
    public void Can_remove_service_by_name()
    {
        var sut = new SearchContext()
            .With<Yahoo>()
            .With<Order>()
            .Without("Yahoo")
            .Without("Order");

        sut.Services.Should().BeEmpty();
    }

    [Fact]
    public void Adding_a_custom_searcher_by_name_throws_NotSupportedException()
    {
        var sut = new SearchContext();

        Action action = () => sut.With("ArbitrarySearcher", new ArbitrarySearcherSettings());

        action.Should().ThrowExactly<NotSupportedException>()
            .WithMessage("ArbitrarySearcher service not found");
    }

    [Fact]
    public void Adding_a_custom_post_processor_by_name_throws_NotSupportedException()
    {
        var sut = new SearchContext();

        Action action = () => sut.With("Marker", new MarkerSettings());

        action.Should().ThrowExactly<NotSupportedException>()
            .WithMessage("Marker service not found");
    }

    [Fact]
    public void Without_removes_only_first_added_service_of_a_given_type()
    {
        var sut = new SearchContext()
            .With<ArbitrarySearcher>(new ArbitrarySearcherSettings { Samples = 5 })
            .With<Order>()
            .With<ArbitrarySearcher>(new ArbitrarySearcherSettings { Samples = 3 })
            .With<Order>()
            .Without<Order>();

        sut.Services.Should().NotBeEmpty()
            .And.HaveCount(3)
            .And.SatisfyRespectively(
                item => item.Should().BeOfType<ArbitrarySearcher>(),
                item => item.Should().BeOfType<ArbitrarySearcher>(),
                item => item.Should().BeOfType<Order>());
    }

    [Fact]
    public async void A_cloned_SearchContext_retains_services_and_maximum_results_not_query()
    {
        var context = new SearchContext(maximumResults: 5)
            .With<ArbitrarySearcher>(new ArbitrarySearcherSettings { Samples = 1 })
            .With<ArbitrarySearcher>(new ArbitrarySearcherSettings { Samples = 2 })
            .With<Order>();
        await context.SearchAsync("query");

        var sut = context.Clone();

        sut.Query.Should().BeNull();
        sut.Settings.MaximumResults.Should().NotBeNull()
            .And.HaveValue()
            .And.Be(5);
        sut.Services.Should().NotBeEmpty()
            .And.HaveCount(context.Services.Count())
            .And.BeEquivalentTo(context.Services);
    }

    [Fact]
    public async void Services_of_cloned_SearchContext_are_unbound_to_original_context()
    {
        var context = new SearchContext()
            .With<ArbitrarySearcher>(new ArbitrarySearcherSettings { Samples = 1 })
            .With<ArbitrarySearcher>(new ArbitrarySearcherSettings { Samples = 2 })
            .With<Order>();
        await context.SearchAsync("query");

        var sut = context.Clone();

        sut.Query.Should().BeNull();
        sut.Services.Cast<Service>().Should().NotBeEmpty()
            .And.HaveCount(context.Services.Count())
            .And.OnlyContain(service => service.Context == null);
    }

    [Fact]
    public void Can_exclude_all_services_by_category()
    {
        var context = new SearchContext(
            typeof(Google),
            typeof(Yahoo),
            typeof(Order),
            typeof(Uniqueness));

        var sut = context.WithoutAll<Searcher>();

        sut.Services.Should().NotBeEmpty()
            .And.HaveCount(2)
            .And.SatisfyRespectively(
                item => item.Should().BeOfType<Order>(),
                item => item.Should().BeOfType<Uniqueness>()
            );

        sut = sut.WithoutAll<PostProcessor>();

        sut.Services.Should().BeEmpty();
    }

    [Fact]
    public void Should_merge_configuration_settings()
    {
        var expected = new ContextSettings
            { EnableRaisingEvents = true,
              MaximumResults = 10 };

        var sut = SearchContext.Default
                    .WithConfiguration(
                        new ContextSettings { EnableRaisingEvents = true }, merge: false)
                    .WithConfiguration(
                        new ContextSettings { MaximumResults = 10 }, merge: true);

        sut.Settings.Should().Be(expected);
    }
}