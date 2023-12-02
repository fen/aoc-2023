namespace AdventOfCode.Solutions.Day02;

public class PartTwo : ISolution {
    public int Day => 2;
    public int Part => 2;

    public async Task<string> RunAsync(FileInfo input) {
        string[] lines = await File.ReadAllLinesAsync(input.FullName);

        var games = lines.Select(Game.Parse).ToArray();
        List<int> power = [];
        foreach (var game in games) {
            var counter = new PowerCounter();

            foreach (var gameSubSet in game.SubSets) {
                foreach (var (count, color) in gameSubSet) {
                    counter.Set(color, count);
                }
            }

            power.Add(counter.Power);
        }

        return power.Sum().ToString();
    }
}

file struct PowerCounter(int red, int green, int blue) {
    public int Power => red * green * blue;

    public void Set(Color color, int value) {
        switch (color) {
            case Color.Blue:
                blue = Math.Max(blue, value);
                break;
            case Color.Green:
                green = Math.Max(green, value);
                break;
            case Color.Red:
                red = Math.Max(red, value);
                break;
        }
    }
}

file class Game(int gameId, List<List<Cube>> subSets) {
    public int GameId => gameId;
    public List<List<Cube>> SubSets => subSets;

    public static Game Parse(string line) {
        var (rawGameId, rawSets) = line.Split(':');
        int gameId = int.Parse(rawGameId.Split(' ')[1]);
        var rawSubSets = rawSets.Trim().Split(';');

        List<List<Cube>> subSets = [];
        foreach (var rawSubSet in rawSubSets) {
            List<Cube> cubes = [];
            foreach (var (rawCount, rawColor) in rawSubSet.Split(',').Select(p => p.Trim().Split(' '))) {
                var count = int.Parse(rawCount);
                var color = (Color)Enum.Parse(typeof(Color), rawColor, ignoreCase: true);
                cubes.Add(new Cube(count, color));
            }

            subSets.Add(cubes);
        }

        return new Game(gameId, subSets);
    }
}

file record Cube(int Count, Color Color);

file enum Color {
    Red, Green, Blue
}
