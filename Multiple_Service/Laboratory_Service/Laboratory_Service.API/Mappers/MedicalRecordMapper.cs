using Laboratory_Service.API.Protos;
using Laboratory_Service.Application.DTOs.MedicalRecords;

namespace Laboratory_Service.API.Mappers
{
    public static class MedicalRecordMapper
    {
        // Map from MedicalRecordDto to gRPC model so API works with DTOs instead of domain entities
        public static MedicalRecordData ToGrpcModel(this MedicalRecordDto dto)
        {
            if (dto == null)
                return null;

            return new MedicalRecordData
            {
                MedicalRecordId = dto.MedicalRecordId,
                PatientId = dto.PatientId,
                IdentifyNumber = dto.PatientName ?? string.Empty,
                TestType = dto.TestType,
                TestResults = dto.TestResults,
                ReferenceRanges = dto.ReferenceRanges,
                Interpretation = dto.Interpretation,
                InstrumentUsed = dto.InstrumentUsed,
                BatchNumber = dto.BatchNumber,
                LotNumber = dto.LotNumber,
                ClinicalNotes = dto.ClinicalNotes,
                ErrorMessages = { dto.ErrorMessages },
                TestDate = dto.TestDate.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                ResultsDate = dto.ResultsDate?.ToString("yyyy-MM-ddTHH:mm:ssZ") ?? string.Empty,
                Status = dto.Status,
                Priority = dto.Priority,
                CreatedAt = dto.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                UpdatedAt = string.Empty,
                CreatedBy = dto.CreatedBy,
                UpdatedBy = string.Empty
            };
        }
    }
}