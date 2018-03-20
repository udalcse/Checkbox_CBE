# To recreate/resave the credentials:

$profile = Login-AzureRmAccount
Save-AzureRMProfile  -Profile $profile -path azureprofile.json
mv ~\azureprofile.json .