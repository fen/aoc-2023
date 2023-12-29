using System.Diagnostics;

namespace AdventOfCode.Solutions.Day21;

public class PartTwo : ISolution
{
    public int Day => 21;
    public int Part => 2;

    public async Task<string> RunAsync(FileInfo file) {
        var gardenMap = await ReadGardenMapAsync(file);

        // Quadratic interpolation is used to find the y (output) value for a given x (input) value based on three
        // known (x,y) pairs. More specifically, the three points are used to form a quadratic equation
        // (ax^2 + bx + c = y), and this equation is used to calculate the unknown y value for the given x value
        // using Newton's divided difference interpolation.
        var steps = Steps(gardenMap).Take(328).ToArray();

        double[] x = [65, 196, 327];
        double[] y = [steps[65], steps[196], steps[327]];

        double[] diffY = [
            (y[1] - y[0]) / (x[1] - x[0]),
            (y[2] - y[1]) / (x[2] - x[1])
        ];

        diffY = [
            ..diffY,
            (diffY[1] - diffY[0]) / (x[2] - x[0])
        ];

        var n = 26501365;
        long r = (long)(y[0] + diffY[0] * (n - x[0]) + diffY[2] * (n - x[0]) * (n - x[1]));

        return r.ToString();
    }

    static async Task<char[,]> ReadGardenMapAsync(FileInfo file) {
        var lines = await File.ReadAllLinesAsync(file.FullName);
        var garden = new char[lines.Length, lines[0].Length];
        for (int i = 0; i < lines.Length; i++) {
            for (int j = 0; j < lines[0].Length; j++) {
                garden[i, j] = lines[i][j];
            }
        }

        return garden;
    }

    static IEnumerable<long> Steps(char[,] garden) {
        HashSet<(long x, long y)> positions = [FindStart(garden)];
        while (true) {
            yield return positions.Count;
            if (positions.Count == 14838) {
                Debugger.Break();
            }

            positions = CalculateReachablePlots(garden, positions);
        }
    }

    static int[] dx = [0, -1, 0, 1];
    static int[] dy = [-1, 0, 1, 0];

    static HashSet<(long x, long y)> CalculateReachablePlots(char[,] garden, HashSet<(long x, long y)> positions) {
        HashSet<(long x, long y)> r = [];

        foreach (var current in positions) {
            for (int i = 0; i < 4; i++) {
                var newRow = current.x + dx[i];
                var newCol = current.y + dy[i];
                if (garden[ModuloWithPositiveRemainder(newRow, 131), ModuloWithPositiveRemainder(newCol, 131)] != '#') {
                    r.Add((newRow, newCol));
                }
            }
        }

        return r;

        static long ModuloWithPositiveRemainder(long a, int b) => ((a % b) + b) % b;
    }

    static (long x, long y) FindStart(char[,] garden) {
        int n = garden.GetLength(0);
        int m = garden.GetLength(1);
        for (int i = 0; i < n; i++) {
            for (int j = 0; j < m; j++) {
                if (garden[i, j] == 'S') {
                    return (i, j);
                }
            }
        }

        throw new UnreachableException();
    }
}