$AppPath = Join-Path -Path  $PSScriptRoot -ChildPath 'app'
if(Test-Path $AppPath) {
    Write-Output "Cleaning $AppPath"
    Remove-Item $AppPath -Recurse
}

dotnet publish -c Release -o $AppPath
docker build . -t "issue-client-cert:latest"