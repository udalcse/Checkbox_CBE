# Deploy the package locally
& "C:\Program Files\IIS\Microsoft Web Deploy V3\msdeploy.exe"  -EnableRule:DoNotDeleteRule -verb:sync -source:package="bin\Debug\appDeployment\CheckboxDb.appDeploy.package.zip" -dest:archivedir="c:\CheckboxDBArchive" -verbose -setParamFile:SetParameters.Local.xml  -declareParamFile:"parameters.xml"
& "C:\Program Files\IIS\Microsoft Web Deploy V3\msdeploy.exe"  -EnableRule:DoNotDeleteRule -verb:sync -source:archivedir="c:\CheckboxDBArchive" -dest:dirpath="c:\CheckboxDBInstaller" -verbose -setParamFile:SetParameters.Local.xml

# Execute the deployment
cd c:\CheckboxDBInstaller 
& .\DatabaseDeploy.ps1

Remove-Item c:\CheckboxDBArchive -Force -Recurse
# Remove-Item c:\CheckboxDBInstaller -Force -Recurse
