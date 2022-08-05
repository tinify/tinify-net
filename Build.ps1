<#
.SYNOPSIS
  This is a helper function that runs a scriptblock and checks the PS variable $lastexitcode
  to see if an error occcured. If an error is detected then an exception is thrown.
  This function allows you to run command-line programs without having to
  explicitly check the $lastexitcode variable.
#>
function Exec
{
    [CmdletBinding()]
    param(
        [Parameter(Position=0,Mandatory=1)][scriptblock]$cmd,
        [Parameter(Position=1,Mandatory=0)][string]$errorMessage = ($msgs.error_bad_command -f $cmd)
    )
    & $cmd
    if ($lastexitcode -ne 0) {
        throw ("Exec: " + $errorMessage)
    }
}

if (Test-Path .\src\Tinify\artifacts) { Remove-Item .\src\Tinify\artifacts -Force -Recurse }

exec { & dotnet restore }

msbuild

exec { & dotnet test .\test\Tinify.Tests -c Release }

# Only run integration tests if have a TINIFY_KEY environment key defined.
# If we don't, we're on a Pull Request most likely

if (Test-Path env:TINIFY_KEY)
{
    exec { & dotnet test .\test\Tinify.Tests.Integration -c Release }
}

exec { & dotnet pack .\src\Tinify -c Release -o .\src\Tinify\artifacts }
