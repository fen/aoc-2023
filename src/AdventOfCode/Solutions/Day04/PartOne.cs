namespace AdventOfCode.Solutions.Day04;

public class PartOne : ISolution
{
    public int Day => 4;
    public int Part => 1;

    public async Task<string> RunAsync(FileInfo input) {
        var lines = await File.ReadAllLinesAsync(input.FullName);
        var cards = lines.Select(Card.Parse).ToArray();
        return cards.Select(c => c.Points).Sum().ToString();
    }
}

file class Card(int points)
{
    public int Points => points;

    public static Card Parse(string line) {
        var (_, rawAllNumbers) = line.Split(':');
        var (rawWinningNumbers, rawNumbers) = rawAllNumbers.Trim().Split('|');

        var winningNumbers = rawWinningNumbers.Trim().Split(' ').Where(n => n != "").Select(int.Parse).ToHashSet();
        var numbers = rawNumbers.Trim().Split(' ').Where(n => n != "").Select(int.Parse);

        var points = 0;
        foreach (var number in numbers) {
            if (winningNumbers.Contains(number)) {
                if (points == 0) {
                    points = 1;
                } else {
                    points *= 2;
                }
            }
        }

        return new Card(
            points
        );
    }
}