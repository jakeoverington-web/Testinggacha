# Compiles Core + Tests (C# 9, same as Unity) with the Roslyn compiler bundled in PowerShell 7,
# then runs the tests in memory. Usage: tools/csharp/test.sh [name-filter]
param([string]$Filter = "", [string]$Main = "")
$ErrorActionPreference = "Stop"
$repo = (Resolve-Path "$PSScriptRoot/../..").Path
$pwshDir = Split-Path ([System.Diagnostics.Process]::GetCurrentProcess().MainModule.FileName)
Add-Type -Path "$pwshDir/Microsoft.CodeAnalysis.dll"
Add-Type -Path "$pwshDir/Microsoft.CodeAnalysis.CSharp.dll"

$files = @()
$files += Get-ChildItem "$repo/Assets/_Game/Scripts/Core" -Recurse -Filter *.cs
$files += Get-ChildItem "$repo/Assets/_Game/Tests" -Recurse -Filter *.cs
$files += Get-ChildItem "$PSScriptRoot" -Filter *.cs
$opts = [Microsoft.CodeAnalysis.CSharp.CSharpParseOptions]::Default.WithLanguageVersion([Microsoft.CodeAnalysis.CSharp.LanguageVersion]::CSharp9)
$trees = foreach ($f in $files) {
  [Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree]::ParseText([System.IO.File]::ReadAllText($f.FullName), $opts, $f.FullName)
}
$refs = Get-ChildItem $pwshDir -Filter "System.*.dll" | Where-Object { $_.Name -notmatch "Management.Automation|Native" } | ForEach-Object {
  try { [Microsoft.CodeAnalysis.MetadataReference]::CreateFromFile($_.FullName) } catch {} }
$refs += [Microsoft.CodeAnalysis.MetadataReference]::CreateFromFile("$pwshDir/netstandard.dll")
$copts = New-Object Microsoft.CodeAnalysis.CSharp.CSharpCompilationOptions([Microsoft.CodeAnalysis.OutputKind]::DynamicallyLinkedLibrary)
$copts = $copts.WithOptimizationLevel([Microsoft.CodeAnalysis.OptimizationLevel]::Release).WithNullableContextOptions([Microsoft.CodeAnalysis.NullableContextOptions]::Disable)
$comp = [Microsoft.CodeAnalysis.CSharp.CSharpCompilation]::Create("GachaTests", [Microsoft.CodeAnalysis.SyntaxTree[]]$trees, [Microsoft.CodeAnalysis.MetadataReference[]]$refs, $copts)
$ms = New-Object System.IO.MemoryStream
$res = $comp.Emit($ms)
$diags = $res.Diagnostics | Where-Object { $_.Severity -eq "Error" -or ($_.Severity -eq "Warning" -and $_.Id -notin @("CS1701","CS1702","CS8019")) }
foreach ($d in $diags) { Write-Host ($d.ToString().Replace("$repo/", "")) }
if (-not $res.Success) { Write-Host "BUILD FAILED"; exit 2 }
$asm = [System.Reflection.Assembly]::Load($ms.ToArray())
Set-Location $repo
if ($Main) { $t = $asm.GetType($Main); exit ([int]$t.GetMethod("Main").Invoke($null, @(,[string[]]$args))) }
$code = $asm.GetType("LocalTestRunner").GetMethod("Run").Invoke($null, @($Filter))
exit $code
