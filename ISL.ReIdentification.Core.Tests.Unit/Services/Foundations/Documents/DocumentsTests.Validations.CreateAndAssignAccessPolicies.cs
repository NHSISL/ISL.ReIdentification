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
        public async Task ShouldThrowValidationExceptionOnCreateAndAssignAccessPoliciesAsync(
            string invalidString,
            List<AccessPolicy> invalidList)
        {
            // given
            string invalidContainer = invalidString;
            List<AccessPolicy> invalidAccessPolicyList = invalidList;

            var invalidDocumentException = new InvalidDocumentException(
                message: "Invalid document. Please correct the errors and try again.");

            invalidDocumentException.AddData(
                key: "Container",
                values: "Text is invalid");

            invalidDocumentException.AddData(
                key: "AccessPolicies",
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
    }
}
