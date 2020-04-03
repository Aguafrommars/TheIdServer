$fileversion = "$env:SemVer.0"
$path = (Get-Location).Path

Get-ChildItem -Path src -rec `
| Where-Object { $_.Name -like "*.csproj"
     } `
| ForEach-Object { 
    dotnet msbuild $_.FullName -t:Build -p:Configuration=Release -p:OutputPath=$path\artifacts\build -p:GeneratePackageOnBuild=true -p:Version=$env:Version -p:FileVersion=$fileversion
    if ($LASTEXITCODE -ne 0) {
            throw "build failed" + $d.FullName
    }
  }