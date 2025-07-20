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

        var query = $"{TextPicker.GetSentence()} {TextPicker.GetName()}";
        var results = await sut.SearchAsync(query);

        Assert.NotEmpty(results);
    }
}