using System.Diagnostics;

namespace AdventOfCode.Solutions.Day13;

public class PartOne : ISolution
{
    public int Day => 13;
    public int Part => 1;

    public async Task<string> RunAsync(FileInfo file) {
        string[] lines = await File.ReadAllLinesAsync(file.FullName);

        int result = 0;

        int start = 0, end = 0;
        while (end < lines.Length) {
            end = lines.IndexOf(l => l.Length == 0, start);
            if (end == -1) {
                end = lines.Length;
            }

            var grid = Grid.Parse(lines[start..end]);
            var x =  grid.Foo();
            Console.WriteLine(x);
            result += x;
            start = end + 1;
        }

        return result.ToString();
    }
}

class Grid(char[,] grid, int rows, int columns)
{
    public int Foo() {
        var point = FindMirrorPoint(isVertical: true);
        if (point == -1) {
            point = FindMirrorPoint(isVertical: false) * 100;
        }

        return point;
    }

    bool IsMirrorVertical(int middle) {
        for (int row = 0; row < rows; row++) {
            for (int i = 1; i <= Math.Min(middle, columns - middle); i++) {
                var a = grid[row, middle - i];
                var b = grid[row, middle + (i-1)];
                if (a != b) {
                    return false;
                }
            }
        }

        return true;
    }

    bool IsMirrorHorizontal(int middle) {
        for (int column = 0; column < columns; column++) {
            for (int row = 1; row <= Math.Min(middle, rows - middle); row++) {
                var a = grid[middle - row, column];
                var b = grid[middle + (row-1), column];
                if (a != b) {
                    return false;
                }
            }
        }

        return true;
    }

    int FindMirrorPoint(bool isVertical = true) {
        int middle = isVertical ? columns / 2 : rows / 2;
        Func<int, bool> IsMirror = isVertical ? IsMirrorVertical : IsMirrorHorizontal;
        for (int i = middle; i >= 1; i--) {
            if (IsMirror(i)) {
                return i;
            }
        }

        for (int i = middle + 1; i <= (isVertical ? columns : rows) - 1; i++) {
            if (IsMirror(i)) {
                return i;
            }
        }

        return -1;
    }

    public static Grid Parse(string[] input) {
        int rows = input.Length;
        int columns = input[0].Length;
        char[,] grid = new char[rows, columns];
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < columns; j++) {
                grid[i, j] = input[i][j];
            }
        }

        return new Grid(grid, rows, columns);
    }
}