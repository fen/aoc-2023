namespace AdventOfCode.Solutions.Day06;

class PartOne : ISolution
{
    public int Day => 6;
    public int Part => 1;

    public async Task<string> RunAsync(FileInfo input) {
        var (timeLine, distanceLine) = await File.ReadAllLinesAsync(input.FullName);
        var (_, rawTimes) = timeLine.Split(':');
        var (_, rawDistances) = distanceLine.Split(':');

        var times = rawTimes.Trim().Split(' ').Where(s => s.Length > 0).Select(int.Parse).ToArray();
        var distances = rawDistances.Trim().Split(' ').Where(s => s.Length > 0).Select(int.Parse).ToArray();

        List<int> possibleWaysOfWinning = [];
        foreach (var (time, distance) in times.Zip(distances)) {
            int win = 0;
            for (var i = 0; i <= time; i++) {
                var totalDistanceTraveled = (time - i) * i;

                if (totalDistanceTraveled > distance) {
                    win += 1;
                }
            }
            possibleWaysOfWinning.Add(win);
        }

        return possibleWaysOfWinning.Product().ToString();
    }
}
