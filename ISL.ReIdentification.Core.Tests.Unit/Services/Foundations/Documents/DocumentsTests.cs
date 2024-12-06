// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using ISL.Providers.Storages.Abstractions.Models.Exceptions;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.Storages.Blob;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses.Exceptions;
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

        private static int GetRandomNumber() =>
            new IntRange(max: 15, min: 2).GetValue();

        private static string GetRandomStringWithLengthOf(int length)
        {
            string result = new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
        }

        private static List<string> GetRandomStringList()
        {
            int randomNumber = GetRandomNumber();
            List<string> randomStringList = new List<string>();

            for (int index = 0; index < randomNumber; index++)
            {
                string randomString = GetRandomStringWithLengthOf(randomNumber);
                randomStringList.Add(randomString);
            }

            return randomStringList;
        }

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

        public static TheoryData<Xeption> DependencyExceptions()
        {
            string randomMessage = GetRandomString();
            string exceptionMessage = randomMessage;
            var innerException = new Xeption(exceptionMessage);

            return new TheoryData<Xeption>
            {
                new StorageProviderDependencyException(
                    message: "Storage provider dependency error occurred, please contact support.",
                    innerException),

                new StorageProviderServiceException(
                    message: "Storage provider service error occurred, please contact support.",
                    innerException),
            };
        }

        public static TheoryData<Xeption> DependencyValidationExceptions()
        {
            string randomMessage = GetRandomString();
            string exceptionMessage = randomMessage;
            var innerException = new Xeption(exceptionMessage);

            return new TheoryData<Xeption>
        {
            new StorageProviderValidationException(
                message: "Storage provider validation error occurred.",
                innerException)
            };
        }
    }
}
