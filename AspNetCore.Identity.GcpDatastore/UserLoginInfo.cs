using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Identity.GcpDatastore
{
    public class UserLoginInfo
    {
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public string ProviderDisplayName { get; set; }
    }
}
