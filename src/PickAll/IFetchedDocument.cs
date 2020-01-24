using System;
using System.Text;

namespace PickAll
{
    public interface  IFetchedDocument : IEquatable<IFetchedDocument>
    {
        byte[] Content { get; }

        int Length { get; }

        long LongLength { get; }

        string Text();
    }
}