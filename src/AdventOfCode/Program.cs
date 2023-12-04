using AdventOfCode.Solutions;

{
    await SolveAsync<AdventOfCode.Solutions.Day01.PartOne>(GetInputFile(1));
    await SolveAsync<AdventOfCode.Solutions.Day01.PartTwo>(GetInputFile(1));
}

{
    await SolveAsync<AdventOfCode.Solutions.Day02.PartOne>(GetInputFile(2));
    await SolveAsync<AdventOfCode.Solutions.Day02.PartTwo>(GetInputFile(2));
}

{
    await SolveAsync<AdventOfCode.Solutions.Day03.PartOne>(GetInputFile(3));
    await SolveAsync<AdventOfCode.Solutions.Day03.PartTwo>(GetInputFile(3));
}

{
    await SolveAsync<AdventOfCode.Solutions.Day04.PartOne>(GetInputFile(4));
    await SolveAsync<AdventOfCode.Solutions.Day04.PartTwo>(GetInputFile(4));
}

FileInfo GetInputFile(int day) => new($"Inputs/day_{day:D2}.input");

async Task SolveAsync<T>(FileInfo input) where T : ISolution, new() {
    ISolution solution = new T();
    Console.WriteLine("D{0:D2}P{1:D2}: {2}", solution.Day, solution.Part, await solution.RunAsync(input));
}