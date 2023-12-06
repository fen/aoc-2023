namespace AdventOfCode.Solutions.Day06;

class PartTwo : ISolution
{
    public int Day => 6;

    public int Part => 2;

    public async Task<string> RunAsync(FileInfo input) {
        var (timeLine, distanceLine) = (await File.ReadAllLinesAsync(input.FullName));
        var (_, rawTimes) = timeLine.Split(':');
        var (_, rawDistances) = distanceLine.Split(':');

        var time = long.Parse(rawTimes.Trim().Replace(" ", ""));
        var distance = long.Parse(rawDistances.Trim().Replace(" ", ""));

        int win = 0;
        for (var i = 0; i <= time; i++) {
            var totalDistanceTraveled = (time - i) * i;

            if (totalDistanceTraveled > distance) {
                win += 1;
            } 
        }

        return win.ToString();
    }
}
