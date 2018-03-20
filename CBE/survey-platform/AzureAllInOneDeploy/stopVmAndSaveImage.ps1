# Stop and save a host to be a template for new azure spin-ups
$resourceGroup = $Args[0]
$vmName = $Args[1]
$destinationContainerName = $Args[2]

#Login-AzureRmAccount
Stop-AzureRmVM -ResourceGroupName $resourceGroup -Name $vmName
Set-AzureRmVm -ResourceGroupName $resourceGroup -Name $vmName -Generalized
$vm = Get-AzureRmVM -ResourceGroupName $resourceGroup -Name $vmName -Status
$vm.Statuses
Save-AzureRmVMImage -ResourceGroupName $resourceGroup -Name $vmName `
    -DestinationContainerName $destinationContainerName -VHDNamePrefix $destinationContainerName `
    -Path azureimage.json