$fileversion = "$env:SemVer.0"
$path = (Get-Location).Path

dotnet pack -c Release -o $path\artifacts\build -p:Version=$env:Version -p:FileVersion=$fileversion

dotnet publish src\Aguacongas.TheIdServer\Aguacongas.TheIdServer.csproj -c Release -o $path\artifacts\Aguacongas.TheIdServer -p:Version=$env:Version -p:FileVersion=$fileversion
if ($LASTEXITCODE -ne 0) {
    throw "publis failed src/Aguacongas.TheIdServer/Aguacongas.TheIdServer.csproj"
}

dotnet publish src\Aguacongas.TheIdServer.BlazorApp\Aguacongas.TheIdServer.BlazorApp.csproj -c Release -o $path\artifacts\Aguacongas.TheIdServer.BlazorApp -p:Version=$env:Version -p:FileVersion=$fileversion
if ($LASTEXITCODE -ne 0) {
    throw "publish failed src/Aguacongas.TheIdServer.BlazorApp/Aguacongas.TheIdServer.BlazorApp.csproj"
}

7z a $path\artifacts\build\Aguacongas.TheIdServer.$env:version.zip $path\artifacts\Aguacongas.TheIdServer

7z a $path\artifacts\build\Aguacongas.TheIdServer.BlazorApp$env:version.zip $path\artifacts\Aguacongas.TheIdServer.BlazorApp
