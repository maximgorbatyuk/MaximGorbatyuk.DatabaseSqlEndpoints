using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MaximGorbatyuk.DatabaseSqlEndpoints.Middlewares
{
    public class DatabaseTablesMiddleware<TDbContext> : DatabaseTableBaseMiddleware<TDbContext>
        where TDbContext : DbContext
    {
        public DatabaseTablesMiddleware(RequestDelegate next, IOptions<IDatabaseTablesSettingsBase> settingsBase)
            : base(next, settingsBase)
        {
        }

        private async Task<string> PageAsync(TDbContext db, string tableName)
        {
            var table = await new ReadTableSqlCommand<TDbContext>(query: $"SELECT * FROM {tableName}", db).AsDataTableAsync();

            return new DataTableTextOutput(table).AsText();
        }

        private string TableNameOrFail(HttpContext context)
        {
            string tableName = null;
            if (context.Request.Query.TryGetValue("tableName", out var value))
            {
                tableName = value.FirstOrDefault();
            }

            if (tableName is null)
            {
                throw new InvalidOperationException("You have to provide table name");
            }

            return FormatTableName(tableName);
        }

        private string FormatTableName(string tableName)
        {
            return Settings.SqlEngine switch
            {
                SqlEngine.PostgreSQL => $"\"{tableName}\"",
                _ => tableName
            };
        }

        protected override Task<string> ResponseContentAsync(HttpContext httpContext, TDbContext context)
        {
            return PageAsync(context, TableNameOrFail(httpContext));
        }
    }
}