// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.IO;
using System.Security.Cryptography;

namespace ISL.ReIdentification.Core.Brokers.Hashing
{
    public class HashBroker : IHashBroker
    {
        public string GenerateMd5Hash(Stream data)
        {
            if (data == null)
            {
                return string.Empty;
            }

            using (MD5 md5 = MD5.Create())
            {
                data.Position = 0;
                byte[] hashBytes = md5.ComputeHash(data);
                var md5Hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                return md5Hash;
            }
        }

        public string GenerateSha256Hash(Stream data)
        {
            if (data == null)
            {
                return string.Empty;
            }

            using (SHA256 sha256 = SHA256.Create())
            {
                data.Position = 0;
                byte[] hashBytes = sha256.ComputeHash(data);
                var sha256Hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                return sha256Hash;
            }
        }
    }
}
