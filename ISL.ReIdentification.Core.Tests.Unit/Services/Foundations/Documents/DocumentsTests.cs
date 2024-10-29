// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.IO;
using System.Linq.Expressions;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.Storages.Blob;
using ISL.ReIdentification.Core.Services.Foundations.Documents;
using KellermanSoftware.CompareNetObjects;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.Documents
{
    public partial class DocumentsTests
    {
        private readonly Mock<IBlobStorageBroker> blobStorageBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly ICompareLogic compareLogic;
        private readonly DocumentService documentService;

        public DocumentsTests()
        {
            this.blobStorageBrokerMock = new Mock<IBlobStorageBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();
            this.compareLogic = new CompareLogic();

            this.documentService = new DocumentService(
                blobStorageBrokerMock.Object,
                loggingBrokerMock.Object);
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        public class ZeroLengthStream : MemoryStream
        {
            public override long Length => 0;
        }

        public class HasLengthStream : MemoryStream
        {
            public override long Length => 1;
        }

        public static TheoryData<Stream, string> InvalidArgumentsStreamLengthZero()
        {
            Stream stream = new ZeroLengthStream();

            return new TheoryData<Stream, string>
            {
                { null, null },
                { stream, "" },
                { stream, " " }
            };
        }

        public static TheoryData<Stream, string> InvalidArgumentsStreamHasLength()
        {
            Stream stream = new HasLengthStream();

            return new TheoryData<Stream, string>
            {
                { null, null },
                { stream, ""},
                { stream, " " }
            };
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException)
        {
            return actualException =>
                actualException.SameExceptionAs(expectedException);
        }

        private Expression<Func<Stream, bool>> SameStreamAs(Stream expectedStream) =>
            actualStream => this.compareLogic.Compare(expectedStream, actualStream).AreEqual;

        private static byte[] ReadAllBytesFromStream(Stream stream)
        {
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }

            using (MemoryStream memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
