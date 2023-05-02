﻿using AutoMapper;
using Entities.Models;
using Shared.DataTransferObjects;

namespace UltimateASP.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Company, CompanyDto>()
            .ForMember(nameof(CompanyDto.FullAddress),
                opt => opt.MapFrom(x => string.Join(' ', x.Address, x.Country)));

        CreateMap<Employee, EmployeeDto>();

        CreateMap<CompanyForCreationDto, Company>();

        CreateMap<EmployeeForCreationDto, Employee>();
    }
}