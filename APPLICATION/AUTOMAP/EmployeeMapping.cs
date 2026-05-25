using APPLICATION.DTO;
using AutoMapper;
using DOMAIN.ENITITES;

namespace APPLICATION.AUTOMAP;

public class EmployeeMapping : Profile
{
    public EmployeeMapping()
    {
        // Create Entity → DTO
        CreateMap<Employee, EmployeeDto>()
            .ForMember(dest => dest.Role,
                opt => opt.MapFrom(src => src.Role.ToString()))
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()));

        // Create DTO → Entity
        CreateMap<CreateEmployeeDto, Employee>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore());

        // Update DTO → Entity
        CreateMap<UpdateEmployeeDto, Employee>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore());
    }
}