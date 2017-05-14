@cd %~dp0

@"..\_Input\VersionUpdater.exe" "." "nr" "file" "..\RI.Framework.Property.Version.txt" "..\RI.Framework.Property.Company.txt" "..\RI.Framework.Property.Copyright.txt"

@xcopy "..\RI.Documentation.Framework.Unity\Content\Introduction.aml" ".\Content\Introduction.aml" /E /Y
@xcopy "..\RI.Documentation.Framework.Unity\Content\Compatibility.aml" ".\Content\Compatibility.aml" /E /Y
@xcopy "..\RI.Documentation.Framework.Unity\Content\Usage.aml" ".\Content\Usage.aml" /E /Y
@xcopy "..\RI.Documentation.Framework.Unity\Content\ContactSupport.aml" ".\Content\ContactSupport.aml" /E /Y
@xcopy "..\RI.Documentation.Framework.Unity\Content\VersionHistory.aml" ".\Content\VersionHistory.aml" /E /Y

@xcopy "..\RI.Documentation.Framework.Unity\Content\OverviewTutorials\xxx.aml" ".\Content\OverviewTutorials\xxx.aml" /E /Y
@xcopy "..\RI.Documentation.Framework.Unity\Content\OverviewTutorials\xxx.aml" ".\Content\OverviewTutorials\xxx.aml" /E /Y

@xcopy "..\RI.Documentation.Framework.Unity\Media\BootstrapperDragDrop.png" ".\Media\BootstrapperDragDrop.png" /E /Y
@xcopy "..\RI.Documentation.Framework.Unity\Media\BootstrapperObject.png" ".\Media\BootstrapperObject.png" /E /Y
@xcopy "..\RI.Documentation.Framework.Unity\Media\BootstrapperOptions.png" ".\Media\BootstrapperOptions.png" /E /Y