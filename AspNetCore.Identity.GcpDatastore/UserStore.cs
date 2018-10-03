using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.Datastore.Driver;
using Google.Cloud.Datastore.V1;

namespace AspNetCore.Identity.GcpDatastore
{
    public class UserStore<TUser, TRole> :
        IUserLoginStore<TUser>,
        IUserClaimStore<TUser>,
        IUserPasswordStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IUserEmailStore<TUser>,
        IUserLockoutStore<TUser>,
        IUserPhoneNumberStore<TUser>,
        IQueryableUserStore<TUser>,
        IUserAuthenticationTokenStore<TUser>,
        IUserTwoFactorStore<TUser>,
        IUserRoleStore<TUser>

        where TUser : IdentityUser
        where TRole : IdentityRole
    {
        private readonly IDatastoreDbContext<TUser, TRole> _dbContext;

        public UserStore(IDatastoreDbContext<TUser, TRole> dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public IQueryable<TUser> Users => _dbContext.User.AsQueryable<TUser>();

        #region IUserStore implementations

        public async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
        {
            await _dbContext.User.InsertOneAsync(user);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            await _dbContext.User.DeleteOneAsync(user.Id);
            return IdentityResult.Success;
        }

        public void Dispose()
        {
            // Nothing to do here
        }

        public Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return _dbContext.User.GetAsync(Convert.ToInt64(userId));
        }

        public Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var filter = FilterBuilder<TUser>.Equal(x => x.NormalizedName, normalizedUserName);

            return Task.FromResult(_dbContext.User.Find(filter).SingleOrDefault());
        }

        public Task<IList<Microsoft.AspNetCore.Identity.UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken)
        {
            var list = user.Logins
                .Select(c => new Microsoft.AspNetCore.Identity.UserLoginInfo(c.LoginProvider, c.ProviderKey, c.ProviderDisplayName))
                .ToList();

            return Task.FromResult<IList<Microsoft.AspNetCore.Identity.UserLoginInfo>>(list);
        }

        public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedName);
        }

        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            return Task.Run(() => user.RemoveLogin(loginProvider, providerKey), cancellationToken);
        }

        public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
        {
            return Task.Run(() => user.NormalizedName = normalizedName, cancellationToken);
        }

        public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
        {
            return Task.Run(() => user.UserName = userName, cancellationToken);
        }

        public async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            await _dbContext.User.FindOneAndReplaceAsync(user);
            return IdentityResult.Success;
        }
        #endregion


        #region IUserLoginStore implementations

        public Task AddLoginAsync(TUser user, Microsoft.AspNetCore.Identity.UserLoginInfo login, CancellationToken cancellationToken)
        {
            return Task.Run(() => user.AddLogin(new UserLoginInfo
            {
                LoginProvider = login.LoginProvider,
                ProviderDisplayName = login.ProviderDisplayName,
                ProviderKey = login.ProviderKey,
            }), cancellationToken);
        }

        public Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Claims.Select(c => new Claim(c.Type, c.Value)).ToList() as IList<Claim>);
        }
        #endregion


        #region IUserClaimStore implementations

        public Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            return Task.Run(() => user.AddClaims(claims.Select(c => new UserClaim
            {
                Type = c.Type,
                Value = c.Value
            })), cancellationToken);
        }

        public Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            return Task.Run(() => user.ReplaceClaim(new UserClaim(claim), new UserClaim(newClaim)), cancellationToken);
        }

        public Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            return Task.Run(() => user.RemoveClaims(claims.Select(c => new UserClaim
            {
                Type = c.Type,
                Value = c.Value
            })), cancellationToken);
        }

        public Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        #endregion


        #region IUserPasswordStore implementations

        public Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken)
        {
            return Task.Run(() => user.PasswordHash = passwordHash, cancellationToken);
        }

        public Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(!string.IsNullOrWhiteSpace(user.PasswordHash));
        }
        #endregion


        #region IUserSecurityStampStore implementations

        public Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken)
        {
            return Task.Run(() => user.SecurityStamp = stamp, cancellationToken);
        }

        public Task<string> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.SecurityStamp);
        }
        #endregion


        #region IUserEmailStore implementations

        public Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken)
        {
            return Task.Run(() => user.Email = email, cancellationToken);
        }

        public Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            return Task.Run(() => user.EmailConfirmed = confirmed, cancellationToken);
        }

        public Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            return Task.Run(() => user.NormalizedEmail = normalizedEmail, cancellationToken);
        }
        #endregion


        #region IUserLockoutStore implementations

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.LockoutEndDate);
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            return Task.Run(() => user.LockoutEndDate = lockoutEnd, cancellationToken);
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.Run(() => user.AccessFailedCount++, cancellationToken);
        }

        public Task ResetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.Run(() => user.AccessFailedCount = 0, cancellationToken);
        }

        public Task<int> GetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.LockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            return Task.Run(() => user.LockoutEnabled = enabled, cancellationToken);
        }
        #endregion


        #region IUserPhoneNumberStore implementations

        public Task SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            return Task.Run(() => user.PhoneNumber = phoneNumber, cancellationToken);
        }

        public Task<string> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            return Task.Run(() => user.PhoneNumberConfirmed = confirmed, cancellationToken);
        }
        #endregion


        #region IUserAuthenticationTokenStore implementations

        public Task SetTokenAsync(TUser user, string loginProvider, string name, string value, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var authToken = user.AuthTokens.SingleOrDefault(t => t.LoginProvider == loginProvider && t.Name == name);
                if (authToken == null)
                    user.AddToken(new AuthToken
                    {
                        Token = value,
                        Name = name,
                        LoginProvider = loginProvider
                    });
                else
                    authToken.Token = value;
            }, cancellationToken);
        }

        public Task RemoveTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            return Task.Run(() => user.RemoveToken(loginProvider, name), cancellationToken);
        }

        public Task<string> GetTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            var authToken = user.AuthTokens.SingleOrDefault(t => t.LoginProvider == loginProvider && t.Name == name);
            return Task.FromResult(authToken?.Token);
        }
        #endregion


        #region IUserTwoFactorStore implementations

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            return Task.Run(() => user.TwoFactorEnabled = enabled, cancellationToken);
        }

        public Task<bool> GetTwoFactorEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }
        #endregion


        #region IUserRoleStore implementations

        public async Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            var filter = FilterBuilder<TUser>.Equal(r => r.NormalizedName, roleName);

            var roleEntities = await _dbContext.Role.FindAsync(filter);
            var roleEntity = roleEntities.SingleOrDefault();

            if (roleEntity == null)
            {
                throw new InvalidOperationException($"Role '${roleName}' not found");
            }

            user.AddRole(roleName);
        }

        public Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            return Task.Run(() => user.RemoveRole(roleName), cancellationToken);
        }

        public Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult<IList<string>>(user.Roles);
        }

        public Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            return Task.Run(() => user.Roles.Contains(roleName), cancellationToken);
        }

        public Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
