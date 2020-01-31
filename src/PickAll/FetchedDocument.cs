using System.Linq;
using System.Text;

namespace PickAll
{
    public sealed class FetchedDocument : IFetchedDocument
    {
        public static readonly IFetchedDocument Empty = new FetchedDocument();

        FetchedDocument() => Content = new byte[] {};

        #if DEBUG
        public FetchedDocument(byte[] content) => Content = content;
        #else
        internal FetchedDocument(byte[] content) => Content = content;
        #endif

        public override bool Equals(object value) =>
            value is FetchedDocument f &&
                  Enumerable.SequenceEqual(f.Content, Content) &&
                  f.Length.Equals(Length) &&
                  f.LongLength.Equals(LongLength);

        public bool Equals(IFetchedDocument other) =>
                  Enumerable.SequenceEqual(other.Content, Content) &&
                  other.Length.Equals(Length) &&
                  other.LongLength.Equals(LongLength);

        public override int GetHashCode()
        {
            unchecked {
                var hash = 17;
                hash = hash * 23 + Content.GetHashCode();
                hash = hash * 23 + Length.GetHashCode();
                hash = hash * 23 + LongLength.GetHashCode();
                return hash;
            }
        }

        public byte[] Content { get; private set; }

        public string Text() => Encoding.UTF8.GetString(Content);

        public int Length => Content.Length;

        public long LongLength => Content.LongLength;
    }
}