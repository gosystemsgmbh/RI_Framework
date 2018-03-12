@cd %~dp0

@for %%i in (
_Temp,
_Tools,
_Output,
_Output\x86,
_Output\x64,
_Packages
) do @(
  @if not exist "..\%%i" @(
    @echo Creating directory: "..\%%i"
    @md "..\%%i"
  )
)

@rd /S /Q "..\_Temp"
@md "..\_Temp"

@copy "..\LICENSE.txt" "..\_Output\Roten Informatik Framework License 1.0.txt"
