using Fallout.Common;
using Fallout.Common.Git;
using Fallout.Common.IO;
using Fallout.Common.ProjectModel;
using Fallout.Common.Tooling;
using Fallout.Common.Tools.AzureKeyVault;
using Fallout.Common.Tools.Coverlet;
using Fallout.Common.Tools.DotCover;
using Fallout.Common.Tools.DotNet;
using Fallout.Common.Tools.GitVersion;
using Fallout.Common.Tools.ReportGenerator;
using Fallout.Common.Utilities;
using Fallout.Common.Utilities.Collections;
using Fallout.GitHub;
using Fallout.WebDocu;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using static Fallout.Common.ChangeLog.ChangelogTasks;
using static Fallout.Common.IO.XmlTasks;
using static Fallout.Common.Tools.DotNet.DotNetTasks;
using static Fallout.Common.Tools.ReportGenerator.ReportGeneratorTasks;
using static Fallout.GitHub.ChangeLogExtensions;
using static Fallout.GitHub.GitHubTasks;
using static Fallout.WebDocu.WebDocuTasks;

class Build : FalloutBuild
{
    public static int Main() => Execute<Build>(x => x.Compile);

    [AzureKeyVaultConfiguration(
        BaseUrlParameterName = nameof(KeyVaultBaseUrl),
        ClientIdParameterName = nameof(KeyVaultClientId),
        ClientSecretParameterName = nameof(KeyVaultClientSecret),
        TenantIdParameterName = nameof(KeyVaultTenantId))]
    readonly AzureKeyVaultConfiguration KeyVaultSettings;
    [AzureKeyVault] AzureKeyVault KeyVault;

    [Parameter] string KeyVaultBaseUrl;
    [Parameter] string KeyVaultClientId;
    [Parameter] string KeyVaultClientSecret;
    [Parameter] string KeyVaultTenantId;

    private string _configuration;

    [Parameter]
    string Configuration = IsLocalBuild ? "Debug" : "Release"; // Defaults to "Release" in CI server

    [GitVersion(Framework = "netcoreapp3.1")] readonly GitVersion GitVersion;
    [GitRepository] readonly GitRepository GitRepository;

    [Solution("Dangl.Common.sln")] readonly Solution Solution;
    AbsolutePath SolutionDirectory => Solution.Directory;
    AbsolutePath OutputDirectory => SolutionDirectory / "output";
    AbsolutePath SourceDirectory => SolutionDirectory / "src";

    [AzureKeyVaultSecret] string DocuBaseUrl;
    [AzureKeyVaultSecret] readonly string DanglPublicFeedSource;
    [AzureKeyVaultSecret] readonly string FeedzAccessToken;
    [AzureKeyVaultSecret] string NuGetApiKey;
    [AzureKeyVaultSecret("DanglCommon-DocuApiKey")] string DocuApiKey;
    [AzureKeyVaultSecret] string GitHubAuthenticationToken;
    [AzureKeyVaultSecret] string CodeSigningCertificateName;
    [AzureKeyVaultSecret] string CodeSigningCertificateKeyVaultBaseUrl;
    [AzureKeyVaultSecret] string CodeSigningKeyVaultTenantId;

    [NuGetPackage("AzureSignTool", "tools/net10.0/any/AzureSignTool.dll")]
    readonly Tool AzureSign;

    string DocFxFile => SolutionDirectory / "docfx.json";
    string ChangeLogFile => RootDirectory / "CHANGELOG.md";

    Target Clean => _ => _
            .Executes(() =>
            {
                SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(d => d.DeleteDirectory());
                (RootDirectory / "test").GlobDirectories("**/bin", "**/obj").ForEach(d => d.DeleteDirectory());
                OutputDirectory.CreateOrCleanDirectory();
            });

    Target Restore => _ => _
            .DependsOn(Clean)
            .Executes(() =>
            {
                DotNetRestore();
            });

    Target Compile => _ => _
            .DependsOn(Restore)
            .Executes(() =>
            {
                DotNetBuild(x => x
                    .SetConfiguration(Configuration)
                    .EnableNoRestore()
                    .SetFileVersion(GitVersion.AssemblySemFileVer)
                    .SetAssemblyVersion(GitVersion.AssemblySemVer)
                    .SetInformationalVersion(GitVersion.InformationalVersion));
            });

    Target SignDlls => _ => _
        .DependsOn(Compile)
        .OnlyWhenDynamic(() => IsServerBuild)
        .Executes(() =>
        {
            Assert.NotNull(CodeSigningCertificateKeyVaultBaseUrl);
            Assert.NotNull(KeyVaultClientId);
            Assert.NotNull(KeyVaultClientSecret);
            Assert.NotNull(CodeSigningKeyVaultTenantId);
            Assert.NotNull(CodeSigningCertificateName);

            var inputFiles = (SourceDirectory).GlobFiles("**/*Dangl.AVA.IO*.dll").ToList();
            var filesListPath = OutputDirectory / $"{Guid.NewGuid()}.txt";
            filesListPath.WriteAllText(inputFiles.Select(f => f.ToString()).Join(Environment.NewLine) + Environment.NewLine);
            var azureSignArguments = string.Empty;
            azureSignArguments += "sign";
            azureSignArguments += $" --azure-key-vault-url \"{CodeSigningCertificateKeyVaultBaseUrl}\"";
            azureSignArguments += $" --azure-key-vault-client-id \"{KeyVaultClientId}\"";
            azureSignArguments += $" --azure-key-vault-client-secret \"{KeyVaultClientSecret}\"";
            azureSignArguments += $" --azure-key-vault-tenant-id \"{CodeSigningKeyVaultTenantId}\"";
            azureSignArguments += $" --azure-key-vault-certificate \"{CodeSigningCertificateName}\"";
            azureSignArguments += $" --input-file-list \"{filesListPath}\"";
            azureSignArguments += $" --timestamp-rfc3161 \"{"http://timestamp.digicert.com"}\"";
            AzureSign($"{azureSignArguments:nq}");
        });

    Target Pack => _ => _
        .DependsOn(SignDlls)
        .Executes(() =>
        {
            var changeLog = GetCompleteChangeLog(ChangeLogFile)
                .EscapeStringPropertyForMsBuild();

            DotNetPack(x => x
                .SetConfiguration(Configuration)
                .SetPackageReleaseNotes(changeLog)
                .EnableNoBuild()
                .SetOutputDirectory(OutputDirectory)
                .SetVersion(GitVersion.NuGetVersion));
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            var testProjects = (SolutionDirectory / "test").GlobFiles("**/*.csproj");
            var testRun = 1;

            try
            {
                DotNetTest(x => x
                    .SetNoBuild(true)
                    .SetTestAdapterPath(".")
                    .CombineWith(cc => testProjects
                        .SelectMany(testProject => GetTestFrameworksForProjectFile(testProject)
                            .Select(targetFramework => cc
                                .SetFramework(targetFramework)
                                .SetProcessWorkingDirectory(Path.GetDirectoryName(testProject))
                                .SetLoggers($"xunit;LogFilePath={OutputDirectory / $"{testRun++}_testresults-{targetFramework}.xml"}")))),
                                degreeOfParallelism: Environment.ProcessorCount);
            }
            finally
            {
                PrependFrameworkToTestresults();
            }
        });

    Target LinuxTest => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            try
            {
                DotNetTest(x => x
                   .SetTestAdapterPath(".")
                   .SetFramework("net10.0")
                   .SetLoggers($"xunit;LogFilePath={OutputDirectory / $"testresults-linux.xml"}"));
            }
            finally
            {
                PrependFrameworkToTestresults();
            }
        });

    Target Coverage => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            var testProjects = (SolutionDirectory / "test").GlobFiles("**/*.csproj").ToList();

            var hasFailedTests = false;
            try
            {
                DotNetTest(c => c
                    .SetDataCollector("XPlat Code Coverage")
                    .SetResultsDirectory(OutputDirectory)
                    .AddRunSetting("DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format", "cobertura")
                    .AddRunSetting("DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Include", "[Dangl.Common]*")
                    .AddRunSetting("DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.ExcludeByAttribute", "Obsolete,GeneratedCodeAttribute,CompilerGeneratedAttribute")
                    .EnableNoBuild()
                    .SetTestAdapterPath(".")
                    .CombineWith(cc => testProjects
                        .SelectMany(testProject =>
                        {
                            var projectDirectory = Path.GetDirectoryName(testProject);
                            var projectName = Path.GetFileNameWithoutExtension(testProject);
                            var targetFrameworks = GetTestFrameworksForProjectFile(testProject);
                            return targetFrameworks.Select(targetFramework => cc
                                .SetProjectFile(testProject)
                                .SetFramework(targetFramework)
                                .SetLoggers($"xunit;LogFilePath={OutputDirectory / projectName}_testresults-{targetFramework}.xml"));
                        })),
                            degreeOfParallelism: Environment.ProcessorCount,
                            completeOnFailure: true);
            }
            catch
            {
                hasFailedTests = true;
            }

            PrependFrameworkToTestresults();

            // Merge coverage reports, otherwise they might not be completely
            // picked up by Jenkins
            ReportGenerator(c => c
                .SetFramework("net6.0")
                .SetReports(OutputDirectory / "**/*cobertura.xml")
                .SetTargetDirectory(OutputDirectory)
                .SetReportTypes(ReportTypes.Cobertura));

            MakeSourceEntriesRelativeInCoberturaFormat(OutputDirectory / "Cobertura.xml");

            if (hasFailedTests)
            {
                Assert.Fail("Some tests have failed");
            }
        });

    private void MakeSourceEntriesRelativeInCoberturaFormat(AbsolutePath coberturaReportPath)
    {
        var originalText = coberturaReportPath.ReadAllText();
        var xml = XDocument.Parse(originalText);

        var xDoc = XDocument.Load(coberturaReportPath);

        var sourcesEntry = xDoc
            .Root
            .Elements()
            .Where(e => e.Name.LocalName == "sources")
            .Single();
        var basePath = sourcesEntry.Value;

        var filenameAttributes = xDoc
            .Root
            .Descendants()
            .Where(d => d.Attributes().Any(a => a.Name.LocalName == "filename"))
            .Select(d => d.Attributes().First(a => a.Name.LocalName == "filename"));
        foreach (var filenameAttribute in filenameAttributes)
        {
            filenameAttribute.Value = filenameAttribute.Value.Substring(basePath.Length);
        }

        xDoc.Save(coberturaReportPath);
    }

    private IEnumerable<string> GetTestFrameworksForProjectFile(string projectFile)
    {
        var targetFrameworks = XmlPeek(projectFile, "//Project/PropertyGroup//TargetFrameworks")
            .Concat(XmlPeek(projectFile, "//Project/PropertyGroup//TargetFramework"))
            .Distinct()
            .SelectMany(f => f.Split(';'))
            .Distinct();

        return targetFrameworks;
    }

    Target Push => _ => _
        .DependsOn(Pack)
        .Requires(() => DanglPublicFeedSource)
        .Requires(() => FeedzAccessToken)
        .Requires(() => NuGetApiKey)
        .Requires(() => Configuration.EqualsOrdinalIgnoreCase("Release"))
        .Executes(() =>
        {
            var packages = OutputDirectory.GlobFiles("*.nupkg")
                .Where(x => !x.ToString().EndsWith("symbols.nupkg"))
                .ToList();
            Assert.NotEmpty(packages);
            packages
                .ForEach(x =>
                {
                    DotNetNuGetPush(s => s
                        .SetTargetPath(x)
                        .SetSource(DanglPublicFeedSource)
                        .SetApiKey(FeedzAccessToken));

                    if (GitVersion.BranchName.Equals("master") || GitVersion.BranchName.Equals("origin/master"))
                    {
                        // Stable releases are published to NuGet
                        DotNetNuGetPush(s => s
                            .SetTargetPath(x)
                            .SetSource("https://api.nuget.org/v3/index.json")
                            .SetApiKey(NuGetApiKey));
                    }
                });
        });

    Target BuildDocFxMetadata => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            var environmentVariables = EnvironmentInfo.Variables.ToDictionary();
            environmentVariables.Add("DOCFX_SOURCE_BRANCH_NAME", GitVersion.BranchName);
            var docFxPath = NuGetToolPathResolver.GetPackageExecutable("docfx", "tools/net8.0/any/docfx.dll");
            DotNet($"{docFxPath} metadata {DocFxFile}", environmentVariables: environmentVariables);
        });

    Target BuildDocumentation => _ => _
        .DependsOn(Clean)
        .DependsOn(BuildDocFxMetadata)
        .Executes(() =>
        {
            // Using README.md as index.md
            if (File.Exists(SolutionDirectory / "index.md"))
            {
                File.Delete(SolutionDirectory / "index.md");
            }

            File.Copy(SolutionDirectory / "README.md", SolutionDirectory / "index.md");

            var environmentVariables = EnvironmentInfo.Variables.ToDictionary();
            environmentVariables.Add("DOCFX_SOURCE_BRANCH_NAME", GitVersion.BranchName);
            var docFxPath = NuGetToolPathResolver.GetPackageExecutable("docfx", "tools/net8.0/any/docfx.dll");
            DotNet($"{docFxPath} {DocFxFile}", environmentVariables: environmentVariables);

            File.Delete(SolutionDirectory / "index.md");
            Directory.Delete(SolutionDirectory / "api", true);
        });

    Target UploadDocumentation => _ => _
        .DependsOn(Push) // To have a relation between pushed package version and published docs version
        .DependsOn(BuildDocumentation)
        .Requires(() => DocuApiKey)
        .Requires(() => DocuBaseUrl)
        .Executes(() =>
        {
            var changeLog = GetCompleteChangeLog(ChangeLogFile);

            WebDocu(s => s
                .SetDocuBaseUrl(DocuBaseUrl)
                .SetDocuApiKey(DocuApiKey)
                .SetMarkdownChangelog(changeLog)
                .SetSourceDirectory(OutputDirectory / "docs")
                .SetVersion(GitVersion.NuGetVersion)
            );
        });

    Target PublishGitHubRelease => _ => _
        .DependsOn(Pack)
        .Requires(() => GitHubAuthenticationToken)
        .OnlyWhenDynamic(() => GitVersion.BranchName.Equals("master") || GitVersion.BranchName.Equals("origin/master"))
        .Executes(async () =>
        {
            var releaseTag = $"v{GitVersion.MajorMinorPatch}";

            var changeLogSectionEntries = ExtractChangelogSectionNotes(ChangeLogFile);
            var latestChangeLog = changeLogSectionEntries
                .Aggregate((c, n) => c + Environment.NewLine + n);
            var completeChangeLog = $"## {releaseTag}" + Environment.NewLine + latestChangeLog;

            var repositoryInfo = GetGitHubRepositoryInfo(GitRepository);
            var nuGetPackages = OutputDirectory.GlobFiles("*.nupkg").ToArray();
            Assert.NotEmpty(nuGetPackages);

            await PublishRelease(x => x
                    .SetArtifactPaths(nuGetPackages.Select(a => a.ToString()).ToArray())
                    .SetCommitSha(GitVersion.Sha)
                    .SetReleaseNotes(completeChangeLog)
                    .SetRepositoryName(repositoryInfo.repositoryName)
                    .SetRepositoryOwner(repositoryInfo.gitHubOwner)
                    .SetTag(releaseTag)
                    .SetToken(GitHubAuthenticationToken));
        });

    private void PrependFrameworkToTestresults()
    {
        var testResults = OutputDirectory.GlobFiles("*testresults*.xml").ToList();
        Serilog.Log.Information($"Found {testResults.Count} test result files on which to append the framework.");
        foreach (var testResultFile in testResults)
        {
            var frameworkName = GetFrameworkNameFromFilename(testResultFile);
            var xDoc = XDocument.Load(testResultFile);

            foreach (var testType in ((IEnumerable)xDoc.XPathEvaluate("//test/@type")).OfType<XAttribute>())
            {
                testType.Value = frameworkName + "+" + testType.Value;
            }

            foreach (var testName in ((IEnumerable)xDoc.XPathEvaluate("//test/@name")).OfType<XAttribute>())
            {
                testName.Value = frameworkName + "+" + testName.Value;
            }

            xDoc.Save(testResultFile);
        }

        // Merge all the results to a single file
        // The "run-time" attributes of the single assemblies is ensured to be unique for each single assembly by this test,
        // since in Jenkins, the format is internally converted to JUnit. Aterwards, results with the same timestamps are
        // ignored. See here for how the code is translated to JUnit format by the Jenkins plugin:
        // https://github.com/jenkinsci/xunit-plugin/blob/d970c50a0501f59b303cffbfb9230ba977ce2d5a/src/main/resources/org/jenkinsci/plugins/xunit/types/xunitdotnet-2.0-to-junit.xsl#L75-L79
        Serilog.Log.Information("Updating \"run-time\" attributes in assembly entries to prevent Jenkins to treat them as duplicates");
        var firstXdoc = XDocument.Load(testResults[0]);
        var runtime = DateTime.Now;
        var firstAssemblyNodes = firstXdoc.Root.Elements().Where(e => e.Name.LocalName == "assembly");
        foreach (var assemblyNode in firstAssemblyNodes)
        {
            assemblyNode.SetAttributeValue("run-time", $"{runtime:HH:mm:ss}");
            runtime = runtime.AddSeconds(1);
        }
        for (var i = 1; i < testResults.Count; i++)
        {
            var xDoc = XDocument.Load(testResults[i]);
            var assemblyNodes = xDoc.Root.Elements().Where(e => e.Name.LocalName == "assembly");
            foreach (var assemblyNode in assemblyNodes)
            {
                assemblyNode.SetAttributeValue("run-time", $"{runtime:HH:mm:ss}");
                runtime = runtime.AddSeconds(1);
            }
            firstXdoc.Root.Add(assemblyNodes);
        }

        firstXdoc.Save(OutputDirectory / "testresults.xml");
        testResults.ForEach(d => d.DeleteFile());
    }

    private string GetFrameworkNameFromFilename(string filename)
    {
        var name = Path.GetFileName(filename);
        name = name.Substring(0, name.Length - ".xml".Length);
        var startIndex = name.LastIndexOf('-');
        name = name.Substring(startIndex + 1);
        return name;
    }
}
