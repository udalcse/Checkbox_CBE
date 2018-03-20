echo "Web site deployed!"
cp c:\license\* c:\inetpub\wwwroot\bin

# TODO Clean this up
echo "Cleaning Up"
Remove-Item c:\inetpub\wwwroot\Install -Force -Recurse

