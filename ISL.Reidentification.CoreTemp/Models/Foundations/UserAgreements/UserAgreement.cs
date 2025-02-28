// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;

namespace ISL.ReIdentification.Core.Models.Foundations.UserAgreements
{
    public class UserAgreement : IKey, IAudit
    {
        public Guid Id { get; set; }
        public string EntraUserId { get; set; }
        public string AgreementType { get; set; }
        public string AgreementVersion { get; set; }
        public DateTimeOffset AgreementDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
    }
}
