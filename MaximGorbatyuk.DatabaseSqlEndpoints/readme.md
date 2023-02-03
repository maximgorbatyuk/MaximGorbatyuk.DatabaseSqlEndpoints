# Database Tables viewer

This nuget allows you to view table content of your ASP.NET core application during runtime. The nuget creates a special endpoint and then return tables and data represented in html form.

## Get started

1. Install the [nuget](https://www.nuget.org/packages/MaximGorbatyuk.Utils.AspNetCore.DatabaseView):

```bash
dotnet add package MaximGorbatyuk.Utils.AspNetCore.DatabaseView
```

2. Add routing line into your `Startup.cs` file before UseEndpoints():

```csharp

class Startup
{
    public void Configure(IApplicationBuilder app)
    {
        // ... some settings

        app
            .UseDatabaseTable<AwesomeDbContext>() 
            .UseTableOutputEndpoint() // default route is /database-tables/view
            .UseReadEndpoint() // default route is /database-tables/read
            .UseExecuteEndpoint(); // default route is /database-tables/execute

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        // ... some settings
    }
}

```

3. Open `https:localhost:5001/database-tables/view?tableName=<tableName>` in your browser and view your data