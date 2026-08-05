@echo off

REM =========================
REM 1. VERIFICAR REVIT
REM =========================
REM OJO con el filtro: NO usar "tasklist | find /I "Revit.exe"".
REM "Revit.exe" es SUBCADENA de "OptimizarDumpDeRevit.exe" (el watcher de geometria,
REM que corre de forma perpetua). Con el find suelto, ese proceso hacia match SIEMPRE,
REM el bat creia que Revit ya estaba abierto y NUNCA lo lanzaba -- pero igual copiaba
REM pedidos.txt, con lo que el sintoma era "cambia pedidos pero Revit no arranca".
REM /FI "IMAGENAME eq Revit.exe" compara el nombre de imagen COMPLETO, no por subcadena.
tasklist /FI "IMAGENAME eq Revit.exe" /NH | find /I "Revit.exe" >nul
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
