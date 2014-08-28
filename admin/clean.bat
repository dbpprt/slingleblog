@echo off
echo Executing really ugly and hacky clean script to delete long file paths (more than 260 characters)
call:hackyDelete ".\node_modules"
call:hackyDelete ".\packages"
call:hackyDelete ".\bower_components"
call:hackyDelete ".\.nuget"
call:hackyDelete ".\build"

echo Cleaning should be successfull
goto:eof

:hackyDelete
setlocal
set folder=%~1
set MT="%TEMP%\DelFolder_%RANDOM%"
MD %MT%
RoboCopy %MT% %folder% /MIR > nul
RD /S /Q %MT%
RD /S /Q %folder%
endlocal
goto:eof