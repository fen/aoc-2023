using System.Numerics;
using System.Text;

namespace AdventOfCode;

static partial class Helpers
{
    public static void Deconstruct<T>(this T[] seq, out T first, out T second) {
        first = seq[0];
        second = seq.Length == 1 ? default! : seq[1];
    }

    public static void Deconstruct<T>(this T[] seq, out T first, out T second, out T third) {
        first = seq[0];
        second = seq.Length == 1 ? default! : seq[1];
        third = seq.Length == 2 ? default! : seq[2];
    }
}

static partial class Helpers
{
    public static IEnumerable<(T, T)> Range<T>(this (T, T) from, (T, T) to) where T : INumber<T> {
        var (fromRow, fromColumn) = from;
        var (toRow, toColumn) = to;
        for (var row = fromRow; row < toRow; row += T.One) {
            for (var column = fromColumn; column < toColumn; column += T.One) {
                yield return (column, row);
            }
        }
    }

    /// <summary>
    /// Creates a new instance of the <see cref="IEqualityComparer{T}"/> interface
    /// using the specified functions for determining equality and calculating hash codes. </summary>
    /// <typeparam name="T">The type of objects to compare.</typeparam>
    /// <param name="equals">A function that compares two objects for equality.</param>
    /// <param name="getHashCode">A function that calculates the hash code for an object.</param>
    /// <returns>
    /// A new instance of the <see cref="IEqualityComparer{T}"/> interface that uses the specified
    /// functions for equality comparison and hash code calculation.
    /// </returns>
    public static IEqualityComparer<T> EqualityComparer<T>(Func<T, T, bool> equals, Func<T, int> getHashCode) {
        return new InternalEqualityComparer<T>(equals, getHashCode);
    }

    private readonly struct InternalEqualityComparer<T>(Func<T, T, bool> equals, Func<T, int> getHashCode) : IEqualityComparer<T>
    {
        public bool Equals(T? x, T? y) {
            if (x == null && y == null) {
                return true;
            }

            if (x == null || y == null) {
                return false;
            }

            return equals(x, y);
        }

        public int GetHashCode(T obj) {
            return getHashCode(obj);
        }
    }

    /// <summary>
    /// Searches for the first occurrence of an element in the sequence that satisfies a specified condition and returns its index.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the sequence.</typeparam>
    /// <param name="source">The sequence to search in.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="startIndex">The zero-based starting index for the search. Default is 0.</param>
    /// <returns>The index of the first occurrence of an element that satisfies the condition; otherwise, -1.</returns>
    public static int IndexOf<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, int startIndex = 0) {
        int index = 0;
        foreach (TSource element in source) {
            if (index <= startIndex) {
                index += 1;
                continue;
            }

            if (predicate(element)) {
                return index;
            }

            index += 1;
        }

        return -1;
    }

    /// <summary>
    /// Searches for the specified element in a sequence and returns the zero-based index of the first occurrence, using the specified comparer to compare elements for equality.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the sequence.</typeparam>
    /// <param name="source">The source sequence to search in.</param>
    /// <param name="locate">The element to locate in the sequence.</param>
    /// <param name="comparer">The equality comparer to use.</param>
    /// <returns>
    /// The zero-based index of the first occurrence of the element in the sequence, if found; otherwise, -1.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="comparer"/> is null.</exception>
    /// <remarks>
    /// This method enumerates the source sequence until the specified element is found or the end of the sequence is reached.
    /// </remarks>
    public static int IndexOf<TSource>(this IEnumerable<TSource> source, TSource locate, IEqualityComparer<TSource> comparer) where TSource : notnull {
        int index = 0;
        foreach (TSource element in source) {
            if (comparer.GetHashCode(locate) == comparer.GetHashCode(element) && comparer.Equals(locate, element)) {
                return index;
            }

            index += 1;
        }

        return -1;
    }

    /// <summary>
    /// Counts the number of elements in the sequence that satisfy a condition, until the condition is false.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the sequence.</typeparam>
    /// <param name="source">The sequence to count elements from.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>Returns the number of elements in the sequence until the condition becomes false.</returns>
    public static int CountWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) {
        int count = 0;
        foreach (TSource element in source) {
            if (!predicate(element)) {
                return count;
            }

            count += 1;
        }

        return count;
    }

    /// <summary>
    /// Splits the given string by the specified separator character and trims each element.
    /// </summary>
    /// <param name="self">The input string to be split and trimmed.</param>
    /// <param name="separator">The separator character used to split the string.</param>
    /// <returns>An array of strings, where each element is a trimmed substring from the original input string.</returns>
    public static string[] SplitAndTrimEach(this string self, char separator) {
        var splits = self.Split(separator);
        for (var i = 0; i < splits.Length; i++) {
            splits[i] = splits[i].Trim();
        }

        return splits;
    }

    /// <summary>
    /// Returns the first and last elements of the given sequence.
    /// </summary>
    /// <typeparam name="T">The type of elements in the sequence.</typeparam>
    /// <param name="items">The sequence of elements.</param>
    /// <returns>A tuple consisting of the first and last elements of the sequence.</returns>
    public static (T first, T last) FirstAndLast<T>(this IEnumerable<T> items) {
        using var enumerator = items.GetEnumerator();
        enumerator.MoveNext();
        var first = enumerator.Current;
        T last = first;
        while (enumerator.MoveNext()) {
            last = enumerator.Current;
        }

        return (first, last);
    }

    /// <summary>
    /// Enumerates over the given collection while also providing the index of each item.
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    /// <param name="items">The collection to enumerate.</param>
    /// <returns>An enumerable of tuples containing each item in the collection along with its index.</returns>
    public static IEnumerable<(T, int)> EnumerateWithIndex<T>(this IEnumerable<T> items) {
        using var enumerator = items.GetEnumerator();
        for (int i = 0; enumerator.MoveNext(); i++) {
            yield return (enumerator.Current, i);
        }
    }

    /// <summary>
    /// Returns an enumerable sequence of tuples where each tuple contains two consecutive elements from the input sequence.
    /// </summary>
    /// <typeparam name="T">The type of elements in the sequence.</typeparam>
    /// <param name="seq">The input sequence.</param>
    /// <returns>An enumerable sequence of tuples where each tuple contains two consecutive elements from the input sequence.</returns>
    /// <remarks>
    /// The method returns tuples of the form <c>(T First, T Second)</c> where <c>First</c> corresponds to the first element
    /// and <c>Second</c> corresponds to the second element.
    /// The method internally uses an enumerator to iterate through the input sequence, retrieving two consecutive elements
    /// at a time until there are no more elements left. If the input sequence is empty or has an odd number of elements,
    /// no tuple will be returned.
    /// </remarks>
    public static IEnumerable<(T first, T second)> ByTwo<T>(this IEnumerable<T> seq) {
        using var enumerator = seq.GetEnumerator();
        do {
            if (!enumerator.MoveNext()) {
                yield break;
            }

            var first = enumerator.Current;
            enumerator.MoveNext();
            var second = enumerator.Current;
            yield return (first, second);
        } while (true);
    }

    /// <summary>
    /// Generates adjacent pairs of elements from a given sequence.
    /// </summary>
    /// <typeparam name="T">The type of elements in the sequence.</typeparam>
    /// <param name="seq">The sequence of elements.</param>
    /// <returns>An enumerable of tuples representing adjacent pairs of elements.</returns>
    public static IEnumerable<(T first, T second)> AdjacentPairs<T>(this IEnumerable<T> seq) {
        using var enumerator = seq.GetEnumerator();
        enumerator.MoveNext();
        var first = enumerator.Current;
        do {
            if (!enumerator.MoveNext()) {
                yield break;
            }

            var second = enumerator.Current;
            yield return (first, second);
            first = second;
        } while (true);
    }
}

static partial class Helpers
{
    /// <summary>
    /// Calculates the product of all elements in the given sequence.
    /// </summary>
    /// <param name="source">The sequence of elements</param>
    /// <returns>The product of all elements in the sequence</returns>
    public static int Product(this IEnumerable<int> source) => Product<int, int>(source);

    /// <summary>
    /// Calculates the product of all elements in the specified sequence.
    /// </summary>
    /// <param name="source">The sequence to calculate the product for.</param>
    /// <returns>The product of all elements in the sequence.</returns>
    public static long Product(this IEnumerable<long> source) => Product<long, long>(source);

    static TResult Product<TSource, TResult>(this IEnumerable<TSource> source)
        where TSource : struct, INumber<TSource>
        where TResult : struct, INumber<TResult> {
        TResult sum = TResult.One;
        foreach (TSource value in source) {
            checked { sum *= TResult.CreateChecked(value); }
        }

        return sum;
    }
}

static partial class Helpers
{
    /// <summary>
    /// Converts a TimeSpan object to a human-readable string representation.
    /// </summary>
    /// <param name="timeSpan">The TimeSpan object to convert.</param>
    /// <returns>A string representation of the TimeSpan object in days, hours, minutes, seconds, and milliseconds.</returns>
    public static string ToReadableString(this TimeSpan timeSpan) {
        var stringBuilder = new StringBuilder();

        if (timeSpan.Days > 1) {
            stringBuilder.Append($"{timeSpan.Days} d, ");
        } else if (timeSpan.Days > 0) {
            stringBuilder.Append("1 d, ");
        }

        if (timeSpan.Hours > 1) {
            stringBuilder.Append($"{timeSpan.Hours} h, ");
        } else if (timeSpan.Hours > 0) {
            stringBuilder.Append("1 h, ");
        }

        if (timeSpan.Minutes > 1) {
            stringBuilder.Append($"{timeSpan.Minutes} m, ");
        } else if (timeSpan.Minutes > 0) {
            stringBuilder.Append("1 m, ");
        }

        if (timeSpan.Seconds > 1) {
            stringBuilder.Append($"{timeSpan.Seconds} s, ");
        } else if (timeSpan.Seconds > 0) {
            stringBuilder.Append("1 s, ");
        }

        if (timeSpan.Milliseconds > 1) {
            stringBuilder.Append($"{timeSpan.Milliseconds} ms, ");
        } else if (timeSpan.Milliseconds > 0) {
            stringBuilder.Append("1 ms");
        }

        return stringBuilder
            .ToString()
            .TrimEnd(", ")
            .ReplaceLastOccurrence(", ", " and ");
    }

    static string TrimEnd(this string source, string value) {
        return source.EndsWith(value)
            ? source.Remove(source.LastIndexOf(value, StringComparison.Ordinal))
            : source;
    }

    static string ReplaceLastOccurrence(this string source, string find, string replace) {
        var index = source.LastIndexOf(find, StringComparison.Ordinal);

        if (index == -1) {
            return source;
        }

        return source.Remove(index, find.Length).Insert(index, replace);
    }
}