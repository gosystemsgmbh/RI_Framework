@cd %~dp0

@call "..\_Temp\versioning.bat"

@set build_version=%build_version_major%.%build_version_minor%.%build_version_fix%.%build_version_revision%

REM ---------- Common ----------

@set package_file=duf-common-%build_version%.zip

@for %%i in (
RI.Framework.Common
) do @(
  "..\_Tools\7za.exe" a -r "..\_Temp\%package_file%" "..\_Output\%%i.*"
)

"..\_Tools\7za.exe" a -r "..\_Temp\%package_file%" "..\_Output\RI.Documentation.Framework.Common.chm"
"..\_Tools\7za.exe" a -r "..\_Temp\%package_file%" "..\_Output\RI.Documentation.Framework.Common.zip"
"..\_Tools\7za.exe" a -r "..\_Temp\%package_file%" "..\_Output\LICENSE.txt"

@copy "..\_Temp\%package_file%" "..\_Packages\%package_file%"

REM ---------- .NET Core ----------

@set package_file=duf-netcore-%build_version%.zip

@for %%i in (
RI.Framework.Common,
RI.Framework.Net.Common,
RI.Framework.Net.Core
) do @(
  "..\_Tools\7za.exe" a -r "..\_Temp\%package_file%" "..\_Output\%%i.*"
)

"..\_Tools\7za.exe" a -r "..\_Temp\%package_file%" "..\_Output\RI.Documentation.Framework.NetCore.chm"
"..\_Tools\7za.exe" a -r "..\_Temp\%package_file%" "..\_Output\RI.Documentation.Framework.NetCore.zip"
"..\_Tools\7za.exe" a -r "..\_Temp\%package_file%" "..\_Output\LICENSE.txt"

@copy "..\_Temp\%package_file%" "..\_Packages\%package_file%"

REM ---------- .NET Framework ----------

@set package_file=duf-netfx-%build_version%.zip

@for %%i in (
RI.Framework.Common,
RI.Framework.Net.Common,
RI.Framework.Net.Fx,
RI.Framework.CrossPlatform.Common,
RI.Framework.Linux.Common,
RI.Framework.Windows.Common,
RI.Framework.Windows.Forms,
RI.Framework.Windows.Service,
RI.Framework.Windows.Wpf,
RI.Framework.Extensions.BouncyCastle,
RI.Framework.Extensions.DotNetZip,
RI.Framework.Extensions.EF6,
RI.Framework.Extensions.EPPlus,
RI.Framework.Extensions.FluentRibbon,
RI.Framework.Extensions.Json,
RI.Framework.Extensions.Nancy,
RI.Framework.Extensions.NancyJson,
RI.Framework.Extensions.SQLite,
RI.Framework.Extensions.SQLiteEF6,
RI.Framework.Extensions.SqlServer,
RI.Framework.Extensions.SqlServerEF6,
RI.Framework.Extensions.WpfToolkit
) do @(
  "..\_Tools\7za.exe" a -r "..\_Temp\%package_file%" "..\_Output\%%i.*"
)

"..\_Tools\7za.exe" a -r "..\_Temp\%package_file%" "..\_Output\RI.Documentation.Framework.NetFx.chm"
"..\_Tools\7za.exe" a -r "..\_Temp\%package_file%" "..\_Output\RI.Documentation.Framework.NetFx.zip"
"..\_Tools\7za.exe" a -r "..\_Temp\%package_file%" "..\_Output\LICENSE.txt"

@copy "..\_Temp\%package_file%" "..\_Packages\%package_file%"

REM ---------- Unity ----------

@set package_file=duf-unity-%build_version%.zip

@for %%i in (
RI.Framework.Common,
RI.Framework.Unity
) do @(
  "..\_Tools\7za.exe" a -r "..\_Temp\%package_file%" "..\_Output\%%i.*"
)

"..\_Tools\7za.exe" a -r "..\_Temp\%package_file%" "..\_Output\RI.Documentation.Framework.Unity.Light.docx"
"..\_Tools\7za.exe" a -r "..\_Temp\%package_file%" "..\_Output\RI.Documentation.Framework.Unity.Light.pdf"
"..\_Tools\7za.exe" a -r "..\_Temp\%package_file%" "..\_Output\RI.Documentation.Framework.Unity.Full.chm"
"..\_Tools\7za.exe" a -r "..\_Temp\%package_file%" "..\_Output\RI.Documentation.Framework.Unity.Full.zip"
"..\_Tools\7za.exe" a -r "..\_Temp\%package_file%" "..\_Output\LICENSE.txt"

@copy "..\_Temp\%package_file%" "..\_Packages\%package_file%"
