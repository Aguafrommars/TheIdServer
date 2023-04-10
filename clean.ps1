Get-ChildItem -rec `
| Where-Object { $_.Name.StartsWith("Aguacongas - Backup") } `
| ForEach-Object {
	Remove-Item  $_.FullName
}

Get-ChildItem -rec `
| Where-Object { $_.Name -eq "obj" } `
| ForEach-Object {
	Remove-Item  $_.FullName -Recurse
}

Get-ChildItem -rec `
| Where-Object { $_.Name -eq "bin" } `
| ForEach-Object {
	Remove-Item  $_.FullName -Recurse
}
