#pragma warning disable SA1005, SA1515
//
//            God Bless         No Bugs
//
//
//
//                      _oo0oo_
//                     o8888888o
//                     88" . "88
//                     (| -_- |)
//                     0\  =  /0
//                   ___/`---'\___
//                 .' \\|     |// '.
//                / \\|||  :  |||// \
//               / _||||| -:- |||||- \
//              |   | \\\  -  /// |   |
//              | \_|  ''\---/''  |_/ |
//              \  .-\__  '-'  ___/-. /
//            ___'. .'  /--.--\  `. .'___
//         ."" '<  `.___\_<|>_/___.' >' "".
//        | | :  `- \`.;`\ _ /`;.`/ - ` : | |
//        \  \ `_.   \_ __\ /__ _/   .-` /  /
//    =====`-.____`.___ \_____/___.-`___.-'=====
//                      `=---='
//
//  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//
#pragma warning restore SA1005, SA1515

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

namespace MaximGorbatyuk.DatabaseSqlEndpoints
{
    internal class DatabaseTablesSettings<TContext> : IDatabaseTablesSettings<TContext>
        where TContext : DbContext
    {
        public IApplicationBuilder App { get; }

        public int? Port { get; }

        public bool CheckForAuthentication { get; }

        public string RoleToCheckForAuthorization { get; }

        public SqlEngine SqlEngine { get; }

        public DatabaseTablesSettings(IApplicationBuilder app, int? port, bool checkForAuthentication, string roleToCheckForAuthorization, SqlEngine sqlEngine)
        {
            App = app;
            Port = port;
            CheckForAuthentication = checkForAuthentication;
            RoleToCheckForAuthorization = roleToCheckForAuthorization;
            SqlEngine = sqlEngine;
        }
    }
}