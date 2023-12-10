namespace AdventOfCode.Solutions.Day09;

public class PartOne : ISolution
{
    public int Day => 9;
    public int Part => 1;

    public async Task<string> RunAsync(FileInfo file) {
        var histories = ParseHistories(await File.ReadAllLinesAsync(file.FullName));

        List<int> result = [];
        foreach (var history in histories) {
            var steps = CalculateSteps(history);
            result.Add(CalculateNextValue(steps));
        }

        return result.Sum().ToString();
    }

    static int[][] ParseHistories(string[] lines) {
        return lines
            .Select(line => line.Split(' ').Select(int.Parse).ToArray())
            .ToArray();
    }

    readonly List<int> _cache = [];

    int[][] CalculateSteps(int[] history) {
        List<int[]> steps = [history];
        while (true) {
            _cache.Clear();
            foreach (var (first, second) in history.AdjacentPairs()) {
                _cache.Add(second-first);
            }

            if (_cache.All(static i => i == 0)) {
                break;
            }

            history = [.. _cache];
            steps.Add(history);
        }

        return [.. steps];
    }

    static int CalculateNextValue(int[][] steps) {
        int n = 0;
        for (var i = steps.Length - 1; i >= 0; i--) {
            n = steps[i][^1] + n;
        }
        return n;
    }
}
