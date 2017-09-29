$testProjects = "Dangl.Common.Tests"

# Get the most recent OpenCover NuGet package from the dotnet nuget packages
$nugetOpenCoverPackage = Join-Path -Path $env:USERPROFILE -ChildPath "\.nuget\packages\OpenCover"
$latestOpenCover = Join-Path -Path ((Get-ChildItem -Path $nugetOpenCoverPackage | Sort-Object Fullname -Descending)[0].FullName) -ChildPath "tools\OpenCover.Console.exe"
# Get the most recent OpenCoverToCoberturaConverter from the dotnet nuget packages
$nugetCoberturaConverterPackage = Join-Path -Path $env:USERPROFILE -ChildPath "\.nuget\packages\OpenCoverToCoberturaConverter"
$latestCoberturaConverter = Join-Path -Path (Get-ChildItem -Path $nugetCoberturaConverterPackage | Sort-Object Fullname -Descending)[0].FullName -ChildPath "tools\OpenCoverToCoberturaConverter.exe"

If (Test-Path "$PSScriptRoot\OpenCover.coverageresults"){
	Remove-Item "$PSScriptRoot\OpenCover.coverageresults"
}

If (Test-Path "$PSScriptRoot\Cobertura.coverageresults"){
	Remove-Item "$PSScriptRoot\Cobertura.coverageresults"
}

& dotnet restore

$testRuns = 1;
foreach ($testProject in $testProjects){
    # Arguments for running dotnet
    $dotnetArguments = "xunit", "-xml `"`"$PSScriptRoot\testRuns_$testRuns.testresults`"`""

    "Running tests with OpenCover"
    & $latestOpenCover `
        -register:user `
        -target:dotnet.exe `
        -targetdir:$PSScriptRoot\test\$testProject `
        "-targetargs:$dotnetArguments" `
        -returntargetcode `
        -output:"$PSScriptRoot\OpenCover.coverageresults" `
        -mergeoutput `
        -oldStyle `
        -excludebyattribute:System.CodeDom.Compiler.GeneratedCodeAttribute `
        "-filter:+[Dangl*]* -[*.Tests]* -[*.Tests.*]*"

        $testRuns++
}

"Prepending framework to test method name for better CI visualization"
$resultsGlobPattern = "testRuns_*.testresults"
$prependFrameworkScript = ".\AppendxUnitFramework.ps1"
& $prependFrameworkScript $resultsGlobPattern "$PSScriptRoot\"

"Converting coverage reports to Cobertura format"
& $latestCoberturaConverter `
    -input:"$PSScriptRoot\OpenCover.coverageresults" `
    -output:"$PSScriptRoot\Cobertura.coverageresults" `
    "-sources:$PSScriptRoot"
