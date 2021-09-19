$fileversion = "$env:SemVer.0"
$path = (Get-Location).Path

dotnet pack -c Release -o $path\artifacts\build -p:Version=$env:Version -p:FileVersion=$fileversion

dotnet publish src\Aguacongas.TheIdServer.IS4\Aguacongas.TheIdServer.IS4.csproj -c Release -o $path\artifacts\Aguacongas.TheIdServer.IS4 -p:Version=$env:Version -p:FileVersion=$fileversion
if ($LASTEXITCODE -ne 0) {
    throw "publis failed src/Aguacongas.TheIdServer/Aguacongas.TheIdServer.IS4.csproj"
}

dotnet publish src\Aguacongas.TheIdServer.Duende\Aguacongas.TheIdServer.Duende.csproj -c Release -o $path\artifacts\Aguacongas.TheIdServer.Duende -p:Version=$env:Version -p:FileVersion=$fileversion
if ($LASTEXITCODE -ne 0) {
    throw "publis failed src/Aguacongas.TheIdServer/Aguacongas.TheIdServer.Duende.csproj"
}

dotnet publish src\Aguacongas.TheIdServer.BlazorApp\Aguacongas.TheIdServer.BlazorApp.csproj -c Release -o $path\artifacts\Aguacongas.TheIdServer.BlazorApp -p:Version=$env:Version -p:FileVersion=$fileversion
if ($LASTEXITCODE -ne 0) {
    throw "publish failed src/Aguacongas.TheIdServer.BlazorApp/Aguacongas.TheIdServer.BlazorApp.csproj"
}

7z a $path\artifacts\build\Aguacongas.TheIdServer.IS4.$env:version.zip $path\artifacts\Aguacongas.TheIdServer.IS4
7z a $path\artifacts\build\Aguacongas.TheIdServer.Duende.$env:version.zip $path\artifacts\Aguacongas.TheIdServer.Duende
7z a $path\artifacts\build\Aguacongas.TheIdServer.BlazorApp$env:version.zip $path\artifacts\Aguacongas.TheIdServer.BlazorApp
