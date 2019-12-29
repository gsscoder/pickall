using System.Collections.Generic;
using System.Linq;
using Xunit;
using FluentAssertions;
using PickAll.PostProcessors;
using PickAll.Tests.Fakes;
using CSharpx;

namespace PickAll.Tests.Unit
{
    public class UniquenessTests
    {
        [Fact]
        public void Should_exclude_duplicate_urls()
        {
            var results = new List<ResultInfo>();
            results.AddRange(ResultInfoBuilder.GenerateUnique("random", 10));
            results.Add(results.ToArray().Choice().UsingIndex(0));
            var sut = new Uniqueness(null);
            var processed = sut.Process(results);

            processed.Should().NotBeEmpty()
                .And.HaveCount(results.Count() - 1);
        }
    }
}