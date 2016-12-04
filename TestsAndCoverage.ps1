$testProjects = "Dangl.Common.Tests"
$testFrameworks = "net461", "netcoreapp1.1"

# Get the most recent OpenCover NuGet package from the dotnet nuget packages
$nugetOpenCoverPackage = Join-Path -Path $env:USERPROFILE -ChildPath "\.nuget\packages\OpenCover"
$latestOpenCover = Join-Path -Path ((Get-ChildItem -Path $nugetOpenCoverPackage | Sort-Object Fullname -Descending)[0].FullName) -ChildPath "tools\OpenCover.Console.exe"
# Get the most recent OpenCoverToCoberturaConverter from the dotnet nuget packages
$nugetCoberturaConverterPackage = Join-Path -Path $env:USERPROFILE -ChildPath "\.nuget\packages\OpenCoverToCoberturaConverter"
$latestCoberturaConverter = Join-Path -Path (Get-ChildItem -Path $nugetCoberturaConverterPackage | Sort-Object Fullname -Descending)[0].FullName -ChildPath "tools\OpenCoverToCoberturaConverter.exe"

$testRuns = 1;

foreach ($testProject in $testProjects){
    foreach ($testFramework in $testFrameworks) {
        # Arguments for running dotnet
        $testFramework
        $dotnetArguments = "test", "`"`"$PSScriptRoot\test\$testProject`"`"", "-f $testFramework", "-xml `"`"$PSScriptRoot\testRuns_$testRuns.testresults`"`""

        "Running tests with OpenCover"
        & $latestOpenCover `
            -register:user `
            -target:dotnet.exe `
            "-targetargs:$dotnetArguments" `
            -returntargetcode `
            -output:"$PSScriptRoot\OpenCover.coverageresults" `
            -mergeoutput `
            -excludebyattribute:System.CodeDom.Compiler.GeneratedCodeAttribute `
            "-filter:+[Dangl*]* -[*.Tests]* -[*.Tests.*]*"

            $testRuns++
    }
}

"Converting coverage reports to Cobertura format"
& $latestCoberturaConverter `
    -input:"$PSScriptRoot\OpenCover.coverageresults" `
    -output:"$PSScriptRoot\Cobertura.coverageresults" `
    "-sources:$PSScriptRoot"