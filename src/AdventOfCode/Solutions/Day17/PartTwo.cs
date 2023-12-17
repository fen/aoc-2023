using System.Diagnostics;
using Position = (int x, int y);
using Direction = (int x, int y);
using static AdventOfCode.Helpers;

namespace AdventOfCode.Solutions.Day17;

using static Solver;
using Map = int[][];

public class PartTwo : ISolution
{
    public int Day => 17;
    public int Part => 2;

    public async Task<string> RunAsync(FileInfo file) {
        var lines = await File.ReadAllLinesAsync(file.FullName);
        Map map = lines.Select(l => l.Select(c => c - '0').ToArray()).ToArray();
        return FindMinTotalPath(map).ToString();
    }
}

file static class Solver
{
    public static int FindMinTotalPath(Map map) {
        int n = map.Length;
        int m = map[0].Length;

        SortedSet<Crucible> minHeap = [
            new((0, 0), (1, 0), 0, 0),
            new((0, 0), (0, 1), 0, 0)
        ];

        HashSet<Crucible> seen = new(EqualityComparer<Crucible>(
                (l, r) => l.Position == r.Position && l.Direction == r.Direction && l.StraitMoves == r.StraitMoves,
                (l) => HashCode.Combine(l.Position, l.Direction, l.StraitMoves)
            )
        );

        while (minHeap.Count > 0) {
            var crucible = minHeap.Min;
            if (crucible.Position == (n - 1, m - 1) && crucible.StraitMoves >= 4) {
                return crucible.Cost;
            }

            minHeap.Remove(crucible);

            foreach (var next in Next(map, crucible)) {
                if (next.IsContained(n, m) && seen.Add(next)) {
                    minHeap.Add(next);
                }
            }
        }

        throw new UnreachableException();
    }

    static IEnumerable<Crucible> Next(Map map, Crucible crucible) {
        if (crucible.StraitMoves < 10) {
            var p = crucible.Position.Add(crucible.Direction);
            yield return crucible with {
                Position = p, Cost = crucible.Cost + map.Get(p), StraitMoves = crucible.StraitMoves + 1
            };
        }

        if (crucible.StraitMoves >= 4) {
            var a = (crucible.Direction.y, -crucible.Direction.x);
            var b = (-crucible.Direction.y, crucible.Direction.x);
            yield return new Crucible(crucible.Position.Add(a), a, crucible.Cost + map.Get(crucible.Position.Add(a)), 1);
            yield return new Crucible(crucible.Position.Add(b), b, crucible.Cost + map.Get(crucible.Position.Add(b)), 1);
        }
    }

    static bool IsContained(in this Crucible self, int n, int m) {
        var (x, y) = self.Position;
        if (x >= 0 && x < n && y >= 0 && y < m) {
            return true;
        } else {
            return false;
        }
    }
}

file record struct Crucible(Position Position, Direction Direction, int Cost, int StraitMoves) : IComparable<Crucible>
{
    public int CompareTo(Crucible other) {
        int c = Cost.CompareTo(other.Cost);
        if (c == 0) {
            return GetHashCode().CompareTo(other.GetHashCode());
        }

        return c;
    }
}

file static class Helpers
{
    public static Position Add(in this Position self, in Direction p) {
        return (self.x + p.x, self.y + p.y);
    }

    public static int Get(this Map self, in Position p) {
        if (p.x < 0 || p.x >= self[0].Length || p.y < 0 || p.y >= self.Length) {
            return 0;
        }

        return self[p.x][p.y];
    }
}