// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.Documents;
using ISL.ReIdentification.Core.Models.Foundations.Documents.Exceptions;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.Documents
{
    public partial class DocumentsTests
    {
        [Theory]
        [MemberData(nameof(InvalidCreateAccessPolicyArguments))]
        public async Task ShouldThrowValidationExceptionOnCreateAndAssignAccessPoliciesInvalidArgumentsAndLogitAsync(
            string invalidString,
            List<AccessPolicy> invalidList)
        {
            // given
            string invalidContainer = invalidString;
            List<AccessPolicy> invalidAccessPolicyList = invalidList;

            var invalidDocumentException = new InvalidDocumentException(
                message: "Invalid document. Please correct the errors and try again.");

            invalidDocumentException.AddData(
                key: "container",
                values: "Text is invalid");

            invalidDocumentException.AddData(
                key: "accessPolicies",
                values: "List is invalid");

            var expectedDocumentValidationException = new DocumentValidationException(
                message: "Document validation error occurred, please fix errors and try again.",
                innerException: invalidDocumentException);

            // when
            ValueTask createAndAssignAccessPoliciesTask =
                this.documentService.CreateAndAssignAccessPoliciesAsync(invalidContainer, invalidAccessPolicyList);

            DocumentValidationException actualDocumentValidationException =
                await Assert.ThrowsAsync<DocumentValidationException>(createAndAssignAccessPoliciesTask.AsTask);

            // then
            actualDocumentValidationException.Should().BeEquivalentTo(expectedDocumentValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDocumentValidationException))),
                        Times.Once);

            this.blobStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowValidationExceptionOnCreateAndAssignAccessPoliciesInvalidPermissionsAndLogitAsync()
        {
            // given
            string someContainer = GetRandomString();
            string somePolicyname = GetRandomString();
            List<string> invalidPermissions = GetRandomStringList();

            List<AccessPolicy> invalidAccessPolicies = new List<AccessPolicy>
            {
                new AccessPolicy
                {
                    PolicyName = somePolicyname,
                    Permissions = invalidPermissions
                }
            };

            var invalidPermissionDocumentException = new InvalidPermissionDocumentException(
                message: "Invalid permission. Read, write, delete, create, add and list " +
                    "permissions are supported at this time.");

            var expectedDocumentValidationException = new DocumentValidationException(
                message: "Document validation error occurred, please fix errors and try again.",
                innerException: invalidPermissionDocumentException);

            // when
            ValueTask createAndAssignAccessPoliciesTask =
                this.documentService.CreateAndAssignAccessPoliciesAsync(someContainer, invalidAccessPolicies);

            DocumentValidationException actualDocumentValidationException =
                await Assert.ThrowsAsync<DocumentValidationException>(createAndAssignAccessPoliciesTask.AsTask);

            // then
            actualDocumentValidationException.Should().BeEquivalentTo(expectedDocumentValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDocumentValidationException))),
                        Times.Once);

            this.blobStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
