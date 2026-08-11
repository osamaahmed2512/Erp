using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Token
{
    public class TokenConfig
    {
        public string Key { get; set; }
        public double Expiration { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public double RefreshTokenExpirationDays { get; set; }
        public double SessionExpiryTime { get; set; }
    }
}
