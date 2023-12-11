namespace AdventOfCode.Solutions.Day11;

public class PartTwo : ISolution
{
    public int Day => 11;
    public int Part => 2;

    public async Task<string> RunAsync(FileInfo file) {
        var input = await File.ReadAllLinesAsync(file.FullName);
        var columns = input[0].Length;

        int[] columnGalaxyCount = new int[columns];
        long distance = 0;
        long seen = 0;
        long result = 0;

        foreach (var row in input) {
            bool emptyRow = true;
            int startIndex = 0;
            while (startIndex < row.Length) {
                int galaxyLocation = row.IndexOf('#', startIndex);
                if (galaxyLocation == -1) {
                    break;
                }

                result += distance;

                emptyRow = false;
                seen += 1;
                columnGalaxyCount[galaxyLocation] += 1;
                startIndex = galaxyLocation + 1;
            }

            if (emptyRow) {
                distance += seen * 1_000_000;
            } else {
                distance += seen;
            }
        }

        seen = 0;
        distance = 0;
        foreach (var count in columnGalaxyCount) {
            if (count == 0) {
                distance += seen * 1_000_000;
            } else {
                result += count * distance;

                seen += count;
                distance += seen;
            }
        }

        return result.ToString();
    }
}