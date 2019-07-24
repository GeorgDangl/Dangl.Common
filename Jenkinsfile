pipeline {
    agent {
        node {
            label 'master'
            customWorkspace 'workspace/Dangl.Common'
        }
    }
    environment {
        KeyVaultBaseUrl = credentials('AzureCiKeyVaultBaseUrl')
        KeyVaultClientId = credentials('AzureCiKeyVaultClientId')
        KeyVaultClientSecret = credentials('AzureCiKeyVaultClientSecret')
    }
    stages {
        stage ('Tests') {
			parallel {
				stage ('Linux Test') {
					agent {
						node {
							label 'linux'
						}
					}
					steps {
						sh 'bash build.sh LinuxTest -Configuration Debug'
					}
					post {
						always {
							xunit(
								testTimeMargin: '3000',
								thresholdMode: 1,
								thresholds: [
									failed(failureNewThreshold: '0', failureThreshold: '0', unstableNewThreshold: '0', unstableThreshold: '0'),
									skipped(failureNewThreshold: '0', failureThreshold: '0', unstableNewThreshold: '0', unstableThreshold: '0')
								],
								tools: [
									xUnitDotNet(deleteOutputFiles: true, failIfNotNew: true, pattern: '**/*testresults.xml', stopProcessingIfError: false)
								])
						}
					}
				}
				stage ('Windows Test') {
					steps {
						powershell './build.ps1 Coverage -configuration Debug'
					}
					post {
						always {
							warnings(
								canComputeNew: false,
								canResolveRelativePaths: false,
								categoriesPattern: '',
								consoleParsers: [[parserName: 'MSBuild']],
								defaultEncoding: '',
								excludePattern: '',
								healthy: '',
								includePattern: '',
								messagesPattern: '',
								unHealthy: '')
							openTasks(
								canComputeNew: false,
								defaultEncoding: '',
								excludePattern: '',
								healthy: '',
								high: 'HACK, FIXME',
								ignoreCase: true,
								low: '',
								normal: 'TODO',
								pattern: '**/*.cs, **/*.g4, **/*.ts, **/*.js',
								unHealthy: '')
							xunit(
								testTimeMargin: '3000',
								thresholdMode: 1,
								thresholds: [
									failed(failureNewThreshold: '0', failureThreshold: '0', unstableNewThreshold: '0', unstableThreshold: '0'),
									skipped(failureNewThreshold: '0', failureThreshold: '0', unstableNewThreshold: '0', unstableThreshold: '0')
								],
								tools: [
									xUnitDotNet(deleteOutputFiles: true, failIfNotNew: true, pattern: '**/*testresults.xml', stopProcessingIfError: false)
								])
							cobertura(
								coberturaReportFile: 'output/cobertura_coverage.xml',
								failUnhealthy: false,
								failUnstable: false,
								maxNumberOfBuilds: 0,
								onlyStable: false,
								zoomCoverageChart: false)
							publishHTML([
								allowMissing: false,
								alwaysLinkToLastBuild: false,
								keepAll: false,
								reportDir: 'output/CoverageReport',
								reportFiles: 'index.htm',
								reportName: 'Coverage Report',
								reportTitles: ''])
						}
					}
				}
			}
		}
        stage ('Deploy') {
            steps {
                powershell './build.ps1 UploadDocumentation+PublishGitHubRelease'
            }
        }
    }
    post {
        always {
            step([$class: 'Mailer',
                notifyEveryUnstableBuild: true,
                recipients: "georg@dangl.me",
                sendToIndividuals: true])
        }
    }
}
