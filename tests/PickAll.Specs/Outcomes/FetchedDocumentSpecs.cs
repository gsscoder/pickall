using System.Text;
using Xunit;
using FluentAssertions;
using PickAll;

public class FetchedDocumentSpecs
{
    [Fact]
    public void An_empty_FetchedDocument_should_be_equal_to_Empty_value()
    {
        var sut = new FetchedDocument(new byte[] {});

        sut.Equals(FetchedDocument.Empty).Should().BeTrue();
    }

    [Fact]
    public void A_FetchedDocument_should_be_equal_to_an_identical_one()
    {
        var sut = new FetchedDocument(Encoding.UTF8.GetBytes("foo"));
        var other = new FetchedDocument(Encoding.UTF8.GetBytes("foo"));

        sut.Equals(other).Should().BeTrue();
    }

    [Fact]
    public void A_FetchedDocument_should_be_equal_to_a_different_one()
    {
        var sut = new FetchedDocument(Encoding.UTF8.GetBytes("foo"));
        var other = new FetchedDocument(Encoding.UTF8.GetBytes("bar"));

        sut.Equals(other).Should().BeFalse();
    }
}
