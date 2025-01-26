using FluentAssertions;
using PickAll;
using Xunit;

namespace Tests;

public class SearchersTests
{
    [Fact]
    public async Task Test_Google()
    {
        using var sut = new SearchContext()
            .With("Google");

        var results = await sut.SearchAsync("all about Mike Tyson");

        results.Should().NotBeEmpty();
    }
}

