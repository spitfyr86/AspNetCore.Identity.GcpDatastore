using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.Datastore.Adapter;
using Google.Cloud.Datastore.V1;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.GcpDatastore
{
    public class RoleStore<TUser, TRole> : IQueryableRoleStore<TRole>
        where TUser : IdentityUser
        where TRole : IdentityRole
    {
        private readonly IDatastoreDbContext<TUser, TRole> _dbContext;

        public RoleStore(IDatastoreDbContext<TUser, TRole> dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public IQueryable<TRole> Roles => _dbContext.Role.AsQueryable();

        public async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
        {
            await _dbContext.Role.InsertOneAsync(role);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
        {
            //var filter = Builders<TRole>.Filter.Eq(c => c.Id, role.Id);

            await _dbContext.Role.DeleteOneAsync(role.Id);

            return IdentityResult.Success;
        }

        public void Dispose()
        {

        }

        public Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            //var filter = Builders<TRole>.Filter.Eq(c => c.NormalizedName, normalizedRoleName);
            var filter = FilterBuilder<TRole>.Equal(x => x.NormalizedName, normalizedRoleName);

            return Task.FromResult(_dbContext.Role.Find(filter).SingleOrDefault());
        }

        public Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.NormalizedName);
        }

        public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Id.ToString());
        }

        public Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
        {
            return Task.Run(() => role.NormalizedName = normalizedName, cancellationToken);
        }

        public Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
        {
            return Task.Run(() => role.Name = roleName, cancellationToken);
        }

        public async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
        {
            //var filter = Builders<TRole>.Filter.Eq(c => c.Id, role.Id);

            await _dbContext.Role.FindOneAndReplaceAsync(role);

            return IdentityResult.Success;
        }
    }
}
