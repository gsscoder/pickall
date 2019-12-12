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
        public void Can_add_service_by_name()
        {
            var context = new SearchContext()
                .With("DuckDuckGo")
                .With("Uniqueness");
            
            context.Services.Should().NotBeEmpty()
                .And.HaveCount(2)
                .And.SatisfyRespectively(
                    item => item.Should().BeOfType<DuckDuckGo>(),
                    item => item.Should().BeOfType<Uniqueness>());
        }

        [Fact]
        public void Can_add_service_by_name_ignoring_case()
        {
            var context = new SearchContext()
                .With("DUCKDUCKgo")
                .With("uniQueness");
            
            context.Services.Should().NotBeEmpty()
                .And.HaveCount(2)
                .And.SatisfyRespectively(
                    item => item.Should().BeOfType<DuckDuckGo>(),
                    item => item.Should().BeOfType<Uniqueness>());
        }

        [Fact]
        public void Can_remove_service_by_name_ignoring_case()
        {
            var context = new SearchContext()
                .With<DuckDuckGo>()
                .With<Uniqueness>()
                .Without("DUCKDUCKgo")
                .Without("uniQueness");
            
            context.Services.Should().BeEmpty();
        }

        [Fact]
        public void Can_add_post_processor_service_with_parameters_by_name()
        {
            var context = new SearchContext()
                .With("FuzzyMatch", new FuzzyMatchSettings {Text = "nothing", MaximumDistance = 10 });
            
            context.Services.Should().NotBeEmpty()
                .And.ContainSingle()
                .And.ContainItemsAssignableTo<FuzzyMatch>();
        }

        [Fact]
        public void Can_add_service_with_generic_or_non_generic_With_method()
        {
            var context = new SearchContext()
                .With<Google>()
                .With("DuckDuckGo")
                .With<Uniqueness>()
                .With("Order");

            context.Services.Should().NotBeEmpty()
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
            var context = new SearchContext()
                .With<Yahoo>()
                .With<Order>()
                .Without("Yahoo")
                .Without("Order");

            context.Services.Should().BeEmpty();
        }

        [Fact]
        public void Adding_a_custom_searcher_by_name_throws_NotSupportedException()
        {
            var context = new SearchContext();

            Action action = () => context.With("Searcher_with_three_results");
            
            action.Should().ThrowExactly<NotSupportedException>()
                .WithMessage("Searcher_with_three_results service not found");
        }

        [Fact]
        public void Adding_a_custom_post_processor_by_name_throws_NotSupportedException()
        {
            var context = new SearchContext();

            Action action = () => context.With("Post_processor_marker");
            
            action.Should().ThrowExactly<NotSupportedException>()
                .WithMessage("Post_processor_marker service not found");
        }

        [Fact]
        public async void When_none_searcher_is_set_Search_returns_an_empty_collection()
        {
            var context = new SearchContext();

            var results = await context.SearchAsync("query");

            results.Should().BeEmpty();
        }

        [Fact]
        public async void When_two_searchers_are_set_Search_returns_a_merged_collection()
        {
            var context = new SearchContext()
                .With<ArbitrarySearcher>(new ArbitrarySearcherSettings { Samples = 8 })
                .With<ArbitrarySearcher>(new ArbitrarySearcherSettings { Samples = 12 });
            var results = await context.SearchAsync("query");

            results.Should().NotBeEmpty()
                .And.HaveCount(20);
        }

        [Fact]
        public void Search_invokes_services_by_addition_order()
        {
            var context = new SearchContext()
                .With<Post_processor_marker>(
                    new Post_processor_marker_settings{ Stamp = "STAMP/0" }) // unuseful here
                .With<Searcher_with_five_results>()
                .With<Post_processor_marker>(new Post_processor_marker_settings{ Stamp = "STAMP/1" })
                .With<Post_processor_marker>(new Post_processor_marker_settings{ Stamp = "STAMP/2" });
            var results = context.Search();

            var expected = Utilities.SearcherFor<Searcher_with_five_results, string>(
                searcher => $"STAMP/2|STAMP/1|{searcher.Search().First().Description}");

            results.First().Description.Should().Be(expected);
        }

        [Fact]
        public void Removed_searcher_doesnt_produce_results()
        {
            var expected = Utilities.ResultsCountOf<Searcher_with_five_results>();

            var context = new SearchContext()
                .With<Searcher_with_three_results>()
                .With<Searcher_with_five_results>()
                .Without<Searcher_with_three_results>();
            var results = context.Search();

            results.Should().NotBeEmpty()
                .And.HaveCount(expected);
        }

        [Fact]
        public void Removed_post_processor_doesnt_take_effect()
        {
            var context = new SearchContext()
                .With<Searcher_with_five_results>()
                .With<Post_processor_marker>(new Post_processor_marker_settings { Stamp = "STAMP"})
                .Without<Post_processor_marker>();
            var results = context.Search();

            results.Should().NotBeEmpty()
                .And.OnlyContain(x => !x.Description.StartsWith("STAMP"));
        }

        [Fact]
        public void Without_removes_only_first_service_of_a_given_type()
        {
            var context = new SearchContext()
                .With<Searcher_with_three_results>()
                .With<Order>()
                .With<Searcher_with_five_results>()
                .With<Order>()
                .Without<Order>();

            context.Services.Should().NotBeEmpty()
                .And.HaveCount(3)
                .And.SatisfyRespectively(
                    item => item.Should().BeOfType<Searcher_with_three_results>(),
                    item => item.Should().BeOfType<Searcher_with_five_results>(),
                    item => item.Should().BeOfType<Order>());
        }
    }
}