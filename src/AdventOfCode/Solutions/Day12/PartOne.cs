namespace AdventOfCode.Solutions.Day12;

public class PartOne : ISolution
{
    public int Day => 12;
    public int Part => 1;

    public async Task<string> RunAsync(FileInfo file) {
        var lines = await File.ReadAllLinesAsync(file.FullName);

        int result = 0;
        foreach (var line in lines) {
            var (springStatus, groupInfo) = line.Split(' ');
            result += CountMatchingCombinations(springStatus, groupInfo);
        }

        return result.ToString();
    }

    int CountMatchingCombinations(string springStatus, string groupInfo) {
        int[] groupSizes = groupInfo.Split(',').Select(int.Parse).ToArray();

        var foo = GenerateCombinations(springStatus);
        var count = foo.Where(f => IsMatch(f, groupSizes)).ToArray();
        return count.Length;

        static bool IsMatch(string springStatus, int[] groupSizes) {
            var groupings = springStatus.Split('.')
                .Where(s => !string.IsNullOrEmpty(s))
                .Select(s => s.Length)
                .ToArray();
            return groupSizes.SequenceEqual(groupings);
        }
    }

    static List<string> GenerateCombinations(string input) {
        List<string> combinations = new List<string>();
        Queue<string> processQueue = new Queue<string>();
        processQueue.Enqueue(input);

        while (processQueue.Count != 0) {
            string nextPattern = processQueue.Dequeue();
            int charLocation = nextPattern.IndexOf('?');

            if (charLocation != -1) {
                processQueue.Enqueue(nextPattern.Remove(charLocation, 1).Insert(charLocation, "#"));
                processQueue.Enqueue(nextPattern.Remove(charLocation, 1).Insert(charLocation, "."));
            } else {
                combinations.Add(nextPattern);
            }
        }

        return combinations;
    }
}