using System.Text.RegularExpressions;

namespace AdventOfCode.Solutions.Day20;

public class PartTwo : ISolution
{
    public int Day => 20;
    public int Part => 2;

    public async Task<string> RunAsync(FileInfo file) {
        string[] lines = await File.ReadAllLinesAsync(file.FullName);

        var configuration = CreateConfiguration(lines);
        var rx = (BroadcastModule)configuration.Modules["rx"];
        var conjuctionModule = (ConjuctionModule)configuration.Modules[rx.Inputs.Single()];

        return conjuctionModule.Inputs.Aggregate(1L, (m, branch) => m * CreateConfiguration(lines).Length(branch))
            .ToString();

        static Configuration CreateConfiguration(string[] lines) {
            return Configuration.Parse([..lines, "rx ->"]);
        }
    }
}

file class Configuration(Dictionary<string, Module> modules)
{
    public Dictionary<string, Module> Modules => modules;

    public IEnumerable<Signal> PressButton() {
        Queue<Signal> signalQueue = new();
        signalQueue.Enqueue(new("button", "broadcaster", false));

        while (signalQueue.TryDequeue(out var signal)) {
            yield return signal;

            if (modules.TryGetValue(signal.To, out var module)) {
                foreach (var receivedSignal in module.Execute(signal)) {
                    signalQueue.Enqueue(receivedSignal);
                }
            }
        }
    }

    public long Length(string branch) {
        for (int i = 1; ; i++) {
            if (PressButton().Any(s => s.From == branch && s.Value)) {
                return i;
            }
        }
    }

    public static Configuration Parse(string[] rawData) {
        var descriptions =
            from line in rawData
            let words = Regex.Matches(line, "\\w+").Select(m => m.Value).ToArray()
            select (kind: line[0], name: words.First(), outputs: words[1..]);

        var inputs = (string name) => (
            from d in descriptions where d.outputs.Contains(name) select d.name
        ).ToArray();

        return new(descriptions.ToDictionary(m => m.name,
            m => (Module)(m.kind switch {
                '%' => new FlipFlopModule(m.name, m.outputs),
                '&' => new ConjuctionModule(m.name, inputs(m.name), m.outputs),
                _ => new BroadcastModule(m.name, inputs(m.name), m.outputs)
            })
        ));
    }
}

file readonly record struct Signal(string From, string To, bool Value);

file abstract class Module(string name)
{
    public string Name { get; } = name;

    public abstract IEnumerable<Signal> Execute(Signal signal);
}

file sealed class FlipFlopModule(string name, string[] outputs) : Module(name)
{
    private bool _state;

    public override IEnumerable<Signal> Execute(Signal signal) {
        if (!signal.Value) {
            _state = !_state;
            return outputs.Select(to => new Signal(name, to, _state));
        } else {
            return [];
        }
    }
}

file sealed class ConjuctionModule(string name, string[] inputs, string[] outputs) : Module(name)
{
    public string[] Inputs => inputs;

    private Dictionary<string, bool> _state = inputs.ToDictionary(i => i, _ => false);

    public override IEnumerable<Signal> Execute(Signal signal) {
        _state[signal.From] = signal.Value;
        var outputValue = !_state.Values.All(v => v);
        return outputs.Select(to => new Signal(name, to, outputValue));
    }
}

file sealed class BroadcastModule(string name, string[] inputs, string[] outputs) : Module(name)
{
    public string[] Inputs => inputs;

    public override IEnumerable<Signal> Execute(Signal signal) {
        return outputs.Select(to => new Signal(name, to, signal.Value));
    }
}
