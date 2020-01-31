//#define CSX_ENUM_INTERNAL // Uncomment or define at build time to set accessibility to internal.
//#define CSX_REM_MAYBE_FUNC // Uncomment or define at build time to remove dependency to Maybe.cs.
//#define CSX_REM_CRYPTORAND // Uncomment or define at build time to remove dependency to CryptoRandom.cs.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using LinqEnumerable = System.Linq.Enumerable;

namespace CSharpx
{
#if !CSX_ENUM_INTERNAL
    public
#endif
    static class EnumerableExtensions
    {
        #region Internal
        static IEnumerable<TSource> AssertCountImpl<TSource>(IEnumerable<TSource> source,
            int count, Func<int, int, Exception> errorSelector)
        {
            var collection = source as ICollection<TSource>; // Optimization for collections
            if (collection != null) {
                if (collection.Count != count) {
                    throw errorSelector(collection.Count.CompareTo(count), count);
                }   
                return source;
            }
            return ExpectingCountYieldingImpl(source, count, errorSelector);
        }

        static IEnumerable<TSource> ExpectingCountYieldingImpl<TSource>(IEnumerable<TSource> source,
            int count, Func<int, int, Exception> errorSelector)
        {
            var iterations = 0;
            foreach (var element in source) {
                iterations++;
                if (iterations > count) {
                    throw errorSelector(1, count);
                }
                yield return element;
            }
            if (iterations != count) {
                throw errorSelector(-1, count);
            }
        }
        #endregion

        /// <summary>Returns the cartesian product of two sequences by combining each element of the
        /// first set with each in the second and applying the user=define projection to the
        /// pair.</summary>
        public static IEnumerable<TResult> Cartesian<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));
            if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));

            return from element1 in first
                   from element2 in second // TODO buffer to avoid multiple enumerations
                   select resultSelector(element1, element2);
        }

        /// <summary>Prepends a single value to a sequence.</summary>
        public static IEnumerable<TSource> Prepend<TSource>(this IEnumerable<TSource> source, TSource value)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return LinqEnumerable.Concat(LinqEnumerable.Repeat(value, 1), source);
        }

        #region Concat
        /// <summary>Returns a sequence consisting of the head element and the given tail
        /// elements.</summary>
        public static IEnumerable<T> Concat<T>(this T head, IEnumerable<T> tail)
        {
            if (tail == null) throw new ArgumentNullException(nameof(tail));

            return tail.Prepend(head);
        }

        /// <summary>Returns a sequence consisting of the head elements and the given tail element.
        /// </summary>
        public static IEnumerable<T> Concat<T>(this IEnumerable<T> head, T tail)
        {
            if (head == null) throw new ArgumentNullException(nameof(head));

            return LinqEnumerable.Concat(head, LinqEnumerable.Repeat(tail, 1));
        }
        #endregion

        #region Exclude
        /// <summary>Excludes <paramref name="count"/> elements from a sequence starting at a given
        /// index.</summary>
        public static IEnumerable<T> Exclude<T>(this IEnumerable<T> source, int startIndex, int count)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (startIndex < 0) throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));

            return ExcludeImpl(source, startIndex, count);
        }

        static IEnumerable<T> ExcludeImpl<T>(IEnumerable<T> source, int startIndex, int count)
        {
            var index = -1;
            var endIndex = startIndex + count;
            using (var iter = source.GetEnumerator())
            {
                // yield the first part of the sequence
                while (iter.MoveNext() && ++index < startIndex) {
                    yield return iter.Current;
                }
                // skip the next part (up to count elements)
                while (++index < endIndex && iter.MoveNext()) {
                    continue;
                }
                // yield the remainder of the sequence
                while (iter.MoveNext()) {
                    yield return iter.Current;
                }
            }
        }
        #endregion

        #region Index
        /// <summary>Returns a sequence of <c>KeyValuePair</c> where the key is the zero-based index
        /// of the value in the source sequence.</summary>
        public static IEnumerable<KeyValuePair<int, TSource>> Index<TSource>(
            this IEnumerable<TSource> source) => source.Index(0);

        /// <summary>Returns a sequence of <c>KeyValuePair</c> where the key is the index of the value
        /// in the source sequence. An additional parameter specifies the starting index.</summary>
        public static IEnumerable<KeyValuePair<int, TSource>> Index<TSource>(
            this IEnumerable<TSource> source, int startIndex) => source.Select((element, index) =>
                new KeyValuePair<int, TSource>(startIndex + index, element));
        #endregion

        #region Fold
        /// <summary>Returns the result of applying a function to a sequence of 1 element.</summary>
        public static TResult Fold<T, TResult>(this IEnumerable<T> source,
            Func<T, TResult> folder) => FoldImpl(source, 1, folder, null, null, null);

        /// <summary>Returns the result of applying a function to a sequence of 2 elements.</summary>
        public static TResult Fold<T, TResult>(this IEnumerable<T> source,
            Func<T, T, TResult> folder) => FoldImpl(source, 2, null, folder, null, null);

        /// <summary>Returns the result of applying a function to a sequence of 3 elements.</summary>
        public static TResult Fold<T, TResult>(this IEnumerable<T> source,
            Func<T, T, T, TResult> folder) => FoldImpl(source, 3, null, null, folder, null);

        /// <summary>Returns the result of applying a function to a sequence of 4 elements.</summary>
        public static TResult Fold<T, TResult>(this IEnumerable<T> source,
            Func<T, T, T, T, TResult> folder) => FoldImpl(source, 4, null, null, null, folder);

        static TResult FoldImpl<T, TResult>(IEnumerable<T> source, int count,
            Func<T, TResult> folder1,
            Func<T, T, TResult> folder2,
            Func<T, T, T, TResult> folder3,
            Func<T, T, T, T, TResult> folder4)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (count == 1 && folder1 == null
                || count == 2 && folder2 == null
                || count == 3 && folder3 == null
                || count == 4 && folder4 == null)
            {                                                // ReSharper disable NotResolvedInText
                throw new ArgumentNullException("folder");   // ReSharper restore NotResolvedInText
            }

            var elements = new T[count];
            foreach (var e in AssertCountImpl(
                source.Index(), count, OnFolderSourceSizeErrorSelector)) {
                elements[e.Key] = e.Value;
            }

            switch (count) {
                case 1: return folder1(elements[0]);
                case 2: return folder2(elements[0], elements[1]);
                case 3: return folder3(elements[0], elements[1], elements[2]);
                case 4: return folder4(elements[0], elements[1], elements[2], elements[3]);
                default: throw new NotSupportedException();
            }
        }

        static readonly Func<int, int, Exception> OnFolderSourceSizeErrorSelector = OnFolderSourceSizeError;

        static Exception OnFolderSourceSizeError(int cmp, int count)
        {
            var message = cmp < 0
                        ? "Sequence contains too few elements when exactly {0} {1} expected."
                        : "Sequence contains too many elements when exactly {0} {1} expected.";
            return new Exception(string.Format(message, count.ToString("N0"), count == 1 ? "was" : "were"));
        }
        #endregion

        /// <summary>Immediately executes the given action on each element in the source sequence.</summary>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (action == null) throw new ArgumentNullException(nameof(action));

            foreach (var element in source) {
                action(element);
            }
        }

        #region Pairwise
        /// <summary>Returns a sequence resulting from applying a function to each element in the
        /// source sequence and its predecessor, with the exception of the first element which is 
        /// only returned as the predecessor of the second element.</summary>
        public static IEnumerable<TResult> Pairwise<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TSource, TResult> resultSelector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));

            return PairwiseImpl(source, resultSelector);
        }

        static IEnumerable<TResult> PairwiseImpl<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TSource, TResult> resultSelector)
        {
            using (var e = source.GetEnumerator()) {
                if (!e.MoveNext()) {
                    yield break;
                }

                var previous = e.Current;
                while (e.MoveNext()) {
                    yield return resultSelector(previous, e.Current);
                    previous = e.Current;
                }
            }
        }
        #endregion

        # region ToDelimitedString
        /// <summary>Creates a delimited string from a sequence of values. The delimiter used depends
        /// on the current culture of the executing thread.</summary>
        public static string ToDelimitedString<TSource>(
            this IEnumerable<TSource> source) => ToDelimitedString(source, null);

        /// <summary>Creates a delimited string from a sequence of values and
        /// a given delimiter.</summary>
        public static string ToDelimitedString<TSource>(
            this IEnumerable<TSource> source, string delimiter)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return ToDelimitedStringImpl(source, delimiter, (sb, e) => sb.Append(e));
        }

        static string ToDelimitedStringImpl<T>(IEnumerable<T> source, string delimiter,
            Func<StringBuilder, T, StringBuilder> append)
        {
            delimiter = delimiter ?? CultureInfo.CurrentCulture.TextInfo.ListSeparator;
            var builder = new StringBuilder();
            var iterations = 0;
            foreach (var value in source) {
                if (iterations++ > 0) builder.Append(delimiter);
                append(builder, value);
            }
            return builder.ToString();
        }
        #endregion

        #region DistinctBy
        /// <summary>Returns all distinct elements of the given source, where "distinctness"
        /// is determined via a projection and the default equality comparer for the projected
        /// type.</summary>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            return source.DistinctBy(keySelector, null);
        }

        /// <summary>Returns all distinct elements of the given source, where "distinctness"
        /// is determined via a projection and the specified comparer for the projected type.</summary>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

            return _(); IEnumerable<TSource> _() {
                var knownKeys = new HashSet<TKey>(comparer);
                foreach (var element in source) {
                    if (knownKeys.Add(keySelector(element)))
                        yield return element;
                }
            }
        }
        #endregion

        #region Repeat
        /// <summary>Repeats the sequence the specified number of times.</summary>
        public static IEnumerable<T> Repeat<T>(this IEnumerable<T> source, int count)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count),
                "Repeat count must be greater than or equal to zero.");

            return RepeatImpl(source, count);
        }

        /// <summary>Repeats the sequence forever.</summary>
        public static IEnumerable<T> Repeat<T>(this IEnumerable<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return RepeatImpl(source, null);
        }

        static IEnumerable<T> RepeatImpl<T>(IEnumerable<T> source, int? count)
        {
            var memo = source.Materialize();
            using (memo as IDisposable)
            {
                while (count == null || count-- > 0)
                {
                    foreach (var item in memo)
                        yield return item;
                }
            }
        }
        #endregion

        /// <summary>Return everything except first element and throws exception if empty.</summary>
        public static IEnumerable<T> Tail<T>(this IEnumerable<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return _(); IEnumerable<T> _()
            {
                using (var e = source.GetEnumerator()) {
                    if (!e.MoveNext()) {
                        throw new ArgumentException(
                            "The input sequence has an insufficient number of elements.");
                    }
                    while (e.MoveNext()) {
                        yield return e.Current;
                    }
                }
            }
        }

        /// <summary>Return everything except first element without throwing exception if empty.</summary>
        public static IEnumerable<T> TailOrEmpty<T>(this IEnumerable<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return _(); IEnumerable<T> _()
            {
                using (var e = source.GetEnumerator()) {
                    if (e.MoveNext()) {
                        while (e.MoveNext()) {
                            yield return e.Current;
                        }
                    }
                }
            }
        }

        #region Materialize
        class MaterializedEnumerable<T> : IEnumerable<T>
        {
            readonly ICollection<T> _inner;

            internal MaterializedEnumerable(IEnumerable<T> enumerable) =>
                _inner = enumerable as ICollection<T> ?? enumerable.ToArray();

            public IEnumerator<T> GetEnumerator() => _inner.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        /// <summary>Captures the current state of a sequence.</summary>
        public static IEnumerable<T> Materialize<T>(this IEnumerable<T> source)
        {
            switch (source) {
                case null                        : throw new ArgumentNullException(nameof(source));
                case MaterializedEnumerable<T> _ : return source;
                default : return new MaterializedEnumerable<T>(source);
            }
        }
        #endregion

        /// <summary>Selects a random element.</summary>
        public static T Choice<T>(this IEnumerable<T> source)
        {
        if (source == null) throw new ArgumentNullException(nameof(source));

        #if CSX_REM_CRYPTORAND
            var index = new Random().Next(source.Count() - 1);
        #else
            var index = new CryptoRandom().Next(source.Count() - 1);
        #endif
            return source.ElementAt(index);
        }

        /// <summary>Takes an element and a sequence and `intersperses' that element between its
        /// elements.</summary>
        public static IEnumerable<T> Intersperse<T>(this IEnumerable<T> source, T element)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (element == null) throw new ArgumentNullException(nameof(element));

            return _(); IEnumerable<T> _()
            {
                var count = source.Count();
                var last = count - 1;
                for (var i = 0; i < count; i++) {
                    yield return source.ElementAt(i);
                    if (i != last) {
                        yield return element;
                    }
                }
            }
        }

        /// <summary>Flattens a sequence by one level.</summary>
        public static IEnumerable<T> FlattenOnce<T>(this IEnumerable<IEnumerable<T>> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return _(); IEnumerable<T> _()
            {
                foreach (var element in source) {
                    foreach (var subelement in element) {
                        yield return subelement;
                    }
                }
            }
        }

        #if !CSX_REM_MAYBE_FUNC
        /// <summary>Safe function that returns Just(first element) or Nothing.</summary>
        public static Maybe<T> TryHead<T>(this IEnumerable<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            using (var e = source.GetEnumerator()) {
                return e.MoveNext()
                    ? Maybe.Just(e.Current)
                    : Maybe.Nothing<T>();
            }
        }

        /// <summary>Safe function that returns Just(last element) or Nothing.</summary>
        public static Maybe<T> TryLast<T>(this IEnumerable<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            using (var e = source.GetEnumerator()) {
                if (!e.MoveNext()) return Maybe.Nothing<T>();
                T result = e.Current;
                while (e.MoveNext()) result = e.Current;
                return Maybe.Just(result);
            }
        }

        /// <summary>Turns an empty sequence to Nothing, otherwise Just(sequence).</summary>
        public static Maybe<IEnumerable<T>> ToMaybe<T>(this IEnumerable<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            using (var e = source.GetEnumerator()) {
                return e.MoveNext()
                    ? Maybe.Just(source)
                    : Maybe.Nothing<IEnumerable<T>>();
            }
        }

        /// <summary>Applies a function to each element of the source sequence and returns a new
        /// sequence of elements where the function returns Just(value).</summary>
        public static IEnumerable<TResult> Choose<T, TResult>(this IEnumerable<T> source,
            Func<T, Maybe<TResult>> chooser)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (chooser == null) throw new ArgumentNullException(nameof(chooser));

            return _(); IEnumerable<TResult> _()
            {
                foreach (var item in source) {
                    var result = chooser(item);
                    if (result.MatchJust(out TResult value)) {
                        yield return value;
                    }
                }
            }
        }
        #endif

        /// <summary>Partition a sequence in to chunks of given size. Each chunk is an array of the
        /// resulting sequence.</summary>
        public static IEnumerable<T[]> ChunkBySize<T>(this IEnumerable<T> source, int chunkSize)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (chunkSize <= 0) throw new ArgumentException("The input must be positive.");

            return _(); IEnumerable<T[]> _()
            {
                using (var e = source.GetEnumerator()) {
                    while (e.MoveNext()) {
                        var result = new T[chunkSize];
                        result[0] = e.Current;
                        var i = 1;
                        while (i < chunkSize && e.MoveNext()) {
                            result[i] = e.Current;
                            i++;
                        }
                        yield return i == chunkSize? result : SubArray(result, 0, i);
                    }
                }
            }

            T[] SubArray(T[] array, int index, int length) {
                T[] result = new T[length];
                Array.Copy(array, index, result, 0, length);
                return result;
            }
        }
    }
}