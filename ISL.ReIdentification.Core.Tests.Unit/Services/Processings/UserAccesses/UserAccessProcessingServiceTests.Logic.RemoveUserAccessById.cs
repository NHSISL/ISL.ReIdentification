// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Processings.UserAccesses
{
    public partial class UserAccessProcessingServiceTests
    {
        [Fact]
        public async Task ShouldRemoveUserAccessByIdAsync()
        {
            // given
            Guid randomId = Guid.NewGuid();
            Guid inputId = randomId;
            UserAccess randomUserAccess = CreateRandomUserAccess();
            UserAccess inputUserAccess = randomUserAccess;
            UserAccess storageUserAccess = inputUserAccess.DeepClone();
            UserAccess expectedUserAccess = inputUserAccess.DeepClone();

            this.userAccessServiceMock.Setup(service =>
                service.RemoveUserAccessByIdAsync(inputId))
                    .ReturnsAsync(storageUserAccess);

            // when
            UserAccess actualUserAccess = await this.userAccessProcessingService
                .RemoveUserAccessByIdAsync(inputId);

            // then
            actualUserAccess.Should().BeEquivalentTo(expectedUserAccess);

            this.userAccessServiceMock.Verify(service =>
                service.RemoveUserAccessByIdAsync(inputId),
                    Times.Once);

            this.userAccessServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
