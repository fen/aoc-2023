namespace AdventOfCode.Solutions.Day22;

using static Methods;

public class PartTwo : ISolution
{
    public int Day => 22;
    public int Part => 2;

    public async Task<string> RunAsync(FileInfo file) {
        var lines = await File.ReadAllLinesAsync(file.FullName);
        Block[] blocks = lines.Select(Block.Parse).ToArray();
        blocks = ApplyGravity(blocks);
        FindSupports(blocks);
        return Desintegrates(blocks).ToString();
    }
}

file static class Methods
{
    public static int Desintegrates(Block[] blocks) {
        var falling = new HashSet<Block>();
        var blockQueue = new Queue<Block>();

        int sum = 0;
        foreach (var block in blocks) {
            falling.Clear();
            blockQueue.Clear();

            blockQueue.Enqueue(block);

            while (blockQueue.TryDequeue(out var desintegratedBlock)) {
                falling.Add(desintegratedBlock);

                var fallingBlocks = desintegratedBlock.Above
                    .Where(blockT => blockT.Below.IsSubsetOf(falling));

                foreach (var blockT in fallingBlocks) {
                    blockQueue.Enqueue(blockT);
                }
            }

            sum += falling.Count - 1;
        }

        return sum;
    }

    public static Block[] ApplyGravity(Block[] blocks) {
        blocks = blocks.OrderBy(b => b.Bottom).ToArray();

        for (var i = 0; i < blocks.Length; i++) {
            int bottom = 1;
            for (int j = 0; j < i; j++) {
                if (blocks[i].Intersects(blocks[j])) {
                    bottom = Math.Max(bottom, blocks[j].Top + 1);
                }
            }

            blocks[i] = blocks[i].Move(bottom);
        }

        return blocks;
    }

    public static void FindSupports(Block[] blocks) {
        for (var i = 0; i < blocks.Length; i++) {
            for (var j = i + 1; j < blocks.Length; j++) {
                var a = blocks[i];
                var b = blocks[j];

                bool isNeighbours = b.Bottom == 1 + a.Top;
                if (isNeighbours && a.Intersects(b)) {
                    b.Below.Add(a);
                    a.Above.Add(b);
                }
            }
        }
    }
}

file record struct Range(int Begin, int End)
{
    public bool Intersects(Range other) {
        return Begin <= other.End && other.Begin <= End;
    }
}

file record Block(Range X, Range Y, Range Z)
{
    public HashSet<Block> Above { get; init; } = new();
    public HashSet<Block> Below { get; init; } = new();

    public int Top => Z.End;
    public int Bottom => Z.Begin;

    public static Block Parse(string line) {
        int[] numbers = line.Split(',', '~').Select(int.Parse).ToArray();
        return new Block(
            X: new Range(numbers[0], numbers[3]),
            Y: new Range(numbers[1], numbers[4]),
            Z: new Range(numbers[2], numbers[5])
        );
    }

    public bool Intersects(Block other) {
        return X.Intersects(other.X) && Y.Intersects(other.Y);
    }

    public Block Move(int bottomPosition) {
        int d = Bottom - bottomPosition;
        return this with { Z = new Range(Bottom - d, Top - d) };
    }
}