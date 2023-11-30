namespace AdventOfCode.Solutions.Day01;

public class PartOne : ISolution
{
    public async Task RunAsync(FileInfo input)
    {
        var text = await File.ReadAllTextAsync(input.FullName);
        Console.WriteLine(text);
    }
}
