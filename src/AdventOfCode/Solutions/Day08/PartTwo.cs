namespace AdventOfCode.Solutions.Day08;

public class PartTwo : ISolution
{
    public int Day => 7;
    public int Part => 2;

    public async Task<string> RunAsync(FileInfo input) {
        var lines = await File.ReadAllLinesAsync(input.FullName);
        var graph = Graph.Parse(lines);

        var nodes = graph.Nodes.Where(n => n.Id.EndsWith('A')).ToArray();
        var navigations = new ulong[nodes.Length];
        for (var i = 0; i < nodes.Length; i++) {
            ulong count = 0;
            var currentNode = nodes[i];
            while (!currentNode.Id.EndsWith('Z')) {
                count += graph.Navigate(ref currentNode);
            }

            navigations[i] = count;
        }

        ulong result = 1;
        foreach (var count in navigations) {
            result = Lcm(result, count);
        }

        return result.ToString();
    }

    /// <summary>
    /// The Least Common Multiple (LCM) is the smallest number that is multiple of each of the two numbers
    /// <c>lcm(l, r) = |l.r| / gcd(l, r)</c>
    /// </summary>
    static ulong Lcm(ulong l, ulong r) {
        return l / Gcd(l, r) * r;
    }

    /// <summary>
    /// Greatest Common Divisor (GCD) of two numbers is the largest number that divides both of
    /// them without leaving a remainder.
    /// </summary>
    static ulong Gcd(ulong l, ulong r) {
        // Euclidean algorithm
        while (r != 0) {
            ulong temp = l % r;
            l = r;
            r = temp;
        }

        return l;
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

    public ulong Navigate(ref Node node) {
        ulong count = 0;
        foreach (var instruction in instructions) {
            if (node.Id.EndsWith('Z')) {
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
    public Node Execute(Node node) =>
        direction switch {
            'L' => node.Left,
            _ => node.Right
        };

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
        var (rawId, rawNavigation) = line.TrimSplit('=');
        var id = rawId.Trim();
        var navigation = rawNavigation.TrimStart('(').TrimEnd(')');
        var (left, right) = navigation.TrimSplit(',');
        return new Node(id, left, right);
    }

    public void Populate(Node[] nodes) {
        _left = nodes.Single(n => n.Id == LeftId);
        _right = nodes.Single(n => n.Id == RightId);
    }
}
