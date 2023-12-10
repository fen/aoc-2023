namespace AdventOfCode.Solutions.Day10;

public class PartOne : ISolution
{
    public int Day => 10;
    public int Part => 1;

    public async Task<string> RunAsync(FileInfo file) {
        var input = await File.ReadAllLinesAsync(file.FullName);
        var maze = ParseMaze(input);
        var farthestPosition = FindFarthestPosition(maze);
        return farthestPosition.ToString();
    }

    static char[,] ParseMaze(string[] lines) {
        int rows = lines.Length;
        int cols = lines[0].Length;

        char[,] maze = new char[rows, cols];

        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                maze[i, j] = lines[i][j];
            }
        }

        return maze;
    }


    static int[,] directions = new int[4, 2] {
        { -1, 0 }, // north
        { 0, 1 }, // east
        { 1, 0 }, // south
        { 0, -1 }, // west
    };

    static Dictionary<char, bool[]> pipes = new() {
        { '.', [false, false, false, false] },
        { '|', [true, false, true, false] },
        { '-', [false, true, false, true] },
        { 'L', [true, true, false, false] },
        { 'J', [true, false, false, true] },
        { '7', [false, false, true, true] },
        { 'F', [false, true, true, false] },
        { 'S', [true, true, true, true] }
    };

    static int FindFarthestPosition(char[,] maze) {
        int n = maze.GetLength(0);
        int m = maze.GetLength(1);
        bool[,] visited = new bool[n, m];
        Queue<(int x, int y, int distance)> queue = new();
        int maxDistance = 0;

        for (int i = 0; i < n; i++) {
            for (int j = 0; j < m; j++) {
                if (maze[i, j] == 'S') {
                    queue.Enqueue((i, j, maxDistance));
                }
            }
        }

        while (queue.Count > 0) {
            var (x, y, dist) = queue.Dequeue();
            if (visited[x, y]) {
                continue;
            }

            visited[x, y] = true;
            maxDistance = Math.Max(maxDistance, dist);

            for (var d = 0; d < 4; d++) {
                if (!pipes[maze[x, y]][d]) {
                    continue;
                }

                int nx = x + directions[d, 0];
                int ny = y + directions[d, 1];
                if (nx >= 0 && ny >= 0 && nx < n && ny < m && !visited[nx, ny] && pipes[maze[nx, ny]][(d + 2) % 4]) {
                    queue.Enqueue((nx, ny, dist + 1));
                }
            }
        }

        return maxDistance;
    }
}