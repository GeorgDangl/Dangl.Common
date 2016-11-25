# Get the most recent OpenCover NuGet package from the dotnet nuget packages
$nugetOpenCoverPackage = Join-Path -Path $env:USERPROFILE -ChildPath "\.nuget\packages\OpenCover"
$latestOpenCover = Join-Path -Path ((Get-ChildItem -Path $nugetOpenCoverPackage | Sort-Object Fullname -Descending)[0].FullName) -ChildPath "tools\OpenCover.Console.exe"
# Get the most recent OpenCoverToCoberturaConverter from the dotnet nuget packages
$nugetCoberturaConverterPackage = Join-Path -Path $env:USERPROFILE -ChildPath "\.nuget\packages\OpenCoverToCoberturaConverter"
$latestCoberturaConverter = Join-Path -Path (Get-ChildItem -Path $nugetCoberturaConverterPackage | Sort-Object Fullname -Descending)[0].FullName -ChildPath "tools\OpenCoverToCoberturaConverter.exe"
# Arguments for running dotnet
$dotnetArguments = "test", "`"`"$PSScriptRoot\test\Dangl.Common.Tests`"`"", "-xml `"`"$PSScriptRoot\xUnit.testresults`"`""
"Running tests with OpenCover"
& $latestOpenCover `
-register:user `
-target:dotnet.exe `
"-targetargs:$dotnetArguments" `
-returntargetcode `
-output:"$PSScriptRoot\OpenCover.coverageresults" `
-excludebyattribute:System.CodeDom.Compiler.GeneratedCodeAttribute `
"-filter:+[Dangl*]* -[*.Tests]* -[*.Tests.*]*"
 "Converting coverage reports to Cobertura format"
& $latestCoberturaConverter `
-input:"$PSScriptRoot\OpenCover.coverageresults" `
-output:"$PSScriptRoot\Cobertura.coverageresults" `
"-sources:$PSScriptRoot"