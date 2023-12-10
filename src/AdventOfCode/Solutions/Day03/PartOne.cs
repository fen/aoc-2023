namespace AdventOfCode.Solutions.Day03;

using Coordinate = (int x, int y);

public class PartOne : ISolution
{
    public int Day => 3;
    public int Part => 1;

    public async Task<string> RunAsync(FileInfo file) {
        var lines = await File.ReadAllLinesAsync(file.FullName);
        return Grid.Parse(lines).FindNotLonelyNumbers()
            .Select(n => n.Value).Sum().ToString();
    }
}

file class Grid
{
    private readonly List<Number> _numbers = new();
    private readonly List<Symbol> _symbols = new();

    public IEnumerable<Number> FindNotLonelyNumbers() {
        foreach (var number in _numbers) {
            foreach (var symbol in _symbols) {
                if (symbol.IsAdjacent(number)) {
                    yield return number;
                    break;
                }
            }
        }
    }

    public static Grid Parse(string[] lines) {
        var grid = new Grid();
        foreach (var (line, row) in lines.EnumerateWithIndex()) {
            for (var column = 0; column < line.Length; column++) {
                if (char.IsNumber(line[column])) {
                    var number = Number.Parse(row, ref column, line);
                    grid._numbers.Add(number);
                    column -= 1;
                } else if (line[column] == '.') {
                    continue;
                } else {
                    var symbol = new Symbol(row, column);
                    grid._symbols.Add(symbol);
                }
            }
        }

        return grid;
    }
}

file struct Number(int row, int column, int length, int value)
{
    public int Row => row;
    public int Column => column;
    public int Length => length;
    public int Value => value;

    public static Number Parse(int row, ref int column, string line) {
        int start = column;
        while (column < line.Length && char.IsNumber(line[column])) {
            column++;
        }

        int length = column - start;
        int number = int.Parse(line.AsSpan(start, length));
        return new Number(row, start, length, number);
    }
}

file struct Symbol(int row, int column)
{
    public int Row => row;
    public int Column => column;

    public bool IsAdjacent(in Number number) {
        Coordinate[] leftSide = [(-1, 1), (-1, 0), (-1, -1)];
        if (Look(this, leftSide, number)) {
            return true;
        }

        int n = number.Length;
        Coordinate[] rightSide = [(n, 1), (n, 0), (n, -1)];
        if (Look(this, rightSide, number)) {
            return true;
        }

        var topBottom = Enumerable.Range(0, number.Length)
            .SelectMany(x => new Coordinate[] { (x, 1), (x, -1) });
        if (Look(this, topBottom, number)) {
            return true;
        }

        return false;

        bool Look(in Symbol symbol, IEnumerable<Coordinate> coordinates, in Number number) {
            foreach (var (x, y) in coordinates) {
                if (symbol.Row == number.Row + y && symbol.Column == number.Column + x) {
                    return true;
                }
            }

            return false;
        }
    }
}