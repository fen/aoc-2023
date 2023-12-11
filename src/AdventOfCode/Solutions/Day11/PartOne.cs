namespace AdventOfCode.Solutions.Day11;

public class PartOne : ISolution
{
    public int Day => 11;
    public int Part => 1;

    public async Task<string> RunAsync(FileInfo file) {
        var input = await File.ReadAllLinesAsync(file.FullName);
        var columns = input[0].Length;

        int[] columnGalaxyCount = new int[columns];
        int distance = 0;
        int seen = 0;
        int result = 0;

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
                distance += seen * 2;
            } else {
                distance += seen;
            }
        }

        seen = 0;
        distance = 0;
        foreach (var count in columnGalaxyCount) {
            if (count == 0) {
                distance += seen * 2;
            } else {
                result += count * distance;

                seen += count;
                distance += seen;
            }
        }

        return result.ToString();
    }
}