# Setup the params file
Echo "<parameters>" > c:\checkboxdb\t
Dir env: | Where-Object { $_.Name.Contains("deploy_") } | %{"<setParameter 
    name=`'{0}'` value='`{1}'`/>" -f $_.Name,$_.Value} | foreach-object {$_ -replace "deploy_",""} >> c:\checkboxdb\t
Echo "</parameters>" >> c:\checkboxdb\t

Remove-Item C:\CheckboxDBInstall\UpgradeScripts\* -Force -Recurse

# Deploy the package locally
& "C:\Program Files\IIS\Microsoft Web Deploy V3\msdeploy.exe"  -EnableRule:DoNotDeleteRule -verb:sync -source:dirpath="c:\checkboxdb\Content\C_C\projects\survey-platform-d4h6k\CheckboxDB\bin\Debug\appDeploy-FilesToPackage" -dest:archivedir="c:\CheckboxDBArchive" -verbose -setParamFile:c:\checkboxdb\t  -declareParamFile:"c:\checkboxdb\parameters.xml"
& "C:\Program Files\IIS\Microsoft Web Deploy V3\msdeploy.exe"  -EnableRule:DoNotDeleteRule -verb:sync -source:archivedir="c:\CheckboxDBArchive" -dest:dirpath="c:\CheckboxDBInstall" -verbose -setParamFile:"c:\checkboxdb\t" 
# Execute the deployment
cd c:\CheckboxDBInstall\
& .\DatabaseDeploy.ps1
CD c:\

Remove-Item  C:\CheckboxDB -Force -Recurse
Remove-Item c:\CheckboxDBArchive -Force -Recurse
# Remove-Item c:\CheckboxDBInstall -Force -Recurse
