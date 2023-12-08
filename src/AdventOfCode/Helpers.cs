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
    public static string[] TrimSplit(this string self, char separator) {
        var splits = self.Split(separator);
        for (var i = 0; i < splits.Length; i++) {
            splits[i] = splits[i].Trim();
        }

        return splits;
    }

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

    public static IEnumerable<(T, int)> Iter<T>(this IEnumerable<T> items) {
        using var enumerator = items.GetEnumerator();
        for (int i = 0; enumerator.MoveNext(); i++) {
            yield return (enumerator.Current, i);
        }
    }

    /// <summary>
    /// Group IEnumerable sequence in a enumerable tuple of three items.
    /// </summary>
    public static IEnumerable<(T First, T Second)> ByTwo<T>(this IEnumerable<T> seq) {
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
}

static partial class Helpers
{
    public static int Product(this IEnumerable<int> source) => Product<int, int>(source);

    public static long Product(this IEnumerable<long> source) => Product<long, long>(source);

    static TResult Product<TSource, TResult>(this IEnumerable<TSource> source)
        where TSource : struct, INumber<TSource>
        where TResult : struct, INumber<TResult>
    {
        TResult sum = TResult.One;
        foreach (TSource value in source)
        {
            checked { sum *= TResult.CreateChecked(value); }
        }

        return sum;
    }
}

static partial class Helpers
{
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
