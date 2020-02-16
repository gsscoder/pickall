using Xunit;
using FluentAssertions;
using PickAll;

public partial class SearchContextSpecs
{
    public class Events
    {
        [Fact]
        public async void Should_fire_SearchStart_event()
        {
            var evidence = string.Empty;
            var sut = new SearchContext(new ContextSettings { EnableRaisingEvents = true })
                .With<ArbitrarySearcher>(new ArbitrarySearcherSettings { Samples = 10 });
            sut.SearchBegin += (sender, e) => evidence = e.Query;

            await sut.SearchAsync("query");

            evidence.Should().Be("query");
        }

        [Fact]
        public async void Should_fire_SearchEnd_event()
        {
            var evidence = false;
            var sut = new SearchContext(new ContextSettings { EnableRaisingEvents = true })
                .With<ArbitrarySearcher>(new ArbitrarySearcherSettings { Samples = 10 });
            sut.SearchEnd += (sender, e) => evidence = true;

            await sut.SearchAsync("query");

            evidence.Should().BeTrue();
        }

        [Fact]
        public async void Should_fire_ServiceLoad_event()
        {
            var evidence = false;
            var sut = new SearchContext(new ContextSettings { EnableRaisingEvents = true })
                .With<ArbitrarySearcher>(new ArbitrarySearcherSettings { Samples = 10 });
            sut.ServiceLoad += (sender, e) => evidence = true;

            await sut.SearchAsync("query");

            evidence.Should().BeTrue();
        }

        [Fact]
        public async void Should_fire_ResultCreated_event()
        {
            var evidence = 0;
            var sut = new SearchContext(new ContextSettings { EnableRaisingEvents = true })
                .With<ArbitrarySearcher>(new ArbitrarySearcherSettings { Samples = 10 });
            sut.ResultCreated += (sender, e) => evidence++;

            await sut.SearchAsync("query");

            evidence.Should().Be(10);
        }

        [Fact]
        public async void Should_fire_ResultProcessed_event()
        {
            var evidence = 0;
            void sut_ResultProcessed(object sender, ResultHandledEventArgs e) { evidence++; }

            var sut = new SearchContext(new ContextSettings { EnableRaisingEvents = true })
                .With<ArbitrarySearcher>(new ArbitrarySearcherSettings { Samples = 10 })
                .With<Marker>(new MarkerSettings { Stamp = string.Empty });
            sut.ResultProcessed += sut_ResultProcessed;

            await sut.SearchAsync("query");

            evidence.Should().Be(10);
        }
    }
}