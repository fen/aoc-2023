using System.Diagnostics;

namespace AdventOfCode.Solutions.Day19;

public class PartOne : ISolution
{
    public int Day => 19;
    public int Part => 1;

    public async Task<string> RunAsync(FileInfo file) {
        string[] workflowLines = await File.ReadAllLinesAsync(file.FullName);

        var workflows = workflowLines.TakeWhile(l => l.Length > 0).Select(Workflow.Parse)
            .ToDictionary(w => w.Name);

        foreach (var workflow in workflows.Values) {
            workflow.Populate(workflows);
        }

        var ratings = workflowLines.SkipWhile(l => l.Length != 0)
            .Skip(1)
            .Select(Rating.Parse)
            .ToList();

        var @in = workflows["in"];

        int sum = 0;
        foreach (var rating in ratings) {
            if (@in.Execute(rating)) {
                sum += rating.Sum;
            }
        }

        return sum.ToString();
    }
}

file record struct Rating(int X, int M, int A, int S)
{
    public int Sum => X + M + A + S;

    public int GetValue(Variable variable) {
        return variable switch {
            Variable.X => X,
            Variable.M => M,
            Variable.A => A,
            Variable.S => S,
            _ => throw new ArgumentOutOfRangeException(nameof(variable), variable, null)
        };
    }

    public static Rating Parse(string raw) {
        raw = raw[1..^1];
        int x = 0, m = 0, a = 0, s = 0;
        foreach (var assignment in raw.Split(',')) {
            var (rawVariable, rawValue) = assignment.Split('=');

            int value = int.Parse(rawValue);
            switch (rawVariable.ToVariable()) {
            case Variable.X:
                x = value;
                break;
            case Variable.M:
                m = value;
                break;
            case Variable.A:
                a = value;
                break;
            case Variable.S:
                s = value;
                break;
            default:
                throw new ArgumentOutOfRangeException();
            }
        }

        return new(x, m, a, s);
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

    public bool Execute(in Rating rating) {
        foreach (var rule in Rules) {
            if (rule.TryExecute(rating, out bool accepted)) {
                return accepted;
            }
        }

        throw new UnreachableException();
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
            return ConditionalRule.ParseConditionalRule(rawRule);
        } else {
            return new DestinationRule(rawRule);
        }
    }

    public abstract bool TryExecute(in Rating rating, out bool accepted);
}

file class AcceptRule : Rule
{
    public override bool TryExecute(in Rating rating, out bool accepted) {
        accepted = true;
        return true;
    }
}

file class RejectRule : Rule
{
    public override bool TryExecute(in Rating rating, out bool accepted) {
        accepted = false;
        return true;
    }
}

file class DestinationRule(string destinationWorkflow) : Rule
{
    public string DestinationWorkflow { get; } = destinationWorkflow;
    public Workflow? Destination { get; set; }

    public override bool TryExecute(in Rating rating, out bool accepted) {
        accepted = Destination!.Execute(rating);
        return true;
    }
}

file class ConditionalRule(Variable variable, Operator @operator, int value, Rule rule) : Rule
{
    public Variable Variable { get; } = variable;
    public Operator Operator { get; } = @operator;
    public int Value { get; } = value;
    public Rule Rule { get; } = rule;

    public static ConditionalRule ParseConditionalRule(string rawRule) {
        var (rawCondition, rawDestination) = rawRule.Split(':');
        var (variable, op, value) = ParseCondition(rawCondition);
        return new ConditionalRule(
            variable, op, value,
            Parse(rawDestination)
        );
    }

    public override bool TryExecute(in Rating rating, out bool accepted) {
        var left = rating.GetValue(Variable);
        if (Operator == Operator.GreaterThan && left > Value) {
            return Rule.TryExecute(in rating, out accepted);
        } else if (Operator == Operator.LessThan && left < Value) {
            return Rule.TryExecute(in rating, out accepted);
        } else {
            accepted = false;
            return false;
        }
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
    X = 1,
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
    public static Variable ToVariable(this string self) {
        if (self.Length != 1) {
            throw new Exception("Unexpected variable name Hask");
        }

        return self[0].ToVariable();
    }

    public static Variable ToVariable(this char self) {
        return self switch {
            'x' => Variable.X,
            'm' => Variable.M,
            'a' => Variable.A,
            's' => Variable.S,
            _ => throw new UnreachableException()
        };
    }

    public static Operator ToOperator(this char self) {
        return self switch {
            '<' => Operator.LessThan,
            '>' => Operator.GreaterThan,
            _ => throw new UnreachableException()
        };
    }
}