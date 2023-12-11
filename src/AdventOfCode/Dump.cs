#pragma warning disable

using System.Collections;
using System.Data;
using System.Reflection;

namespace AdventOfCode;

internal struct ColorString(string s, ConsoleColor foreground, ConsoleColor background)
{
    public readonly string String = s;
    public readonly ConsoleColor Foreground = foreground;
    public readonly ConsoleColor Background = background;
}

internal class ColumnDetails(DataColumn column) : MemberDetails(column.ColumnName, TypeDetails.Get(column.DataType))
{
    public readonly DataColumn DataColumn = column;

    public override MemberValue GetValue(object instance) {
        throw new NotImplementedException();
    }

    public override string ToString() => DataColumn.ToString();
}

internal class ConsoleWriter
{
    protected virtual void WritePlain(string s) {
        Console.Write(s);
    }

    protected virtual void Write(string s, ConsoleColor foreground, ConsoleColor background) {
        Console.ForegroundColor = foreground;
        Console.BackgroundColor = background;
        Console.Write(s);
        Console.ResetColor();
    }

    protected virtual void WriteLine() {
        Console.WriteLine();
    }

    static readonly ColorString _Null = new("null", ConsoleColor.Green, ConsoleColor.Black);
    static readonly ColorString _Separator = new(" ", ConsoleColor.White, ConsoleColor.DarkMagenta);

    static readonly ColorString
        _Ellipsis = new("\u2026", ConsoleColor.DarkMagenta, ConsoleColor.Cyan); // TODO see how this looks

    const ConsoleColor HEADING_FOREGROUND = ConsoleColor.White;
    const ConsoleColor HEADING_BACKGROUND = ConsoleColor.DarkCyan;

    private void Write(ColorString cs) {
        Write(cs.String, cs.Foreground, cs.Background);
    }

    private void WriteFixed(ColorString value, int width, TypeCode typeCode) {
        var len = value.String.Length;
        if (len == width) {
            Write(value);
        } else if (len < width) {
            // right align numbers
            string padded = typeCode >= TypeCode.SByte && typeCode <= TypeCode.Decimal
                ? value.String.PadLeft(width)
                : value.String.PadRight(width);
            Write(padded, value.Foreground, value.Background);
        } else {
            Write(value.String.Substring(0, width - 1), value.Foreground, value.Background);
            Write(_Ellipsis);
        }
    }

    private void WritePadding(int padding) {
        if (padding < 1) {
            return;
        }

        WritePlain(new string(' ', padding));
    }

    private void WriteLabel(string label, int padding) {
        WritePadding(padding);
        Write(label, ConsoleColor.White, ConsoleColor.DarkBlue);
    }

    private void WriteEnumerableLabel(TypeDetails details, int shown, int? count, int padding, int enumerableLimit) {
        string label;
        if (shown < enumerableLimit || count == enumerableLimit) {
            label = " (" + shown + " items)";
        } else if (count.HasValue) {
            label = " (First " + shown + " items of " + count + ")";
        } else {
            label = " (First " + shown + " items)";
        }

        WriteLabel(details.TypeLabel + label, padding);
        WriteLine();
    }

    private ColorString GetString(TypeDetails details, object? instance) {
        if (instance == null) {
            return _Null;
        }

        var typeCode = details.SimpleTypeCode;

        var foreground =
            typeCode == TypeCode.String ? ConsoleColor.Cyan : // string
            typeCode != TypeCode.Object ? ConsoleColor.White : // primitive
            details.Type.IsClass ? ConsoleColor.Magenta : // class
            ConsoleColor.Yellow; // struct

        return new ColorString(
            instance.ToString(),
            foreground,
            details.NullableStruct ? ConsoleColor.DarkGreen : ConsoleColor.Black);
    }

    private ColorString GetString(MemberValue value) {
        if (value.Exception != null) {
            return new ColorString(value.Exception.Message, ConsoleColor.Red, ConsoleColor.Black);
        } else {
            return GetString(value.Details, value.Value);
        }
    }

    private void DumpComplexEnumerable(TypeDetails details, object instance, int padding, int enumerableLimit) {
        int? count;
        var items = details.GetEnumerableMemberValues(instance, enumerableLimit, out count);

        var itemType = details.ItemDetails;
        int columnCount = itemType.Members.Length;
        var columnWidths = new int[columnCount];
        int columnIndex;
        ColorString[] row;
        var allValues = new List<ColorString[]>(items.Count + 1);

        // get the column headings
        columnIndex = 0;
        row = new ColorString[columnCount];
        foreach (var member in itemType.Members) {
            row[columnIndex] = new ColorString(member.Name, HEADING_FOREGROUND, HEADING_BACKGROUND);
            columnWidths[columnIndex] = member.Name.Length;

            columnIndex++;
        }

        allValues.Add(row);

        // get all the values
        foreach (var item in items) {
            if (item == null) {
                allValues.Add(null);
                continue;
            }

            columnIndex = 0;
            row = new ColorString[columnCount];
            foreach (var value in item) {
                var cs = GetString(value);
                row[columnIndex] = cs;
                if (cs.String.Length > columnWidths[columnIndex]) {
                    columnWidths[columnIndex] = cs.String.Length;
                }

                columnIndex++;
            }

            allValues.Add(row);
        }


        // echo
        WriteEnumerableLabel(details, items.Count, count, padding, enumerableLimit);
        padding++;

        int rowCount = 0;
        foreach (var item in allValues) {
            WritePadding(padding);
            if (item == null) {
                Write(_Null);
            } else {
                columnIndex = 0;
                foreach (var value in item) {
                    // grab TypeCode but not for headings
                    var typeCode = rowCount == 0
                        ? TypeCode.String
                        : itemType.Members[columnIndex].TypeDetails.SimpleTypeCode;

                    Write(_Separator);
                    WriteFixed(value, columnWidths[columnIndex], typeCode);
                    columnIndex++;
                }
            }

            WriteLine();
            rowCount++;
        }
    }

    private void DumpSimpleEnumerable(TypeDetails details, object instance, int padding, int enumerableLimit) {
        int? count;
        var items = details.GetEnumerableSimpleValues(instance, enumerableLimit, out count);

        // get all strings
        int maxLength = 0;
        var values = new List<ColorString>(items.Count);
        foreach (var value in items) {
            var cs = GetString(details.ItemDetails, value);
            values.Add(cs);

            if (cs.String.Length > maxLength) {
                maxLength = cs.String.Length;
            }
        }

        // echo
        WriteEnumerableLabel(details, items.Count, count, padding, enumerableLimit);
        padding++;
        foreach (var value in values) {
            WritePadding(padding);
            WriteFixed(value, maxLength, details.ItemDetails.SimpleTypeCode);
            WriteLine();
        }
    }

    private void DumpMembers(TypeDetails details, object instance, int padding) {
        WriteLabel(details.TypeLabel, padding);
        WriteLine();

        var stringified = instance.ToString();
        if (!string.IsNullOrEmpty(stringified) && stringified != details.Type.FullName &&
            stringified != instance.GetType().FullName) {
            WritePadding(padding);
            Write(
                stringified,
                details.Type.IsClass
                    ? ConsoleColor.Magenta
                    : // class
                    ConsoleColor.Yellow, // struct
                ConsoleColor.Black);
            WriteLine();
        }

        padding++;
        var values = details.GetMemberValues(instance);

        foreach (var value in values) {
            WritePadding(padding);
            Write(value.MemberName.PadRight(details.MaxMemberNameLength, ' '), HEADING_FOREGROUND, HEADING_BACKGROUND);
            Write(_Separator);
            Write(GetString(value));
            WriteLine();
        }
    }

    private void Dump(TypeDetails details, object instance, int padding, int enumerableLimit) {
        if (instance == null || details.SimpleTypeCode != TypeCode.Object) {
            WritePadding(padding);
            Write(GetString(details, instance));
            WriteLine();
        } else if (!details.IsEnumerable) {
            DumpMembers(details, instance, padding);
            WriteLine(); // extra
        } else if (details.ItemDetails.SimpleTypeCode != TypeCode.Object) {
            DumpSimpleEnumerable(details, instance, padding, enumerableLimit);
            WriteLine(); // extra
        } else {
            DumpComplexEnumerable(details, instance, padding, enumerableLimit);
            WriteLine(); // extra
        }
    }

    public T Dump<T>(T it, string label, int enumerableLimit) {
        object o = it;
        int padding = 0;

        if (!string.IsNullOrEmpty(label)) {
            Write(label, ConsoleColor.Black, ConsoleColor.Gray);
            WriteLine();
            padding = 1;
        }

        if (o == null) {
            WritePadding(padding);
            Write(_Null);
            WriteLine();
            return it;
        }

        var type = o.GetType();
        if (type != typeof(T) && typeof(T) != typeof(object) && !type.IsPublic && typeof(T).IsPublic) {
            type = typeof(T);
        }

        TypeDetails details;
        if (it is DataTable datatable) {
            details = new DataTableDetails(datatable);
        } else if (it is DataSet dataset) {
            foreach (DataTable table in dataset.Tables) {
                Dump(table, table.Namespace, enumerableLimit);
            }

            return it;
        } else {
            details = TypeDetails.Get(type);
        }

        Dump(details, o, padding, enumerableLimit);

        return it;
    }
}

internal class DataTableDetails : TypeDetails
{
    readonly DataTable _table;

    public DataTableDetails(DataTable table) : base(table) {
        _table = table;
    }

    public override List<MemberValue[]> GetEnumerableMemberValues(object instance, int limit, out int? count) {
        var size = Math.Min(_table.Rows.Count, limit);
        var values = new List<MemberValue[]>(size);
        var rows = _table.Rows;
        count = rows.Count;
        var columns = _table.Columns.Count;
        var details = Members;

        for (var i = 0; i < size; i++) {
            var members = new MemberValue[columns];
            values.Add(members);

            for (int j = 0; j < members.Length; j++) {
                members[j] = new MemberValue(details[j], rows[i][j], null);
            }
        }

        return values;
    }

    public override bool IsEnumerable => true;
}

public static class Extensions
{
    public static ushort? RowLimit { get; set; }

    private static readonly ConsoleWriter _Writer = new();

    public static T Dump<T>(this T it, string label = null) {
        return _Writer.Dump(it, label, GetEnumerableLimit());
    }

    // Non generic version for easier calling from reflection, powershell, etc.
    public static void DumpObject(object it, string label = null) {
        _Writer.Dump(it, label, GetEnumerableLimit());
    }

    private static int GetEnumerableLimit() {
        if (RowLimit is ushort limit) {
            return limit;
        }

        int height;
        try {
            height = Console.WindowHeight;
        } catch (Exception) {
            return 24;
        }

        return Math.Max(height - 5, 16);
    }
}

internal class MemberDetails
{
    public readonly TypeDetails TypeDetails;
    public readonly FieldInfo FieldInfo;
    public readonly PropertyInfo PropertyInfo;
    public readonly string Name;

    private MemberDetails(FieldInfo fi) {
        FieldInfo = fi;
        Name = fi.Name;
        TypeDetails = TypeDetails.Get(fi.FieldType);
    }

    private MemberDetails(PropertyInfo pi) {
        PropertyInfo = pi;
        Name = pi.Name;
        TypeDetails = TypeDetails.Get(pi.PropertyType);
    }

    protected MemberDetails(string name, TypeDetails details) {
        Name = name;
        TypeDetails = details;
    }

    public virtual MemberValue GetValue(object instance) {
        Exception exception = null;
        object value;

        if (FieldInfo != null) {
            value = FieldInfo.GetValue(instance);
        } else {
            try {
                value = PropertyInfo.GetValue(instance, null);
            } catch (TargetInvocationException tie) {
                value = null;
                exception = tie.InnerException ?? tie;
            }
        }

        return new MemberValue(this, value, exception);
    }

    public static MemberDetails[] GetAllMembers(TypeDetails details) {
        var properties = details.Type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        var fields = details.Type.GetFields(BindingFlags.Instance | BindingFlags.Public);

        var members = properties
            .Where(pi => pi.GetGetMethod() != null && pi.GetIndexParameters().Length == 0)
            .Select(pi => new MemberDetails(pi));

        members = members.Concat(
            fields.Select(fi => new MemberDetails(fi))
        );

        return members.ToArray();
    }

    public override string ToString() {
        return ((object)PropertyInfo ?? FieldInfo).ToString();
    }
}

internal struct MemberValue
{
    public readonly TypeDetails Details;
    public readonly string MemberName;
    public readonly object Value;
    public readonly Exception? Exception;

    public MemberValue(MemberDetails md, object value, Exception exception) : this(md.TypeDetails, md.Name, value,
        exception) {
    }

    public MemberValue(TypeDetails td, string memberName, object value, Exception exception) {
        Details = td;
        MemberName = memberName;
        Value = value;
        Exception = exception;
    }
}

internal class TypeDetails
{
    const string _AnonymousLabel = "\u00f8";

    public readonly Type Type;
    public readonly string TypeLabel;

    /// <summary>
    /// TypeCode unwrapping Nullable<> if needed
    /// </summary>
    public readonly TypeCode SimpleTypeCode;

    public readonly bool NullableStruct;

    public readonly MemberDetails[] Members;
    public readonly TypeDetails ItemDetails;
    private readonly GetGenericEnumeratorDelegate _GetItemEnumerator;
    public readonly int MaxMemberNameLength;

    private TypeDetails(Type type) {
        Type = type;
        _UnderConstruction.Push(this);
        try {
            TypeLabel = GetFullName(type);
            Members = null;
            var underlying = Nullable.GetUnderlyingType(type);
            NullableStruct = underlying != null;
            SimpleTypeCode = Type.GetTypeCode(underlying ?? type);

            if (SimpleTypeCode != TypeCode.Object) {
                // primitive-ish, that's enough details
                return;
            }

            // IEnumerable<>
            Type elementType = null;
            if (type.IsArray) {
                elementType = type.GetElementType();
            } else {
                IEnumerable<Type> interfaces = type.GetInterfaces();
                if (type.IsInterface) {
                    interfaces = new[] { type }.Concat(interfaces);
                }

                var ienumerableType = interfaces.FirstOrDefault(ti =>
                    ti.IsGenericType && ti.GetGenericTypeDefinition() == typeof(IEnumerable<>));

                if (ienumerableType != null) {
                    elementType = ienumerableType.GetGenericArguments()[0];
                }
            }

            if (elementType != null) {
                ItemDetails = Get(elementType);
                _GetItemEnumerator = CreateDelegate(ItemDetails.Type);
                return;
            }

            // not IEnumerable<>
            Members = MemberDetails.GetAllMembers(this);
            if (Members.Any()) {
                MaxMemberNameLength = Members.Max(m => m.Name.Length);
            }
        } finally {
            _UnderConstruction.Pop();
        }
    }

    protected TypeDetails(DataTable table) {
        SimpleTypeCode = TypeCode.Object;
        ItemDetails = this;
        Members = new MemberDetails[table.Columns.Count];
        for (int i = 0; i < Members.Length; i++) {
            Members[i] = new ColumnDetails(table.Columns[i]);
        }
    }

    public virtual bool IsEnumerable => _GetItemEnumerator != null;

    public MemberValue[] GetMemberValues(object instance) {
        var members = Members;
        var values = new MemberValue[members.Length];
        for (int i = 0; i < values.Length; i++) {
            values[i] = members[i].GetValue(instance);
        }

        return values;
    }

    public List<object> GetEnumerableSimpleValues(object instance, int limit, out int? count) {
        IEnumerator enumerator;
        IDisposable disposable;

        _GetItemEnumerator(instance, out enumerator, out disposable, out count);
        var itemValues = new List<object>(Math.Min(count ?? ushort.MaxValue, limit));
        using (disposable) {
            int total = 0;
            while (enumerator.MoveNext() && total++ < limit) {
                itemValues.Add(enumerator.Current);
            }
        }

        return itemValues;
    }

    public virtual List<MemberValue[]> GetEnumerableMemberValues(object instance, int limit, out int? count) {
        var values = GetEnumerableSimpleValues(instance, limit, out count);
        var memberValues = new List<MemberValue[]>(values.Count);
        foreach (var value in values) {
            memberValues.Add(value == null ? null : ItemDetails.GetMemberValues(value));
        }

        return memberValues;
    }

    public override string ToString() {
        return Type.ToString();
    }

    private static string[] _Namespaces = { "System", "System.Collections.Generic" };

    internal static string GetFullName(Type type) {
        var underlying = Nullable.GetUnderlyingType(type);
        if (underlying != null) {
            return GetFullName(underlying) + "?";
        }

        if (type.IsArray) {
            return GetFullName(type.GetElementType()) + "[]";
        }

        var fullName = _Namespaces.Contains(type.Namespace) ? type.Name : type.FullName;

        if (type.IsGenericType) {
            if (fullName.StartsWith("<>", StringComparison.Ordinal)) {
                return _AnonymousLabel;
            }

            int index = fullName.IndexOf('`');
            if (index != -1) {
                fullName = fullName.Substring(0, index);
            }

            var genericArgs = type.GetGenericArguments();
            fullName +=
                "<" +
                string.Join(", ", genericArgs.Select(t => GetFullName(t))) +
                ">";
        }

        return fullName;
    }

    private delegate void GetGenericEnumeratorDelegate(object enumerable, out IEnumerator enumerator,
        out IDisposable disposable, out int? count);

    private static void GetGenericEnumerator<T>(object enumerable, out IEnumerator enumerator,
        out IDisposable disposable, out int? count) {
        var genericEnumerator = ((IEnumerable<T>)enumerable).GetEnumerator();
        enumerator = (IEnumerator)genericEnumerator;
        disposable = (IDisposable)genericEnumerator;

        var collection = enumerable as ICollection<T>;
        count = collection == null ? (int?)null : collection.Count;
    }

    private static MethodInfo _GenericDefinition =
        ((GetGenericEnumeratorDelegate)GetGenericEnumerator<int>).Method.GetGenericMethodDefinition();

    private static GetGenericEnumeratorDelegate CreateDelegate(Type itemType) {
        return (GetGenericEnumeratorDelegate)Delegate.CreateDelegate(
            typeof(GetGenericEnumeratorDelegate),
            _GenericDefinition.MakeGenericMethod(itemType));
    }


    private static Dictionary<Type, TypeDetails> _Cache = new();

    [ThreadStatic]
    static Stack<TypeDetails> _UnderConstruction;

    public static TypeDetails Get(Type type) {
        TypeDetails md;
        lock (_Cache) {
            if (_Cache.TryGetValue(type, out md)) {
                return md;
            }
        }

        var underConstruction = _UnderConstruction;
        if (underConstruction == null) {
            _UnderConstruction = new Stack<TypeDetails>();
        } else {
            md = underConstruction.FirstOrDefault(td => td.Type == type);
            if (md != null) {
                return md;
            }
        }

        md = new TypeDetails(type);

        lock (_Cache) {
            _Cache[type] = md;
        }

        return md;
    }
}

#pragma warning restore