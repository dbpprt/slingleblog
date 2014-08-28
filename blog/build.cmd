@echo off
cd %~dp0

FOR /f "tokens=*" %%G IN ('dir /b packages\Gulp.js*') DO CALL packages\%%G\tools\gulp.cmd %*