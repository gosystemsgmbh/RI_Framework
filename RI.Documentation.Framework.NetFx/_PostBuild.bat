@cd %~dp0

@copy ".\Help\RI.Documentation.Framework.NetFx.chm" "..\_Output\RI.Documentation.Framework.NetFx.chm"

@cd Help
@del ".\_doc.zip" /F
@"..\..\_Tools\7za.exe" a -r ".\_doc.zip" ".\*.*"
@"..\..\_Tools\7za.exe" d ".\_doc.zip" "*.config"
@"..\..\_Tools\7za.exe" d ".\_doc.zip" "*.aspx"
@"..\..\_Tools\7za.exe" d ".\_doc.zip" "*.php"
@"..\..\_Tools\7za.exe" d ".\_doc.zip" "*.chm"
@"..\..\_Tools\7za.exe" d ".\_doc.zip" "*.log"
@cd..
@copy ".\Help\_doc.zip" "..\_Output\RI.Documentation.Framework.NetFx.zip"
