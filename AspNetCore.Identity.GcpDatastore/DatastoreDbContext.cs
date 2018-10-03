using System;
using System.Collections.Generic;
using System.Text;
using Google.Cloud.Datastore.Adapter;
using Microsoft.Extensions.Options;

namespace AspNetCore.Identity.GcpDatastore
{
    public class DatastoreDbContext<TUser, TRole> : IDatastoreDbContext<TUser, TRole>
        where TUser : IdentityUser
        where TRole : IdentityRole
    {
        public DatastoreDbContext(IDatastoreDatabase database, IOptions<GcpDatastoreOption> option)
        {
            User = database.GetKind<TUser>(option.Value.User.Kind);
            Role = database.GetKind<TRole>(option.Value.Role.Kind);
        }

        public IDatastoreKind<TUser> User { get; }
        public IDatastoreKind<TRole> Role { get; }
    }
}
