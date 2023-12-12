using System.Diagnostics;

namespace AdventOfCode.Solutions.Day12;

using Memo = Dictionary<(string, ReadOnlyMemory<int>), long>;

public class PartTwo : ISolution
{
    public int Day => 12;
    public int Part => 2;

    public async Task<string> RunAsync(FileInfo file) {
        var lines = await File.ReadAllLinesAsync(file.FullName);

        long result = 0;
        foreach (var line in lines) {
            var (springStatus, groupInfo) = line.Split(' ');
            result += CountMatchingCombinations(
                Unfold(springStatus, '?'),
                Unfold(groupInfo, ',')
            );
        }

        return result.ToString();

        static string Unfold(string value, char separator) {
            return string.Join(separator, Enumerable.Repeat(value, 5));
        }
    }

    const char OperationalChar = '.';
    const char DamagedChar = '#';
    const char UnknownChar = '?';

    static long CountMatchingCombinations(string springStatus, string groupInfo) {
        ReadOnlyMemory<int> groupSizes = groupInfo.Split(',').Select(int.Parse).ToArray();
        Memo memo = new();

        return Count(springStatus, groupSizes);

        long Count(string pattern, ReadOnlyMemory<int> sizes) {
            if (memo.TryGetValue((pattern, sizes), out var found)) {
                return found;
            }

            return memo[(pattern, sizes)] = Find(pattern, sizes);
        }

        long Find(string pattern, ReadOnlyMemory<int> sizes) {
            if (pattern.Length == 0) {
                return Complete(sizes);
            }

            if (pattern[0] == OperationalChar) {
                return Operational(pattern, sizes);
            } else if (pattern[0] == UnknownChar) {
                return Unknown(pattern, sizes);
            } else if (pattern[0] == DamagedChar) {
                return Damaged(pattern, sizes);
            } else {
                return Error();
            }
        }

        long Error() {
            throw new UnreachableException();
        }

        long Operational(string pattern, ReadOnlyMemory<int> sizes) {
            return Count(pattern[1..], sizes);
        }

        long Unknown(string pattern, ReadOnlyMemory<int> sizes) {
            return Count('.' + pattern[1..], sizes) +
                   Count('#' + pattern[1..], sizes);
        }

        long Damaged(string pattern, ReadOnlyMemory<int> sizes) {
            if (sizes.Length == 0) {
                return 0;
            }

            int size = sizes.Span[0];
            sizes = sizes[1..];

            int damagedOrUnknown = pattern.CountWhile(c => c is DamagedChar or UnknownChar);
            if (damagedOrUnknown < size) {
                return 0;
            } else if (pattern.Length == size) {
                return Count("", sizes);
            } else if (pattern[size] == DamagedChar) {
                return 0;
            } else {
                return Count(pattern[(size + 1)..], sizes);
            }
        }

        long Complete(ReadOnlyMemory<int> sizes) {
            if (sizes.Length == 0) {
                return 1;
            } else {
                return 0;
            }
        }
    }
}
