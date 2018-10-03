using System;
using System.Collections.Generic;
using System.Text;
using Google.Cloud.Datastore.Driver;
using Google.Cloud.Datastore.Driver.Serialization;

namespace AspNetCore.Identity.GcpDatastore
{
    public interface IDatastoreDbContext<TUser, TRole>
        where TUser : DatastoreEntity
        where TRole : DatastoreEntity
    {
        IDatastoreKind<TUser> User { get; }

        IDatastoreKind<TRole> Role { get; }
    }
}
