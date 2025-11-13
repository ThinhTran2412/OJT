using Grpc.Core;
using IAM_Service.API.Protos;
using Laboratory_Service.Application.DTOs.User;
using Laboratory_Service.Application.Interface;
using Microsoft.Extensions.Logging; 

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

    // MAPPER helper method - Rất quan trọng để tách biệt tầng
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
            userData.Address
        );
    }

    /// <summary>
    /// Gets user information from IAM Service by IdentifyNumber (Citizen ID)
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
    /// Checks if user exists in IAM Service by IdentifyNumber (Citizen ID)
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
    /// Gets multiple users from IAM Service by list of IdentifyNumbers
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

                // Map tất cả proto UserData sang Application/Domain Model
                return response.Users
                        .Select(u => ToUserDataModel(u)) 
                        .ToList();
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
}