@cd %~dp0

@copy ".\Help\RI.Documentation.Framework.Unity.Light.docx" "..\_Output\RI.Documentation.Framework.Unity.Light.docx"

@cscript "..\_Output\ConvDoc2Pdf.vbs" ".\Help\RI.Documentation.Framework.Unity.Light.docx"
@copy ".\Help\RI.Documentation.Framework.Unity.Light.pdf" "..\_Output\RI.Documentation.Framework.Unity.Light.pdf"
