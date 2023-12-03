namespace AdventOfCode;

internal static class Helpers
{
    public static void Deconstruct<T>(this T[] seq, out T first, out T second) {
        first = seq[0];
        second = seq.Length == 1 ? default! : seq[1];
    }

    public static IEnumerable<(T, int)> Iter<T>(this IEnumerable<T> items) {
        using var enumerator = items.GetEnumerator();
        for (int i = 0; enumerator.MoveNext(); i++) {
            yield return (enumerator.Current, i);
        }
    }
}