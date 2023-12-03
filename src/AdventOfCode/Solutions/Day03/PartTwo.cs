namespace AdventOfCode.Solutions.Day03;

public class PartTwo : ISolution
{
    public int Day => 3;
    public int Part => 2;

    public async Task<string> RunAsync(FileInfo input) {
        var lines = await File.ReadAllLinesAsync(input.FullName);
        var grid = Grid.Parse(lines);

        List<int> products = [];
        foreach (var gear in grid.FindAllGearSymbols()) {
            var numbers = grid.FindNumberAdjacentTo(gear).ToList();
            if (numbers.Count == 2) {
                products.Add(numbers[0].Value * numbers[1].Value);
            }
        }

        return products.Sum().ToString();
    }
}

file class Grid
{
    private readonly List<Number> _numbers = new();
    private readonly List<Symbol> _symbols = new();

    public IEnumerable<Symbol> FindAllGearSymbols() {
        foreach (var symbol in _symbols) {
            if (symbol.Character == '*') {
                yield return symbol;
            }
        }
    }

    public IEnumerable<Number> FindNumberAdjacentTo(Symbol symbol) {
        foreach (var number in _numbers) {
            if (number.IsAdjacentTo(symbol)) {
                yield return number;
            }
        }
    }

    public static Grid Parse(string[] lines) {
        var grid = new Grid();
        foreach (var (line, row) in lines.Iter()) {
            for (var column = 0; column < line.Length; column++) {
                if (char.IsNumber(line[column])) {
                    var number = Number.Parse(row, ref column, line);
                    grid._numbers.Add(number);
                    column -= 1;
                } else if (line[column] == '.') {
                    continue;
                } else {
                    var symbol = new Symbol(row, column, line[column]);
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

    public bool IsAdjacentTo(Symbol symbol) {
        if (Look(symbol, -1, 1, this) ||
            Look(symbol, -1, 0, this) ||
            Look(symbol, -1, -1, this)) {
            return true;
        }

        int n = this.Length;
        if (Look(symbol, n, 1, this) ||
            Look(symbol, n, 0, this) ||
            Look(symbol, n, -1, this)) {
            return true;
        }

        for (int x = 0; x < this.Length; x++) {
            if (Look(symbol, x, 1, this) ||
                Look(symbol, x, -1, this)) {
                return true;
            }
        }

        return false;

        static bool Look(Symbol symbol, int x, int y, Number number) {
            if (symbol.Row == number.Row + y && symbol.Column == number.Column + x) {
                return true;
            }

            return false;
        }
    }

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

file struct Symbol(int row, int column, char symbol)
{
    public int Row => row;
    public int Column => column;
    public char Character => symbol;
}