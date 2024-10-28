// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.Storages.Blob;
using ISL.ReIdentification.Core.Services.Foundations.Documents;
using Moq;
using Tynamix.ObjectFiller;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.Documents
{
    public partial class DocumentsTests
    {
        private readonly Mock<IBlobStorageBroker> blobStorageBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly DocumentService documentService;

        public DocumentsTests()
        {
            this.blobStorageBrokerMock = new Mock<IBlobStorageBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.documentService = new DocumentService(
                blobStorageBrokerMock.Object,
                loggingBrokerMock.Object);
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();
    }
}
