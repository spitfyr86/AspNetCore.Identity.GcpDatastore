using Microsoft.Extensions.Options;
using Spitfyr.GCP.Datastore.Adapter;

namespace Spitfyr.NetCore.Identity.GcpDS
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
