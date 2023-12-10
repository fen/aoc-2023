namespace AdventOfCode.Solutions.Day05;

using static Helper;

public class PartTwo : ISolution
{
    public int Day => 5;
    public int Part => 2;

    public async Task<string> RunAsync(FileInfo file) {
        var input = (await File.ReadAllLinesAsync(file.FullName)).ToList();

        long[] seeds;
        {
            var (_, rawSeeds) = input[0].Split(':');
            seeds = rawSeeds.Trim().Split(' ').Where(n => n != "").Select(long.Parse).ToArray();
        }

        List<List<IntervalMap>> maps = [];

        int line = 2;
        while (line < input.Count) {
            line += 1;

            List<IntervalMap> ranges = [];
            while (line < input.Count && input[line].Length != 0) {
                var (rawDestinationRange, rawSourceRange, rawRangeLength) = input[line].Split(' ');
                ranges.Add(new(
                    long.Parse(rawDestinationRange.Trim()),
                    long.Parse(rawSourceRange.Trim()),
                    long.Parse(rawRangeLength.Trim())
                ));

                line += 1;
            }

            maps.Add(ranges);
            line += 1;
        }

        long ret = long.MaxValue;

        for (int i = 0; i < seeds.Length; i += 2) {
            ret = Math.Min(ret, Dfs(maps, 0, new(seeds[i], seeds[i] + seeds[i + 1] - 1)));
        }

        return ret.ToString();
    }
}

public class IntervalMap(long destinationStart, long sourceStart, long length)
{
    public long DestinationStart { get; } = destinationStart;
    public long SourceStart { get; } = sourceStart;
    public long Length { get; } = length;
}

public class Interval(long lhs, long rhs)
{
    public long Lhs { get; } = lhs;
    public long Rhs { get; } = rhs;
}

file class Helper
{
    public static long Dfs(List<List<IntervalMap>> maps, int idx, Interval currInterval) {
        if (idx == maps.Count) {
            return currInterval.Lhs;
        }

        long ret = long.MaxValue;
        var intervals = new List<Interval> { currInterval };
        foreach (var meta in maps[idx]) {
            long shift = meta.DestinationStart - meta.SourceStart;
            var nIntervals = new List<Interval>();
            foreach (var i in intervals) {
                var candInterval = new Interval(
                    Math.Max(i.Lhs, meta.SourceStart),
                    Math.Min(i.Rhs, meta.SourceStart + meta.Length - 1)
                );

                if (candInterval.Lhs > candInterval.Rhs) {
                    nIntervals.Add(i);
                    continue;
                }

                ret = Math.Min(ret,
                    Dfs(maps, idx + 1, new Interval(shift + candInterval.Lhs, shift + candInterval.Rhs)));
                if (candInterval.Lhs > i.Lhs) {
                    nIntervals.Add(new Interval(i.Lhs, candInterval.Lhs - 1));
                }

                if (candInterval.Rhs < i.Rhs) {
                    nIntervals.Add(new Interval(candInterval.Rhs + 1, i.Rhs));
                }
            }

            intervals = nIntervals;
        }


        foreach (var interval in intervals) {
            ret = Math.Min(ret, Dfs(maps, idx + 1, interval));
        }

        return ret;
    }
}