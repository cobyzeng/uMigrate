@echo off
call nuget pack uMigrate\uMigrate.csproj -Build -Properties Configuration=Release
call nuget pack uMigrate.UI\uMigrate.UI.csproj -Build -Properties Configuration=Release