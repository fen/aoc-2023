namespace AdventOfCode.Solutions.Day13;

public class PartTwo : ISolution
{
    public int Day => 13;
    public int Part => 2;

    public async Task<string> RunAsync(FileInfo file) {
        string[] lines = await File.ReadAllLinesAsync(file.FullName);

        int result = 0;

        int start = 0, end = 0;
        while (end < lines.Length) {
            end = lines.IndexOf(l => l.Length == 0, start);
            if (end == -1) {
                end = lines.Length;
            }

            result += Grid.Parse(lines[start..end]).FindMirrorPoint();
            start = end + 1;
        }

        return result.ToString();
    }
}

file class Grid(char[,] grid, int rows, int columns)
{
    public int FindMirrorPoint() {
        bool smudge = true;
        var p1 = FindMirrorPoint(ref smudge, isVertical: true);
        if (p1 == -1 || smudge) {
            var p2 = FindMirrorPoint(ref smudge, isVertical: false) * 100;

            if (p2 != -100) {
                return p2;
            }
        }

        return p1;
    }

    bool IsMirrorVertical(int middle, ref bool smudge) {
        for (int row = 0; row < rows; row++) {
            for (int i = 1; i <= Math.Min(middle, columns - middle); i++) {
                var a = grid[row, middle - i];
                var b = grid[row, middle + (i-1)];
                if (a != b) {
                    if (smudge) {
                        smudge = false;
                    } else {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    bool IsMirrorHorizontal(int middle, ref bool smudge) {
        for (int column = 0; column < columns; column++) {
            for (int row = 1; row <= Math.Min(middle, rows - middle); row++) {
                var a = grid[middle - row, column];
                var b = grid[middle + (row-1), column];
                if (a != b) {
                    if (smudge) {
                        smudge = false;
                    } else {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    delegate bool IsMirrorDelegate(int middle, ref bool smudge);

    int FindMirrorPoint(ref bool smudge, bool isVertical = true) {
        int middle = isVertical ? columns / 2 : rows / 2;
        IsMirrorDelegate IsMirror = isVertical ? IsMirrorVertical : IsMirrorHorizontal;
        for (int i = middle; i >= 1; i--) {
            if (IsMirror(i, ref smudge)) {
                if (smudge) {
                    continue;
                }

                return i;
            }

            smudge = true;
        }

        for (int i = middle + 1; i <= (isVertical ? columns : rows) - 1; i++) {
            if (IsMirror(i, ref smudge)) {
                if (smudge) {
                    continue;
                }
                return i;
            }

            smudge = true;
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