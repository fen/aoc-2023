namespace AdventOfCode;

internal static class Helpers
{
    public static void Deconstruct<T>(this T[] seq, out T first, out T second)
    {
        first = seq[0];
        second = seq.Length == 1 ? default! : seq[1];
    }
}
