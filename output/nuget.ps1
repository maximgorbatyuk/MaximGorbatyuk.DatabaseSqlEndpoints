$data = @(
   "./../src/MaximGorbatyuk.DatabaseSqlEndpoints/"
);

$data | ForEach-Object {
    dotnet build $_
    nuget pack $_
}
