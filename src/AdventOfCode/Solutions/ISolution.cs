namespace AdventOfCode.Solutions;

public interface ISolution
{
    public int Day { get; }
    public int Part { get; }
    Task<string> RunAsync(FileInfo input);
}