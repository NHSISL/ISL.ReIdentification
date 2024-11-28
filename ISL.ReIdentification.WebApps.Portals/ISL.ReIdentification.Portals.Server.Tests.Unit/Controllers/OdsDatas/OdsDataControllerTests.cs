// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas.Exceptions;
using ISL.ReIdentification.Core.Services.Foundations.OdsDatas;
using ISL.ReIdentification.Portals.Server.Controllers;
using Microsoft.EntityFrameworkCore;
using Moq;
using RESTFulSense.Controllers;
using Tynamix.ObjectFiller;
using Xeptions;

namespace ISL.ReIdentification.Portals.Server.Tests.Unit.Controllers.OdsDatas
{
    public partial class OdsDataControllerTests : RESTFulController
    {

        private readonly Mock<IOdsDataService> odsDataServiceMock;
        private readonly OdsDataController odsDataController;

        public OdsDataControllerTests()
        {
            odsDataServiceMock = new Mock<IOdsDataService>();
            odsDataController = new OdsDataController(odsDataServiceMock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            var someInnerException = new Xeption();
            string someMessage = GetRandomString();

            return new TheoryData<Xeption>
            {
                new OdsDataValidationException(
                    message: someMessage,
                    innerException: someInnerException),

                new OdsDataDependencyValidationException(
                    message: someMessage,
                    innerException: someInnerException)
            };
        }

        public static TheoryData<Xeption> ServerExceptions()
        {
            var someInnerException = new Xeption();
            string someMessage = GetRandomString();

            return new TheoryData<Xeption>
            {
                new OdsDataDependencyException(
                    message: someMessage,
                    innerException: someInnerException),

                new OdsDataServiceException(
                    message: someMessage,
                    innerException: someInnerException)
            };
        }

        private static string GetRandomString() =>
            new MnemonicString(wordCount: GetRandomNumber()).GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static IQueryable<OdsData> CreateRandomOdsDatas()
        {
            return CreateOdsDataFiller(dateTimeOffset: GetRandomDateTimeOffset())
                .Create(count: GetRandomNumber())
                    .AsQueryable();
        }

        private static List<OdsData> CreateRandomOdsDataChildren(HierarchyId parentHierarchyId, int count = 0)
        {
            if (parentHierarchyId == null)
            {
                parentHierarchyId = HierarchyId.Parse("/");
            }

            if (count == 0)
            {
                count = GetRandomNumber();
            }

            List<OdsData> children = CreateOdsDataFiller(dateTimeOffset: GetRandomDateTimeOffset())
                .Create(count)
                    .ToList();

            HierarchyId lastChildHierarchy = null;

            foreach (var child in children)
            {
                child.OdsHierarchy = parentHierarchyId.GetDescendant(lastChildHierarchy, null);
                lastChildHierarchy = child.OdsHierarchy;
            }

            return children;
        }

        private static OdsData CreateRandomOdsData() =>
            CreateOdsDataFiller(dateTimeOffset: GetRandomDateTimeOffset()).Create();

        private static OdsData CreateRandomParentOdsData() =>
            CreateOdsDataFiller(dateTimeOffset: GetRandomDateTimeOffset()).Create();

        private static OdsData CreateRandomOdsData(DateTimeOffset dateTimeOffset) =>
            CreateOdsDataFiller(dateTimeOffset).Create();


        private static Filler<OdsData> CreateOdsDataFiller(
            DateTimeOffset dateTimeOffset, HierarchyId hierarchyId = null)
        {
            string user = Guid.NewGuid().ToString();
            var filler = new Filler<OdsData>();

            if (hierarchyId == null)
            {
                hierarchyId = HierarchyId.Parse("/");
            }

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use((DateTimeOffset?)default)
                .OnProperty(odsData => odsData.OdsHierarchy).Use(hierarchyId);

            return filler;
        }
    }
}