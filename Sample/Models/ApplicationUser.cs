using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.Models
{
    public class ApplicationUser : AspNetCore.Identity.GcpDatastore.IdentityUser
    {
    }
}
