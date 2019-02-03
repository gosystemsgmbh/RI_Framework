@cd %~dp0

@for %%i in (
_Output,
_Output\x64,
_Output\x86,
_Packages,
_Temp,
_Test,
_Tools
) do @(
  @if not exist "..\%%i" @(
    @echo Creating directory: "..\%%i"
    @md "..\%%i"
  )
)

@rd /S /Q "..\_Temp"
@md "..\_Temp"

@copy "..\LICENSE.txt" "..\_Output\LICENSE.txt"
