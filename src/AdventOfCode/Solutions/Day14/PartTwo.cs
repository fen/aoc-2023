using System.Collections.Immutable;
using Point = (int x, int y);

namespace AdventOfCode.Solutions.Day14;

using Map = Dictionary<Point, char>;
using Lookup = HashSet<Point>;
using ImmutableLookup = ImmutableHashSet<Point>;
using Points = IEnumerable<Point>;

public class PartTwo : ISolution
{
    public int Day => 14;
    public int Part => 2;

    private readonly Dictionary<ImmutableLookup, ImmutableLookup> _cache =
        new(new CacheEqualityComparer());

    public async Task<string> RunAsync(FileInfo file) {
        var input = await File.ReadAllLinesAsync(file.FullName);
        rows = input.Length;
        columns = input[0].Length;

        var (map, lookup) = Parse(input);

        List<ImmutableLookup> iterations = new List<ImmutableLookup>();
        for (int i = 0; i < 1_000_000_000; i++) {
            if (_cache.TryGetValue(lookup, out var found)) {
                // Calculate the start and length of the loop
                int loopStart = iterations.IndexOf(lookup, new CacheEqualityComparer());
                int loopSize = iterations.Count - loopStart;

                // Calculate the index corresponding to the billionth step
                int indexInLoop = (1_000_000_000 - loopStart) % loopSize;

                var finalLookup = _cache[iterations[loopStart + indexInLoop - 1]];
                var result = Calc(finalLookup);

                return result.ToString();
            } else {
                var seed = lookup;
                iterations.Add(seed);
                lookup = Tilt(Direction.North, map, lookup);
                lookup = Tilt(Direction.West, map, lookup);
                lookup = Tilt(Direction.South, map, lookup);
                lookup = Tilt(Direction.East, map, lookup);
                _cache[seed] = lookup;
            }
        }

        return "";
    }

    int Calc(ImmutableLookup lookup) {
        int result = 0;
        foreach (Point p in lookup) {
            result += p.x + 1;
        }

        return result;
    }

    int rows;
    int columns;

    const char RoundRockChar = 'O';
    const char CubeRockChar = '#';
    const char EmptySpaceChar = '.';

    static readonly Point North = (1, 0);
    static readonly Point South = (-1, 0);
    static readonly Point West = (0, -1);
    static readonly Point East = (0, 1);

    private static readonly Dictionary<Direction, (Point direction, Func<Points, Points> order)> Directions = new() {
        { Direction.North, (North, it => it.OrderByDescending(p => p.x)) },
        { Direction.South, (South, it => it.OrderBy(p => p.x)) },
        { Direction.West, (West, it => it.OrderBy(p => p.y)) },
        { Direction.East, (East, it => it.OrderByDescending(p => p.y)) }
    };

    void Print(Map map) {
        for (int i = rows - 1; i >= 0; i--) {
            for (int j = 0; j < columns; j++) {
                Console.Write(map[(i, j)]);
            }

            Console.WriteLine();
        }

        Console.WriteLine();
    }

    ImmutableLookup Tilt(Direction direction, Map map, ImmutableLookup rocks) {
        var (d, order) = Directions[direction];
        Lookup newRockLookup = new();
        foreach (Point rock in order(rocks)) {
            var p = Move(rock, d, map);
            map[rock] = EmptySpaceChar;
            map[p] = RoundRockChar;
            newRockLookup.Add(p);
        }

        return (newRockLookup.ToImmutableHashSet());

        Point Move(Point rock, Point direction, Map map) {
            var position = rock;
            while (true) {
                var p = position.Add(direction);
                if (p.x >= 0 && p.y >= 0 && p.x < rows && p.y < columns && map[p] is EmptySpaceChar) {
                    position = p;
                } else {
                    break;
                }
            }

            return position;
        }
    }

    static (Map, ImmutableLookup) Parse(string[] input) {
        Map map = new();
        Lookup lookup = new();
        int rows = input.Length;
        int columns = input[0].Length;
        char[,] grid = new char[rows, columns];
        for (int i = 0, x = rows - 1; i < rows; i++, x--) {
            for (int j = 0; j < columns; j++) {
                char c = input[i][j];
                if (c == 'O') {
                    lookup.Add((x, j));
                }

                map[(x, j)] = c;
            }
        }

        return (map, lookup.ToImmutableHashSet());
    }

    enum Direction
    {
        North = 1,
        South,
        West,
        East
    }
}

file static class Helpers
{
    public static Point Add(this Point self, Point other) {
        return (self.x + other.x, self.y + other.y);
    }
}

file class CacheEqualityComparer : IEqualityComparer<ImmutableLookup>
{
    public bool Equals(ImmutableLookup? x, ImmutableLookup? y) {
        if (x == null || y == null) {
            return false;
        }

        return x.SetEquals(y);
    }

    public int GetHashCode(ImmutableLookup obj) {
        return obj.Order().Aggregate(17, static (current, item) => {
            unchecked {
                return current * 23 + HashCode.Combine(item.x, item.y);
            }
        });
    }
}