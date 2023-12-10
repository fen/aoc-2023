namespace AdventOfCode.Solutions.Day02;

public class PartOne : ISolution
{
    public int Day => 2;
    public int Part => 1;

    public async Task<string> RunAsync(FileInfo file) {
        string[] lines = await File.ReadAllLinesAsync(file.FullName);

        var games = lines.Select(Game.Parse).ToArray();

        List<int> possibleGames = [];
        foreach (var game in games) {
            bool possible = true;
            foreach (var gameSubSet in game.SubSets) {
                foreach (var (count, color) in gameSubSet) {
                    switch (color) {
                    case Color.Red when count > 12:
                    case Color.Green when count > 13:
                    case Color.Blue when count > 14:
                        possible = false;
                        break;
                    }
                }

                if (!possible) break;
            }

            if (possible) {
                possibleGames.Add(game.GameId);
            }
        }

        return possibleGames.Sum().ToString();
    }
}

file class Game(int gameId, List<List<Cube>> subSets)
{
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

file enum Color
{
    Red, Green, Blue
}