using AspNetCore.Identity.GcpDatastore;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Datastore.Driver;
using Google.Cloud.Datastore.V1;
using Grpc.Auth;
using Microsoft.AspNetCore.Identity;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection ConfigureDatastoreDbOption(this IServiceCollection services, Action<GcpDatastoreOption> configure)
        {
            services.Configure(configure);

            return services;
        }

        public static IServiceCollection AddGcpDatastoreDatabase(this IServiceCollection services)
        {
            services.AddTransient(provider =>
            {
                var options = provider.GetService<IOptions<GcpDatastoreOption>>();

                using (var stream = new FileStream(options.Value.CredentialsFilePath, FileMode.Open))
                {
                    var googleCredential = GoogleCredential.FromStream(stream);
                    var channel = new Grpc.Core.Channel(
                        DatastoreClient.DefaultEndpoint.Host,
                        googleCredential.ToChannelCredentials());
                    
                    var client = DatastoreClient.Create(channel);
                    var datastoreDb = DatastoreDb.Create(options.Value.ProjectId, options.Value.Namespace, client);
                    IDatastoreDatabase database = new DatastoreDatabase(datastoreDb);
                    return database;
                }
            });

            return services;
        }

        public static IServiceCollection AddGcpDatastoreDbContext<TUser, TRole>(this IServiceCollection services)
            where TUser : IdentityUser
            where TRole : IdentityRole
        {
            services.AddTransient<IDatastoreDbContext<TUser, TRole>, DatastoreDbContext<TUser, TRole>>();

            return services;
        }

        public static IServiceCollection AddGcpDatastoreStore<TUser, TRole>(this IServiceCollection services)
            where TUser : IdentityUser
            where TRole : IdentityRole
        {
            services.AddTransient<IUserStore<TUser>, UserStore<TUser, TRole>>();
            services.AddTransient<IRoleStore<TRole>, RoleStore<TUser, TRole>>();
            
            return services;
        }
    }
}
