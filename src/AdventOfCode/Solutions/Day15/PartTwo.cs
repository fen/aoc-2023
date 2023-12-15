using System.Collections;

namespace AdventOfCode.Solutions.Day15;

using Slot = (int box, int slot, AddOperation);

public class PartTwo : ISolution
{
    public int Day => 15;
    public int Part => 2;

    public async Task<string> RunAsync(FileInfo file) {
        var lines = await File.ReadAllLinesAsync(file.FullName);
        var operations = lines.SelectMany(l => l.Split(','))
            .Select(Operation.Parse);

        HashMap hashMap = new();
        foreach (var operation in operations) {
            hashMap.Add(operation);
        }

        int result = 0;
        foreach (var (box, slot, op) in hashMap) {
            result += (box + 1) * slot * op.FocalLength;
        }

        return result.ToString();
    }
}

file sealed class HashMap : IEnumerable<Slot>
{
    private readonly Dictionary<int, List<Operation>> _buckets = new();

    public void Add(Operation operation) {
        if (operation is AddOperation add) {
            int hash = operation.GetHashCode();
            if (_buckets.TryGetValue(hash, out var bucket)) {
                var index = bucket.IndexOf(add);
                if (index == -1) {
                    bucket.Add(add);
                } else {
                    bucket[index] = add;
                }
            } else {
                _buckets[hash] = [add];
            }
        } else if (operation is RemoveOperation remove) {
            int hash = operation.GetHashCode();
            if (_buckets.TryGetValue(hash, out var bucket)) {
                bucket.Remove(remove);
            }
        }
    }

    public IEnumerator<Slot> GetEnumerator() {
        return Iterate();
    }

    private IEnumerator<Slot> Iterate() {
        foreach (var operations in _buckets) {
            foreach (var (operation, i) in operations.Value.EnumerateWithIndex()) {
                yield return (operations.Key, i + 1, (AddOperation)operation);
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}

file abstract class Operation(string label)
{
    public string Label => label;

    public static Operation Parse(string input) {
        var index = input.IndexOf(i => i is '-' or '=');
        string label = input[..index];

        return input[index] switch {
            '-' => new RemoveOperation(label),
            '=' => new AddOperation(label, int.Parse(input[(index + 1)..])),
            _ => throw new InvalidOperationException()
        };
    }

    public override int GetHashCode() {
        int asciiCode;
        int currentValue = 0;

        for (int i = 0; i < label.Length; i++) {
            asciiCode = label[i];
            currentValue += asciiCode;
            currentValue *= 17;
            currentValue %= 256;
        }

        return currentValue;
    }

    public override bool Equals(object? obj) {
        if (obj is not Operation op) {
            return false;
        }

        return Label.Equals(op.Label);
    }
}

file sealed class RemoveOperation(string label) : Operation(label);

file sealed class AddOperation(string label, int focalLength) : Operation(label)
{
    public int FocalLength => focalLength;
}