using System.Collections.Immutable;
using Node = long;
using Position = (int X, int Y);

namespace AdventOfCode.Solutions.Day23;

using Map = ImmutableDictionary<Position, char>;
using Edge = (Node start, Node end, int distancec);

public class PartOne : ISolution
{
    public int Day => 23;
    public int Part => 1;

    public async Task<string> RunAsync(FileInfo file) {
        var lines = await File.ReadAllLinesAsync(file.FullName);
        Map map = (0, 0).Range((lines.Length, lines[0].Length))
            .ToImmutableDictionary(p => (Position)p, p => lines[p.Item2][p.Item1]);

        var positions = map.Select(m => m.Key)
            .OrderBy(p => p.Y)
            .ThenBy(p => p.X)
            .Where(p => map.IsFree(p) && !map.IsRoad(p))
            .ToArray();

        var nodes = new Node[positions.Length];
        List<Edge> edges = [];
        for (int i = 0; i < positions.Length; i++) {
            nodes[i] = Pow(i);
            for (var j = 0; j < positions.Length; j++) {
                if (i == j) {
                    continue;
                }

                int distance = map.Distance(positions[i], positions[j]);
                if (distance > 0) {
                    edges.Add((Pow(i), Pow(j), distance));
                }
            }
        }

        var start = nodes.First();
        var end = nodes.Last();

        return LongestPath(edges, end, start, 0).ToString();

        static long Pow(int i) => 1L << (i - 1);
    }

    Dictionary<(Node, long), int> cache = new();

    int LongestPath(List<Edge> edges, Node end, Node node, long visited) {
        if (node == end) {
            return 0;
        } else if ((visited & node) != 0) {
            return int.MinValue;
        }

        var key = (node, visited);
        if (!cache.ContainsKey(key)) {
            cache[key] = edges
                .Where(e => e.start == node)
                .Select(e => e.distancec + LongestPath(edges, end, e.end, visited | node))
                .Max();
        }

        return cache[key];
    }
}

file static class Methods
{
    public static readonly Position Up = (0, -1);
    public static readonly Position Down = (0, 1);
    public static readonly Position Left = (-1, 0);
    public static readonly Position Right = (1, 0);

    public static Position[] Directions = [Up, Down, Left, Right];

    static Dictionary<char, Position[]> Exits = new() {
        ['<'] = [Left],
        ['>'] = [Right],
        ['^'] = [Up],
        ['v'] = [Down],
        ['.'] = [Up, Down, Left, Right],
        ['#'] = []
    };

    public static int Distance(this Map map, Position a, Position b) {
        var q = new Queue<(Position, int)>();
        q.Enqueue((a, 0));

        HashSet<Position> visited = [a];
        while (q.Any()) {
            var (pos, dist) = q.Dequeue();
            foreach (var dir in Exits[map[pos]]) {
                var posT = pos.Add(dir);
                if (posT == b) {
                    return dist + 1;
                } else if (map.IsRoad(posT) && !visited.Contains(posT)) {
                    visited.Add(posT);
                    q.Enqueue((posT, dist + 1));
                }
            }
        }

        return -1;
    }

    public static Position Add(this Position self, Position other) {
        return (self.X + other.X, self.Y + other.Y);
    }

    public static bool IsFree(this Map map, Position p) {
        return map.ContainsKey(p) && map[p] != '#';
    }

    public static bool IsRoad(this Map map, Position p) {
        return Directions.Count(d => IsFree(map, p.Add(d))) == 2;
    }
}