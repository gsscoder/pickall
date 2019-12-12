using System.Collections.Generic;
using System.Linq;
using Xunit;
using FluentAssertions;
using PickAll.PostProcessors;

namespace PickAll.Tests.Unit
{
    public class UniquenessTests
    {
        [Fact]
        public async void Should_exclude_duplicate_urls()
        {
            var results = new List<ResultInfo>();
            results.AddRange(ResultInfoGenerator.GenerateUnique("random", 10));
            results.Add(results.Random().WithIndex(0));
            var uniqueness = new Uniqueness();
            var processed = await uniqueness.ProcessAsync(results);

            processed.Should().NotBeEmpty()
                .And.HaveCount(results.Count() - 1);
        }
    }
}