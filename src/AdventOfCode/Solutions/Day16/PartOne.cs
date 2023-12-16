using System.Diagnostics;
using Point = (int x, int y);

namespace AdventOfCode.Solutions.Day16;

using Map = Dictionary<Point, char>;
using Beam = (Point p, Point dir);

public class PartOne : ISolution
{
    public int Day => 16;
    public int Part => 1;

    public async Task<string> RunAsync(FileInfo file) {
        var input = await File.ReadAllLinesAsync(file.FullName);

        return new LightBeam(input)
            .Run()
            .ToString();
    }
}

file class LightBeam
{
    static readonly Point Up = (0, -1);
    static readonly Point Down = (0, 1);
    static readonly Point Left = (-1, 0);
    static readonly Point Right = (1, 0);

    int _rows;
    int _columns;
    HashSet<Beam> _visited;
    Map _map;
    Point _position;
    Point _direction;
    List<LightBeam> _splits = new();
    bool _root;

    public LightBeam(int rows, int columns, HashSet<Beam> visited, Map map, Point position, Point direction) {
        _rows = rows;
        _columns = columns;
        _visited = visited;
        _map = map;
        _position = position;
        _direction = direction;
    }

    public LightBeam(string[] input) {
        _root = true;
        _rows = input.Length;
        _columns = input[0].Length;
        _visited = new();
        _map = new();
        _position = (0, 0);
        _direction = Right;
        for (var row = 0; row < input.Length; row++) {
            for (int column = 0; column < input[row].Length; column++) {
                _map[(column, row)] = input[row][column];
            }
        }
    }

    public int Run() {
        Move();

        foreach (var lightBeam in _splits) {
            lightBeam.Run();
        }

        if (_root) {
            return _visited.Select(v => v.p).Distinct().Count();
        } else {
            return 0;
        }
    }

    void Move() {
        while (true) {
            _visited.Add((_position, _direction));

            char c = _map[_position];

            if (c == '.') {
                // continue to move
            } else if (c == '/') {
                if (_direction == Right) {
                    _direction = Up;
                } else if (_direction == Left) {
                    _direction = Down;
                } else if (_direction == Up) {
                    _direction = Right;
                } else if (_direction == Down) {
                    _direction = Left;
                } else {
                    Unreachable();
                }
            } else if (c == '\\') {
                if (_direction == Right) {
                    _direction = Down;
                } else if (_direction == Left) {
                    _direction = Up;
                } else if (_direction == Up) {
                    _direction = Left;
                } else if (_direction == Down) {
                    _direction = Right;
                } else {
                    Unreachable();
                }
            } else if (c == '|') {
                if (_direction == Left || _direction == Right) {
                    SplitBeam(Up);
                    _direction = Down;
                }
            } else if (c == '-') {
                if (_direction == Up || _direction == Down) {
                    SplitBeam(Left);
                    _direction = Right;
                }
            } else {
                Unreachable();
            }

            _position = _position.Add(_direction);
            if (IsOusideGrid(_position) || _visited.Contains((_position, _direction))) {
                break;
            }
        }
    }

    void SplitBeam(Point direction) {
        var p = _position.Add(direction);
        if (IsOusideGrid(p) || _visited.Contains((p, direction))) {
            return;
        }

        _splits.Add(new(_rows, _columns, _visited, _map, p, direction));
    }

    bool IsOusideGrid(Point position) {
        var (x, y) = position;
        if (x < 0 || y < 0 || x >= _columns || y >= _rows) {
            return true;
        }

        return false;
    }

    void Unreachable() => throw new UnreachableException();
}

file static class Helpers
{
    public static Point Add(in this Point self, in Point p) {
        return (self.x + p.x, self.y + p.y);
    }
}