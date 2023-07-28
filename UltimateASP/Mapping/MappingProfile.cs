using AutoMapper;
using Entities.Models;
using Shared.DataTransferObjects;
using Shared.DataTransferObjects.Company;
using Shared.DataTransferObjects.Employee;

namespace UltimateASP.Mapping;

// ReSharper disable once UnusedMember.Global
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        MapCompanyDtos();

        MapEmployeeDtos();

        CreateMap<UserForRegistrationDto, User>();
    }

    private void MapCompanyDtos()
    {
        CreateMap<Company, CompanyDto>()
            .ForMember(nameof(CompanyDto.FullAddress),
                opt => opt.MapFrom(x => string.Join(' ', x.Address, x.Country)));

        CreateMap<CompanyForCreationDto, Company>();

        CreateMap<CompanyForUpdateDto, Company>();
    }

    private void MapEmployeeDtos()
    {
        CreateMap<Employee, EmployeeDto>();

        CreateMap<EmployeeForCreationDto, Employee>();

        CreateMap<EmployeeForUpdateDto, Employee>().ReverseMap();
    }
}