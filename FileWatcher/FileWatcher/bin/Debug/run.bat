@echo off
robocopy /s /e C:\dev\temp_filewatch\%1 C:\dev\temp_log\%1
echo robocopy /s /e %1 C:\dev\temp_log\%1 >>run.log
exit /b %errorlevel%

REM ROBOCOPY ERRORLEVEL
REM 0 No error, No copy
REM 1 >1 copied
REM 2 same(no copied)
REM 3 additional files copied
