using System.Diagnostics;
using Position = (int x, int y);
using Direction = (int x, int y);
using Color = string;

namespace AdventOfCode.Solutions.Day18;

using Map = Dictionary<Position, (bool digged, Color color)>;

public class PartOne : ISolution
{
    public int Day => 18;
    public int Part => 1;

    public async Task<string> RunAsync(FileInfo file) {
        var lines = await File.ReadAllLinesAsync(file.FullName);
        DigInstruction[] instructions = lines.Select(DigInstruction.Parse).ToArray();

        Map map = new();
        Position start = (0, 0);
        foreach (var instruction in instructions) {
            start = instruction.Execute(map, start);
        }

        {
            var minX = map.Keys.Select(p => p.x).Min();
            var minY = map.Keys.Select(p => p.y).Min();

            var maxX = map.Keys.Select(p => p.x).Max();
            var maxY = map.Keys.Select(p => p.y).Max();

            for (int i = minY; i <= maxY; i++) {
                for (int j = minX; j <= maxX; j++) {
                    if (map.ContainsKey((j, i))) {
                    } else {
                        map[(j, i)] = (false, "");
                    }
                }
            }
        }

        FloodFill(map, (1, 1));

        return map.Values.Count(v => v.digged).ToString();
    }


    void FloodFill(Map map, Position start) {
        Stack<Position> stack = new Stack<Position>();
        stack.Push(start);

        while (stack.Count > 0) {
            Position current = stack.Pop();

            if (!map.ContainsKey(current) || map[current].digged) {
                continue;
            }

            map[current] = (true, "");

            stack.Push(new Position(current.x + 1, current.y)); // Right
            stack.Push(new Position(current.x - 1, current.y)); // Left
            stack.Push(new Position(current.x, current.y + 1)); // Up
            stack.Push(new Position(current.x, current.y - 1)); // Down
        }
    }
}

file record struct DigInstruction(Direction Direction, int Steps, string Color)
{
    public static DigInstruction Parse(string input) {
        var (rawDirection, rawLength, rawColor) = input.Split(' ');

        return new(
            GetDirection(rawDirection),
            int.Parse(rawLength),
            rawColor[1..^1]
        );

        static Direction GetDirection(string value) {
            return value switch {
                "U" => (0, -1),
                "D" => (0, 1),
                "R" => (1, 0),
                "L" => (-1, 0),
                _ => throw new UnreachableException()
            };
        }
    }

    public Position Execute(Map map, Position start) {
        Position end = start;
        for (int step = 0; step < Steps; step++) {
            end = end.Add(Direction);
            map[end] = (true, Color);
        }

        return end;
    }
}

file static class Helpers
{
    public static Position Add(in this Position self, in Direction p) {
        return (self.x + p.x, self.y + p.y);
    }
}