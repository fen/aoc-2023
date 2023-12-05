namespace AdventOfCode.Solutions.Day04;

public class PartTwo : ISolution
{
    public int Day => 4;
    public int Part => 2;

    public async Task<string> RunAsync(FileInfo input) {
        var lines = await File.ReadAllLinesAsync(input.FullName);

        var cards = lines.Select(Card.Parse)
            .ToArray();

        for (int i = 0; i < cards.Length; i++) {
            var card = cards[i];

            if (card.Points == 0) {
                continue;
            }

            for (int j = i + 1; j <= i + card.Points; j++) {
                cards[j].Copies += card.Copies;
            }
        }

        return cards.Select(c => c.Copies).Sum().ToString();
    }
}

file class Card(int points)
{
    public int Points => points;
    public int Copies { get; set; } = 1;

    public static Card Parse(string line) {
        var (_, rawAllNumbers) = line.Split(':');
        var (rawWinningNumbers, rawNumbers) = rawAllNumbers.Trim().Split('|');

        var winningNumbers = rawWinningNumbers.Trim().Split(' ').Where(n => n != "").Select(int.Parse).ToHashSet();
        var numbers = rawNumbers.Trim().Split(' ').Where(n => n != "").Select(int.Parse);

        var points = 0;
        foreach (var number in numbers) {
            if (winningNumbers.Contains(number)) {
                points += 1;
            }
        }
        return new Card(
            points
        );
    }
}
