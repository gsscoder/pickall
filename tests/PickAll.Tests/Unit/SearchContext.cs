using System;
using System.Linq;
using Xunit;
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
            
            Assert.Equal(2, context.Services.Count());
            Assert.Collection(context.Services,
                item => Assert.IsType<DuckDuckGo>(item),
                item => Assert.IsType<Uniqueness>(item));
        }

        [Fact]
        public void Can_add_service_by_name_ignoring_case()
        {
            var context = new SearchContext()
                .With("DUCKDUCKgo")
                .With("uniQueness");
            
            Assert.Equal(2, context.Services.Count());
            Assert.Collection(context.Services,
                item => Assert.IsType<DuckDuckGo>(item),
                item => Assert.IsType<Uniqueness>(item));
        }

        [Fact]
        public void Can_remove_service_by_name_ignoring_case()
        {
            var context = new SearchContext()
                .With<DuckDuckGo>()
                .With<Uniqueness>()
                .Without("DUCKDUCKgo")
                .Without("uniQueness");
            
            Assert.Empty(context.Services);
        }

        [Fact]
        public void Can_add_post_processor_service_with_parameters_by_name()
        {
            var context = new SearchContext()
                .With("FuzzyMatch", new FuzzyMatchSettings {Text = "nothing", MaximumDistance = 10 });
            
            Assert.Single(context.Services);
            Assert.Collection(context.Services,
                item => Assert.IsType<FuzzyMatch>(item));
        }

        [Fact]
        public void Can_add_service_with_generic_or_non_generic_With_method()
        {
            var context = new SearchContext()
                .With<Google>()
                .With("DuckDuckGo")
                .With<Uniqueness>()
                .With("Order");

            Assert.Collection(context.Services,
                item => Assert.IsType<Google>(item),
                item => Assert.IsType<DuckDuckGo>(item),
                item => Assert.IsType<Uniqueness>(item),
                item => Assert.IsType<Order>(item));
        }

        [Fact]
        public void Can_remove_service_with_using_name()
        {
            var context = new SearchContext()
                .With<Yahoo>()
                .With<Order>()
                .Without("Yahoo")
                .Without("Order");

            Assert.Empty(context.Services);
        }

        [Fact]
        public void Adding_a_custom_searcher_by_name_throws_NotSupportedException()
        {
            Action action = () => new SearchContext().With("Searcher_with_three_results");
            
            var exception = Assert.Throws<NotSupportedException>(action);

            Assert.Equal("Searcher_with_three_results service not found", exception.Message);
        }

        [Fact]
        public void Adding_a_custom_post_processor_by_name_throws_NotSupportedException()
        {
            Action action = () => new SearchContext().With("Post_processor_marker");
            
            var exception = Assert.Throws<NotSupportedException>(action);

            Assert.Equal("Post_processor_marker service not found", exception.Message);
        }

        [Fact]
        public void When_none_searcher_is_set_Search_returns_an_empty_collection()
        {
            var context = new SearchContext();
            var result = context.Search("query");

            Assert.Empty(result);
        }

        [Fact]
        public void When_two_searchers_are_set_Search_returns_a_merged_collection()
        {
            var firstCount = Utilities.ResultsCountOf<Searcher_with_three_results>();
            var secondCount = Utilities.ResultsCountOf<Searcher_with_five_results>();

            var context = new SearchContext()
                .With<Searcher_with_three_results>()
                .With<Searcher_with_five_results>();
            var results = context.Search();

            Assert.Equal(firstCount + secondCount, results.Count());
        }

        [Fact]
        public void When_uniqueness_is_set_Search_excludes_duplicates_url()
        {
            var firstCount = Utilities.ResultsCountOf<Searcher_with_three_results>();
            var secondCount = Utilities.ResultsCountOf<Searcher_with_five_results>();
            
            var context = new SearchContext()
                .With<Searcher_with_three_results>()
                .With<Searcher_with_five_results>()
                .With<Uniqueness>();
            var results = context.Search();

            Assert.Equal(firstCount + secondCount - 2, results.Count());
        }

        [Fact]
        public void When_order_is_set_Search_results_are_ordered_by_index()
        {
            var firstCount = Utilities.ResultsCountOf<Searcher_with_three_results>();
            var secondCount = Utilities.ResultsCountOf<Searcher_with_five_results>();

            var context = new SearchContext()
                .With<Searcher_with_three_results>()
                .With<Searcher_with_five_results>()
                .With<Order>();
            var results = context.Search();

            Assert.Equal(0, results.First().Index);
            Assert.Equal(0, results.Second().Index);
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

            Assert.Equal(expected, results.First().Description);
        }

        [Fact]
        public void Removed_searcher_doesnt_produce_results()
        {
            var context = new SearchContext()
                .With<Searcher_with_three_results>()
                .With<Searcher_with_five_results>()
                .Without<Searcher_with_three_results>();
            var results = context.Search();

            Assert.Equal(5, results.Count());
        }

        [Fact]
        public void Removed_post_processor_doesnt_take_effect()
        {
            var context = new SearchContext()
                .With<Searcher_with_five_results>()
                .With<Post_processor_marker>(new Post_processor_marker_settings { Stamp = "STAMP"})
                .Without<Post_processor_marker>();
            var results = context.Search();

            Assert.All(results, result => Assert.False(result.Description.StartsWith("STAMP")));
        }

        [Fact]
        public void Without_remove_only_first_service_of_a_given_type()
        {
            var context = new SearchContext()
                .With<Searcher_with_three_results>()
                .With<Order>()
                .With<Searcher_with_five_results>()
                .With<Order>()
                .Without<Order>();

            Assert.Collection(context.Services,
                item => Assert.IsType<Searcher_with_three_results>(item),
                item => Assert.IsType<Searcher_with_five_results>(item),
                item => Assert.IsType<Order>(item));
        }
    }
}