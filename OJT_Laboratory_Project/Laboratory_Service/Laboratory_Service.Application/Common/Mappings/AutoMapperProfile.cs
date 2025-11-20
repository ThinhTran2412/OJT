using AutoMapper;
using Laboratory_Service.Application.DTOs.Comment;
using Laboratory_Service.Application.DTOs.MedicalRecords;
using Laboratory_Service.Application.DTOs.Patients;
using Laboratory_Service.Application.DTOs.User;
using Laboratory_Service.Domain.Entity;
using DomainComment = Laboratory_Service.Domain.Entity.Comment;

/// <summary>
/// AutoMapper profile for mapping between domain entities and DTOs.
/// </summary>
namespace Laboratory_Service.Application.Common.Mappings
{
    /// <summary>
    /// Configuration profile for AutoMapper.
    ///     </summary>
    public class AutoMapperProfile : Profile
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMapperProfile"/> class.
        ///     </summary>
        public AutoMapperProfile()
        {
            // Patient mappings
            CreateMap<Patient, PatientDto>()
                .ForMember(dest => dest.LastTestDate, opt => opt.Ignore());
            CreateMap<CreatePatientClient, Patient>();

            // Medical Record mappings
            CreateMap<MedicalRecord, MedicalRecordDto>()
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient.FullName));

            // User Info mappings
            CreateMap<UserInfo, Patient>()
                .ForMember(dest => dest.PatientId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.MedicalRecord, opt => opt.Ignore());

            CreateMap<UserInfo, UserDataDTO>();

            // Comment mappings
            CreateMap<CreateCommentDto, DomainComment>();
        }
    }
}
