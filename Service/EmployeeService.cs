using AutoMapper;
using Contracts.Manager;
using Entities.Exceptions;
using Entities.Models;
using LoggerService;
using Service.Contracts.ServiceInterfaces;
using Shared.DataTransferObjects;
using Shared.DataTransferObjects.Employee;

namespace Service;

internal sealed class EmployeeService : IEmployeeService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;

    public EmployeeService(IRepositoryManager repository,
        ILoggerManager logger, IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    public IEnumerable<EmployeeDto> GetEmployees(Guid companyId, bool trackChanges)
    {
        var company = _repository.Company.GetCompany(companyId, trackChanges);

        if (company is null)
        {
            throw new CompanyNotFoundException(companyId);
        }
            
        var employeesFromDb = _repository.Employee.GetEmployees(companyId, trackChanges);

        var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesFromDb);

        return employeesDto;
    }

    public EmployeeDto GetEmployee(Guid companyId, Guid id, bool trackChanges)
    {
        var company = _repository.Company.GetCompany(companyId, trackChanges);

        if (company is null)
        {
            throw new CompanyNotFoundException(companyId);
        }

        var employeeFromDb = _repository.Employee.GetEmployee(companyId, id, trackChanges);

        if (employeeFromDb is null)
        {
            throw new EmployeeNotFoundException(id);
        }

        var employeeDto = _mapper.Map<EmployeeDto>(employeeFromDb);

        return employeeDto;
    }

    public EmployeeDto CreateEmployeeForCompany(Guid companyId, EmployeeForCreationDto employeeForCreation, bool trackChanges)
    {
        var company = _repository.Company.GetCompany(companyId, trackChanges);

        if (company is null)
        {
            throw new CompanyNotFoundException(companyId);
        }

        var employeeEntity = _mapper.Map<Employee>(employeeForCreation);

        _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);
        _repository.Save();

        var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);

        return employeeToReturn;

    }

    public void DeleteEmployeeForCompany(Guid companyId, Guid id, bool trackChanges)
    {
        var company = _repository.Company.GetCompany(companyId, trackChanges);

        if (company is null)
        {
            throw new CompanyNotFoundException(companyId);
        }

        var employeeForCompany = _repository.Employee.GetEmployee(companyId, id, trackChanges);

        if (employeeForCompany is null)
        {
            throw new EmployeeNotFoundException(id);
        }

        _repository.Employee.DeleteEmployee(employeeForCompany);
        _repository.Save();
    }

    public void UpdateEmployeeForCompany(Guid companyId, Guid id, EmployeeForUpdateDto?
            employeeForUpdate,
        bool companyTrackChanges, bool employeeTrackChanges)
    {
        var company = _repository.Company.GetCompany(companyId, companyTrackChanges);

        if (company is null)
        {
            throw new CompanyNotFoundException(companyId);
        }

        var employeeEntity = _repository.Employee.GetEmployee(companyId, id, employeeTrackChanges);

        if (employeeEntity is null)
        {
            throw new EmployeeNotFoundException(id);
        }

        //thus changing the state of the employeeEntity object to Modified.
        _mapper.Map(employeeForUpdate, employeeEntity);

        _repository.Save();
    }

}