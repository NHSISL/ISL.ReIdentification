// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Brokers.Identifiers;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.NECS;
using ISL.ReIdentification.Core.Models.Brokers.NECS;
using ISL.ReIdentification.Core.Models.Brokers.NECS.Requests;
using ISL.ReIdentification.Core.Models.Brokers.NECS.Responses;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications;

namespace ISL.ReIdentification.Core.Services.Foundations.ReIdentifications
{
    public partial class ReIdentificationService : IReIdentificationService
    {
        private readonly INECSBroker necsBroker;
        private readonly IIdentifierBroker identifierBroker;
        private readonly NECSConfiguration necsConfiguration;
        private readonly ILoggingBroker loggingBroker;

        public ReIdentificationService(
            INECSBroker necsBroker,
            IIdentifierBroker identifierBroker,
            NECSConfiguration necsConfiguration,
            ILoggingBroker loggingBroker)
        {
            this.necsBroker = necsBroker;
            this.identifierBroker = identifierBroker;
            this.necsConfiguration = necsConfiguration;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<IdentificationRequest> ProcessReidentificationRequest(
            IdentificationRequest identificationRequests) =>
            TryCatch(async () =>
            {
                ValidateIdentificationRequestOnProcess(identificationRequests);

                IdentificationRequest processedItems =
                    await BulkProcessRequestsAsync(identificationRequests, necsConfiguration.ApiMaxBatchSize);

                return processedItems;
            });

        virtual internal async ValueTask<IdentificationRequest> BulkProcessRequestsAsync(
            IdentificationRequest identificationRequest, int batchSize)
        {
            int totalRecords = identificationRequest.IdentificationItems.Count;
            var exceptions = new List<Exception>();

            for (int i = 0; i < totalRecords; i += batchSize)
            {
                try
                {
                    await TryCatch(async () =>
                    {
                        NecsReidentificationRequest necsReidentificationRequest = new NecsReidentificationRequest
                        {
                            RequestId = await this.identifierBroker.GetIdentifierAsync(),
                            UserIdentifier = identificationRequest.EntraUserId.ToString(),
                            Purpose = identificationRequest.Purpose,
                            Organisation = identificationRequest.Organisation,
                            Reason = identificationRequest.Reason,
                        };

                        List<NecsPseudonymisedItem> batch = identificationRequest.IdentificationItems.Skip(i)
                            .Take(batchSize).ToList().Select(item =>
                                new NecsPseudonymisedItem { RowNumber = item.RowNumber, Psuedo = item.Identifier })
                                    .ToList();

                        necsReidentificationRequest.PseudonymisedNumbers.AddRange(batch);

                        NecsReIdentificationResponse necsReIdentificationResponse =
                            await necsBroker.ReIdAsync(necsReidentificationRequest);

                        foreach (var item in necsReIdentificationResponse.Results)
                        {
                            var record = identificationRequest.IdentificationItems
                                .FirstOrDefault(i => i.RowNumber == item.RowNumber);

                            if (record != null)
                            {
                                record.Identifier = item.NhsNumber;
                                record.Message = item.Message;
                                record.IsReidentified = true;
                            }
                        }
                    });
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            if (exceptions.Any())
            {
                throw new AggregateException(
                    message: $"Unable to process addresses in 1 of the batch(es) from {identificationRequest.Id}",
                    innerExceptions: exceptions);
            }

            return identificationRequest;
        }
    }
}
