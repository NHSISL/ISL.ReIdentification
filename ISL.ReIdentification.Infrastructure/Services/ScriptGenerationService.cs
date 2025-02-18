// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using ADotNet.Clients;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets.Tasks;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets.Tasks.SetupDotNetTaskV3s;

namespace ISL.ReIdentification.Infrastructure.Services
{
    internal class ScriptGenerationService
    {
        private readonly ADotNetClient adotNetClient;

        public ScriptGenerationService() =>
            adotNetClient = new ADotNetClient();

        public void GenerateBuildScript(string branchName, string projectName, string dotNetVersion)
        {
            var githubPipeline = new GithubPipeline
            {
                Name = "Build",

                OnEvents = new Events
                {
                    Push = new PushEvent { Branches = [branchName] },

                    PullRequest = new PullRequestEvent
                    {
                        Types = ["opened", "synchronize", "reopened", "closed"],
                        Branches = [branchName]
                    }
                },

                Jobs = new Dictionary<string, Job>
                {
                    {
                        "build",
                        new Job
                        {
                            Name = "Build",
                            RunsOn = BuildMachines.WindowsLatest,

                            EnvironmentVariables = new Dictionary<string, string>
                            {
                                { "NOTIFICATIONCONFIGURATIONS__APIKEY", "${{ secrets.NOTIFICATIONCONFIGURATIONS__APIKEY }}" },
                                { "NOTIFICATIONCONFIGURATIONS__CSVAPPROVEDREQUESTTEMPLATEID", "${{ secrets.NOTIFICATIONCONFIGURATIONS__CSVAPPROVEDREQUESTTEMPLATEID }}" },
                                { "NOTIFICATIONCONFIGURATIONS__CSVDENIEDREQUESTTEMPLATEID", "${{ secrets.NOTIFICATIONCONFIGURATIONS__CSVDENIEDREQUESTTEMPLATEID }}" },
                                { "NOTIFICATIONCONFIGURATIONS__CSVPENDINGAPPROVALREQUESTTEMPLATEID", "${{ secrets.NOTIFICATIONCONFIGURATIONS__CSVPENDINGAPPROVALREQUESTTEMPLATEID }}" },
                                { "NOTIFICATIONCONFIGURATIONS__IMPERSONATIONAPPROVEDREQUESTTEMPLATEID", "${{ secrets.NOTIFICATIONCONFIGURATIONS__IMPERSONATIONAPPROVEDREQUESTTEMPLATEID }}" },
                                { "NOTIFICATIONCONFIGURATIONS__IMPERSONATIONDENIEDREQUESTTEMPLATEID", "${{ secrets.NOTIFICATIONCONFIGURATIONS__IMPERSONATIONDENIEDREQUESTTEMPLATEID }}" },
                                { "NOTIFICATIONCONFIGURATIONS__IMPERSONATIONPENDINGAPPROVALREQUESTTEMPLATEID", "${{ secrets.NOTIFICATIONCONFIGURATIONS__IMPERSONATIONPENDINGAPPROVALREQUESTTEMPLATEID }}" },
                                { "NOTIFICATIONCONFIGURATIONS__IMPERSONATIONTOKENSGENERATEDTEMPLATEID", "${{ secrets.NOTIFICATIONCONFIGURATIONS__IMPERSONATIONTOKENSGENERATEDTEMPLATEID }}" },
                                { "PROJECTSTORAGECONFIGURATION__STORAGEACCOUNTACCESSKEY", "${{ secrets.PROJECTSTORAGECONFIGURATION__STORAGEACCOUNTACCESSKEY }}" },
                                { "PROJECTSTORAGECONFIGURATION__STORAGEACCOUNTNAME", "${{ secrets.PROJECTSTORAGECONFIGURATION__STORAGEACCOUNTNAME }}"},
                                { "PROJECTSTORAGECONFIGURATION__SERVICEURI", "${{ secrets.PROJECTSTORAGECONFIGURATION__SERVICEURI }}"},
                            },

                            Steps = new List<GithubTask>
                            {
                                new CheckoutTaskV3
                                {
                                    Name = "Check out"
                                },

                                new SetupDotNetTaskV3
                                {
                                    Name = "Setup .Net",

                                    With = new TargetDotNetVersionV3
                                    {
                                        DotNetVersion = dotNetVersion
                                    }
                                },

                                new RestoreTask
                                {
                                    Name = "Restore"
                                },

                                new DotNetBuildTask
                                {
                                    Name = "Build"
                                },

                                new GithubTask
                                {
                                    Name = "Install EF Tools",
                                    Run = "dotnet tool install --global dotnet-ef"
                                },

                                new GithubTask
                                {
                                    Name = "Drop Database If Exists",
                                    Run = $"dotnet ef database drop --project {projectName}/{projectName}.csproj --startup-project {projectName}/{projectName}.csproj --force"
                                },

                                new GithubTask
                                {
                                    Name = "Deploy Database",
                                    Run = $"dotnet ef database update --project {projectName}/{projectName}.csproj --startup-project {projectName}/{projectName}.csproj"
                                },

                                new TestTask
                                {
                                    Name = "Test"
                                }
                            }
                        }
                    },
                    {
                        "add_tag",
                        new TagJob(
                            runsOn: BuildMachines.UbuntuLatest,
                            dependsOn: "build",
                            projectRelativePath: $"{projectName}/{projectName}.csproj",
                            githubToken: "${{ secrets.PAT_FOR_TAGGING }}",
                            branchName: branchName)
                        {
                            Name = "Tag and Release"
                        }
                    },
                }
            };

            string buildScriptPath = "../../../../.github/workflows/build.yml";
            string directoryPath = Path.GetDirectoryName(buildScriptPath);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            adotNetClient.SerializeAndWriteToFile(
                adoPipeline: githubPipeline,
                path: buildScriptPath);
        }

        public void GeneratePrLintScript(string branchName)
        {
            var githubPipeline = new GithubPipeline
            {
                Name = "PR Linter",

                OnEvents = new Events
                {
                    PullRequest = new PullRequestEvent
                    {
                        Types = ["opened", "edited", "synchronize", "reopened", "closed"],
                        Branches = [branchName]
                    }
                },

                Jobs = new Dictionary<string, Job>
                {
                    {
                        "label",
                        new LabelJobV2(runsOn: BuildMachines.UbuntuLatest)
                        {
                            Name = "Label",
                        }
                    },
                    {
                        "requireIssueOrTask",
                        new RequireIssueOrTaskJob()
                        {
                            Name = "Require Issue Or Task Association",
                        }
                    },
                }
            };

            string buildScriptPath = "../../../../.github/workflows/prLinter.yml";
            string directoryPath = Path.GetDirectoryName(buildScriptPath);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            adotNetClient.SerializeAndWriteToFile(
                adoPipeline: githubPipeline,
                path: buildScriptPath);
        }
    }
}
