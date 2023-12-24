using Position = (double x, double y, double z);
using Velocity = (double x, double y, double z);

namespace AdventOfCode.Solutions.Day24;

using Hailstone = (Position position, Velocity velocity);

public class PartOne : ISolution
{
    public int Day => 24;
    public int Part => 1;

    public async Task<string> RunAsync(FileInfo file) {
        List<Hailstone> hailstones = [];
        foreach (var line in await File.ReadAllLinesAsync(file.FullName)) {
            var numbers = line.Split([" @ ", ", "], StringSplitOptions.TrimEntries).Select(double.Parse).ToArray();
            hailstones.Add(((numbers[0], numbers[1], numbers[2]), (numbers[3], numbers[4], numbers[5])));
        }

        double ans = 0;
        for (var i = 0; i < hailstones.Count; i++) {
            for (var j = i + 1; j < hailstones.Count; j++) {
                var a = hailstones[i];
                var b = hailstones[j];

                if (FindIntersection(a, b)) {
                    ans += 1;
                }
            }
        }

        return ans.ToString();
    }

    static bool FindIntersection(Hailstone a, Hailstone b) {
        double x1 = a.position.x;
        double x2 = a.position.x + a.velocity.x;
        double x3 = b.position.x;
        double x4 = b.position.x + b.velocity.x;
        double y1 = a.position.y;
        double y2 = a.position.y + a.velocity.y;
        double y3 = b.position.y;
        double y4 = b.position.y + b.velocity.y;

        double denom = ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4));
        if (denom == 0) {
            return false;
        }

        double px = (((x1 * y2 - y1 * x2) * (x3 - x4)) - ((x1 - x2) * (x3 * y4 - y3 * x4))) /
                    ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4));
        double py = ((x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4)) /
                    ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4));
        bool validA = (px > x1) == (x2 > x1);
        bool validB = (px > x3) == (x4 > x3);

        if (px is >= 200000000000000 and <= 400000000000000 && py is >= 200000000000000 and <= 400000000000000 &&
            validA && validB) {
            return true;
        }

        return false;
    }
}