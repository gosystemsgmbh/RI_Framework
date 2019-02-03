@cd %~dp0

@xcopy ".\bin\%1\RI.Tools.Framework.VersionUpdater.exe" "..\_Tools\VersionUpdater.exe"

@echo set build_version_major=X> "..\_Temp\versioning.bat"
@echo set build_version_minor=X>> "..\_Temp\versioning.bat"
@echo set build_version_fix=X>> "..\_Temp\versioning.bat"
@echo set build_version_revision=X>> "..\_Temp\versioning.bat"
@"..\_Tools\VersionUpdater.exe" "..\_Temp" "r" "file" "..\SolutionProperty.Product.txt" "..\SolutionProperty.Version.txt" "..\SolutionProperty.Company.txt" "..\SolutionProperty.Copyright.txt" "..\SolutionProperty.Trademark.txt"
