// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.ImpersonationContexts;

namespace ISL.ReIdentification.Portals.Server.Tests.Acceptance.Apis
{
    public partial class ImpersonationContextsApiTests
    {
        [Fact]
        public async Task ShouldPostImpersonationContextAsync()
        {
            // given
            ImpersonationContext randomImpersonationContext = CreateRandomImpersonationContext();
            ImpersonationContext inputImpersonationContext = randomImpersonationContext;
            ImpersonationContext expectedImpersonationContext = inputImpersonationContext;

            // when 
            await this.apiBroker.PostImpersonationContextAsync(inputImpersonationContext);

            ImpersonationContext actualImpersonationContext =
                await this.apiBroker.GetImpersonationContextByIdAsync(inputImpersonationContext.Id);

            // then
            actualImpersonationContext.Should().BeEquivalentTo(expectedImpersonationContext, options =>
                options.Excluding(ctx => ctx.CreatedBy)
                       .Excluding(ctx => ctx.CreatedDate)
                       .Excluding(ctx => ctx.UpdatedBy)
                       .Excluding(ctx => ctx.UpdatedDate));
            await this.apiBroker.DeleteImpersonationContextByIdAsync(actualImpersonationContext.Id);
        }

        [Fact]
        public async Task ShouldGetAllImpersonationContextsAsync()
        {
            // given
            List<ImpersonationContext> randomImpersonationContexts = await PostRandomImpersonationContextsAsync();
            List<ImpersonationContext> expectedImpersonationContexts = randomImpersonationContexts;

            // when
            var actualImpersonationContexts = await this.apiBroker.GetAllImpersonationContextsAsync();

            // then
            foreach (ImpersonationContext expectedImpersonationContext in expectedImpersonationContexts)
            {
                ImpersonationContext actualImpersonationContext = actualImpersonationContexts
                    .Single(actualImpersonationContext =>
                        actualImpersonationContext.Id == expectedImpersonationContext.Id);

                actualImpersonationContext.Should().BeEquivalentTo(expectedImpersonationContext);
                await this.apiBroker.DeleteImpersonationContextByIdAsync(actualImpersonationContext.Id);
            }
        }

        [Fact]
        public async Task ShouldGetImpersonationContextByIdAsync()
        {
            // given
            ImpersonationContext randomImpersonationContext = await PostRandomImpersonationContextAsync();
            ImpersonationContext expectedImpersonationContext = randomImpersonationContext;

            // when
            var actualImpersonationContext = await this.apiBroker
                .GetImpersonationContextByIdAsync(randomImpersonationContext.Id);

            // then
            actualImpersonationContext.Should().BeEquivalentTo(expectedImpersonationContext);
            await this.apiBroker.DeleteImpersonationContextByIdAsync(actualImpersonationContext.Id);
        }

        [Fact]
        public async Task ShouldPutImpersonationContextAsync()
        {
            // given
            ImpersonationContext randomImpersonationContext = await PostRandomImpersonationContextAsync();

            ImpersonationContext modifiedImpersonationContext =
                UpdateImpersonationContextWithRandomValues(randomImpersonationContext);

            // when
            await this.apiBroker.PutImpersonationContextAsync(modifiedImpersonationContext);

            var actualImpersonationContext = await this.apiBroker
                .GetImpersonationContextByIdAsync(randomImpersonationContext.Id);

            // then
            actualImpersonationContext.Should().BeEquivalentTo(modifiedImpersonationContext, options =>
                options.Excluding(ctx => ctx.UpdatedBy)
                       .Excluding(ctx => ctx.UpdatedDate));
            await this.apiBroker.DeleteImpersonationContextByIdAsync(actualImpersonationContext.Id);
        }

        [Fact]
        public async Task ShouldDeleteImpersonationContextAsync()
        {
            // given
            ImpersonationContext randomImpersonationContext = await PostRandomImpersonationContextAsync();
            ImpersonationContext inputImpersonationContext = randomImpersonationContext;
            ImpersonationContext expectedImpersonationContext = inputImpersonationContext;

            // when
            ImpersonationContext deletedImpersonationContext =
                await this.apiBroker.DeleteImpersonationContextByIdAsync(inputImpersonationContext.Id);

            List<ImpersonationContext> actualResult =
                await this.apiBroker.GetSpecificImpersonationContextByIdAsync(inputImpersonationContext.Id);

            // then
            actualResult.Count().Should().Be(0);
        }
    }
}
