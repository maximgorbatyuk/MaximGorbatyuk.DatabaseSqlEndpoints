using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MaximGorbatyuk.DatabaseSqlEndpoints.Middlewares
{
    public class ReadSQlMiddleware<TDbContext> : BasePostRequestMiddleware<TDbContext>
        where TDbContext : DbContext
    {
        public ReadSQlMiddleware(RequestDelegate next, IOptions<IDatabaseTablesSettingsBase> settingsBase)
            : base(next, settingsBase)
        {
        }

        protected override async Task<string> ResponseContentAsync(
            string query,
            HttpContext httpContext,
            TDbContext context)
        {
            return new DataTableTextOutput(
                await new ReadTableSqlCommand<TDbContext>(
                        query,
                        context,
                        Settings.TimeoutSec)
                    .AsDataTableAsync())
                .AsText();
        }
    }
}