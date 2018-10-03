using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Identity.GcpDatastore
{
    public class AuthToken
    {
        public string LoginProvider { get; set; }
        public string Token { get; set; }

        public string Name { get; set; }
    }
}
