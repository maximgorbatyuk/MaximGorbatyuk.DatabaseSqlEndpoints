using MaximGorbatyuk.DatabaseSqlEndpoints.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MaximGorbatyuk.DatabaseSqlEndpoints
{
    public static class DatabaseTablesViewExtensions
    {
        public const string DefaultOutputRoute = "/database-sql-endpoints/table";
        public const string DefaultReadRoute = "/database-sql-endpoints/read";
        public const string DefaultTablesMetaRoute = "/database-sql-endpoints/tables-meta";
        public const string DefaultExecuteRoute = "/database-sql-endpoints/execute";

        public static IDatabaseTablesSettings<TDbContext> UseSqlEndpoints<TDbContext>(
            this IApplicationBuilder app,
            int? port = null,
            bool checkForAuthentication = false,
            string roleToCheckForAuthorization = null,
            SqlEngine sqlEngine = default,
            int timeoutSeconds = Constants.DefaultSqlCommandTimeoutSec)
            where TDbContext : DbContext =>
            new DatabaseTablesSettings<TDbContext>(
                app,
                port,
                checkForAuthentication,
                roleToCheckForAuthorization,
                sqlEngine,
                timeoutSeconds);

        /// <summary>
        /// Returns content of the table.
        ///
        /// GET /database-sql-endpoints/table is a default route.
        /// </summary>
        /// <typeparam name="TDbContext">Database context.</typeparam>
        /// <param name="settings">Settings.</param>
        /// <param name="path">Path.</param>
        /// <returns>Settings instance.</returns>
        public static IDatabaseTablesSettings<TDbContext> UseTableOutputEndpoint<TDbContext>(
            this IDatabaseTablesSettings<TDbContext> settings,
            PathString path = default)
            where TDbContext : DbContext =>
            new MiddlewareRoute<DatabaseTablesMiddleware<TDbContext>, TDbContext>(
                settings: settings,
                path: path,
                methodName: HttpMethods.Get,
                defaultPathRoute: DefaultOutputRoute).Setup();

        /// <summary>
        /// Executes and read any SQL command.
        ///
        /// POST /database-sql-endpoints/read is a default route.
        /// </summary>
        /// <typeparam name="TDbContext">Database context.</typeparam>
        /// <param name="settings">Settings.</param>
        /// <param name="path">Path.</param>
        /// <returns>Settings instance.</returns>
        public static IDatabaseTablesSettings<TDbContext> UseReadEndpoint<TDbContext>(
            this IDatabaseTablesSettings<TDbContext> settings,
            PathString path = default)
            where TDbContext : DbContext =>
            new MiddlewareRoute<ReadSQlMiddleware<TDbContext>, TDbContext>(
                settings: settings,
                path: path,
                methodName: HttpMethods.Post,
                defaultPathRoute: DefaultReadRoute).Setup();

        /// <summary>
        /// Returns tables, their sizes and count of rows.
        ///
        /// POST /database-sql-endpoints/tables-meta is a default route.
        /// </summary>
        /// <typeparam name="TDbContext">Database context.</typeparam>
        /// <param name="settings">Settings.</param>
        /// <param name="path">Path.</param>
        /// <returns>Settings instance.</returns>
        public static IDatabaseTablesSettings<TDbContext> UseTablesInfoEndpoint<TDbContext>(
            this IDatabaseTablesSettings<TDbContext> settings,
            PathString path = default)
            where TDbContext : DbContext =>
            new MiddlewareRoute<TablesWithSizeAndRowsMiddleware<TDbContext>, TDbContext>(
                settings: settings,
                path: path,
                methodName: HttpMethods.Post,
                defaultPathRoute: DefaultTablesMetaRoute).Setup();

        /// <summary>
        /// Executes any changing SQL command. POST /database-sql-endpoints/execute is a default route.
        /// </summary>
        /// <typeparam name="TDbContext">Database context.</typeparam>
        /// <param name="settings">Settings.</param>
        /// <param name="path">Path.</param>
        /// <returns>Settings instance.</returns>
        public static IDatabaseTablesSettings<TDbContext> UseExecuteEndpoint<TDbContext>(
            this IDatabaseTablesSettings<TDbContext> settings,
            PathString path = default)
            where TDbContext : DbContext =>
            new MiddlewareRoute<ExecuteSQlMiddleware<TDbContext>, TDbContext>(
                settings: settings,
                path: path,
                methodName: HttpMethods.Post,
                defaultPathRoute: DefaultExecuteRoute).Setup();
    }
}