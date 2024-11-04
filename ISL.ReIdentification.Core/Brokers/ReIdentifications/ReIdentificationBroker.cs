// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISL.Providers.Notifications.Abstractions;
using ISL.Providers.ReIdentification.Abstractions;
using ISL.Providers.ReIdentification.Abstractions.Models;

namespace ISL.ReIdentification.Core.Brokers.ReIdentifications
{
    public class ReIdentificationBroker : IReIdentificationBroker
    {
        private readonly IReIdentificationAbstractionProvider reIdentificationAbstractionProvider;

        public ReIdentificationBroker(IReIdentificationAbstractionProvider reIdentificationAbstractionProvider)
        {
            this.reIdentificationAbstractionProvider = reIdentificationAbstractionProvider;
        }

        /// <summary>
        /// Re-identifies a patient from a list of pseudo identifiers.
        /// </summary>
        /// <returns>
        /// A re-identification request where the pseudo identfiers has been replaced by real identifiers.
        /// If the re-identification could not happen due to pseudo identifiers being valid, the identifier will be
        /// replaced by 0000000000 and the message field will be populated with a reason.
        /// </returns>
        public async ValueTask<ReIdentificationRequest> ReIdentifyAsync(ReIdentificationRequest reIdentificationRequest) =>
            await this.reIdentificationAbstractionProvider.ReIdentifyAsync(reIdentificationRequest);
    }
}
