using Point = (int X, int Y);

namespace AdventOfCode.Solutions.Day10;

using Map = Dictionary<Point, char>;

public class PartTwo : ISolution
{
    public int Day => 10;
    public int Part => 2;

    public async Task<string> RunAsync(FileInfo file) {
        var input = await File.ReadAllLinesAsync(file.FullName);
        var map = ParseMap(input);
        var loop = FindLoopPositionsInMap(map);
        CleanupMap(map, loop);
        return map.Keys.Count(position => IsEnclosed(map, position)).ToString();
    }

    static readonly Point Up = (0, -1);
    static readonly Point Down = (0, 1);
    static readonly Point Left = (-1, 0);
    static readonly Point Right = (1, 0);
    static readonly Point[] Directions = [Up, Right, Down, Left];

    static readonly Dictionary<char, Point[]> DirectionMapping = new() {
        { '7', [Left, Down] },
        { 'F', [Right, Down] },
        { 'L', [Up, Right] },
        { 'J', [Up, Left] },
        { '|', [Up, Down] },
        { '-', [Left, Right] },
        { 'S', [Up, Down, Left, Right] },
    };

    Map ParseMap(string[] input) {
        int rows = input.Length;
        int columns = input[0].Length;
        Map map = new();
        for (int row = 0; row < rows; row++) {
            for (int column = 0; column < columns; column++) {
                map[(column, row)] = input[row][column];
            }
        }

        return map;
    }

    HashSet<Point> FindLoopPositionsInMap(Map map) {
        var startPosition = map.Keys.First(p => map[p] == 'S');

        var startDirection = Directions
            .First(dir => DirectionMapping[map[Add(startPosition, dir)]].Contains(Negate(dir)));

        Point position = startPosition;
        Point direction = startDirection;
        HashSet<Point> positions = [];
        // Keep finding loop points until the start point is reached again
        for (;;) {
            positions.Add(position);
            position = Add(position, direction);
            if (map[position] == 'S') {
                break;
            }

            direction = DirectionMapping[map[position]]
                .Single(dirOut => dirOut != Negate(direction));
        }

        return positions;
    }

    void CleanupMap(Map map, HashSet<Point> loop) {
        foreach (Point point in map.Keys) {
            if (!loop.Contains(point)) {
                map[point] = GroundChar;
            }
        }
    }

    const char GroundChar = '.';

    /// <summary>
    /// Point-in-polygon algorithm using even-odd rule analysis.
    ///
    /// 1-dimensional even-odd rule (or winding number algorithm) considers a point to be
    /// interior if an infinitely long ray beginning at the point and extending in any
    /// direction crosses an odd number of polygon edges. If it crosses an even number of edges,
    /// it is considered to be situated outside of the polygon. This is premised on the concept
    /// that each time you cross a boundary, you transition from inside to outside or vice versa.
    ///
    /// So, in the method <see cref="IsEnclosed"/>, the pipeline characters ('S', 'J', '|', or 'L')
    /// are considered boundaries. While moving left across the line, we count the number of crossings
    /// (flips of the inside variable). If we end with inside set to true, we must have crossed an odd
    /// number of boundaries, so we were inside an enclosure.
    ///
    /// </summary>
    bool IsEnclosed(Map map, Point position) {
        if (map[position] != GroundChar) {
            return false;
        }

        // You move leftwards through the map, every time you cross a pipe
        // opening towards east or west ('S', 'J', 'L', '|'), you transition
        // from 'inside to outside' or 'outside to inside'.
        // The method considers 'S', 'J', 'L', and '|' as pipe sections which
        // basically mark the boundaries of an enclosed ground.
        var inside = false;
        position = MoveLeft(position);
        while (map.ContainsKey(position)) {
            if ("SJL|".Contains(map[position])) {
                inside = !inside;
            }

            position = MoveLeft(position);
        }

        // After this process, you are left with an inside flag.
        // If it is true, then the original position was on enclosed ground
        // (it was 'inside'). If false, it was not enclosed (it was 'outside').
        return inside;
    }

    static Point MoveLeft(Point p) {
        return (p.X-1, p.Y);
    }

    static Point Add(Point l, Point r) {
        return (l.X + r.X, l.Y + r.Y);
    }

    static Point Negate(Point p) {
        return (-p.X, -p.Y);
    }
}