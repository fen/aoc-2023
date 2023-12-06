namespace AdventOfCode.Solutions.Day05;

using static Helper;

public class PartTwo : ISolution
{
    public int Day => 5;
    public int Part => 2;

    public async Task<string> RunAsync(FileInfo input) {
        var lines = await File.ReadAllLinesAsync(input.FullName);
        var (seedRanges, maps) = Parse(lines);

        // REVISIT: This is not an optimal way of solving this will try to get
        // some time and revisit it and solve it with ranges instead.

        long min = long.MaxValue;
        foreach (var (start, length) in seedRanges) {
            for (long i = start; i <= start + length; i++) {
                var seed = i;
                foreach (var m in maps) {
                    seed = m.Convert(seed);
                }

                min = Math.Min(min, seed);
            }
        }

        return min.ToString();
    }
}

file class Map(string from, string to, Range[] ranges)
{
    public string From => from;
    public string To => to;
    public Range[] Ranges => ranges;

    public long Convert(long source) {
        foreach (var range in ranges) {
            if (range.TryConvert(source, out var destination)) {
                return destination;
            }
        }

        return source;
    }
}

file struct Range(long destinationStart, long sourceStart, long length)
{
    public long DestinationStart => destinationStart;
    public long SourceStart => sourceStart;
    public long Length => length;

    public bool TryConvert(long source, out long destination) {
        if (sourceStart <= source && source < sourceStart + length) {
            destination = destinationStart + source - sourceStart;
            return true;
        }

        destination = 0;
        return false;
    }
}

file class Helper
{
    public static ((long, long)[], List<Map>) Parse(string[] lines) {
        (long, long)[] seedRanges = [];
        {
            var (_, rawSeeds) = lines[0].Split(':');
            seedRanges = rawSeeds.Trim()
                .Split(' ')
                .Where(n => n != "")
                .Select(long.Parse)
                .ByTwo()
                .ToArray();
        }

        List<Map> maps = [];

        int line = 2;
        while (line < lines.Length) {
            var (rawMapName, _) = lines[line].Split(' ');
            var (from, to) = rawMapName.Split('-').FirstAndLast();
            line += 1;

            List<Range> ranges = [];
            while (line < lines.Length && lines[line].Length != 0) {
                var (rawDestinationRange, rawSourceRange, rawRangeLength) = lines[line].Split(' ');
                ranges.Add(new(
                    long.Parse(rawDestinationRange.Trim()),
                    long.Parse(rawSourceRange.Trim()),
                    long.Parse(rawRangeLength.Trim())
                ));

                line += 1;
            }

            maps.Add(new(
                from, to,
                ranges.ToArray()
            ));

            line += 1;
        }

        return (seedRanges, maps);
    }
}