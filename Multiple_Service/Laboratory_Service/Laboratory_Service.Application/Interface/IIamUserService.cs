using Laboratory_Service.Application.DTOs.User;

namespace Laboratory_Service.Application.Interface
{
    /// <summary>
    /// Create methods for interface IamUserService
    /// </summary>
    public interface IIamUserService
    {
        /// <summary>
        /// Gets the user by identify number asynchronous.
        /// </summary>
        /// <param name="identifyNumber">The identify number.</param>
        /// <returns></returns>
        Task<UserDataDTO?> GetUserByIdentifyNumberAsync(string identifyNumber);
        /// <summary>
        /// Checks the user exists asynchronous.
        /// </summary>
        /// <param name="identifyNumber">The identify number.</param>
        /// <returns></returns>
        Task<bool> CheckUserExistsAsync(string identifyNumber);
        /// <summary>
        /// Gets the users by identify numbers asynchronous.
        /// </summary>
        /// <param name="identifyNumbers">The identify numbers.</param>
        /// <returns></returns>
        Task<List<UserDataDTO>> GetUsersByIdentifyNumbersAsync(List<string> identifyNumbers);
    }
}
