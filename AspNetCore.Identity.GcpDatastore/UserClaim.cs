﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Identity.GcpDatastore
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
        public UserClaim(System.Security.Claims.Claim claim)
        {
            Type = claim.Type;
            Value = claim.Value;
        }

        public string Type { get; set; }
        public string Value { get; set; }

    }
}
