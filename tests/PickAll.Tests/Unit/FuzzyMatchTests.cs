using Xunit;
using FluentAssertions;
using PickAll.PostProcessors;
using PickAll.Tests.Fakes;
using CSharpx;

namespace PickAll.Tests.Unit
{
    public class FuzzyMatchTests
    {
        [Fact]
        public void Matching_text_with_minimum_distance_of_zero_excludes_other_results()
        {
            var results = ResultInfoBuilder.GenerateUnique("tests", 10);
            var expected = results.Choice();

            var sut = new FuzzyMatch(new FuzzyMatchSettings { Text = expected.Description });
            var processed = sut.Process(results);

            processed.Should().NotBeEmpty()
                .And.ContainSingle()
                .And.ContainEquivalentOf(expected);
        }
    }
}