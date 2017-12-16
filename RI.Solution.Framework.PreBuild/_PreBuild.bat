@cd %~dp0

@for %%i in (
_Tools,
_Output,
_Output\x86,
_Output\x64
) do @(
  @if not exist "..\%%i" @(
    @echo Creating directory: "..\%%i"
    @md "..\%%i"
  )
)

@copy "..\LICENSE.txt" "..\_Output\Roten Informatik Framework License.txt"
