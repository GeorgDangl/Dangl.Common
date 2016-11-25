# Get the most recent OpenCover NuGet package from the dotnet nuget packages
$nugetOpenCoverPackage = Join-Path -Path $env:USERPROFILE -ChildPath "\.nuget\packages\OpenCover"
$latestOpenCover = Join-Path -Path ((Get-ChildItem -Path $nugetOpenCoverPackage | Sort-Object Fullname -Descending)[0].FullName) -ChildPath "tools\OpenCover.Console.exe"
# Get the most recent ReportGenerator NuGet package from the dotnet nuget packages
$nugetReportGeneratorPackage = Join-Path -Path $env:USERPROFILE -ChildPath "\.nuget\packages\ReportGenerator"
$latestReportGenerator = Join-Path -Path ((Get-ChildItem -Path $nugetReportGeneratorPackage | Sort-Object Fullname -Descending)[0].FullName) -ChildPath "tools\ReportGenerator.exe"
# Arguments for running dotnet
$dotnetArguments = "test", "`"`"$PSScriptRoot`"`"", "-xml `"`"$PSScriptRoot\xUnit.testresults`"`""
"Running tests with OpenCover"
& $latestOpenCover `
-register:user `
-target:dotnet.exe `
"-targetargs:$dotnetArguments" `
-returntargetcode `
-output:"$PSScriptRoot\OpenCover.coverageresults" `
-excludebyattribute:System.CodeDom.Compiler.GeneratedCodeAttribute `
"-filter:+[Dangl*]* -[*.Tests]* -[*.Tests.*]*"
"Creating Code Coverage Html report"& $latestReportGenerator `"-reports:$PSScriptRoot\OpenCover.coverageresults" `"-targetdir:$PSScriptRoot\CoverageReport" `"-historydir:$PSScriptRoot\CoverageHistories"