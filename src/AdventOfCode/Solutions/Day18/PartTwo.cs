using System.Diagnostics;
using System.Globalization;
using Point = (long X, long Y);
using Direction = (long X, long Y);

namespace AdventOfCode.Solutions.Day18;

public class PartTwo : ISolution
{
    public int Day => 18;
    public int Part => 2;

    public async Task<string> RunAsync(FileInfo file) {
        var rawData = await File.ReadAllLinesAsync(file.FullName);
        var displacementVectors = ParseRawDataToDirectionVectors(rawData);
        var polygonVertices = ConvertDirectionVectorsToVertices(displacementVectors).ToList();

        double polygonArea = CalculatePolygonArea(polygonVertices);

        double boundary = 0.0;
        foreach (var point in displacementVectors) {
            boundary += Math.Max(Math.Abs(point.X), Math.Abs(point.Y));
        }

        double interior = polygonArea - boundary / 2 + 1;

        return (boundary + interior).ToString();
    }

    static List<Point> ParseRawDataToDirectionVectors(string[] lines) {
        return lines.Select(Parse).ToList();
    }

    static Point Parse(string input) {
        var (_, _, rawColor) = input.Split(' ');
        var (direction, distance) = HexToDistanceAndDirection(rawColor[1..^1]);
        return direction.Mul(distance);

        static Direction GetDirection(string value) {
            return value switch {
                "U" => (0, -1),
                "D" => (0, 1),
                "R" => (1, 0),
                "L" => (-1, 0),
                _ => throw new UnreachableException()
            };
        }

        static (Direction direction, int distance) HexToDistanceAndDirection(string hexValue) {
            var distanceHexString = hexValue.Substring(1, 5);

            Direction direction = hexValue[^1] switch {
                '0' => GetDirection("R"),
                '1' => GetDirection("D"),
                '2' => GetDirection("L"),
                '3' => GetDirection("U"),
                _ => throw new UnreachableException()
            };

            return new(direction, int.Parse(distanceHexString, NumberStyles.HexNumber));
        }
    }

    static IEnumerable<Point> ConvertDirectionVectorsToVertices(List<Point> displacementsVectors) {
        Point p = (0, 0);
        foreach (var vector in displacementsVectors) {
            p = p.Add(vector);
            yield return p;
        }
    }

    double CalculatePolygonArea(List<Point> points) {
        int n = points.Count;
        double area = 0.0;
        for (int i = 0; i < n; i++) {
            int j = (i + 1) % n;
            area += (points[i].X * points[j].Y) - (points[i].Y * points[j].X);
        }

        return Math.Abs(area) / 2.0;
    }
}

file static class Helpers
{
    public static Direction Mul(this Direction self, int distance) {
        return (self.X * distance, self.Y * distance);
    }

    public static Point Add(in this Point self, in Direction p) {
        return (self.X + p.X, self.Y + p.Y);
    }
}