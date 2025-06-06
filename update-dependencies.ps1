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
    $packageLineList = dotnet list package --outdated
    
    foreach ($line in $packageLineList) {
        Write-Host $line
       
        $match = $line -match '>\s(\S*)\s*\S*\s*\S*\s*(\S*)'
        if (!$match) {
            # the line doesn't contain a package information, continue
            continue
        }
       
        Write-Host $match
       
        $packageName = $Matches.1
        $version = $Matches.2
        if ($version -eq "Not") {
            # latest version not found
            $preReleasePackageLineList = dotnet list package --outdated --include-prerelease

            foreach ($preReleaseLine in $preReleasePackageLineList) {
                Write-Host $preReleaseLine
                $preReleaseMatch = $preReleaseLine -match '>\s(\S*)\s*\S*\s*\S*\s*(\S*)'
                if (!$preReleaseMatch) {
                    # the line doesn't contain a package information, continue
                    continue
                }

                if ($packageName -ne $Matches.1) {
                    continue
                }

                $latestVersion = $Matches.2
                if ($latestVersion -match '-rc') {
                    $version = $latestVersion
                    Write-Host "Release candidate version found for $packageName : $version"
                }
                break    
            }
        }

        if ($version -eq "Not") {
            continue
        }

        # update an outdated package
        $added = dotnet add package $packageName --version $version
        if ($LASTEXITCODE -ne 0) {
            # error while updating the package

            Write-Error "dotnet add package $packageName --version $version exit with code $LASTEXITCODE"
            Write-Host $added
            break
        }

        Write-Host 'package' $Matches.1 'version' $Matches.2 'updated'
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

$exit = 0;
foreach ($path in $projectList) {
    if ($path -eq "Project(s)" -or $path -eq "----------") {
        # The line doesn't contain a path, continue
        continue
    }

    # Update project dependencies
    $projectUpdated = UpdatePackages -project $path
        
    if ($LASTEXITCODE -ne 0) {
        #The update fail, exit
        $exit = $LASTEXITCODE
    }

    $updated = $updated -or $projectUpdated
}

if ($exit -ne 0) {
    exit $exit
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
}
catch {

}

# Create a pull request
$authorization = "Bearer $env:GITHUB_TOKEN"
$createPrUrl = "https://api.github.com/repos/$env:GITHUB_REPOSITORY/pulls"
$headers = @{
    Authorization = $authorization
    Accept        = "application/vnd.github.v3+json"
}
$payload = "{ ""title"": ""update packages"", ""head"": ""$src"", ""base"": ""$dest"" }"
Write-Host "Invoke-WebRequest -Uri $createPrUrl -Body $payload"
Invoke-WebRequest -Uri $createPrUrl -Headers $headers -Method "POST" -Body $payload
