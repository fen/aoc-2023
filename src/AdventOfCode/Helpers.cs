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