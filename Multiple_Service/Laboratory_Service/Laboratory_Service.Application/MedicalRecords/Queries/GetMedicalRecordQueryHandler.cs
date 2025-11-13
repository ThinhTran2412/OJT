using Laboratory_Service.Application.DTOs.MedicalRecords;
using Laboratory_Service.Application.Interface;
using MediatR;

namespace Laboratory_Service.Application.MedicalRecords.Queries
{
    /// <summary>
    /// Create GetAllMedicalRecordsQueryHandler
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;Laboratory_Service.Application.MedicalRecords.Queries.GetAllMedicalRecordsQuery, System.Collections.Generic.List&lt;Laboratory_Service.Application.DTOs.MedicalRecords.MedicalRecordViewDto&gt;&gt;" />
    public class GetAllMedicalRecordsQueryHandler : IRequestHandler<GetAllMedicalRecordsQuery, List<MedicalRecordViewDto>>
    {
        /// <summary>
        /// The repository
        /// </summary>
        private readonly IMedicalRecordRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllMedicalRecordsQueryHandler"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        public GetAllMedicalRecordsQueryHandler(IMedicalRecordRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<List<MedicalRecordViewDto>> Handle(GetAllMedicalRecordsQuery request, CancellationToken cancellationToken)
        {
            var records = await _repository.GetAllAsync();

            return records.Select(record => new MedicalRecordViewDto
            {
                MedicalRecordId = record.MedicalRecordId,
                CreatedAt = record.CreatedAt,
                UpdatedAt = record.UpdatedAt,
                CreatedBy = record.CreatedBy,
                UpdatedBy = record.UpdatedBy,
                PatientName = record.Patient.FullName,
                DateOfBirth = record.Patient.DateOfBirth,
                Age = record.Patient.Age,
                Gender = record.Patient.Gender,
                Address = record.Patient.Address,
                PhoneNumber = record.Patient.PhoneNumber,
                Email = record.Patient.Email,
                TestOrders = record.Patient.TestOrders
                    .Where(t => !t.IsDeleted)
                    .Select(t => new TestOrderDto
                    {
                        TestOrderId = t.TestOrderId,
                        OrderCode = t.OrderCode,
                        Priority = t.Priority,
                        Status = t.Status,
                        TestType = t.TestType,
                        TestResults = t.TestResults,
                        RunDate = t.RunDate,
                        RunBy = t.RunBy
                    }).ToList()
            }).ToList();
        }
    }
}
