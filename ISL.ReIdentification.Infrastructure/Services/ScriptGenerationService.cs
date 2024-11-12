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

        public void GenerateBuildScript(string branchName, string projectName)
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
                        "label",
                        new LabelJob(
                            runsOn: BuildMachines.UbuntuLatest,
                            githubToken: "${{ secrets.PAT_FOR_TAGGING }}")
                    },
                    {
                        "build",
                        new Job
                        {
                            Name = "Build",
                            RunsOn = BuildMachines.WindowsLatest,

                            EnvironmentVariables = new Dictionary<string, string>
                            {
                                { "NOTIFICATIONCONFIGURATIONS__APIKEY", "${{ secrets.NOTIFICATIONCONFIGURATIONS__APIKEY }}" },
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
                                        DotNetVersion = "8.0.302"
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
                                    Name = "Deploy Database",
                                    Run = $"dotnet ef database update --project {projectName}/{projectName}.csproj --startup-project {projectName}/{projectName}.csproj"
                                },

                                new TestTask
                                {
                                    Name = "Run Tests - ISL.ReIdentification.Core.Tests.Unit",
                                    Run = "dotnet test --no-build --verbosity normal ISL.ReIdentification.Core.Tests.Unit/ISL.ReIdentification.Core.Tests.Unit.csproj"
                                },

                                new TestTask
                                {
                                    Name = "Run Tests - ISL.ReIdentification.Configurations.Server.Tests.Unit",
                                    Run = "dotnet test --no-build --verbosity normal ISL.ReIdentification.WebApps.Configurations/ISL.ReIdentification.Configurations.Server.Tests.Unit/ISL.ReIdentification.Configurations.Server.Tests.Unit.csproj"
                                },

                                new TestTask
                                {
                                    Name = "Run Tests - ISL.ReIdentification.Configurations.Server.Tests.Acceptance",
                                    Run = "dotnet test --no-build --verbosity normal ISL.ReIdentification.WebApps.Configurations/ISL.ReIdentification.Configurations.Server.Tests.Acceptance/ISL.ReIdentification.Configurations.Server.Tests.Acceptance.csproj"
                                },

                                new TestTask
                                {
                                    Name = "Run Tests - ISL.ReIdentification.Portals.Server.Tests.Unit",
                                    Run = "dotnet test --no-build --verbosity normal ISL.ReIdentification.WebApps.Portals/ISL.ReIdentification.Portals.Server.Tests.Unit/ISL.ReIdentification.Portals.Server.Tests.Unit.csproj"
                                },

                                new TestTask
                                {
                                    Name = "Run Tests - ISL.ReIdentification.Portals.Server.Tests.Acceptance",
                                    Run = "dotnet test --no-build --verbosity normal ISL.ReIdentification.WebApps.Portals/ISL.ReIdentification.Portals.Server.Tests.Acceptance/ISL.ReIdentification.Portals.Server.Tests.Acceptance.csproj"
                                },
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
    }
}
