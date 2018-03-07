@cd %~dp0

@echo set build_version_major=X> "..\_Temp\versioning.bat"
@echo set build_version_minor=X>> "..\_Temp\versioning.bat"
@echo set build_version_fix=X>> "..\_Temp\versioning.bat"
@echo set build_version_revision=X>> "..\_Temp\versioning.bat"
@"..\_Tools\VersionUpdater.exe" "..\_Temp" "r" "file" "..\SolutionProperty.Product.txt" "..\SolutionProperty.Version.txt" "..\SolutionProperty.Company.txt" "..\SolutionProperty.Copyright.txt" "..\SolutionProperty.Trademark.txt"
@call "..\_Temp\versioning.bat"

@set build_version=%build_version_major%.%build_version_minor%.%build_version_fix%.%build_version_revision%
@set package_file=duf-unity-%build_version%.zip

@for %%i in (
RI.Framework.Unity
) do @(
  "..\_Tools\7za.exe" a -r "..\_Temp\%package_file%" "..\_Output\%%i.*"
)

"..\_Tools\7za.exe" a -r "..\_Temp\%package_file%" "..\_Output\RI.Documentation.Framework.Unity.Light.docx"
"..\_Tools\7za.exe" a -r "..\_Temp\%package_file%" "..\_Output\RI.Documentation.Framework.Unity.Light.pdf"
"..\_Tools\7za.exe" a -r "..\_Temp\%package_file%" "..\_Output\RI.Documentation.Framework.Unity.Full.chm"
"..\_Tools\7za.exe" a -r "..\_Temp\%package_file%" "..\_Output\RI.Documentation.Framework.Unity.Full.zip"
"..\_Tools\7za.exe" a -r "..\_Temp\%package_file%" "..\_Output\Roten Informatik Framework License.txt"

@copy "..\_Temp\%package_file%" "..\_Packages\%package_file%"
