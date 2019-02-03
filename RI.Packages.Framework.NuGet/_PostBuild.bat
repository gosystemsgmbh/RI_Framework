@cd %~dp0

@call "..\_Temp\versioning.bat"

@set build_version=%build_version_major%.%build_version_minor%.%build_version_fix%.%build_version_revision%

"..\_Tools\nuget.exe" pack "RI.Framework.Common.nuspec" -OutputDirectory "..\_Packages" -Properties build_version=%build_version%
"..\_Tools\nuget.exe" pack "RI.Framework.NetCore.nuspec" -OutputDirectory "..\_Packages"
"..\_Tools\nuget.exe" pack "RI.Framework.NetFx.nuspec" -OutputDirectory "..\_Packages"
"..\_Tools\nuget.exe" pack "RI.Framework.Unity.nuspec" -OutputDirectory "..\_Packages"
