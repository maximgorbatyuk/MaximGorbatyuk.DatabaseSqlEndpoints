using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MaximGorbatyuk.DatabaseSqlEndpoints.Middlewares
{
    public abstract class DatabaseTableBaseMiddleware<TDbContext>
        where TDbContext : DbContext
    {
        private readonly RequestDelegate _next;
        private readonly string _contentType;

        protected IDatabaseTablesSettingsBase Settings { get; }

        protected DatabaseTableBaseMiddleware(RequestDelegate next, IOptions<IDatabaseTablesSettingsBase> settingsBase, string contentType = "text/plain; charset=UTF-8")
        {
            _next = next;
            _contentType = contentType;
            Settings = settingsBase.Value;
        }

        public async Task InvokeAsync(HttpContext context, TDbContext db)
        {
            context.Response.ContentType = _contentType;
            await context.Response.WriteAsync(await ResponseContentAsync(context, db));
        }

        protected abstract Task<string> ResponseContentAsync(HttpContext httpContext, TDbContext context);
    }
}