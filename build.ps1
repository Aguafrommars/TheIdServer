$result = 0

if ($isLinux) {
	Get-ChildItem -rec `
	| Where-Object { $_.Name -like "*.IntegrationTest.csproj" `
		   -Or $_.Name -like "*.Test.csproj" `
		 } `
	| ForEach-Object { 
		Set-Location $_.DirectoryName
		dotnet test
	
		if ($LASTEXITCODE -ne 0) {
			$result = $LASTEXITCODE
		}
	}
} else {
	$prNumber = $env:APPVEYOR_PULL_REQUEST_NUMBER
	if ($prNumber) {
        $prArgs = "-d:sonar.pullrequest.key=$prNumber"
    } elseif ($env:APPVEYOR_REPO_BRANCH) {
        $prArgs = "-d:sonar.branch.name=$env:APPVEYOR_REPO_BRANCH"
    }
	dotnet sonarscanner begin /k:aguacongas_TheIdServer -o:aguacongas -d:sonar.host.url=https://sonarcloud.io -d:sonar.login=$env:sonarqube -d:sonar.coverageReportPaths=coverage\SonarQube.xml $prArgs -v:$env:Version -d:sonar.import_unknown_files=true

	dotnet test -c Release --settings coverletArgs.runsettings

	$merge = ""
	Get-ChildItem -rec `
	| Where-Object { $_.Name -like "coverage.cobertura.xml" } `
	| ForEach-Object { 
		$path = $_.FullName
		$merge = "$merge;$path"
	}
	Write-Host $merge
	ReportGenerator\tools\netcoreapp3.0\ReportGenerator.exe "-reports:$merge" "-targetdir:coverage" "-reporttypes:SonarQube"
	
	dotnet sonarscanner end -d:sonar.login=$env:sonarqube
}
exit $result

  