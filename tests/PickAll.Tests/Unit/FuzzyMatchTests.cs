using Xunit;
using FluentAssertions;
using PickAll.PostProcessors;
using PickAll.Tests.Fakes;

namespace PickAll.Tests.Unit
{
    public class FuzzyMatchTests
    {
        [Fact]
        public async void Matching_text_with_minimum_distance_of_zero_excludes_other_results()
        {
            var results = ResultInfoGenerator.GenerateUnique("random", 10);
            var expected = results.Random();

            var fuzzyMatch = new FuzzyMatch(new FuzzyMatchSettings { Text = expected.Description });
            var processed = await fuzzyMatch.ProcessAsync(results);

            processed.Should().NotBeEmpty()
                .And.ContainSingle()
                .And.ContainEquivalentOf(expected);
        }
    }
}