using System.Collections.Generic;
using System.Linq;
using Xunit;
using FluentAssertions;
using CSharpx;
using PickAll;

public class UniquenessSpecs
{
    [Fact]
    public void Should_exclude_duplicate_urls()
    {
        var results = new List<ResultInfo>();
        results.AddRange(ResultInfoBuilder.GenerateUnique("random", 10));
        results.Add(results.Choice().CloneWithIndex(0));
        var sut = new Uniqueness(null);
        var processed = sut.Process(results);

        processed.Should().NotBeEmpty()
            .And.HaveCount(results.Count() - 1);
    }
}