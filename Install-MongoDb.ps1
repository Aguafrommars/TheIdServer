
#Make sure 7za is installed
choco install 7zip.commandline

# Create mongodb and data directory
md $env:temp\mongo\data

# Go to mongodb dir
Push-Location $env:temp\mongo

# Download zipped mongodb binaries to mongodbdir
Invoke-WebRequest https://fastdl.mongodb.org/windows/mongodb-windows-x86_64-8.0.13.zip -OutFile mongodb.zip

# Extract mongodb zip
cmd /c 7za e mongodb.zip

# Install mongodb as a windows service
.\mongod.exe --logpath=$env:temp\mongo\log --dbpath=$env:temp\mongo\data\ --port 27018 --install

# Sleep as a hack to fix an issue where the service sometimes does not finish installing quickly enough
Start-Sleep -Seconds 5

# Start mongodb service
net start MongoDB

# Return to last location, to run the build
Pop-Location

Write-Host
Write-Host "monogdb installation complete"

$currentLocation = Get-Location

# Create mongosh and data directory
md $env:temp\mongosh

Push-Location $env:temp\mongosh

# Download zipped mongosh binaries to mongodbdir
Invoke-WebRequest https://downloads.mongodb.com/compass/mongosh-2.5.8-win32-x64.zip -OutFile mongosh.zip

# Extract mongodb zip
cmd /c 7za e mongosh.zip

$v = .\mongosh.exe mongodb://localhost:27018 -f $currentLocation\mangoversion.js
Write-Host $v

# Return to last location, to run the build
Pop-Location


