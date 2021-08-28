using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace LiveChat.Business.Commons
{
    
        public class AuthOptions
        {
            public string Issuer { get; set; }
            public string Audience { get; set; }
            public string Secret { get; set; }
            public int TokenLifetime { get; set; } // in seconds
            public int SaltSize { get; set; }

            public SymmetricSecurityKey SymmetricSecurityKey => new (Encoding.ASCII.GetBytes(Secret));
        }
    
}
