using System.Text;

namespace AdventOfCode.Solutions.Day01;

public class PartOne : ISolution
{
    public int Day => 1;
    public int Part => 1;

    public async Task<string> RunAsync(FileInfo input)
    {
        List<int> numbers = [];

        StringBuilder sb = new();
        await foreach (var line in File.ReadLinesAsync(input.FullName))
        {
            sb.Clear();

            foreach (var c in line)
            {
                if (char.IsNumber(c))
                {
                    sb.Append(c);
                }
            }

            var value = sb.ToString();
            if (value.Length == 1)
            {
                value = $"{value}{value}";
                numbers.Add(int.Parse(value));
            }
            else
            {
                value = $"{value[0]}{value[^1]}";
                numbers.Add(int.Parse(value));
            }
        }

        int sum = 0;
        foreach (var number in numbers)
        {
            sum += number;
        }

        return sum.ToString();
    }
}
