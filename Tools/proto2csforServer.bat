
@echo off


set "RELATIVE_PROTO_FILES_PATH=..\PtotoFiles"
set "RELATIVE_OUTPUT_CS_FILES_PATH=..\GameServer\Scripts\proto"
set "RELATIVE_PROTOC_PATH=.\protoc-22.1-win64\bin\protoc.exe"

set "PROTO_FILES_PATH="
set "OUTPUT_CS_FILES_PATH="
set "PROTOC_PATH="

for %%i in ("%RELATIVE_PROTO_FILES_PATH%") do set "PROTO_FILES_PATH=%%~fi"

for %%i in ("%RELATIVE_OUTPUT_CS_FILES_PATH%") do set "OUTPUT_CS_FILES_PATH=%%~fi"

for %%i in ("%RELATIVE_PROTOC_PATH%") do set "PROTOC_PATH=%%~fi"

if not exist "%OUTPUT_CS_FILES_PATH%" (
    mkdir "%OUTPUT_CS_FILES_PATH%"
) else (
    del /Q /F "%OUTPUT_CS_FILES_PATH%\*.*"
)

setlocal enabledelayedexpansion
pushd "%PROTO_FILES_PATH%"
for %%f in (*.proto) do (
    set "CS_FILE_NAME=%%~nf.cs"
    "%PROTOC_PATH%" --proto_path="." --csharp_out="%OUTPUT_CS_FILES_PATH%" "%%f"
    if errorlevel 1 (
        echo [ERROR] Failed to generate .cs file for %%f: %OUTPUT_CS_FILES_PATH%\!CS_FILE_NAME!
    ) else (
        echo [SUCCESS] Generated  !CS_FILE_NAME! file for %%f: %OUTPUT_CS_FILES_PATH%\!CS_FILE_NAME!
    )
)
popd
endlocal

pause


