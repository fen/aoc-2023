namespace AdventOfCode.Solutions.Day21;

public class PartOne : ISolution
{
    public int Day => 21;
    public int Part => 1;

    public async Task<string> RunAsync(FileInfo file) {
        var gardenMap = await ReadGardenMapAsync(file);

        return CalculateReachablePlots(gardenMap, 64).ToString();
    }

    static async Task<char[,]> ReadGardenMapAsync(FileInfo file) {
        var lines = await File.ReadAllLinesAsync(file.FullName);
        var garden = new char[lines.Length, lines[0].Length];
        for (int i = 0; i < lines.Length; i++) {
            for (int j = 0; j < lines[0].Length; j++) {
                garden[i, j] = lines[i][j];
            }
        }

        return garden;
    }

    static int[] dx = [0, -1, 0, 1];
    static int[] dy = [-1, 0, 1, 0];

    static int CalculateReachablePlots(char[,] garden, int steps) {
        int n = garden.GetLength(0);
        int m = garden.GetLength(1);
        bool[,,] visited = new bool[n, m, steps + 1];
        Queue<(int x, int y, int depth)> queue = new Queue<(int x, int y, int depth)>();

        for (int i = 0; i < n; i++) {
            for (int j = 0; j < m; j++) {
                if (garden[i, j] == 'S') {
                    queue.Enqueue((i, j, 0));
                }
            }
        }

        while (queue.Count > 0) {
            var current = queue.Dequeue();
            if (current.depth > steps) {
                continue;
            }

            if (visited[current.x, current.y, current.depth]) {
                continue;
            }

            visited[current.x, current.y, current.depth] = true;
            for (int i = 0; i < 4; i++) {
                int newRow = current.x + dx[i];
                int newCol = current.y + dy[i];
                // Ensure we are within bounds and not hitting a rock
                if (newRow >= 0 && newRow < n && newCol >= 0 && newCol < m && garden[newRow, newCol] != '#') {
                    queue.Enqueue((newRow, newCol, current.depth + 1));
                }
            }
        }

        int count = 0;
        for (int i = 0; i < n; i++) {
            for (int j = 0; j < m; j++) {
                if (visited[i, j, steps]) {
                    count++;
                }
            }
        }

        return count;
    }
}