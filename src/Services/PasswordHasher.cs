using System.Security.Cryptography;
using System.Text;

namespace WalletWiseApi.Services
{
    public static class PasswordHasher
    {
        public static string GenerateSalt()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var byteSalt = new byte[16];
                rng.GetBytes(byteSalt);
                var salt = Convert.ToBase64String(byteSalt);
                return salt;
            }
        }

        public static string ComputeHash(string password, string salt, string pepper, int iteration)
        {
            if (iteration <= 0)
                return password;

            using (var sha256 = SHA256.Create())
            {
                var passwordSaltPepper = $"{password}{salt}{pepper}";
                var passwordSaltPepperByte = Encoding.UTF8.GetBytes(passwordSaltPepper);
                var passwordSaltPepperHash = sha256.ComputeHash(passwordSaltPepperByte);
                var hash = Convert.ToBase64String(passwordSaltPepperHash);

                return ComputeHash(hash, salt, pepper, iteration - 1);
            }
        }
    }
}
