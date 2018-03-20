$AppveyorEnvKey = $Args[0] 
$domain = $Args[1] # Both the host and domain for the public-facing site IE dev.cbesurveys.com
$rg = $Args[2] # The resource group Name
$globalStorageHost = $Args[3]
$recvault = $Args[4]

$iissitename = "CheckboxWeb"
$certStore="MY"

# Format the data disk
Get-Disk | ` 
Where partitionstyle -eq 'raw' | ` 
Initialize-Disk -PartitionStyle MBR -PassThru | ` 
New-Partition -DriveLetter F -UseMaximumSize | ` 
Format-Volume -FileSystem NTFS -NewFileSystemLabel "datadisk" -Confirm:$false
f:
mkdir f:\sqlserver
mkdir f:\sqlserver\data
mkdir f:\sqlserver\logs
mkdir f:\sqlserver\backups
mkdir f:\inetpub
mkdir F:\inetpub\wwwroot
mkdir f:\inetpub\logs
mkdir F:\inetpub\wwwroot\CheckboxWeb
mkdir f:\backups

# Install appveyor
Invoke-WebRequest "https://${globalStorageHost}/public/AppveyorDeploymentAgent.msi" -OutFile "f:\AppveyorDeploymentAgent.msi"
msiexec /i f:\AppveyorDeploymentAgent.msi /quiet /qn /norestart /log f:\install.log ENVIRONMENT_ACCESS_KEY=$AppveyorEnvKey

#
# Set SSL cert
#
if ( $domain -like "*boardevaluations.com*") { 
    echo "Setting BoardEvaluations generic cert"
    
    # Get the cert thumb
    $Thumbprint = (Get-ChildItem -Path Cert:\LocalMachine\My | Where-Object {$_.Subject -like "CN=*.boardevaluations.com"}).Thumbprint; 
}
elseif ( $domain -like "*cbesurveys.com*") { 
    echo "Setting CBESurveys generic cert"
    
    # Get the cert thumb
    $Thumbprint = (Get-ChildItem -Path Cert:\LocalMachine\My | Where-Object {$_.Subject -like "CN=*.cbesurveys.com*"}).Thumbprint;
}
elseif ( $domain -like "*corpgov.tech*") { 
    echo "Setting CorpGov generic cert"
    
    # Get the cert thumb
    $Thumbprint = (Get-ChildItem -Path Cert:\LocalMachine\My | Where-Object {$_.Subject -like "CN=*.corpgov.tech*"}).Thumbprint;
}

# Assign it
echo "Assigning SSL certificate"
$obj = Get-WebBinding $iissitename -Port 443 
$method = $obj.Methods["AddSslCertificate"]
$methodInstance = $method.CreateInstance()
$methodInstance.Input.SetAttributeValue("certificateHash", $Thumbprint)
$methodInstance.Input.SetAttributeValue("certificateStoreName", $certStore)
$methodInstance.Execute()

#
# End Set SSL Cert
#


# Download the backup script
Invoke-WebRequest "https://${globalStorageHost}/public/backupDBsToDisk.ps1" -OutFile "f:\backupDBsToDisk.ps1"

# Schedule Backup
$Action = New-ScheduledTaskAction -Execute 'C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe' -Argument "-File f:\backupDBsToDisk.ps1"
$Trigger = New-ScheduledTaskTrigger -Daily -At '4:00AM'
$P = "checkbox_admin"
$S = New-ScheduledTaskSettingsSet
$Task = New-ScheduledTask -Action $Action -Trigger $Trigger -Settings $S  
Register-ScheduledTask SqlBackup -InputObject $Task -User $P 

#
# Schedule VM backups
#
Import-Module AzureRM
Import-Module Azure

$azureAccountName ="a81c2e4f-7b07-461a-bdbb-65a53a5e8ba8"
$azurePassword = ConvertTo-SecureString -String "TlMxXJDRXVxS5K23LzRx" -AsPlainText -Force
$psCred = New-Object System.Management.Automation.PSCredential($azureAccountName, $azurePassword)

echo "Adding account"
Add-AzureRmAccount -Credential $psCred -TenantId 6eb8aed8-3a80-435b-b838-af0140dddd0c -ServicePrincipal

Get-AzureRmLog -StartTime (Get-Date).AddMinutes(-10)

Register-AzureRmResourceProvider -ProviderNamespace "Microsoft.RecoveryServices"

Get-AzureRmRecoveryServicesVault -Name "$recvault" | Set-AzureRmRecoveryServicesVaultContext

$pol=Get-AzureRmRecoveryServicesBackupProtectionPolicy -Name "DailyPolicy"
write-host $pol
$hostname=hostname
write-host "Setting up backups for $rg / $hostname"

Enable-AzureRmRecoveryServicesBackupProtection -Policy $pol -Name $hostname -ResourceGroupName $rg