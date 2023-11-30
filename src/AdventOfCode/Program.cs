using AdventOfCode.Solutions;

FileInfo input = new("Inputs/day_01.input");

ISolution solution = new AdventOfCode.Solutions.Day01.PartOne();
await solution.RunAsync(input);
