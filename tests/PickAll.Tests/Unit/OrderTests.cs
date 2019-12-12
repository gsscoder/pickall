using System.Collections.Generic;
using System.Linq;
using Xunit;
using FluentAssertions;
using PickAll.PostProcessors;

namespace PickAll.Tests.Unit
{
    public class OrderTests
    {
        [Fact]
        public async void Should_ordered_by_index()
        {
            var results = new List<ResultInfo>();
            results.AddRange(ResultInfoGenerator.Generate("random1", 3));
            results.AddRange(ResultInfoGenerator.Generate("random2", 5));
            var order = new Order();
            var processed = await order.ProcessAsync(results);

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
}