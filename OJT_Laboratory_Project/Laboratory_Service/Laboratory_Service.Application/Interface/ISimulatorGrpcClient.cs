using Laboratory_Service.Application.DTOs.TestResult;

namespace Laboratory_Service.Application.Interface
{
    /// <summary>
    /// Create methods for interface SimulatorGrpcClient
    /// </summary>
    public interface ISimulatorGrpcClient
    {

        /// <summary>
        /// Gets the raw results asynchronous.
        /// </summary>
        /// <param name="testOrderId">The test order identifier.</param>
        /// <returns></returns>
        Task<RawTestResultDTO?> GetRawResultsAsync(Guid testOrderId);
        /// <summary>
        /// Creates the and get raw results asynchronous.
        /// </summary>
        /// <param name="testOrderId">The test order identifier.</param>
        /// <returns></returns>
        Task<RawTestResultDTO?> CreateAndGetRawResultsAsync(Guid testOrderId);
    }
}

