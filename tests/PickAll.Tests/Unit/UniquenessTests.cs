using System.Collections.Generic;
using System.Linq;
using Xunit;
using FluentAssertions;
using PickAll.PostProcessors;
using PickAll.Tests.Fakes;

namespace PickAll.Tests.Unit
{
    public class UniquenessTests
    {
        [Fact]
        public void Should_exclude_duplicate_urls()
        {
            var results = new List<ResultInfo>();
            results.AddRange(ResultInfoGenerator.GenerateUnique("random", 10));
            results.Add(results.Random().WithIndex(0));
            var uniqueness = new Uniqueness();
            var processed = uniqueness.Process(results);

            processed.Should().NotBeEmpty()
                .And.HaveCount(results.Count() - 1);
        }
    }
}