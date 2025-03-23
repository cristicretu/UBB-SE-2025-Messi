$sqlInstance = "np:\\.\pipe\LOCALDB#4B20C342\tsql\query"
$database = "Duo"

$sqlFiles = Get-ChildItem -Path ".\Queries" -Filter "*.sql" -Recurse

Write-Host "Found $($sqlFiles.Count) SQL files to execute"

foreach ($file in $sqlFiles) {
    Write-Host "Executing $($file.FullName)"
    $command = "sqlcmd -S $sqlInstance -d $database -i `"$($file.FullName)`" -E"
    
    try {
        Invoke-Expression $command
        Write-Host "Successfully executed $($file.Name)" -ForegroundColor Green
    } catch {
        Write-Host "Error executing $($file.Name): $_" -ForegroundColor Red
    }
}

Write-Host "All procedures executed successfully!"
