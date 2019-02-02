@cd %~dp0

"..\_Tools\nuget.exe" pack "RI.Framework.Common.nuspec" -OutputDirectory "..\_Packages"
"..\_Tools\nuget.exe" pack "RI.Framework.NetCore.nuspec" -OutputDirectory "..\_Packages"
"..\_Tools\nuget.exe" pack "RI.Framework.NetFx.nuspec" -OutputDirectory "..\_Packages"
"..\_Tools\nuget.exe" pack "RI.Framework.Unity.nuspec" -OutputDirectory "..\_Packages"
