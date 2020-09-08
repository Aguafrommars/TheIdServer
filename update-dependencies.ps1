function UpdatePackages {
    param (
        $project
    )
    $packageLineList = dotnet list $project package --outdated
    $packageList = @()
    foreach($line in $packageLineList)
    {
       $match = $line -match '>\s(\S*)\s*\S*\s*\S*\s*(\S*)'
       if ($match)
       {
          dotnet add $project package $Matches.1 --version $Matches.2
       }
    }
}

$projectList = dotnet sln list
foreach($path in $projectList)
{
    if ($path -ne "Project(s)" -and $path -ne "----------")
    {
        UpdatePackages -project $path
    }
}

dotnet build -c Release