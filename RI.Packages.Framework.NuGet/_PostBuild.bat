@cd %~dp0

@call "..\_Temp\versioning.bat"

@set build_version=%build_version_major%.%build_version_minor%.%build_version_fix%.%build_version_revision%

"..\_Tools\nuget.exe" pack "RI.Framework.Common.nuspec" -OutputDirectory "..\_Packages" -Properties build_version=%build_version%
"..\_Tools\nuget.exe" pack "RI.Framework.Net.Common.nuspec" -OutputDirectory "..\_Packages" -Properties build_version=%build_version%
"..\_Tools\nuget.exe" pack "RI.Framework.Net.Core.nuspec" -OutputDirectory "..\_Packages" -Properties build_version=%build_version%
"..\_Tools\nuget.exe" pack "RI.Framework.Net.Fx.nuspec" -OutputDirectory "..\_Packages" -Properties build_version=%build_version%
"..\_Tools\nuget.exe" pack "RI.Framework.Unity.nuspec" -OutputDirectory "..\_Packages" -Properties build_version=%build_version%
"..\_Tools\nuget.exe" pack "RI.Framework.Linux.Common.nuspec" -OutputDirectory "..\_Packages" -Properties build_version=%build_version%
"..\_Tools\nuget.exe" pack "RI.Framework.Windows.Common.nuspec" -OutputDirectory "..\_Packages" -Properties build_version=%build_version%
"..\_Tools\nuget.exe" pack "RI.Framework.Windows.Wpf.nuspec" -OutputDirectory "..\_Packages" -Properties build_version=%build_version%
"..\_Tools\nuget.exe" pack "RI.Framework.Extensions.DotNetZip.nuspec" -OutputDirectory "..\_Packages" -Properties build_version=%build_version%
"..\_Tools\nuget.exe" pack "RI.Framework.Extensions.EF6.nuspec" -OutputDirectory "..\_Packages" -Properties build_version=%build_version%
"..\_Tools\nuget.exe" pack "RI.Framework.Extensions.FluentRibbon.nuspec" -OutputDirectory "..\_Packages" -Properties build_version=%build_version%
"..\_Tools\nuget.exe" pack "RI.Framework.Extensions.Json.nuspec" -OutputDirectory "..\_Packages" -Properties build_version=%build_version%
"..\_Tools\nuget.exe" pack "RI.Framework.Extensions.SQLite.nuspec" -OutputDirectory "..\_Packages" -Properties build_version=%build_version%
"..\_Tools\nuget.exe" pack "RI.Framework.Extensions.SQLiteEF6.nuspec" -OutputDirectory "..\_Packages" -Properties build_version=%build_version%
"..\_Tools\nuget.exe" pack "RI.Framework.Extensions.SqlServer.nuspec" -OutputDirectory "..\_Packages" -Properties build_version=%build_version%
"..\_Tools\nuget.exe" pack "RI.Framework.Extensions.SqlServerEF6.nuspec" -OutputDirectory "..\_Packages" -Properties build_version=%build_version%
