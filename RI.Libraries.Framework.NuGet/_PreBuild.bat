@cd %~dp0

@for /R "..\packages" %%i in (*.nupkg) do @(
  @xcopy "%%i" ".\*.*" /I /R /Y
)
