::dotnet tool install --global dotnet-ef
::dotnet tool update --global dotnet-ef

:loop
set /p id="Migration className: "
cd ./CodeNotion.Template.Migrator
dotnet-ef migrations add %id% --context MigratingContext
PAUSE
goto :loop