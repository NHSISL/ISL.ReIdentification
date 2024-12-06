using System;
using System.Linq.Expressions;
using Moq;
using ISL.ReIdentification.Core.Brokers.DateTimes;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.Storages.Sql;
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements;
using ISL.ReIdentification.Core.Services.Foundations.UserAgreements;
using Tynamix.ObjectFiller;
using Xeptions;
using Xunit;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.UserAgreements
{
    public partial class UserAgreementServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IUserAgreementService userAgreementService;

        public UserAgreementServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.userAgreementService = new UserAgreementService(
                storageBroker: this.storageBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        public static TheoryData MinutesBeforeOrAfter()
        {
            int randomNumber = GetRandomNumber();
            int randomNegativeNumber = GetRandomNegativeNumber();

            return new TheoryData<int>
            {
                randomNumber,
                randomNegativeNumber
            };
        }

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static int GetRandomNegativeNumber() =>
            -1 * new IntRange(min: 2, max: 10).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static UserAgreement CreateRandomUserAgreement(DateTimeOffset dateTimeOffset) =>
            CreateUserAgreementFiller(dateTimeOffset).Create();

        private static Filler<UserAgreement> CreateUserAgreementFiller(DateTimeOffset dateTimeOffset)
        {
            string user = Guid.NewGuid().ToString();
            var filler = new Filler<UserAgreement>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnProperty(userAgreement => userAgreement.CreatedBy).Use(user)
                .OnProperty(userAgreement => userAgreement.UpdatedBy).Use(user);

            // TODO: Complete the filler setup e.g. ignore related properties etc...

            return filler;
        }
    }
}