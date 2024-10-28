// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Brokers.NECS.Requests;
using ISL.ReIdentification.Core.Models.Brokers.NECS.Responses;

namespace ISL.ReIdentification.Core.Brokers.NECS
{
    public interface INECSBroker
    {
        ValueTask<NecsReIdentificationResponse> ReIdAsync(NecsReIdentificationRequest necsReIdentificationRequest);
    }
}
