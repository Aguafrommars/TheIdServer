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
    & dotnet ef --startup-project $startup migrations add $migrationName --context ApplicationDbContext
    & dotnet ef --startup-project $startup migrations add $migrationName --context ConfigurationDbContext
    & dotnet ef --startup-project $startup migrations add $migrationName --context OperationalDbContext
}
Set-Location $PSScriptRoot