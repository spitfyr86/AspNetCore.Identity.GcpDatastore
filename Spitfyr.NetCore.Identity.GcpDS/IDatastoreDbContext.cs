using Spitfyr.GCP.Datastore.Adapter;
using Spitfyr.GCP.Datastore.Adapter.Serialization;

namespace Spitfyr.NetCore.Identity.GcpDS
{
    public interface IDatastoreDbContext<TUser, TRole>
        where TUser : DatastoreEntity
        where TRole : DatastoreEntity
    {
        IDatastoreKind<TUser> User { get; }

        IDatastoreKind<TRole> Role { get; }
    }
}
