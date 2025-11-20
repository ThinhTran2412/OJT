using Grpc.Core;
using IAM_Service.API.Protos;
using Laboratory_Service.Application.DTOs.User;
using Laboratory_Service.Application.Interface;
using Microsoft.Extensions.Logging;

namespace Laboratory_Service.Infrastructure.GrpcClients
{
    /// <summary>
    /// gRPC Client to communicate with IAM Service (User-related operations).
    /// </summary>
    public class IamUserGrpcClientService : IIamUserService
    {
        private readonly UserService.UserServiceClient _userServiceClient;
        private readonly ILogger<IamUserGrpcClientService> _logger;

        public IamUserGrpcClientService(
            UserService.UserServiceClient userServiceClient,
            ILogger<IamUserGrpcClientService> logger)
        {
            _userServiceClient = userServiceClient;
            _logger = logger;
        }

        #region Mapper
        private UserDataDTO ToUserDataModel(UserData userData)
        {
            const string dateFormat = "yyyy-MM-dd";
            if (!DateOnly.TryParseExact(userData.DateOfBirth, dateFormat, out DateOnly dob))
            {
                throw new FormatException($"Invalid date format from IAM Service: {userData.DateOfBirth}.");
            }

            return new UserDataDTO(
                userData.IdentifyNumber,
                userData.FullName,
                dob,
                userData.Gender,
                userData.PhoneNumber,
                userData.Email,
                userData.Address,
                userData.RoleId,
                userData.Age
            );
        }
        #endregion

        /// <summary>
        /// Gets user information from IAM Service by IdentifyNumber (Citizen ID).
        /// </summary>
        public async Task<UserDataDTO?> GetUserByIdentifyNumberAsync(string identifyNumber)
        {
            try
            {
                _logger.LogInformation("Getting user from IAM Service by IdentifyNumber: {IdentifyNumber}", identifyNumber);

                var request = new GetUserByIdentifyNumberRequest { IdentifyNumber = identifyNumber };
                var response = await _userServiceClient.GetUserByIdentifyNumberAsync(request);

                if (response.Success && response.User != null)
                {
                    _logger.LogInformation("Successfully retrieved user from IAM Service");
                    return ToUserDataModel(response.User);
                }

                _logger.LogWarning("Failed to get user from IAM Service: {Message}", response.Message ?? "No message");
                return null;
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "gRPC error getting user by IdentifyNumber: {IdentifyNumber}", identifyNumber);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by IdentifyNumber: {IdentifyNumber}", identifyNumber);
                return null;
            }
        }

        /// <summary>
        /// Checks if user exists in IAM Service by IdentifyNumber (Citizen ID).
        /// </summary>
        public async Task<bool> CheckUserExistsAsync(string identifyNumber)
        {
            try
            {
                _logger.LogInformation("Checking user existence in IAM Service by IdentifyNumber: {IdentifyNumber}", identifyNumber);

                var request = new CheckUserExistsRequest { IdentifyNumber = identifyNumber };
                var response = await _userServiceClient.CheckUserExistsAsync(request);

                if (response.Success)
                {
                    _logger.LogInformation("User existence check completed: {Exists}", response.Exists);
                    return response.Exists;
                }

                _logger.LogWarning("Failed to check user existence in IAM Service: {Message}", response.Message ?? "No message");
                return false;
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "gRPC error checking user existence by IdentifyNumber: {IdentifyNumber}", identifyNumber);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking user existence by IdentifyNumber: {IdentifyNumber}", identifyNumber);
                return false;
            }
        }

        /// <summary>
        /// Gets multiple users from IAM Service by list of IdentifyNumbers.
        /// </summary>
        public async Task<List<UserDataDTO>> GetUsersByIdentifyNumbersAsync(List<string> identifyNumbers)
        {
            try
            {
                _logger.LogInformation("Getting multiple users from IAM Service. Count: {Count}", identifyNumbers.Count);

                var request = new GetUsersByIdentifyNumbersRequest();
                request.IdentifyNumbers.AddRange(identifyNumbers);

                var response = await _userServiceClient.GetUsersByIdentifyNumbersAsync(request);

                if (response.Success)
                {
                    _logger.LogInformation("Successfully retrieved {Count} users from IAM Service", response.Users.Count);
                    return response.Users.Select(u => ToUserDataModel(u)).ToList();
                }

                _logger.LogWarning("Failed to get users from IAM Service: {Message}", response.Message ?? "No message");
                return new List<UserDataDTO>();
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "gRPC error getting multiple users from IAM Service");
                return new List<UserDataDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting multiple users from IAM Service");
                return new List<UserDataDTO>();
            }
        }

        /// <summary>
        /// Creates a new user in IAM Service using gRPC.
        /// </summary>
        public async Task<bool> CreateUserAsync(UserDataDTO userDetails)
        {
            try
            {
                _logger.LogInformation("Attempting to create user in IAM Service for IdentifyNumber: {IdentifyNumber}", userDetails.IdentifyNumber);

                // 1️⃣ Prepare gRPC Request
                var request = new CreateUserRequest
                {
                    IdentifyNumber = userDetails.IdentifyNumber,
                    FullName = userDetails.FullName,
                    DateOfBirth = userDetails.DateOfBirth.ToString("yyyy-MM-dd"),
                    Gender = userDetails.Gender,
                    PhoneNumber = userDetails.PhoneNumber,
                    Email = userDetails.Email,
                    Address = userDetails.Address,
                    RoleId = userDetails.RoleId ?? 5,
                    Age = userDetails.Age
                };

                // 2️⃣ Call gRPC Service
                var response = await _userServiceClient.CreateUserAsync(request);

                // 3️⃣ Handle response
                if (response.Success)
                {
                    _logger.LogInformation("Successfully created user in IAM Service: {IdentifyNumber}", userDetails.IdentifyNumber);
                    return true;
                }

                _logger.LogWarning("Failed to create user in IAM Service: {Message}", response.Message ?? "No message");
                return false;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.AlreadyExists)
            {
                _logger.LogWarning(ex, "User with IdentifyNumber {IdentifyNumber} already exists in IAM Service.", userDetails.IdentifyNumber);
                return false;
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "gRPC error creating user for IdentifyNumber: {IdentifyNumber}", userDetails.IdentifyNumber);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating user for IdentifyNumber: {IdentifyNumber}", userDetails.IdentifyNumber);
                return false;
            }
        }
    }
}
