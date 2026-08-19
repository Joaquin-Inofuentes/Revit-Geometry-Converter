@echo off
setlocal EnableExtensions

set "MIP=C:\ProgramData\Autodesk\Revit\Addins\2021\MIP"

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
REM 2. NO PISAR UNA COLA QUE YA TIENE TRABAJO
REM =========================
REM Antes este bat copiaba resultados.txt -> pedidos.txt SIEMPRE, sin mirar si ya habia un
REM lote en curso. El addin toma pedidos.txt y lo ACUMULA en tomados.txt, asi que cada
REM corrida del bat volvia a encolar los MISMOS modelos encima de los que ya estaban
REM pendientes. Medido el 2026-08-18: tres corridas del bat dejaron tomados.txt con 36
REM lineas para 12 modelos reales, o sea cada modelo exportandose 3 veces -- horas de
REM trabajo para producir exactamente los mismos archivos.
REM
REM Se agrava con el relanzamiento automatico tras un SubmitPrint() colgado (ver
REM ImageProcessing.AbortarPorSubmitPrintColgado): ahi Revit se reabre SOLO y retoma
REM tomados.txt, asi que si ademas alguien corre el bat, la cola se vuelve a duplicar.
REM
REM Regla: si tomados.txt tiene contenido, hay un lote vivo y no se toca nada. El addin lo
REM termina solo. Recien con la cola vacia se vuelve a encolar.
REM
REM Se mira el TAMANO del archivo y no la cantidad de lineas: un "for /f" que cuente lineas
REM adentro de un bloque IF necesita expansion retardada para poder leer el resultado, y ese
REM es un clasico de bat que falla en silencio (la variable sale vacia y el IF nunca entra).
REM Con %%~zI el dato esta disponible en el acto y no hay nada que expandir despues.
set "HAYLOTE="
if exist "%MIP%\tomados.txt" for %%I in ("%MIP%\tomados.txt") do if %%~zI GTR 0 set "HAYLOTE=1"

if defined HAYLOTE (
    echo Ya hay un lote en curso en tomados.txt: no se vuelve a encolar para no duplicarlo.
    echo Para forzar una cola nueva: borrar tomados.txt y volver a correr este bat.
    goto :FIN
)

REM =========================
REM 3. COPIAR (solo si no habia lote activo)
REM =========================
IF EXIST "%MIP%\resultados.txt" (
    copy "%MIP%\resultados.txt" "%MIP%\pedidos.txt" /Y >nul
    echo Copia realizada: resultados.txt -^> pedidos.txt
) else (
    echo ERROR: resultados.txt no existe.
)

:FIN
echo Proceso terminado.
endlocal
exit /B 0
