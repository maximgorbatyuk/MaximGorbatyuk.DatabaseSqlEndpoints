using Microsoft.EntityFrameworkCore;

namespace MaximGorbatyuk.DatabaseSqlEndpoints
{
    public interface IDatabaseTablesSettings<TContext> : IDatabaseTablesSettingsBase
        where TContext : DbContext
    {
    }
}