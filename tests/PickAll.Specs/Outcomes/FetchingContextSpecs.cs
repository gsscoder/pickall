using System.Net.Http;
using Xunit;
using FluentAssertions;
using PickAll;

public class FetchingContextSpecs
{
    [Fact]
    public async void Should_fetch_a_document()
    {
        var sut = new FetchingContext(new HttpClient());
        
        var document = await sut.FetchAsync("https://google.com/");

        document.Length.Should().BeGreaterThan(0);
    }
}