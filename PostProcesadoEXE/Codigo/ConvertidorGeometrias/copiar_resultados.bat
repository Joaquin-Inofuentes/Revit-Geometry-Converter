@echo off

REM =========================
REM 1. VERIFICAR REVIT
REM =========================
tasklist | find /I "Revit.exe" >nul
IF %ERRORLEVEL% NEQ 0 (
    echo Revit no esta ejecutandose. Iniciando...
    IF EXIST "C:\Program Files\Autodesk\Revit 2021\Revit.exe" (
        start "" "C:\Program Files\Autodesk\Revit 2021\Revit.exe" /language ESP
    ) ELSE (
        echo ERROR: No se encontro Revit 2021 en "C:\Program Files\Autodesk\Revit 2021\Revit.exe"
        pause
        exit /B 1
    )
) else (
    echo Revit ya esta ejecutandose.
)

REM =========================
REM 2. VERIFICAR CONTROLADOR ANULADO PARA SESION 0
REM =========================


REM =========================
REM 3. COPIAR SIEMPRE
REM =========================
IF exist "C:\ProgramData\Autodesk\Revit\Addins\2021\MIP\resultados.txt" (
    copy "C:\ProgramData\Autodesk\Revit\Addins\2021\MIP\resultados.txt" "C:\ProgramData\Autodesk\Revit\Addins\2021\MIP\pedidos.txt" /Y
    echo Copia realizada.
) else (
    echo ERROR: resultados.txt no existe.
)

echo Proceso terminado.
exit
