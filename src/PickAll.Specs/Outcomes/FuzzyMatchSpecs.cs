using Xunit;
using FluentAssertions;
using CSharpx;
using PickAll;

public class FuzzyMatchSpecs
{
    [Fact]
    public void Matching_text_with_minimum_distance_of_zero_excludes_other_results()
    {
        var results = ResultInfoBuilder.GenerateUnique("tests", 10);
        var expected = results.Choice();

        var sut = new FuzzyMatch(new FuzzyMatchSettings { Text = expected.Description });
        var processed = sut.Process(results);

        processed.Should().NotBeNullOrEmpty()
            .And.ContainSingle()
            .And.ContainEquivalentOf(expected);
    }
}