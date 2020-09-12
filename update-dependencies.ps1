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

Write-Host "::set-env name=UPDATE::true"