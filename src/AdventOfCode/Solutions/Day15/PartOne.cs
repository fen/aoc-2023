namespace AdventOfCode.Solutions.Day15;

public class PartOne : ISolution
{
    public int Day => 15;
    public int Part => 1;

    public async Task<string> RunAsync(FileInfo file) {
        var lines = await File.ReadAllLinesAsync(file.FullName);
        return lines.SelectMany(l => l.Split(','))
            .Select(HashFunction)
            .Sum()
            .ToString();
    }

    public static int HashFunction(string str) {
        int asciiCode;
        int currentValue = 0;

        for (int i = 0; i < str.Length; i++) {
            asciiCode = (int)str[i];
            currentValue += asciiCode;
            currentValue *= 17;
            currentValue %= 256;
        }

        return currentValue;
    }
}