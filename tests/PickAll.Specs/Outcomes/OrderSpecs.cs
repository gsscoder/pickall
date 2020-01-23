using System.Collections.Generic;
using Xunit;
using FluentAssertions;
using PickAll;

public class OrderSpecs
{
    [Fact]
    public void Should_ordered_by_index()
    {
        var results = new List<ResultInfo>();
        results.AddRange(ResultInfoBuilder.Generate("Choice1", 3));
        results.AddRange(ResultInfoBuilder.Generate("Choice2", 5));
        var sut = new Order(null);
        var processed = sut.Process(results);

        processed.Should().NotBeEmpty()
            .And.SatisfyRespectively(
                item => item.Index.Should().Be(0),
                item => item.Index.Should().Be(0),
                item => item.Index.Should().Be(1),
                item => item.Index.Should().Be(1),
                item => item.Index.Should().Be(2),
                item => item.Index.Should().Be(2),
                item => item.Index.Should().Be(3),
                item => item.Index.Should().Be(4)
            );
    }
}