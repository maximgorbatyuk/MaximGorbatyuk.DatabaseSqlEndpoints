using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MaximGorbatyuk.DatabaseSqlEndpoints.Middlewares
{
    public class TablesWithSizeAndRowsMiddleware<TDbContext> : DatabaseTableBaseMiddleware<TDbContext>
        where TDbContext : DbContext
    {
        internal const string SqlServerQuery = @"
SELECT
    t.NAME AS TableName,
    s.Name AS SchemaName,
    p.rows,
    SUM(a.total_pages) * 8 AS TotalSpaceKB, 
    CAST(ROUND(((SUM(a.total_pages) * 8) / 1024.00), 2) AS NUMERIC(36, 2)) AS TotalSpaceMB,
    SUM(a.used_pages) * 8 AS UsedSpaceKB, 
    CAST(ROUND(((SUM(a.used_pages) * 8) / 1024.00), 2) AS NUMERIC(36, 2)) AS UsedSpaceMB, 
    (SUM(a.total_pages) - SUM(a.used_pages)) * 8 AS UnusedSpaceKB,
    CAST(ROUND(((SUM(a.total_pages) - SUM(a.used_pages)) * 8) / 1024.00, 2) AS NUMERIC(36, 2)) AS UnusedSpaceMB
FROM
    sys.tables t
INNER JOIN
    sys.indexes i ON t.OBJECT_ID = i.object_id
INNER JOIN
    sys.partitions p ON i.object_id = p.OBJECT_ID AND i.index_id = p.index_id
INNER JOIN 
    sys.allocation_units a ON p.partition_id = a.container_id
LEFT OUTER JOIN
    sys.schemas s ON t.schema_id = s.schema_id
WHERE
    t.NAME NOT LIKE 'dt%' 
    AND t.is_ms_shipped = 0
    AND i.OBJECT_ID > 255 
GROUP BY
    t.Name, s.Name, p.Rows
ORDER BY
    TotalSpaceMB DESC, t.Name";

        public TablesWithSizeAndRowsMiddleware(
            RequestDelegate next,
            IOptions<IDatabaseTablesSettingsBase> settingsBase,
            string contentType = DefaultContentType)
            : base(next, settingsBase, contentType)
        {
        }

        protected override async Task<string> ResponseContentAsync(
            HttpContext httpContext,
            TDbContext context)
        {
            return new DataTableTextOutput(
                    await new ReadTableSqlCommand<TDbContext>(
                            Query(),
                            context,
                            Settings.TimeoutSec)
                        .AsDataTableAsync())
                .AsText();
        }

        private string Query() =>
            Settings.SqlEngine switch
            {
                SqlEngine.MSSQL => SqlServerQuery,
                _ => throw new NotSupportedException(
                    $"Sql engine {Settings.SqlEngine} is not supported")
            };
    }
}