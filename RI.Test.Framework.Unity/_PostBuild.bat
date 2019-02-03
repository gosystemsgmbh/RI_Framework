@cd %~dp0

@copy ".\bin\%1\netstandard2.0\RI.Test.Framework.Unity.*" "..\_Output\*.*"
@copy ".\Composition\Catalogs\Mock_Export.cs" "..\_Output\*.*"