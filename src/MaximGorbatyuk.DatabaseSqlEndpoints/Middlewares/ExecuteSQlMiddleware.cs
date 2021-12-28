using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MaximGorbatyuk.DatabaseSqlEndpoints.Middlewares
{
    public class ExecuteSQlMiddleware<TDbContext> : BasePostRequestMiddleware<TDbContext>
        where TDbContext : DbContext
    {
        public ExecuteSQlMiddleware(RequestDelegate next, IOptions<IDatabaseTablesSettingsBase> settingsBase)
            : base(next, settingsBase)
        {
        }

        protected override async Task<string> ResponseContentAsync(string query, HttpContext httpContext, TDbContext context)
        {
            var result = await context.Database.ExecuteSqlRawAsync(query);
            return $"Rows affected: {result}";
        }
    }
}