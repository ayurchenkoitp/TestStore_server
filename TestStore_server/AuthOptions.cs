using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace TestStore_server
{
    public class AuthOptions
    {
        public const string ISSUER = "TestStoreServer"; // издатель токена
        public const string AUDIENCE = "TestStoreClient"; // потребитель токена
        const string KEY = "mysupersecret_secretkey!123";   // ключ для шифрации
        public const int LIFETIME = 100; // время жизни токена - 1 минута
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
