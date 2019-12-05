using System.Linq;
using Xunit;
using PickAll.PostProcessors;
using PickAll.Tests.Fakes;

namespace PickAll.Tests.Unit
{
    public class SearchContextTests
    {
        [Fact]
        public void When_none_searcher_is_set_Search_returns_an_empty_collection()
        {
            var context = new SearchContext();
            var result = context.SearchSync("query");

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
            var results = context.SearchSync();

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
            var results = context.SearchSync();

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
            var results = context.SearchSync();

            Assert.Equal(0, results.ElementAt(0).Index);
            Assert.Equal(0, results.ElementAt(1).Index);
        }

        [Fact]
        public void Search_invokes_services_by_addition_order()
        {
            var context = new SearchContext()
                .With(new MarkPostProcessor("STAMP/0")) // unuseful here
                .With<Searcher_with_five_results>()
                .With(new MarkPostProcessor("STAMP/1"))
                .With(new MarkPostProcessor("STAMP/2"));
            var results = context.SearchSync();

            var expected = Utilities.SearcherFor<Searcher_with_five_results, string>(
                searcher => $"STAMP/2|STAMP/1|{searcher.SearchSync().ElementAt(0).Description}");

            Assert.Equal(expected, results.ElementAt(0).Description);
        }

        [Fact]
        public void Removed_searcher_doesnt_produce_results()
        {
            var context = new SearchContext()
                .With<Searcher_with_three_results>()
                .With<Searcher_with_five_results>()
                .Without<Searcher_with_three_results>();
            var results = context.SearchSync();

            Assert.Equal(5, results.Count());
        }

        [Fact]
        public void Removed_post_processor_doesnt_take_effect()
        {
            var context = new SearchContext()
                .With<Searcher_with_five_results>()
                .With(new MarkPostProcessor("STAMP"))
                .Without<MarkPostProcessor>();
            var results = context.SearchSync();

            Assert.All(results, result => Assert.False(result.Description.StartsWith("STAMP")));
        }
    }
}