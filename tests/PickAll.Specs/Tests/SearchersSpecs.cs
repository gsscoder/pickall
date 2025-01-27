using FluentAssertions;
using PickAll;
using Xunit;

namespace Tests;

public class SearchersTests
{
    [Fact]
    public async Task Test_Bing()
    {
        using var sut = new SearchContext()
            .With("Bing");

        var results = await sut.SearchAsync($"{TextPicker.GetSentence()} {TextPicker.GetName()}");

        results.Should().NotBeEmpty();
    }
}
