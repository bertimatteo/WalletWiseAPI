using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WalletWiseApi.Services
{
    public class JwtTokenService
    {
        public static string CreateToken(string userName, string key)
        {
            string result = string.Empty;

            if (!String.IsNullOrEmpty(userName))
            {
                List<Claim> claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, userName)
                };

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

                var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject            = new ClaimsIdentity(claims),
                    Expires            = DateTime.Now.AddMinutes(60),
                    SigningCredentials = credential
                };

                var token = new JwtSecurityTokenHandler().CreateToken(tokenDescriptor);

                result = new JwtSecurityTokenHandler().WriteToken(token);
            }

            return result;
        }
    }
}
