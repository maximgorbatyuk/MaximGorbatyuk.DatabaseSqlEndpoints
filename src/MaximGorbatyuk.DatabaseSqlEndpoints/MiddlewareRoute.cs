using System;
using System.Linq;
using System.Security.Claims;
using MaximGorbatyuk.DatabaseSqlEndpoints.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MaximGorbatyuk.DatabaseSqlEndpoints
{
    internal class MiddlewareRoute<TMiddleware, TDbContext>
        where TDbContext : DbContext
    {
        private readonly string _methodName;
        private readonly PathString _path;
        private readonly IDatabaseTablesSettings<TDbContext> _settings;

        public MiddlewareRoute(
            IDatabaseTablesSettings<TDbContext> settings,
            PathString path,
            string methodName,
            string defaultPathRoute)
        {
            _path = !path.HasValue ? new PathString(defaultPathRoute) : path;

            _methodName = methodName;
            _settings = settings;
        }

        public IDatabaseTablesSettings<TDbContext> Setup()
        {
            _settings.App.MapWhen(Condition, b => b.UseMiddleware<TMiddleware>(Options.Create((IDatabaseTablesSettingsBase)_settings)));
            return _settings;
        }

        private bool Condition(HttpContext context)
        {
            // We allow you to listen on all URLs by providing the empty PathString.
            // If you do provide a PathString, want to handle all of the special cases that
            // StartsWithSegments handles, but we also want it to have exact match semantics.
            //
            // Ex: /Foo/ == /Foo (true)
            // Ex: /Foo/Bar == /Foo (false)
            var shouldBeMapped = (_settings.Port == null || context.Connection.LocalPort == _settings.Port) &&
                   context.Request.Method == _methodName &&
                   (!_path.HasValue ||
                    (context.Request.Path.StartsWithSegments(_path, out var remaining) &&
                     string.IsNullOrEmpty(remaining)));

            CheckForAuthIfNecessary(shouldBeMapped, context);

            return shouldBeMapped;
        }

        private void CheckForAuthIfNecessary(bool shouldBeMapped, HttpContext context)
        {
            if (!shouldBeMapped || (!_settings.CheckForAuthentication && !_settings.HasRole))
            {
                return;
            }

            if (context.Request.IsLocal())
            {
                return;
            }

            if (context.User.Identity is not { IsAuthenticated: true })
            {
                throw new UnauthorizedAccessException("The user should be authenticated");
            }

            if (!_settings.HasRole)
            {
                return;
            }

            var role = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role);
            if (role is null || !string.Equals(role.Value, _settings.RoleToCheckForAuthorization, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new UnauthorizedAccessException($"The user should have a role of {_settings.RoleToCheckForAuthorization} to execute the SQL command");
            }
        }
    }
}