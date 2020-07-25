$migrationName = $args[0]
Write-Host "generate migrations $migrationName"

Get-ChildItem `
| Where-Object { $_.Name -match '\.Migrations\.' } `| ForEach-Object {
    $path = $_.FullName
    Set-Location $path
    $segments = $_.Name.Split(".")
    $rdms = $segments[$segments.Length - 1]
    $startup = "../Aguacongas.TheIdServer.$rdms.Startup"
    Write-Host "generate migration for $rdms in $path using startup project $startup"
    & dotnet build
    & dotnet ef --startup-project $startup migrations add $migrationName --context ApplicationDbContext --no-build
    & dotnet ef --startup-project $startup migrations add $migrationName --context ConfigurationDbContext --no-build
    & dotnet ef --startup-project $startup migrations add $migrationName --context OperationalDbContext --no-build
}
Set-Location $PSScriptRoot