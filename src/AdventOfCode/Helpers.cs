namespace AdventOfCode;

internal static class Helpers
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

    public static (T first, T last) FirstAndLast<T>(this IEnumerable<T> items) {
        using var enumerator = items.GetEnumerator();
        enumerator.MoveNext();
        var first = enumerator.Current;
        T last = default;
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
    public static IEnumerable<(T First, T Second)> ByTwo<T>(this IEnumerable<T> seq)
    {
        using var enumerator = seq.GetEnumerator();
        do {
            if (!enumerator.MoveNext()) yield break;
            var first = enumerator.Current;
            enumerator.MoveNext();
            var second = enumerator.Current;
            yield return (first, second);
        } while (true);
    }
}