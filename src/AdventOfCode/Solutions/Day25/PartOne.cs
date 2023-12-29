namespace AdventOfCode.Solutions.Day25;

public class PartOne : ISolution
{
    public int Day => 25;
    public int Part => 1;

    public async Task<string> RunAsync(FileInfo file) {
        var map = Parser(await File.ReadAllLinesAsync(file.FullName));
        var set1 = new HashSet<string>(map.Keys.ToList());
        var set2 = new HashSet<string>();

        Func<string, int> wrongSet = v => {
            var count = 0;
            foreach (var item in map[v]) {
                if (set2.Contains(item)) {
                    count++;
                }
            }

            return count;
        };

        while (set1.Sum(item => wrongSet(item)) != 3) {
            var itemToMove = set1.OrderByDescending(wrongSet).First();
            set2.Add(itemToMove);
            set1.Remove(itemToMove);
        }

        return (set1.Count * set2.Count).ToString();
    }

    Dictionary<string, List<string>> Parser(string[] lines) {
        var graph = new Dictionary<string, List<string>>();

        var registerEdge = (string u, string v) => {
            if (!graph.ContainsKey(u)) {
                graph[u] = new();
            }

            graph[u].Add(v);
        };

        foreach (var line in lines) {
            var parts = line.Split(": ");
            var u = parts[0];
            var nodes = parts[1].Split(' ');
            foreach (var v in nodes) {
                registerEdge(u, v);
                registerEdge(v, u);
            }
        }

        return graph;
    }
}