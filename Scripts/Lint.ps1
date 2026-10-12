using module PSScriptAnalyzer
using module ./Cmdlets.psm1

"Performing the static analysis of source code..."
Invoke-FSharpLint FreeMobile.slnx -Configuration Configuration/FSharpLint.json
Invoke-ScriptAnalyzer $PSScriptRoot -Recurse
