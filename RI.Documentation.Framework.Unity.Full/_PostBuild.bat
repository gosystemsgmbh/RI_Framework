@cd %~dp0

@copy ".\Help\RI.Documentation.Framework.Unity.Full.chm" "..\_Output\RI.Documentation.Framework.Unity.Full.chm"

@cd Help
@del ".\_doc.zip" /F
@"..\..\_Output\7za.exe" a -r ".\_doc.zip" ".\*.*"
@"..\..\_Output\7za.exe" d ".\_doc.zip" "*.config"
@"..\..\_Output\7za.exe" d ".\_doc.zip" "*.aspx"
@"..\..\_Output\7za.exe" d ".\_doc.zip" "*.php"
@"..\..\_Output\7za.exe" d ".\_doc.zip" "*.chm"
@"..\..\_Output\7za.exe" d ".\_doc.zip" "*.log"
@cd..
@copy ".\Help\_doc.zip" "..\_Output\RI.Documentation.Framework.Unity.Full.zip"
