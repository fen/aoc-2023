using System.Diagnostics;
using AdventOfCode;
using AdventOfCode.Solutions;

// {
//     await SolveAsync<AdventOfCode.Solutions.Day01.PartOne>(GetInputFile(1));
//     await SolveAsync<AdventOfCode.Solutions.Day01.PartTwo>(GetInputFile(1));
// }

// {
//     await SolveAsync<AdventOfCode.Solutions.Day02.PartOne>(GetInputFile(2));
//     await SolveAsync<AdventOfCode.Solutions.Day02.PartTwo>(GetInputFile(2));
// }

// {
//     await SolveAsync<AdventOfCode.Solutions.Day03.PartOne>(GetInputFile(3));
//     await SolveAsync<AdventOfCode.Solutions.Day03.PartTwo>(GetInputFile(3));
// }

// {
//     await SolveAsync<AdventOfCode.Solutions.Day04.PartOne>(GetInputFile(4));
//     await SolveAsync<AdventOfCode.Solutions.Day04.PartTwo>(GetInputFile(4));
// }

//{
//    await SolveAsync<AdventOfCode.Solutions.Day05.PartOne>(GetInputFile(5));
//    await SolveAsync<AdventOfCode.Solutions.Day05.PartTwo>(GetInputFile(5));
//}

{
    await SolveAsync<AdventOfCode.Solutions.Day06.PartOne>(GetInputFile(6));
    await SolveAsync<AdventOfCode.Solutions.Day06.PartTwo>(GetInputFile(6));
}

FileInfo GetInputFile(int day) => new($"Inputs/day_{day:D2}.input");

async Task SolveAsync<T>(FileInfo input) where T : ISolution, new() {
    ISolution solution = new T();
    var sw = Stopwatch.StartNew();
    var result = await solution.RunAsync(input);
    sw.Stop();
    Console.WriteLine("D{0:D2}P{1:D2}: {2} in {3}", solution.Day, solution.Part, result, sw.Elapsed.ToReadableString());
}
