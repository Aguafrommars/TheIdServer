$fileversion = "$env:SemVer.0"
$path = (Get-Location).Path

Get-ChildItem -Path src -rec `
| Where-Object { $_.Name -like "*.csproj" -and 
        $_.Name -ne "Aguacongas.TheIdServer.csproj"  
     } `
| ForEach-Object { 
    if ($_.Name -like "*.Startup.csproj")
    {
        continue
    }
    dotnet msbuild $_.FullName -t:Build -p:Configuration=Release -p:OutputPath=$path\artifacts\build -p:GeneratePackageOnBuild=true -p:Version=$env:Version -p:FileVersion=$fileversion
    if ($LASTEXITCODE -ne 0) {
        throw "build failed" + $d.FullName
    }
  }

dotnet msbuild src\Aguacongas.TheIdServer\Aguacongas.TheIdServer.csproj -t:Publish -p:Configuration=Release -p:OutputPath=$path\artifacts\Aguacongas.TheIdServer -p:Version=$env:Version -p:FileVersion=$fileversion -noConsoleLogger
if ($LASTEXITCODE -ne 0) {
    throw "publis failed src/Aguacongas.TheIdServer/Aguacongas.TheIdServer.csproj"
}

dotnet msbuild src\Aguacongas.TheIdServer.BlazorApp\Aguacongas.TheIdServer.BlazorApp.csproj -t:Publish -p:Configuration=Release -p:OutputPath=$path\artifacts\Aguacongas.TheIdServer.BlazorApp -p:Version=$env:Version -p:FileVersion=$fileversion -noConsoleLogger
if ($LASTEXITCODE -ne 0) {
    throw "publish failed src/Aguacongas.TheIdServer.BlazorApp/Aguacongas.TheIdServer.BlazorApp.csproj"
}

7z a $path\artifacts\build\Aguacongas.TheIdServer.$env:version.zip $path\artifacts\Aguacongas.TheIdServer

7z a $path\artifacts\build\Aguacongas.TheIdServer.BlazorApp$env:version.zip $path\artifacts\Aguacongas.TheIdServer.BlazorApp