$IMAGE_DIR="allin1v7"

# US East
$destHost="checkboxveasthdst1.blob.core.windows.net"
$deskKey="sV4Y0fuJEKPdlN3T1unfEsol98eEzHjz+rKCYid3Jv6nVOkZ2kX4Y41sTnwtOJ68PJxIvVEgPX1RHtCoNJkvWA=="

# West Europe
$destHost="checkboxveuwsthdst1.blob.core.windows.net"
$destKey="D1whR7FfcjNUE+BlgbnNNSwWtr+XcCwYD2+RcRuNnT1TqhJZcPjNBUhW/RmgknOyX1A3GvyORS3+EALejWjGww=="

$AZCOPY="C:\Program Files (x86)\Microsoft SDKs\Azure\AzCopy\AzCopy.exe"
& $AZCOPY /Y /Source:https://checkboxvhdst1.blob.core.windows.net/system/Microsoft.Compute/Images/$IMAGE_DIR /Dest:https://${destHost}/system/Microsoft.Compute/Images/$IMAGE_DIR /S /SourceKey:"ldG2nWkXGnhH6uMvaG/Fzwm4P7Gd+jBUYGWGYtPTVG+GHAStmFcNpTaEatl0CjLsqJHs6ztNOUTW67ifDFdP7Q==" /DestKey:"${destKey}"  /SyncCopy
