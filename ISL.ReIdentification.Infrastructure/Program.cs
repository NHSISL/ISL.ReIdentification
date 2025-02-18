// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.ReIdentification.Infrastructure.Services;

namespace ISL.ReIdentification.Infrastructure
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var scriptGenerationService = new ScriptGenerationService();

            scriptGenerationService.GenerateBuildScript(
                branchName: "main",
                projectName: "ISL.ReIdentification.Core",
                dotNetVersion: "9.0.100");

            scriptGenerationService.GeneratePrLintScript(branchName: "main");
        }
    }
}
