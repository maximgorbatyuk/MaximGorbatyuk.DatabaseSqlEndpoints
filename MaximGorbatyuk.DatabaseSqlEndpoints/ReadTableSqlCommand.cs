using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MaximGorbatyuk.DatabaseSqlEndpoints
{
    public class ReadTableSqlCommand<TContext>
        where TContext : DbContext
    {
        private readonly string _query;
        private readonly TContext _context;
        private readonly int _timeoutSeconds;

        public ReadTableSqlCommand(
            string query,
            TContext context,
            int? timeoutSeconds)
        {
            if (string.IsNullOrEmpty(query))
            {
                throw new ArgumentNullException(paramName: nameof(query));
            }

            _query = query;
            _context = context;
            _timeoutSeconds = timeoutSeconds ?? Constants.DefaultSqlCommandTimeoutSec;
        }

        public async Task<DataTable> AsDataTableAsync()
        {
            var table = new DataTable();
            try
            {
                await using var cmd = _context.Database.GetDbConnection().CreateCommand();

#pragma warning disable CA2100
                cmd.CommandTimeout = _timeoutSeconds;
                cmd.CommandText = _query;
#pragma warning restore CA2100

                if (cmd.Connection is not null)
                {
                    await cmd.Connection.OpenAsync();
                    table.Load(await cmd.ExecuteReaderAsync());
                }

                return table;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Could not execute the query {_query}. Error is {e.Message}", e);
            }
        }
    }
}