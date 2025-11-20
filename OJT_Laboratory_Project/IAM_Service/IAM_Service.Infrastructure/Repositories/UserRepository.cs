using IAM_Service.Application.Interface.IEncryptionService;
using IAM_Service.Application.Interface.IUser;
using IAM_Service.Domain.Entity;
using IAM_Service.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for managing user data.
/// </summary>
namespace IAM_Service.Infrastructure.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="IAM_Service.Application.Interface.IUser.IUsersRepository" />
    public class UserRepository : IUsersRepository
    {
        /// <summary>
        /// The database context for accessing user data.
        /// </summary>
        private readonly AppDbContext _dbContext;
        /// <summary>
        /// Constructor for the UserRepository.
        /// </summary>
        private readonly IEncryptionService _encryptionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository" /> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="encryptionService">The encryption service.</param>
        public UserRepository(AppDbContext dbContext, IEncryptionService encryptionService)
        {
            _dbContext = dbContext;
            _encryptionService = encryptionService;
        }

        /// <summary>
        /// Gets the by email asynchronous.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public async Task<User> GetByEmailAsync(string email) =>
           await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        /// <summary>
        /// Retrieves a user by their password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public async Task<User> GetByPasswordAsync(string password) =>
            await _dbContext.Users.FirstOrDefaultAsync(u => u.Password == password);

        /// <summary>
        /// Creates a new user in the database.
        /// </summary>
        /// <param name="user">The user.</param>

        public async Task CreateUser(User user)
        {
            if (!string.IsNullOrEmpty(user.IdentifyNumber))
                user.IdentifyNumber = _encryptionService.Encrypt(user.IdentifyNumber);

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
        }


        /// <summary>
        /// Updates an existing user in the database.
        /// </summary>
        /// <param name="user">The user.</param>
        public async Task UpdateAsync(User user)
        {
            _dbContext.Users.Update(user);
            await Task.CompletedTask;
        }


        /// <summary>
        /// Gets user by ID using IUserRepository interface
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _dbContext.Users.FindAsync(id);
        }

        /// <summary>
        /// Saves changes to the database.
        /// </summary>
        /// <returns></returns>
        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Gets user by identify number
        /// </summary>
        /// <param name="identifyNumbers">The identify numbers.</param>
        /// <returns></returns>
        public async Task<IEnumerable<User>> GetAllUsersByIdentifyNumbersAsync(List<string> identifyNumbers)
        {
            if (identifyNumbers == null || !identifyNumbers.Any())
            {
                return Enumerable.Empty<User>();
            }
            var encryptedIdentifyNumbers = identifyNumbers.Select(_encryptionService.Encrypt).ToList();
            return await _dbContext.Users
                .Include(u => u.Role)
                .Where(u => encryptedIdentifyNumbers.Contains(u.IdentifyNumber))
                .ToListAsync();
        }

        /// <summary>
        /// Checks if a user exists by identify number
        /// </summary>
        /// <param name="identifyNumber">The identify number.</param>
        /// <returns></returns>
        public async Task<bool> CheckUserExistsByIdentifyNumberAsync(string identifyNumber)
        {
            var encryptedIdentifyNumber = _encryptionService.Encrypt(identifyNumber);
            return await _dbContext.Users.AnyAsync(u => u.IdentifyNumber == encryptedIdentifyNumber);
        }

        /// <summary>
        /// Gets the user asynchronous.
        /// </summary>
        /// <returns></returns>
        public async Task<List<User>> GetUserAsync()
        {
            var user = await _dbContext.Users.ToListAsync();
            foreach (var users in user)
            {
                users.IdentifyNumber = _encryptionService.Decrypt(users.IdentifyNumber);
            }
            return await _dbContext.Users.ToListAsync();
        }


        /// <summary>
        /// Gets the users by identify numbers asynchronous.
        /// </summary>
        /// <param name="identifyNumbers">The identify numbers.</param>
        /// <returns></returns>
        public async Task<User?> GetUsersByIdentifyNumbersAsync(string identifyNumbers)
        {
            var encryptedIdentifyNumber = _encryptionService.Encrypt(identifyNumbers);
            return await _dbContext.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.IdentifyNumber == encryptedIdentifyNumber);

        }
        /// <summary>
        /// Gets the user by identifier.
        /// </summary>
        /// <param name="id">The user identifier.</param>
        /// <returns>
        /// The user if found; otherwise, null.
        /// </returns>
        /// <!-- Badly formed XML comment ignored for member "M:IAM_Service.Application.Interface.IUser.IUsersRepository.GetByIdAsync(System.Int32)" -->
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _dbContext.Users.FindAsync(id);
        }

        /// <summary>
        /// Updates the user asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        public async Task UpdateUserAsync(User? user)
        {
            if (!string.IsNullOrEmpty(user.IdentifyNumber))
                user.IdentifyNumber = _encryptionService.Encrypt(user.IdentifyNumber);

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
        }
        /// <summary>
        /// Permanently deletes a user from the database.
        /// </summary>
        /// <param name="user">The user.</param>
        public async Task DeleteAsync(User user)
        {
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
        }
        /// <summary>
        /// Retrieves a user by their Id.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public async Task<User?> GetByUserIdAsync(int userId)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user != null && !string.IsNullOrEmpty(user.IdentifyNumber))
                user.IdentifyNumber = _encryptionService.Decrypt(user.IdentifyNumber);
            return user;
        }

        /// <summary>
        /// Retrieves a user by their UserId.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public async Task<IEnumerable<int>> GetUserPrivilegesAsync(int userId)
        {
            return await _dbContext.UserPrivileges
                .AsNoTracking()
                .Where(up => up.UserId == userId)
                .Select(up => up.PrivilegeId)
                .ToListAsync();
        }

        /// <summary>
        /// Adds the user privileges asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="privilegeIds">The privilege ids.</param>
        public async Task AddUserPrivilegesAsync(int userId, List<int> privilegeIds)
        {
            var newPrivileges = privilegeIds.Select(pid => new UserPrivilege
            {
                UserId = userId,
                PrivilegeId = pid
            });

            await _dbContext.UserPrivileges.AddRangeAsync(newPrivileges);
        }

        /// <summary>
        /// Removes the user privileges asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="privilegeIds">The privilege ids.</param>
        public async Task RemoveUserPrivilegesAsync(int userId, List<int> privilegeIds)
        {
            var privilegesToRemove = await _dbContext.UserPrivileges
                .Where(up => up.UserId == userId && privilegeIds.Contains(up.PrivilegeId))
                .ToListAsync();

            _dbContext.UserPrivileges.RemoveRange(privilegesToRemove);
        }


        /// <summary>
        /// Gets the original privileges asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public async Task<List<int>> GetOriginalPrivilegesAsync(int userId)
        {
            var rolePrivileges = await _dbContext.RolePrivileges
                .Where(rp => rp.RoleId == _dbContext.Users
                    .Where(u => u.UserId == userId)
                    .Select(u => u.RoleId)
                    .FirstOrDefault())
                .Select(rp => rp.PrivilegeId)
                .ToListAsync();

            return rolePrivileges;
        }
    }
}
