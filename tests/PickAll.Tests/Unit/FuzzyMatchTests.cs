using Xunit;
using FluentAssertions;
using PickAll.PostProcessors;
using PickAll.Tests.Fakes;

namespace PickAll.Tests.Unit
{
    public class FuzzyMatchTests
    {
        [Fact]
        public void Matching_text_with_minimum_distance_of_zero_excludes_other_results()
        {
            var expected = Utilities.RandomResultInfoOf<Searcher_with_five_results>();

            var context = new SearchContext()
                .With<Searcher_with_five_results>()
                .With<FuzzyMatch>(new FuzzyMatchSettings { Text = expected.Description });
            var results = context.Search();

            results.Should().ContainSingle()
                .And.ContainEquivalentOf(expected);
        }
    }
}