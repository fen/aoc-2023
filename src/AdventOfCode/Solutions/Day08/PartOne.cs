namespace AdventOfCode.Solutions.Day08;

public class PartOne : ISolution
{
    public int Day => 8;
    public int Part => 1;

    public async Task<string> RunAsync(FileInfo input) {
        var lines = await File.ReadAllLinesAsync(input.FullName);
        var graph = Graph.Parse(lines);

        int count = 0;
        var node = graph.Nodes.Single(n => n.Id == "AAA");
        while (node.Id != "ZZZ") {
            count += graph.Navigate(ref node);
        }

        return count.ToString();
    }
}

file class Graph(Instruction[] instructions, Node[] nodes)
{
    public Node[] Nodes => nodes;

    public static Graph Parse(string[] rawNodes) {
        var instructions = rawNodes[0].Select(Instruction.Parse).ToArray();

        var nodes = rawNodes[2..].Select(Node.Parse).ToArray();
        foreach (var node in nodes) {
            node.Populate(nodes);
        }

        return new Graph(instructions, nodes);
    }

    public int Navigate(ref Node node) {
        int count = 0;
        foreach (var instruction in instructions) {
            if (node.Id == "ZZZ") {
                break;
            }

            node = instruction.Execute(node);
            count += 1;
        }

        return count;
    }
}

file readonly struct Instruction(char direction)
{
    public Node Execute(Node node) {
        if (direction == 'L') {
            return node.Left;
        } else {
            return node.Right;
        }
    }

    public static Instruction Parse(char direction) => new(direction);
}

file class Node(string id, string left, string right)
{
    private Node? _left;
    private Node? _right;

    public string Id => id;
    public string LeftId => left;
    public string RightId => right;

    public Node Left => _left ??
                        throw new InvalidOperationException("Method Populate needs to be called before accessing Left");
    public Node Right => _right ??
                        throw new InvalidOperationException("Method Populate needs to be called before accessing Right");

    public static Node Parse(string line) {
        var (rawId, rawNavigation) = line.SplitAndTrimEach('=');
        var id = rawId.Trim();
        var navigation = rawNavigation.TrimStart('(').TrimEnd(')');
        var (left, right) = navigation.SplitAndTrimEach(',');
        return new Node(id, left, right);
    }

    public void Populate(Node[] nodes) {
        _left = nodes.Single(n => n.Id == LeftId);
        _right = nodes.Single(n => n.Id == RightId);
    }
}
