using MediatR;
using Simulator.Application.Constants;
using Simulator.Application.DTOs;
using Simulator.Application.Interface;

namespace Simulator.Application.SimulateRawData.Command
{
    /// <summary>
    /// Handles the simulate raw data command
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;Simulator.Application.SimulateRawData.Command.SimulateRawDataCommand, Simulator.Application.DTOs.RawTestResultDTO&gt;" />
    public class SimulateRawDataCommandHandler : IRequestHandler<SimulateRawDataCommand, RawTestResultDTO>
    {
        /// <summary>
        /// The random
        /// </summary>
        private readonly Random _random = new Random();
        /// <summary>
        /// The repository
        /// </summary>
        private readonly IRawTestResultRepository _repository;
        /// <summary>
        /// The deviation factor
        /// </summary>
        private const double DeviationFactor = 0.15;
        /// <summary>
        /// Initializes a new instance of the <see cref="SimulateRawDataCommandHandler"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        public SimulateRawDataCommandHandler(IRawTestResultRepository repository)
        {
            _repository = repository;
        }
        /// <summary>
        /// Generates the random value.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <returns></returns>
        private double GenerateRandomValue(TestParameter param)
        {
            double range = param.MaxValue - param.MinValue;
            double effectiveMin = param.MinValue - range * DeviationFactor;
            double effectiveMax = param.MaxValue + range * DeviationFactor;

            effectiveMin = Math.Max(0, effectiveMin);

            double result = _random.NextDouble() * (effectiveMax - effectiveMin) + effectiveMin;

            switch (param.Abbreviation)
            {
                case "WBC":
                case "PLT":
                    return Math.Round(result, 0); 
                case "RBC":
                case "HGB":
                case "HCT":
                    return Math.Round(result, 2); 
                default:
                    return Math.Round(result, 1); 
            }
        }

        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="param">The parameter.</param>
        /// <returns></returns>
        private string GetStatus(double value, TestParameter param)
        {
            if (value < param.MinValue * 0.95) 
                return "Low";
            if (value > param.MaxValue * 1.05) 
                return "High";

            return "Normal"; 
        }


        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<RawTestResultDTO> Handle(SimulateRawDataCommand request, CancellationToken cancellationToken)
        {
            Guid testOrderId = request.TestOrderId;

            string instrument = $"Sysmex-XN-{_random.Next(100, 999)}";
            DateTime performedDate = DateTime.UtcNow.AddMinutes(-_random.Next(1, 60));

            var rawResultItems = new List<RawResultItemDTO>();

            foreach (var param in TestParameterData.Parameters)
            {
                double value = GenerateRandomValue(param);
                string status = GetStatus(value, param);

                var item = new RawResultItemDTO
                {
                    TestCode = param.Abbreviation,
                    Parameter = param.Parameter,
                    ValueNumeric = value,
                    ValueText = value.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    Unit = param.Unit,
                    ReferenceRange = param.NormalRange,
                    Status = status
                };

                rawResultItems.Add(item);
            }

            var rawTestResultDto = new RawTestResultDTO
            {
                TestOrderId = testOrderId, 
                Instrument = instrument,
                PerformedDate = performedDate,
                Results = rawResultItems
            };

            await _repository.AddRangeAsync(new List<RawTestResultDTO> { rawTestResultDto });

            return rawTestResultDto;
        }
    }
}
