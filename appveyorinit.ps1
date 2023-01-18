npm i -g semantic-release@^19.0.5 @semantic-release/exec @semantic-release/changelog @semantic-release/git @semantic-release/release-notes-generator @semantic-release/commit-analyzer 
if (test-path ./nextversion.txt)
{
    Remove-Item ./nextversion.txt
}
semantic-release -b $env:APPVEYOR_REPO_BRANCH -d
if (test-path ./nextversion.txt)
{
    $nextversion = Get-Content ./nextversion.txt
}
else 
{
    $nextversion = $env:GitVersion_MajorMinorPatch
}
$nextversion = $nextversion.Trim()

appveyor SetVariable -Name SemVer -Value $nextversion
appveyor AddMessage "SemVer = $nextversion"

if (![string]::IsNullOrEmpty($env:GitVersion_PreReleaseLabel))
{
    $preReleaseLabel = $env:GitVersion_PreReleaseLabel.Trim()
    if (![string]::IsNullOrEmpty($preReleaseLabel))
    {
        $nextversion = "$nextversion-$preReleaseLabel$env:GitVersion_PreReleaseNumber-$env:GitVersion_CommitsSinceVersionSourcePadded"
    }
}
$nextversion = $nextversion.Trim()
appveyor SetVariable -Name Version -Value $nextversion
appveyor UpdateBuild -Version $nextversion
appveyor AddMessage "Version = $nextversion"

dotnet restore
