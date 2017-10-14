@cd %~dp0

@for %%i in (
_Input,
_Output,
_Output\x86,
_Output\x64,
_Test
) do @(
  @if not exist "..\%%i" @(
    @echo Creating directory: "..\%%i"
    @md "..\%%i"
  )
)
