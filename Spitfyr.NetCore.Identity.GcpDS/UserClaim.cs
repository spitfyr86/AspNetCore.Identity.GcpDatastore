using System.Security.Claims;

namespace Spitfyr.NetCore.Identity.GcpDS
{
    public class UserClaim
    {
        public UserClaim()
        {

        }
        public UserClaim(string type, string value)
        {
            Type = type;
            Value = value;
        }
        public UserClaim(Claim claim)
        {
            Type = claim.Type;
            Value = claim.Value;
        }

        public string Type { get; set; }
        public string Value { get; set; }

    }
}
