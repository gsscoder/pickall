using System;
using System.Text;

namespace PickAll
{
    /// <summary>Represents a document fetched without HTML DOM.</summary>
    public interface  IFetchedDocument : IEquatable<IFetchedDocument>
    {
        byte[] Content { get; }

        int Length { get; }

        long LongLength { get; }

        string Text();
    }
}