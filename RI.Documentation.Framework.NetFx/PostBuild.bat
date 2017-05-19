@cd %~dp0

@md "..\_Output"

@copy ".\Help\RI.Documentation.Framework.NetFx.chm" "..\_Output\RI.Documentation.Framework.NetFx.chm"

@cd Help
@del ".\_doc.zip" /F
@"..\..\_Input\7za.exe" a -r ".\_doc.zip" ".\*.*"
@"..\..\_Input\7za.exe" d ".\_doc.zip" "*.config"
@"..\..\_Input\7za.exe" d ".\_doc.zip" "*.aspx"
@"..\..\_Input\7za.exe" d ".\_doc.zip" "*.php"
@"..\..\_Input\7za.exe" d ".\_doc.zip" "*.chm"
@"..\..\_Input\7za.exe" d ".\_doc.zip" "*.log"
@cd..

@copy ".\Help\_doc.zip" "..\_Output\RI.Documentation.Framework.NetFx.zip"