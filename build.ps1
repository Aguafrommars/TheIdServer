$result = 0
Get-ChildItem -rec `
| Where-Object { $_.Name -eq "TestResults" } `
| ForEach-Object {
	Remove-Item  $_.FullName -Recurse
}

Get-ChildItem -rec `
| Where-Object { $_.Name -eq "SonarQube.xml" } `
| ForEach-Object {
	Remove-Item  $_.FullName
}

$prNumber = $env:APPVEYOR_PULL_REQUEST_NUMBER
if ($prNumber) {
	$prArgs = "-d:sonar.pullrequest.key=$prNumber"
}
elseif ($env:APPVEYOR_REPO_BRANCH) {
	$prArgs = "-d:sonar.branch.name=$env:APPVEYOR_REPO_BRANCH"
}
Write-Host "dotnet sonarscanner begin /k:aguacongas_TheIdServer -o:aguacongas -d:sonar.host.url=https://sonarcloud.io -d:sonar.login=****** -d:sonar.coverageReportPaths=coverage\SonarQube.xml $prArgs -v:$env:Version"
dotnet sonarscanner begin /k:aguacongas_TheIdServer -o:aguacongas -d:sonar.host.url=https://sonarcloud.io -d:sonar.login=$env:sonarqube -d:sonar.coverageReportPaths=coverage\SonarQube.xml $prArgs -v:$env:Version

Write-Host "dotnet test -c Release --settings coverletArgs.runsettings --filter FullyQualifiedName!~MongoDb"

dotnet test -c Release --collect:"XPlat Code Coverage" --settings coverletArgs.runsettings --filter "FullyQualifiedName!~MongoDb" -v q

if ($LASTEXITCODE -ne 0) {
	$result = $LASTEXITCODE
}

$merge = ""
Get-ChildItem -rec `
| Where-Object { $_.Name -eq "coverage.cobertura.xml" } `
| ForEach-Object { 
	$path = $_.FullName
	$merge = "$path;$merge"
}
Write-Host $merge
ReportGenerator\tools\netcoreapp3.0\ReportGenerator.exe "-reports:$merge" "-targetdir:coverage" "-reporttypes:SonarQube"
	
dotnet sonarscanner end -d:sonar.login=$env:sonarqube

exit $result