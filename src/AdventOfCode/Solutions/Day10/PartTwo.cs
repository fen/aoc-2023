namespace AdventOfCode.Solutions.Day10;

public class PartTwo : ISolution
{
    public int Day => 10;
    public int Part => 2;

    public async Task<string> RunAsync(FileInfo file) {
        var input = await File.ReadAllTextAsync(file.FullName);
        var maze = ParseMaze(input);
        var mazeSolver = new MazeSolver();
        mazeSolver.FindLongestPath(maze);
        mazeSolver.FindEnclosedSpaces();

        for (int i = 0; i < maze.GetLength(0); i++) {
            for (int j = 0; j < maze.GetLength(1); j++) {
                Console.Write(maze[i, j]);
            }

            Console.WriteLine();
        }

        return "TODO";
    }


    public static char[,] ParseMaze(string stringMaze) {
        string[] lines = stringMaze.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
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
}

public class MazeSolver
{
    public static int[,] directions = new int[4, 2] {
        { -1, 0 }, // north
        { 0, 1 }, // east
        { 1, 0 }, // south
        { 0, -1 }, // west
    };

    public static Dictionary<char, bool[]> pipes = new Dictionary<char, bool[]>() {
        { '.', new bool[] { false, false, false, false } },
        { '|', new bool[] { true, false, true, false } },
        { '-', new bool[] { false, true, false, true } },
        { 'L', new bool[] { true, true, false, false } },
        { 'J', new bool[] { true, false, false, true } },
        { '7', new bool[] { false, false, true, true } },
        { 'F', new bool[] { false, true, true, false } },
        { 'S', new bool[] { true, true, true, true } },
    };

    private char[,] maze;
    private int n, m;
    public List<Tuple<int, int>> longestPath = new List<Tuple<int, int>>();

    public List<Tuple<int, int>> FindLongestPath(char[,] maze) {
        this.maze = maze;
        this.n = maze.GetLength(0);
        this.m = maze.GetLength(1);

        bool[,] visited = new bool[n, m];

        // Find the 'S' and start DFS
        for (int i = 0; i < n; i++)
        for (int j = 0; j < m; j++)
            if (maze[i, j] == 'S')
                DFS(i, j, visited);


        // for (int i = 0; i < n; i++)
        // for (int j = 0; j < m; j++)
        //     maze[i, j] = '.';

        // mark the cells on longest path with '#'
        // foreach (var cell in longestPath)
        //     maze[cell.Item1, cell.Item2] = '#';

        return longestPath;
    }

    List<Tuple<int, int>> crossings = new List<Tuple<int, int>>();

    private void DFS(int startRow, int startCol, bool[,] visited) {
        Stack<Tuple<int, int, List<Tuple<int, int>>>> stack = new Stack<Tuple<int, int, List<Tuple<int, int>>>>();
        visited[startRow, startCol] = true;
        stack.Push(Tuple.Create(startRow, startCol, new List<Tuple<int, int>> { Tuple.Create(startRow, startCol) }));

        while (stack.Count > 0) {
            var current = stack.Pop();
            var row = current.Item1;
            var col = current.Item2;
            var curPath = current.Item3;

            // Updating longest Path
            if (curPath.Count > longestPath.Count)
                longestPath = new List<Tuple<int, int>>(curPath);

            for (var i = 0; i < 4; i++) {
                int newRow = row + directions[i, 0];
                int newCol = col + directions[i, 1];


                if (maze[newRow, newCol] == '|'
                    || (maze[newRow, newCol] == 'F' && maze[newRow, newCol + 1] == '-'
                                                    && maze[newRow, newCol + 2] == 'J')
                    || (maze[newRow, newCol] == 'L' && maze[newRow, newCol + 1] == '-'
                                                    && maze[newRow, newCol + 2] == '7')) {
                    crossings.Add(new Tuple<int, int>(newRow, newCol));
                }

                if (newRow >= 0 && newRow < n && newCol >= 0 && newCol < m && !visited[newRow, newCol] &&
                    pipes[maze[newRow, newCol]][(i + 2) % 4]) {
                    visited[newRow, newCol] = true;
                    List<Tuple<int, int>> newPath =
                        new List<Tuple<int, int>>(curPath) { Tuple.Create(newRow, newCol) };
                    stack.Push(Tuple.Create(newRow, newCol, newPath));
                }
            }
        }
    }


    public void FindEnclosedSpaces() {
        for (int i = 0; i < n; i++) {
            for (int j = 0; j < m; j++) {
                if (maze[i, j] == '.') {
                    FloodFill(i, j);
                }
            }
        }
    }

    private void FloodFill(int row, int col) {
        if (row < 0 || row >= n || col < 0 || col >= m || maze[row, col] != '.' || IsCrossing(row, col)) {
            return;
        }

        maze[row, col] = 'F';

        FloodFill(row - 1, col); // north
        FloodFill(row, col + 1); // east
        FloodFill(row + 1, col); // south
        FloodFill(row, col - 1); // west
    }

    private bool IsCrossing(int row, int col) {
        // Check if the cell at (row, col) is in crossings list
        return crossings.Any(c => c.Item1 == row && c.Item2 == col);
    }
}