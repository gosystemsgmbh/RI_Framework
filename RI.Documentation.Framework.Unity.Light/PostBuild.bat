@cd %~dp0

@md "..\_Output"

@copy ".\Help\RI.Documentation.Framework.Unity.Light.docx" "..\_Output\RI.Documentation.Framework.Unity.Light.docx"
@copy ".\Help\RI.Documentation.Framework.Unity.Light.docx" "..\_Output\DecouplingUtilities.docx"

@cscript "..\_Input\ConvDoc2Pdf.vbs" ".\Help\RI.Documentation.Framework.Unity.Light.docx"

@copy ".\Help\RI.Documentation.Framework.Unity.Light.pdf" "..\_Output\RI.Documentation.Framework.Unity.Light.pdf"
@copy ".\Help\RI.Documentation.Framework.Unity.Light.pdf" "..\_Output\DecouplingUtilities.pdf"
