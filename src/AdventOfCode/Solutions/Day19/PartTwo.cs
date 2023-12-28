using System.Collections;
using System.Diagnostics;
using System.Numerics;

namespace AdventOfCode.Solutions.Day19;

public class PartTwo : ISolution
{
    public int Day => 19;
    public int Part => 2;

    public async Task<string> RunAsync(FileInfo file) {
        string[] workflowLines = await File.ReadAllLinesAsync(file.FullName);

        var workflows = workflowLines.TakeWhile(l => l.Length > 0)
            .Select(Workflow.Parse)
            .ToDictionary(w => w.Name);

        foreach (var workflow in workflows.Values) {
            workflow.Populate(workflows);
        }

        var @in = workflows["in"];
        var r = new Rating(
            new(1, 4000),
            new(1, 4000),
            new(1, 4000),
            new(1, 4000)
        );

        _ = @in.Execute(ref r);

        return AcceptRule.Result.ToString();
    }
}

file record struct Range(int Begin, int End);

file record struct Rating(Range X, Range M, Range A, Range S) : IEnumerable<Range>
{
    public BigInteger Volume
        => this.Aggregate(BigInteger.One, (m, r) => m * (r.End - r.Begin + 1));

    public Range this[int i] =>
        i switch {
            0 => X,
            1 => M,
            2 => A,
            3 => S,
            _ => throw new ArgumentOutOfRangeException(nameof(i))
        };

    public Rating Set(int index, Range value) {
        switch (index)
        {
        case 0:
            return this with { X = value };
        case 1:
            return this with { M = value };
        case 2:
            return this with { A = value };
        case 3:
            return this with { S = value };
        default:
            throw new ArgumentOutOfRangeException(nameof(index));
        }
    }

    public IEnumerator<Range> GetEnumerator() {
        return Iterator(this).GetEnumerator();

        static IEnumerable<Range> Iterator(Rating r) {
            yield return r.X;
            yield return r.M;
            yield return r.A;
            yield return r.S;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}

file class Workflow(string name, List<Rule> rules)
{
    public string Name { get; set; } = name;
    public List<Rule> Rules { get; set; } = rules;

    public static Workflow Parse(string line) {
        var ruleStart = line.IndexOf('{');
        var name = line[..ruleStart];

        var rawRules = line[(ruleStart + 1)..^1];
        var rules = rawRules.Split(',')
            .Select(Rule.Parse)
            .ToList();

        return new Workflow(name, rules);
    }

    public void Populate(Dictionary<string, Workflow> lookup) {
        foreach (var rule in Rules) {
            if (rule is DestinationRule destinationRule) {
                Set(destinationRule, lookup);
            } else if (rule is ConditionalRule { Rule: DestinationRule conditionalDestination }) {
                Set(conditionalDestination, lookup);
            }
        }

        static void Set(DestinationRule destinationRule, Dictionary<string, Workflow> lookup) {
            if (lookup.TryGetValue(destinationRule.DestinationWorkflow, out var workflow)) {
                destinationRule.Destination = workflow;
            } else {
                throw new UnreachableException();
            }
        }
    }

    public bool Execute(ref Rating rating) {
        var r = rating;
        foreach (var rule in Rules) {
            rule.Execute(ref r);
        }

        return true;
    }
}

file abstract class Rule
{
    public static Rule Parse(string rawRule) {
        if (rawRule.Length == 1) {
            if (rawRule[0] == 'A') {
                return new AcceptRule();
            } else if (rawRule[0] == 'R') {
                return new RejectRule();
            } else {
                throw new UnreachableException();
            }
        } else if (rawRule.IndexOf(':') != -1) {
            return ConditionalRule.Parse(rawRule);
        } else {
            return new DestinationRule(rawRule);
        }
    }

    public abstract void Execute(ref Rating rating);
}

file class AcceptRule : Rule
{
    public static BigInteger Result { get; set; } = BigInteger.Zero;

    public override void Execute(ref Rating rating) {
        Result += rating.Volume;
    }
}

file class RejectRule : Rule
{
    public override void Execute(ref Rating rating) {
    }
}

file class DestinationRule(string destinationWorkflow) : Rule
{
    public string DestinationWorkflow { get; } = destinationWorkflow;
    public Workflow? Destination { get; set; }

    public override void Execute(ref Rating rating) {
        Destination!.Execute(ref rating);
    }
}

file class ConditionalRule(Variable variable, Operator @operator, int value, Rule rule) : Rule
{
    public Variable Variable { get; } = variable;
    public Operator Operator { get; } = @operator;
    public int Value { get; } = value;
    public Rule Rule { get; } = rule;

    public static ConditionalRule Parse(string rawRule) {
        var (rawCondition, rawDestination) = rawRule.Split(':');
        var (variable, op, value) = ParseCondition(rawCondition);
        return new ConditionalRule(
            variable, op, value,
            Day19.Rule.Parse(rawDestination)
        );
    }

    public override void Execute(ref Rating rating) {
        if (Operator == Operator.GreaterThan) {
            var (lo, hi) = Cut(rating, (int)Variable, Value);
            rating = lo;
            Rule.Execute(ref hi);
        } else if (Operator == Operator.LessThan) {
            var (lo, hi) = Cut(rating, (int)Variable, Value - 1);
            rating = hi;
            Rule.Execute(ref lo);
        } else {
            throw new UnreachableException();
        }
    }

    (Rating lo, Rating hi) Cut(Rating rating, int dimension, int value) {
        var r = rating[dimension];
        return (
            rating.Set(dimension, r with { End = Math.Min(value, r.End) }),
            rating.Set(dimension, r with { Begin = Math.Max(r.Begin, value + 1) })
        );
    }

    static (Variable, Operator, int) ParseCondition(string raw) {
        return (
            raw[0].ToVariable(),
            raw[1].ToOperator(),
            int.Parse(raw[2..])
        );
    }
}

file enum Variable
{
    X = 0,
    M,
    A,
    S
}

file enum Operator
{
    LessThan = 1,
    GreaterThan,
}

file static class Helper
{
    public static Variable ToVariable(this char self) {
        return self switch {
            'x' => Variable.X,
            'm' => Variable.M,
            'a' => Variable.A,
            's' => Variable.S
        };
    }

    public static Operator ToOperator(this char self) {
        return self switch {
            '<' => Operator.LessThan,
            '>' => Operator.GreaterThan
        };
    }
}