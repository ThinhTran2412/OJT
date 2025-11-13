using AutoMapper;
using Laboratory_Service.Application.DTOs.TestOrders;
using Laboratory_Service.Application.Interface;
using MediatR;

namespace Laboratory_Service.Application.TestOrders.Queries
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;Laboratory_Service.Application.TestOrders.Queries.GetTestOrderDetailQuery, Laboratory_Service.Application.DTOs.TestOrders.TestOrderDetailDto&gt;" />
    public class GetTestOrderDetailQueryHandler : IRequestHandler<GetTestOrderDetailQuery, TestOrderDetailDto>
    {

        /// <summary>
        /// The test order repository
        /// </summary>
        private readonly ITestOrderRepository _testOrderRepository;
        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;


        /// <summary>
        /// Initializes a new instance of the <see cref="GetTestOrderDetailQueryHandler"/> class.
        /// </summary>
        /// <param name="testOrderRepository">The test order repository.</param>
        /// <param name="mapper">The mapper.</param>
        public GetTestOrderDetailQueryHandler(ITestOrderRepository testOrderRepository, IMapper mapper)
        {
            _testOrderRepository = testOrderRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<TestOrderDetailDto?> Handle(GetTestOrderDetailQuery request, CancellationToken cancellationToken)
        {
            // 1) Query repository: get test order with related entities (Patient, MedicalRecord, TestResults)
            var order = await _testOrderRepository.GetByIdAsync(request.TestOrderId, cancellationToken);

            // 2) If not found, return null (controller will map to NotFound)
            if (order == null)
            {
                return null;
            }

            // 3) Map entity -> DTO. Patient information comes from TestOrder/Patient.
            var dto = new TestOrderDetailDto
            {
                TestOrderId = order.TestOrderId,
                PatientId = order.PatientId,

                // Patient Information
                PatientName = order.PatientName,
                Age = order.Age,
                Gender = order.Gender,
                PhoneNumber = order.PhoneNumber,
                IdentifyNumber = order.Patient?.IdentifyNumber ?? string.Empty,

                // Test Order Details
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                RunDate = order.RunDate,
                TestType = order.TestType
            };

             // 4) Map TestResults (list)
            // if (order.TestResults != null && order.TestResults.Any())
            // {
            //     dto.TestResults = order.TestResults.Select(tr => new TestResultDto
            //     {
            //         TestResultId = tr.TestResultId,
            //         TestCode = tr.TestCode,
            //         Parameter = tr.Parameter,
            //         ValueNumeric = tr.ValueNumeric,
            //         ValueText = tr.ValueText,
            //         Unit = tr.Unit,
            //         ReferenceRange = tr.ReferenceRange,
            //         Instrument = tr.Instrument,
            //         PerformedDate = tr.PerformedDate,
            //         ResultStatus = tr.ResultStatus,
            //         PerformedByUserId = tr.PerformedByUserId,
            //         ReviewedByUserId = tr.ReviewedByUserId,
            //         ReviewedDate = tr.ReviewedDate
            //     }).ToList();
            // }

            return dto;
        }
    }
}
