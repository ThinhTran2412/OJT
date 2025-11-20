using Grpc.Core;
using Grpc.Net.Client;
using IAM_Service.Application.DTOs;
using IAM_Service.Application.Interface.IPatientClient;
using Laboratory_Service.API.Protos;
using Microsoft.Extensions.Configuration;

namespace IAM_Service.Infrastructure.Services
{
    /// <summary>
    /// Create handle for PatientGrpcClientService
    /// </summary>
    /// <seealso cref="IAM_Service.Application.Interface.IPatientClient.IPatientService" />
    public class PatientGrpcClientService : IPatientService
    {
        /// <summary>
        /// The client
        /// </summary>
        private readonly PatientService.PatientServiceClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="PatientGrpcClientService"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <exception cref="System.ArgumentException">LaboratoryServiceUrl configuration is missing</exception>
        public PatientGrpcClientService(IConfiguration configuration)
        {
            var grpcUrl = configuration["GrpcSettings:LaboratoryServiceUrl"]
                ?? throw new ArgumentException("LaboratoryServiceUrl configuration is missing");

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var channel = GrpcChannel.ForAddress(grpcUrl);
            _client = new PatientService.PatientServiceClient(channel);
        }

        /// <summary>
        /// Gets the patient by identity number asynchronous.
        /// </summary>
        /// <param name="identityNumber">The identity number.</param>
        /// <returns></returns>
        /// <exception cref="System.ApplicationException">Failed to communicate with Laboratory Service: {ex.Message}</exception>
        public async Task<PatientDetailDto?> GetPatientByIdentityNumberAsync(string identityNumber)
        {
            try
            {
                var request = new GetPatientByIdentifyNumberRequest { IdentifyNumber = identityNumber };
                var response = await _client.GetPatientByIdentifyNumberAsync(request);

                if (!response.Success || response.Patient == null)
                {
                    return null;
                }

                return MapToPatientDetailDto(response.Patient);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                return null;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to communicate with Laboratory Service: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Maps to patient detail dto.
        /// </summary>
        /// <param name="patientData">The patient data.</param>
        /// <returns></returns>
        private PatientDetailDto MapToPatientDetailDto(PatientData patientData)
        {
            DateOnly? SafeParseDateOnly(string? dateString)
            {
                if (string.IsNullOrEmpty(dateString)) return null;
                if (DateOnly.TryParseExact(dateString, "yyyy-MM-dd", out var result))
                {
                    return result;
                }
                return null;
            }

            DateTimeOffset? SafeParseDateTimeOffset(string? dateTimeString)
            {
                if (string.IsNullOrEmpty(dateTimeString)) return null;
                if (DateTimeOffset.TryParse(dateTimeString, out var result))
                {
                    return result;
                }
                return null;
            }

            return new PatientDetailDto
            {
                PatientId = patientData.PatientId,
                IdentifyNumber = patientData.IdentifyNumber ?? string.Empty,
                FullName = patientData.FullName ?? string.Empty,
                PhoneNumber = patientData.PhoneNumber ?? string.Empty,
                Email = patientData.Email ?? string.Empty,
                Gender = patientData.Gender ?? string.Empty,
                Address = patientData.Address ?? string.Empty,
                Age = patientData.Age,
                DateOfBirth = SafeParseDateOnly(patientData.DateOfBirth),
            };
        }


    }
}