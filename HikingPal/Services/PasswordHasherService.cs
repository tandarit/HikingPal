using HikingPal.Models;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace HikingPal.Services
{
    public sealed class PasswordHasherService : IPasswordHasherService
    {
        private readonly HashingOptions _hashingOptions;

        public PasswordHasherService(IOptions<HashingOptions> options)
        {
            _hashingOptions = options.Value;
        }

        public HashObject Hash(string password, int iteration)
        {
            using (var algorithm = new Rfc2898DeriveBytes(
              password,
              _hashingOptions.SaltSize,
              iteration,
              HashAlgorithmName.SHA512))
            {
                HashObject hashObject = new HashObject();
                hashObject.Key = Convert.ToBase64String(algorithm.GetBytes(_hashingOptions.KeySize));
                hashObject.Salt = Convert.ToBase64String(algorithm.Salt);

                return hashObject;
            }
        }

        public bool Check(string hash, string password)
        {
            var parts = hash.Split('.', 3);

            if (parts.Length != 3)
            {
                throw new FormatException("Unexpected hash format. " +
                  "Should be formatted as `{iterations}.{salt}.{hash}`");
            }

            var iterations = Convert.ToInt32(parts[0]);
            var salt = Convert.FromBase64String(parts[1]);
            var key = Convert.FromBase64String(parts[2]);

            using (var algorithm = new Rfc2898DeriveBytes(
              password,
              salt,
              iterations,
              HashAlgorithmName.SHA512))
            {
                var keyToCheck = algorithm.GetBytes(_hashingOptions.KeySize);

                var verified = keyToCheck.SequenceEqual(key);

                return verified;
            }
        }
    }
}
