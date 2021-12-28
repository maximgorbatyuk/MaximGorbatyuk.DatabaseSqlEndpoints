using Microsoft.AspNetCore.Builder;

namespace MaximGorbatyuk.DatabaseSqlEndpoints
{
    public interface IDatabaseTablesSettingsBase
    {
        IApplicationBuilder App { get; }

        int? Port { get; }

        bool CheckForAuthentication { get; }

        string RoleToCheckForAuthorization { get; }

        SqlEngine SqlEngine { get; }

        bool HasRole => RoleToCheckForAuthorization is not null;
    }
}