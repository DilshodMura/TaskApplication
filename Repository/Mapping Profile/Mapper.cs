using AutoMapper;
using Database.Entities;
using Domain.Models;
using Repository.BusinessModels;
using Service.ServiceModels;

namespace Repository.Mapping_Profile
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<IEmployee, EmployeeDb>();
            CreateMap<EmployeeDb, EmployeeBusiness>();
            CreateMap<EmployeeServiceModel, EmployeeBusiness>();
        }
    }
}
