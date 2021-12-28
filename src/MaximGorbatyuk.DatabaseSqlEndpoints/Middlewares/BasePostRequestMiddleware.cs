using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MaximGorbatyuk.DatabaseSqlEndpoints.Middlewares
{
    public abstract class BasePostRequestMiddleware<TDbContext> : DatabaseTableBaseMiddleware<TDbContext>
        where TDbContext : DbContext
    {
        protected BasePostRequestMiddleware(RequestDelegate next, IOptions<IDatabaseTablesSettingsBase> settingsBase)
            : base(next, settingsBase)
        {
        }

        protected override async Task<string> ResponseContentAsync(HttpContext httpContext, TDbContext context)
        {
            using var reader = new StreamReader(httpContext.Request.Body);
            var body = await reader.ReadToEndAsync();

            var request = DeserializeOrFail(body).ValidOrFail();

            return await ResponseContentAsync(request.Query, httpContext, context);
        }

        private static SqlRequest DeserializeOrFail(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentNullException(nameof(source));
            }

            try
            {
                var sourceToProcess = source
                    .Replace("\t", string.Empty)
                    .Replace("\r", string.Empty)
                    .Replace("\n", string.Empty);

                return JsonSerializer.Deserialize<SqlRequest>(sourceToProcess, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Could not deserialize body: {source}", e);
            }
        }

        protected abstract Task<string> ResponseContentAsync(string query, HttpContext httpContext, TDbContext context);

        public record SqlRequest
        {
            public string Query { get; set; }

            public SqlRequest ValidOrFail()
            {
                if (string.IsNullOrEmpty(Query))
                {
                    throw new InvalidOperationException($"The query should not be empty");
                }

                return this;
            }
        }
    }
}