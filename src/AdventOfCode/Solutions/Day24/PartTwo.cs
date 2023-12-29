using System.Diagnostics;

namespace AdventOfCode.Solutions.Day24;

public class PartTwo : ISolution
{
    public int Day => 24;
    public int Part => 2;

    public async Task<string> RunAsync(FileInfo file) {
        var input = await File.ReadAllLinesAsync(file.FullName);

        var a = input.Select(s => s.Split("@").Select(vs => vs.Split(",")
            .Select(s => s.Trim()).Select(long.Parse).ToList()).ToList()).ToList();

        int[] range = [0, 1, 2];

        var timeMapping = range.Select(k => {
            return a.Select((x, i) => {
                var values = a.Select((y, j) => {
                    if (a[j][1][k] == a[i][1][k]) {
                        return a[j][0][k] == a[i][0][k] ? 0L : -1L;
                    } else {
                        var tn = a[j][0][k] - a[i][0][k];
                        var td = a[i][1][k] - a[j][1][k];
                        return tn % td == 0L ? tn / td : -1L;
                    }
                }).ToList();

                return values.All(val => val >= 0) ? values : null;
            }).FirstOrDefault(x => x != null);
        }).FirstOrDefault(x => x != null);

        if (timeMapping is null) {
            throw new UnreachableException();
        }

        var indices = timeMapping.Select((value, index) => (value, index))
            .Where(x => x.value > 0)
            .Select(x => x.index)
            .ToArray();
        var (i, j) = (indices[0], indices[1]);

        long P(int i, int k, long t) => a[i][0][k] + a[i][1][k] * t;

        return range.Sum(k => P(i, k, timeMapping[i]) -
                                               (P(i, k, timeMapping[i]) - P(j, k, timeMapping[j])) /
                                               (timeMapping[i] - timeMapping[j]) * timeMapping[i]).ToString();
    }
}