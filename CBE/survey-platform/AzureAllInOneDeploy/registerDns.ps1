
$hostname = $Args[0].split(".")[0]
$domain = $Args[0].Substring($Args[0].IndexOf(".") + 1)

$fqdn = $hostname + "." + "$domain"
echo "Mapping DNS for $fqdn"
pause

$headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
$headers.Add("Authorization","sso-key dL3rCCvfZY5s_3xPMgQLV8uw29Uxxkh5pWG:7VNTMBrNA91yp52qN5zPM7")
$headers.Add("Content-type","application/json")
    

$body = " [{ `"type`": `"CNAME`",`"name`": `"$hostname`",`"data`": `"$hostname.$domain`"}]"
     
echo $body
Invoke-RestMethod -Body $body -Headers $headers -Method Patch -Uri https://api.godaddy.com/v1/domains/$domain/records