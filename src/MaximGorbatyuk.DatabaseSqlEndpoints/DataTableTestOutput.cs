using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MaximGorbatyuk.DatabaseSqlEndpoints
{
    public class DataTableTextOutput
    {
        private readonly DataTable _dataTable;

        public DataTableTextOutput(DataTable dataTable)
        {
            _dataTable = dataTable;
        }

        public string AsText()
        {
            var rows = Rows();
            var columns = Columns();

            int[] maxColumnValues = rows
                .Select(x => x.Values.Select(v => v.StringValue.Length))
                .Union(new[] { columns.Select(c => c.ColumnName.Length) })
                .Aggregate(new int[columns.Count + 1].AsEnumerable(), (accumulate, x) => accumulate.Zip(x, Math.Max))
                .ToArray();

            var headerLine = "| " + string.Join(" | ", columns.Select((n, i) => n.ColumnName.PadRight(maxColumnValues[i]))) + " |";
            var headerDataDividerLine = "|-" + string.Join("+-", columns.Select((g, i) => new string('-', maxColumnValues[i] + 1))) + "|";

            var lines = new List<string>()
            {
                headerLine,
                headerDataDividerLine,
            };

            lines.AddRange(rows.Select(row => row.AsLine(maxColumnValues)));
            return lines.Aggregate((p, c) => p + Environment.NewLine + c);
        }

        private IReadOnlyCollection<Column> Columns()
        {
            var list = new List<Column>();
            foreach (DataColumn column in _dataTable.Columns)
            {
                list.Add(new Column(column));
            }

            return list;
        }

        private IReadOnlyCollection<Row> Rows()
        {
            var list = new List<Row>();
            foreach (DataRow row in _dataTable.Rows)
            {
                list.Add(new Row(row));
            }

            return list;
        }

        public record Column
        {
            public Column(DataColumn column)
            {
                ColumnName = column.ColumnName;
                ColumnType = column.DataType;
            }

            public string ColumnName { get; }

            public Type ColumnType { get; }
        }

        public record Row
        {
            public Row(DataRow row)
            {
                Values = row.ItemArray.Select(x => new RowValue(x)).ToArray();
            }

            public IReadOnlyCollection<RowValue> Values { get; }

            public string AsLine(int[] maxColumnValues)
            {
                return "| " + string.Join(" | ", Values.Select((s, i) => s.StringValue.PadRight(maxColumnValues[i]))) + " |";
            }
        }

        public record RowValue
        {
            public RowValue(object value)
            {
                Value = value;
            }

            public object Value { get; }

            public string StringValue => Value?.ToString() ?? string.Empty;
        }
    }
}