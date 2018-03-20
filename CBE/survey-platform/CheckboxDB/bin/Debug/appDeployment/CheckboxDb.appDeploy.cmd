@ECHO OFF
IF "%1"=="" GOTO Readme
:Deploy
ECHO Adding variables to pre/post scripts
ECHO set configuration=Debug> preSync.changed.bat
ECHO set server=%1>> preSync.changed.bat
ECHO set appName=CheckboxDb>> preSync.changed.bat
ECHO set destinationFilePath=c$\Program Files\CheckboxDB>> preSync.changed.bat
type preSync.bat>> preSync.changed.bat
ECHO set configuration=Debug> postSync.changed.bat
ECHO set server=%1>> postSync.changed.bat
ECHO set appName=CheckboxDb>> postSync.changed.bat
ECHO set destinationFilePath=c$\Program Files\CheckboxDB>> postSync.changed.bat
type postSync.bat>> postSync.changed.bat
ECHO Starting deployment of CheckboxDb.appDeploy.cmd ...
@ECHO ON
"C:\Program Files\IIS\Microsoft Web Deploy V3\msdeploy.exe" -verb:sync -source:package="%CD%\CheckboxDb.appDeploy.package.zip" -dest:dirpath="\\%1\c$\Program Files\CheckboxDB",computername=%1,username=,password= -preSync:runCommand="preSync.changed.bat",waitInterval=1000 -postSync:runCommand="postSync.changed.bat",waitInterval=1000 -verbose %~2 %~3 %~4 %~5 %~6 %~7 %~8 %~9
@ECHO OFF
GOTO End
:Readme
ECHO.
ECHO Deploys application to target server using MSDeploy
ECHO.
ECHO CheckboxDb.appDeploy.cmd COMPUTERNAME [Other MSBuild parameters (up to 8)]
ECHO.
ECHO   COMPUTERNAME\t\tThe target computer to deploy to.
ECHO   MSBuild params\t\tYou may optionally add any other MSBuild parameters to the call.  For example:
ECHO     -whatif
ECHO     -retryAttempts
ECHO     -verbose
ECHO.
ECHO You can send the command output to a log file by appending " > msdeploy.log" to the command.
ECHO.
ECHO CheckboxDb.appDeploy.cmd COMPUTERNAME > msdeploy.log
ECHO notepad msdeploy.log
ECHO.
GOTO End
:End
