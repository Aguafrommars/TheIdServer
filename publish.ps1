Write-Host $env:APPVEYOR $env:APPVEYOR_PULL_REQUEST_NUMBER

if ($env:APPVEYOR -and $env:APPVEYOR_PULL_REQUEST_NUMBER) {
    exit 0
}

$fileversion = "$env:SemVer.0"
$path = (Get-Location).Path

dotnet pack -c Release -o $path\artifacts\build -p:Version=$env:Version -p:FileVersion=$fileversion -p:SourceRevisionId=$env:APPVEYOR_REPO_COMMIT

dotnet publish src\Aguacongas.TheIdServer.Duende\Aguacongas.TheIdServer.Duende.csproj -c Release -o $path\artifacts\Aguacongas.TheIdServer.Duende -p:Version=$env:Version -p:FileVersion=$fileversion -p:SourceRevisionId=$env:APPVEYOR_REPO_COMMIT
if ($LASTEXITCODE -ne 0) {
    throw "publis failed src/Aguacongas.TheIdServer/Aguacongas.TheIdServer.Duende.csproj"
}

dotnet publish src\Aguacongas.TheIdServer.BlazorApp\Aguacongas.TheIdServer.BlazorApp.csproj -c Release -o $path\artifacts\Aguacongas.TheIdServer.BlazorApp -p:Version=$env:Version -p:FileVersion=$fileversion -p:SourceRevisionId=$env:APPVEYOR_REPO_COMMIT
if ($LASTEXITCODE -ne 0) {
    throw "publish failed src/Aguacongas.TheIdServer.BlazorApp/Aguacongas.TheIdServer.BlazorApp.csproj"
}

7z a $path\artifacts\build\Aguacongas.TheIdServer.Duende.$env:version.zip $path\artifacts\Aguacongas.TheIdServer.Duende
7z a $path\artifacts\build\Aguacongas.TheIdServer.BlazorApp$env:version.zip $path\artifacts\Aguacongas.TheIdServer.BlazorApp
