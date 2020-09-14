# Update one project packages
function UpdatePackages {
    param (
        $project
    )

    $return = $false
    
    # Get outdated packages
    $packageLineList = dotnet list $project package --outdated
    
    foreach($line in $packageLineList) {
       Write-Host $line
       
       $match = $line -match '>\s(\S*)\s*\S*\s*\S*\s*(\S*)'
       if (!$match) {
          # the line doesn't contain a package information, continue
          continue
       }
       
       # update an outdated package
       $added = dotnet add $project package $Matches.1 --version $Matches.2

       if ($LASTEXITCODE -ne 0) {
           # error while updating the package
           Write-Error "dotnet add $project package $Matches.1 --version $Matches.2 exit with code $LASTEXITCODE"
           Write-Host $added
           break
       }

       $return = $true
    }

    return $return
}

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

# commit changes
Write-Host "git config user.name github-actions"
git config user.name github-actions
Write-Host "git config user.email github-actions@github.com"
git config user.email github-actions@github.com
Write-Host "git add ."
git add .
Write-Host "git commit -m ""fix: update packages"""
git commit -m "fix: update packages"
Write-Host "git push"

try {
git push
} catch {

}

# Create a pull request
$authorization = "Bearer $env:GITHUB_TOKEN"
$createPrUrl = 'https://api.github.com/repos/$env:GITHUB_REPOSITORY/pulls'
$headers = @{
    Authorization = $authorization
    Accept = "application/vnd.github.v3+json"
}
$payload = "{ ""title"": ""update packages"", ""head"": ""fix/dependencies"", ""base"": ""master"" }"
Write-Host "Invoke-WebRequest -Uri $createPrUrl -Body $payload"
Invoke-WebRequest -Uri $createPrUrl -Headers $headers -Method "POST" -Body $payload
