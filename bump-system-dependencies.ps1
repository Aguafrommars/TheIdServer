# Update one project packages
function UpdatePackages {
    param (
        $project
    )

	$currentDirectoy = Get-Location
    $return = $false
    
	$dir = Split-Path $project
    Write-Host 'Set-Location' $dir
    
    Set-Location $dir
	
    # Get outdated packages
    $packageLineList = dotnet list package --outdated --include-prerelease
    
    foreach($line in $packageLineList) {
       Write-Host $line
       
       $match = $line -match '>\s(System.\S*)\s*\S*\s*\S*\s*(\S*)'
       if (!$match) {
          # the line doesn't contain a package information, continue
          continue
       }
       
       # update an outdated package
       $added = dotnet add package $Matches.1 --version $Matches.2
	   
	   Write-Host $Matches.1 'version' $Matches.2 $added
       
       if ($LASTEXITCODE -ne 0) {
           # error while updating the package
           Write-Error "dotnet add $project package $Matches.1 --version $Matches.2 exit with code $LASTEXITCODE"
           Write-Host $added
           break
       }

       $return = $true
    }

	Set-Location $currentDirectoy
    return $return
}

# get branches names
$dest = "master"
if (Test-Path env:DEST_BRANCH) {
    $dest = $env:DEST_BRANCH
}
$src = "fix/dependencies"
if (Test-Path env:SRC_BRANCH) {
    $src = $env:SRC_BRANCH
}

Write-Host "src:$src dest: $dest"

# Restore dependencies
dotnet restore

# Get all project list in the solution
$projectList = dotnet sln list
$updated = $false

foreach($path in $projectList) {
    if ($path -eq "Project(s)" -or $path -eq "----------") {
        # The line doesn't contain a path, continue
        continue
    }

    # Update project dependencies
    $projectUpdated = UpdatePackages -project $path
        
    if ($LASTEXITCODE -ne 0) {
        #The update fail, exit
        exit $LASTEXITCODE
    }

    $updated = $updated -or $projectUpdated
}

if (!$updated) {
    # No packages to update found, exit
    Write-Host "nothing to update"
    exit 0
}

# Try build the solution with new packages
Write-Host "dotnet build -c Release"
dotnet build -c Release
