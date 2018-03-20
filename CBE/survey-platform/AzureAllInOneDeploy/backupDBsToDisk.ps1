﻿

$serverName="localhost\SQLEXPRESS";
$backupDirectory="f:\backups"
$daysToStoreBackups=7

[System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SMO") | Out-Null
[System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SmoExtended") | Out-Null
[System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.ConnectionInfo") | Out-Null
[System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SmoEnum") | Out-Null

$server = New-Object ("Microsoft.SqlServer.Management.Smo.Server") $serverName
#This sets the connection to mixed-mode authentication 
$server.ConnectionContext.LoginSecure=$false; 
$server.ConnectionContext.set_Login("checkbox"); 
$server.ConnectionContext.set_Password("checkbox"); 
$dbs = $server.Databases
foreach ($database in $dbs | where { $_.IsSystemObject -eq $False })  
{
    $dbName = $database.Name

    $timestamp = Get-Date -format yyyy-MM-dd-HHmmss
    $targetPath = $backupDirectory + "\" + $dbName + "_" + $timestamp + ".bak"

    $smoBackup = New-Object ("Microsoft.SqlServer.Management.Smo.Backup")
    $smoBackup.Action = "Database"
    $smoBackup.BackupSetDescription = "Full Backup of " + $dbName
    $smoBackup.BackupSetName = $dbName + " Backup"
    $smoBackup.Database = $dbName
    $smoBackup.MediaDescription = "Disk"
    $smoBackup.Devices.AddDevice($targetPath, "File")
    $smoBackup.SqlBackup($server)

    "backed up $dbName ($serverName) to $targetPath"
}

Get-ChildItem "$backupDirectory\*.bak" |? { $_.lastwritetime -le (Get-Date).AddDays(-$daysToStoreBackups)} |% {Remove-Item $_ -force }  
"removed all previous backups older than $daysToStoreBackups days"