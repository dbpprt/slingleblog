@echo off
cd %~dp0

SETLOCAL
SET CACHED_NUGET=%LocalAppData%\NuGet\NuGet.exe

IF EXIST %CACHED_NUGET% goto copynuget
echo Downloading latest version of NuGet.exe...
IF NOT EXIST %LocalAppData%\NuGet md %LocalAppData%\NuGet
@powershell -NoProfile -ExecutionPolicy unrestricted -Command "$ProgressPreference = 'SilentlyContinue'; Invoke-WebRequest 'https://www.nuget.org/nuget.exe' -OutFile '%CACHED_NUGET%'"

:copynuget
IF EXIST .nuget\nuget.exe goto restore
md .nuget
copy %CACHED_NUGET% .nuget\nuget.exe > nul

:restore
echo Restoring nuget packages
IF NOT EXIST packages\Npm.js* .nuget\NuGet.exe install npm.js -o packages -nocache
IF NOT EXIST packages\Bower.js* .nuget\NuGet.exe install bower.js -o packages -nocache
IF NOT EXIST packages\Gulp.js* .nuget\NuGet.exe install gulp.js -o packages -nocache

:run
echo Restoring node_modules
FOR /f "tokens=*" %%G IN ('dir /b packages\Npm.js*') DO CALL packages\%%G\tools\npm.cmd install
echo Restoring bower packages
FOR /f "tokens=*" %%G IN ('dir /b packages\Bower.js*') DO CALL .\packages\%%G\tools\bower.cmd install
echo Executing gulpfile.js
FOR /f "tokens=*" %%G IN ('dir /b packages\Gulp.js*') DO CALL packages\%%G\tools\gulp.cmd %*