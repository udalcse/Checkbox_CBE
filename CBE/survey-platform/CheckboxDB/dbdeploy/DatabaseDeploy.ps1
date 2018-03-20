# Upgrade the database
# TODO ADD BACKUP!
$ErrorActionPreference = "Stop"
$deploypath = $pwd.Path
$hostandinstance = '@@database_hostname@@'
if ( '@@database_instancename@@')
{
	$hostandinstance = '@@database_hostname@@\@@database_instancename@@'
}
Try
{
	Import-Module "sqlps"
}
catch
{
	Write-Error "You must have SQL Server 2016 installed to run this script"
	exit
}
try
{
	$conn = New-Object system.Data.SqlClient.SqlConnection
	$conn.connectionstring ="Data Source=$($hostandinstance);Initial Catalog=@@database_name@@;User ID=@@database_admin_username@@;Password=@@database_admin_password@@"
	$conn.open()
    $conn.close();
}
catch
{
	cd $deploypath
    Write-Output "Trying to create DB"
	sqlcmd -b -i ".\UpgradeScripts\Rebuild\0001 CreateUserAndPerms.sql" -S $hostandinstance -d master -U @@database_admin_username@@ -P @@database_admin_password@@
}
Write-Output "Starting Update"
cd $deploypath

.\DatabaseDeploy.exe @@databasedeploy_args@@ -o ".\UpgradeScripts\DeployScripts" -c "Data Source=$($hostandinstance);Initial Catalog=@@database_name@@;Persist Security Info=True;User ID=@@database_username@@;Password=@@database_password@@"
If ( Test-Path ".\DbDeploySampleOutput.sql" ) {
	sqlcmd -b -i ".\DbDeploySampleOutput.sql"  -S $hostandinstance -d @@database_name@@ -U @@database_username@@ -P @@database_password@@
} Else {
	Write-Output "No Changes to Apply"
}

