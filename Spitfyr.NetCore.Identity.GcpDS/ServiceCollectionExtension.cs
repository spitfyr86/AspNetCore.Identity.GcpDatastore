﻿using System;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Datastore.V1;
using Grpc.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Spitfyr.GCP.Datastore.Adapter;

namespace Spitfyr.NetCore.Identity.GcpDS
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection ConfigureDatastoreDbOption(this IServiceCollection services, Action<DatastoreOption> configure)
        {
            services.Configure(configure);

            return services;
        }

        public static IServiceCollection AddGcpDatastoreDatabase(this IServiceCollection services, DatastoreOption dsOption)
        {
            services.AddSingleton(provider =>
            {
                //var option = provider.GetService<IOptions<DatastoreOption>>();
                
                var googleCredential = GoogleCredential.FromFile(dsOption.CredentialsFilePath);
                var channel = new Grpc.Core.Channel(
                    DatastoreClient.DefaultEndpoint.Host,
                    googleCredential.ToChannelCredentials());

                var client = DatastoreClient.Create(channel);
                var datastoreDb = DatastoreDb.Create(dsOption.ProjectId, dsOption.Namespace, client);
                IDatastoreDatabase database = new DatastoreDatabase(datastoreDb, dsOption.EntityPrefix);
                return database;
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
