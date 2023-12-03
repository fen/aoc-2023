namespace AdventOfCode.Solutions.Day01;

public class PartTwo : ISolution
{
    static List<int> numbers = [];

    public int Day => 1;
    public int Part => 2;

    public async Task<string> RunAsync(FileInfo input) {
        var lines = await File.ReadAllLinesAsync(input.FullName);

        return lines.Select(static line => {
            GetNumbers(line);
            return numbers[0] * 10 + numbers[^1];
        }).Sum().ToString();
    }

    static string[] digits = ["one", "two", "three", "four", "five", "six", "seven", "eight", "nine"];

    static void GetNumbers(ReadOnlySpan<char> line) {
        numbers.Clear();

        while (line.Length > 0) {
            if (char.IsNumber(line[0])) {
                numbers.Add(int.Parse(line[0].ToString()));
            } else {
                for (var j = 0; j < digits.Length; j++) {
                    if (line.StartsWith(digits[j])) {
                        numbers.Add(j + 1);
                    }
                }
            }

            line = line.Slice(1);
        }
    }
}