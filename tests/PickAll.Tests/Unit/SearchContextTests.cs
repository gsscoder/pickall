using System;
using System.Linq;
using Xunit;
using FluentAssertions;
using PickAll.Searchers;
using PickAll.PostProcessors;
using PickAll.Tests.Fakes;

namespace PickAll.Tests.Unit
{
    public class SearchContextTests
    {
        [Fact]
        public void Can_initialize_SearchContext_using_types()
        {
            var sut = new SearchContext(
                typeof(Google),
                typeof(Yahoo),
                typeof(Uniqueness),
                typeof(Order));

            sut.Services.Should().NotBeEmpty()
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
                .WithMessage("All types must inherit from Searcher or PostProcessor");
        }

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
                .With("FuzzyMatch", new FuzzyMatchSettings {Text = "nothing", MaximumDistance = 10 });
            
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

            results.Should().NotBeEmpty()
                .And.HaveCount(20);
        }

        [Fact]
        public async void Search_invokes_services_by_addition_order()
        {
            var sut = new SearchContext()
                .With<Marker>(
                    new MarkerSettings{ Stamp = "stamp0" })
                .With<ArbitrarySearcher>(new ArbitrarySearcherSettings { Samples = 5 })
                .With<Marker>(new MarkerSettings{ Stamp = "stamp1" })
                .With<Marker>(new MarkerSettings{ Stamp = "stamp2" });
            var results = await sut.SearchAsync("search");

            results.First().Description.Should().StartWith("stamp2|stamp1|");
        }

        [Fact]
        public async void Removed_searcher_doesnt_produce_results()
        {
            var sut = new SearchContext()
                .With<ArbitrarySearcher>(new ArbitrarySearcherSettings { Samples = 8 })
                .With<ArbitrarySearcher>(new ArbitrarySearcherSettings { Samples = 10 })
                .Without<ArbitrarySearcher>();
            var results = await sut.SearchAsync("search");

            results.Should().NotBeEmpty()
                .And.HaveCount(10);
        }

        [Fact]
        public async void Removed_post_processor_doesnt_take_effect()
        {
            var sut = new SearchContext()
                .With<ArbitrarySearcher>(new ArbitrarySearcherSettings { Samples = 5 })
                .With<Marker>(new MarkerSettings { Stamp = "stamp"})
                .Without<Marker>();
            var results = await sut.SearchAsync("query");

            results.Should().NotBeEmpty()
                .And.OnlyContain(x => !x.Description.StartsWith("stamp"));
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
        public async void Context_is_set_in_services()
        {
            var sut = new SearchContext()
                .With<ArbitrarySearcher>(new ArbitrarySearcherSettings { Samples = 1 })
                .With<Uniqueness>();

            await sut.SearchAsync("query");

            sut.Query.Should().Be("query");
            sut.Services.Should().NotBeEmpty()
                .And.HaveCount(2)
                .And.SatisfyRespectively(
                    item => ((Searcher)item).Context.Should().NotBeNull(),
                    item => ((PostProcessor)item).Context.Should().NotBeNull());
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
            sut.MaximumResults.Should().NotBeNull()
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
            sut.Services.Should().NotBeEmpty()
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
        public async void Should_limit_results_if_maximumResults_is_set()
        {
            var sut = new SearchContext(maximumResults: 10)
                .With<ArbitrarySearcher>(new ArbitrarySearcherSettings { Samples = 20 });
            
            var results = await sut.SearchAsync("query");

            results.Should().NotBeEmpty()
                .And.HaveCount(10);
        }
    }
}