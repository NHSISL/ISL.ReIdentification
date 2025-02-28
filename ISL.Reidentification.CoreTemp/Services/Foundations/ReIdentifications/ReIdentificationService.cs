// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using ISL.Providers.ReIdentification.Abstractions.Models;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.ReIdentifications;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications;

namespace ISL.ReIdentification.Core.Services.Foundations.ReIdentifications
{
    public partial class ReIdentificationService : IReIdentificationService
    {
        private readonly IReIdentificationBroker reIdentificationBroker;
        private readonly ILoggingBroker loggingBroker;

        public ReIdentificationService(
            IReIdentificationBroker reIdentificationBroker,
            ILoggingBroker loggingBroker)
        {
            this.reIdentificationBroker = reIdentificationBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<IdentificationRequest> ProcessReIdentificationRequest(
            IdentificationRequest identificationRequest) =>
        TryCatch(async () =>
        {
            ValidateIdentificationRequestOnProcess(identificationRequest);

            ReIdentificationRequest reIdentificationRequest = new ReIdentificationRequest
            {
                RequestId = identificationRequest.Id,
                UserIdentifier = identificationRequest.EntraUserId.ToString(),
                Organisation = identificationRequest.Organisation,
                Reason = identificationRequest.Reason,
                ReIdentificationItems = identificationRequest.IdentificationItems
                    .Select(item => new ReIdentificationItem
                    {
                        RowNumber = item.RowNumber,
                        Identifier = item.Identifier
                    }).ToList()
            };

            ReIdentificationRequest reIdentificationResponse =
                await this.reIdentificationBroker.ReIdentifyAsync(reIdentificationRequest);

            foreach (var responseItem in reIdentificationResponse.ReIdentificationItems)
            {
                var record = identificationRequest.IdentificationItems
                    .FirstOrDefault(i => i.RowNumber == responseItem.RowNumber);

                if (record != null)
                {
                    record.Identifier = responseItem.Identifier;
                    record.Message = responseItem.Message;
                    record.IsReidentified = true;
                }
            }

            return identificationRequest;
        });
    }
}
