@cd %~dp0

@"..\_Input\VersionUpdater.exe" "." "nr" "file" "..\RI.Framework.Property.Version.txt" "..\RI.Framework.Property.Company.txt" "..\RI.Framework.Property.Copyright.txt"

@xcopy "..\RI.Documentation.Framework.Unity.Heavy\Content\Introduction.aml" ".\Content\Introduction.aml" /E /Y
@xcopy "..\RI.Documentation.Framework.Unity.Heavy\Content\Compatibility.aml" ".\Content\Compatibility.aml" /E /Y
@xcopy "..\RI.Documentation.Framework.Unity.Heavy\Content\Usage.aml" ".\Content\Usage.aml" /E /Y
@xcopy "..\RI.Documentation.Framework.Unity.Heavy\Content\ContactSupport.aml" ".\Content\ContactSupport.aml" /E /Y
@xcopy "..\RI.Documentation.Framework.Unity.Heavy\Content\VersionHistory.aml" ".\Content\VersionHistory.aml" /E /Y

@xcopy "..\RI.Documentation.Framework.Unity.Heavy\Content\OverviewTutorials\OverviewTutorials.aml" ".\Content\OverviewTutorials\OverviewTutorials.aml" /E /Y
@xcopy "..\RI.Documentation.Framework.Unity.Heavy\Content\OverviewTutorials\Bootstrapper.aml" ".\Content\OverviewTutorials\Bootstrapper.aml" /E /Y
@xcopy "..\RI.Documentation.Framework.Unity.Heavy\Content\OverviewTutorials\CompositionContainer.aml" ".\Content\OverviewTutorials\CompositionContainer.aml" /E /Y
@xcopy "..\RI.Documentation.Framework.Unity.Heavy\Content\OverviewTutorials\ServiceLocator.aml" ".\Content\OverviewTutorials\ServiceLocator.aml" /E /Y
@xcopy "..\RI.Documentation.Framework.Unity.Heavy\Content\OverviewTutorials\ModuleService.aml" ".\Content\OverviewTutorials\ModuleService.aml" /E /Y
@xcopy "..\RI.Documentation.Framework.Unity.Heavy\Content\OverviewTutorials\DispatcherServiceMessages.aml" ".\Content\OverviewTutorials\DispatcherServiceMessages.aml" /E /Y
@xcopy "..\RI.Documentation.Framework.Unity.Heavy\Content\OverviewTutorials\DispatcherServiceJobs.aml" ".\Content\OverviewTutorials\DispatcherServiceJobs.aml" /E /Y
@xcopy "..\RI.Documentation.Framework.Unity.Heavy\Content\OverviewTutorials\ExampleMods.aml" ".\Content\OverviewTutorials\ExampleMods.aml" /E /Y

@xcopy "..\RI.Documentation.Framework.Unity.Heavy\Media\BootstrapperDragDrop.png" ".\Media\BootstrapperDragDrop.png" /E /Y
@xcopy "..\RI.Documentation.Framework.Unity.Heavy\Media\BootstrapperObject.png" ".\Media\BootstrapperObject.png" /E /Y
@xcopy "..\RI.Documentation.Framework.Unity.Heavy\Media\BootstrapperOptions.png" ".\Media\BootstrapperOptions.png" /E /Y