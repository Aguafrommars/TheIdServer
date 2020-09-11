function UpdatePackages {
    param (
        $project
    )

    $return = $false
    $packageLineList = dotnet list $project package --outdated
    foreach($line in $packageLineList) {
       Write-Host $line
       
       $match = $line -match '>\s(\S*)\s*\S*\s*\S*\s*(\S*)'
       if (!$match) {
          continue
       }
       
       $added = dotnet add $project package $Matches.1 --version $Matches.2

       if ($LASTEXITCODE -ne 0) {
           Write-Error "dotnet add $project package $Matches.1 --version $Matches.2 exit with code $LASTEXITCODE"
           Write-Host $added
           break
       }

       $return = $true
    }

    return $return
}


dotnet restore
$projectList = dotnet sln list
$updated = $false

foreach($path in $projectList) {
    if ($path -eq "Project(s)" -or $path -eq "----------") {
        continue
    }

    $projectUpdated = UpdatePackages -project $path
        
    if ($LASTEXITCODE -ne 0) {
        exit $LASTEXITCODE
    }

    $updated = $updated -or $projectUpdated
}

if (!$updated) {
    Write-Host "nothing to update"
    exit 0
}

$branchName = "fix/dependencies-$env:GITHUB_RUN_ID"

if (Test-Path env:GITHUB_ACTOR) {
    Write-Host "git config --local user.email ""aguacongas@gmail.com"""
    git config --local user.email "aguacongas@gmail.com"
    Write-Host "git config --local user.name $env:GITHUB_ACTOR"
    git config --local user.name $env:GITHUB_ACTOR
    Write-Host "git remote set-url origin $env:GITHUB_SERVER_URL/$env:GITHUB_REPOSITORY"
    git remote set-url origin $env:GITHUB_SERVER_URL/$env:GITHUB_REPOSITORY
}

Write-Host "git add ."
git add .
Write-Host "git commit -m ""fix: update packages"""
git commit -m "fix: update packages"
Write-Host "git checkout -b $branchName"
git checkout -b $branchName
Write-Host "git push -u origin $branchName"
git push -u origin $branchName

$authorization = "Bearer $env:GITHUB_TOKEN"
$createPrUrl = 'https://api.github.com/repos/Aguafrommars/TheIdServer/pulls'
$headers = @{
    Authorization = $authorization
    Accept = "application/vnd.github.v3+json"
}
$payload = "{ ""title"": ""update packages"", ""head"": ""$branchName"", ""base"": ""master"" }"
Write-Host "Invoke-WebRequest -Uri $createPrUrl -Body $payload"
Invoke-WebRequest -Uri $createPrUrl -Headers $headers -Method "POST" -Body $payload
