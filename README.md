# MaximGorbatyuk.DatabaseSqlEndpoints

![GitHub Workflow Status](https://img.shields.io/github/workflow/status/maximgorbatyuk/MaximGorbatyuk.DatabaseSqlEndpoints/Build%20and%20test) ![Nuget](https://img.shields.io/nuget/dt/MaximGorbatyuk.DatabaseSqlEndpoints) ![GitHub release (latest by date)](https://img.shields.io/github/v/release/maximgorbatyuk/MaximGorbatyuk.DatabaseSqlEndpoints) ![GitHub](https://img.shields.io/github/license/maximgorbatyuk/MaximGorbatyuk.DatabaseSqlEndpoints)

This nuget allows you to view table content of your ASP.NET core application during runtime. The nuget creates a special endpoint and then return tables and data represented in html form.

## Get started

1. Install the [nuget](https://www.nuget.org/packages/MaximGorbatyuk.DatabaseSqlEndpoints/):

```bash
dotnet add package MaximGorbatyuk.DatabaseSqlEndpoints
```

2. Add routing line into your `Startup.cs` file before UseEndpoints():

```csharp

class Startup
{
    public void Configure(IApplicationBuilder app)
    {
        // ... some settings

        app
            .UseSqlEndpoints<AwesomeDbContext>() 
            .UseTableOutputEndpoint() // default route is /database-sql-endpoints/table
            .UseReadEndpoint() // default route is /database-sql-endpoints/read
            .UseExecuteEndpoint(); // default route is /database-sql-endpoints/execute

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        // ... some settings
    }
}

```

## Requests

### 1. Table content

Open `https:localhost:5001/database-sql-endpoints/table?tableName=<tableName>` in your browser and view your data

### 2. Reading some data with the SQL command

Send the following POST request:

```bash

POST https:localhost:5001/database-sql-endpoints/read

BODY:
{
    "query": "select 1;"
}

```

### 3. Execute any SQL script

Send the following POST request:

```bash

POST https:localhost:5001/database-sql-endpoints/execute

BODY:
{
    "query": "delete fronm users;"
}

```
