using System.Linq;
using Xunit;
using FluentAssertions;
using CSharpx;
using PickAll.PostProcessors;
using PickAll.Tests.Fakes;

namespace PickAll.Tests.Unit
{
    public class FuzzyMatchTests
    {
        [Fact]
        public void Matching_text_with_minimum_distance_of_zero_excludes_other_results()
        {
            var results = ResultInfoGenerator.GenerateUnique("Choice", 10);
            var expected = results.ToArray().Choice();

            var sut = new FuzzyMatch(new FuzzyMatchSettings { Text = expected.Description });
            var processed = sut.Process(results);

            processed.Should().NotBeEmpty()
                .And.ContainSingle()
                .And.ContainEquivalentOf(expected);
        }
    }
}