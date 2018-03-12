@cd %~dp0

@echo set build_version_major=X> "..\_Temp\versioning.bat"
@echo set build_version_minor=X>> "..\_Temp\versioning.bat"
@echo set build_version_fix=X>> "..\_Temp\versioning.bat"
@echo set build_version_revision=X>> "..\_Temp\versioning.bat"
@"..\_Tools\VersionUpdater.exe" "..\_Temp" "r" "file" "..\SolutionProperty.Product.txt" "..\SolutionProperty.Version.txt" "..\SolutionProperty.Company.txt" "..\SolutionProperty.Copyright.txt" "..\SolutionProperty.Trademark.txt"
@call "..\_Temp\versioning.bat"

@set build_version=%build_version_major%.%build_version_minor%.%build_version_fix%.%build_version_revision%
@set package_file=duf-netfx-%build_version%.zip

@for %%i in (
RI.Framework.NetFx,
RI.Framework.CrossPlatform.Common,
RI.Framework.Linux.Common,
RI.Framework.Windows.Common,
RI.Framework.Windows.Service,
RI.Framework.Windows.Forms,
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
"..\_Tools\7za.exe" a -r "..\_Temp\%package_file%" "..\_Output\Roten Informatik Framework License 1.0.txt"

@copy "..\_Temp\%package_file%" "..\_Packages\%package_file%"
