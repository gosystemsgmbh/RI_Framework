@cd %~dp0

@"..\_Input\VersionUpdater.exe" "." "nr" "file" "..\RI.Framework.Property.Version.txt" "..\RI.Framework.Property.Company.txt" "..\RI.Framework.Property.Copyright.txt"

@xcopy "..\RI.Documentation.Framework.Unity\ContentLayout.content" ".\ContentLayout.content" /E /Y
@xcopy "..\RI.Documentation.Framework.Unity\Content\*.*" ".\Content\*.*" /E /Y
@xcopy "..\RI.Documentation.Framework.Unity\Media\*.*" ".\Media\*.*" /E /Y