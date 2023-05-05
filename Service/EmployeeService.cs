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

    public async Task<IEnumerable<EmployeeDto>> GetEmployeesAsync(Guid companyId, bool trackChanges)
    {
        var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges);

        if (company is null)
        {
            throw new CompanyNotFoundException(companyId);
        }
            
        var employeesFromDb = await _repository.Employee.GetEmployeesAsync(companyId, trackChanges);

        var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesFromDb);

        return employeesDto;
    }

    public async Task<EmployeeDto> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges)
    {
        var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges);

        if (company is null)
        {
            throw new CompanyNotFoundException(companyId);
        }

        var employeeFromDb = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges);

        if (employeeFromDb is null)
        {
            throw new EmployeeNotFoundException(id);
        }

        var employeeDto = _mapper.Map<EmployeeDto>(employeeFromDb);

        return employeeDto;
    }

    public async Task<EmployeeDto?> CreateEmployeeForCompanyAsync(Guid companyId, EmployeeForCreationDto employeeForCreation, bool trackChanges)
    {
        var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges);

        if (company is null)
        {
            throw new CompanyNotFoundException(companyId);
        }

        var employeeEntity = _mapper.Map<Employee>(employeeForCreation);

        _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);
        await _repository.SaveAsync();

        var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);

        return employeeToReturn;

    }

    public async Task DeleteEmployeeForCompanyAsync(Guid companyId, Guid id, bool trackChanges)
    {
        var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges);

        if (company is null)
        {
            throw new CompanyNotFoundException(companyId);
        }

        var employeeForCompany = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges);

        if (employeeForCompany is null)
        {
            throw new EmployeeNotFoundException(id);
        }

        _repository.Employee.DeleteEmployee(employeeForCompany);
        await _repository.SaveAsync();
    }

    public async Task UpdateEmployeeForCompanyAsync(Guid companyId, Guid id, EmployeeForUpdateDto?
            employeeForUpdate,
        bool companyTrackChanges, bool employeeTrackChanges)
    {
        var company = await _repository.Company.GetCompanyAsync(companyId, companyTrackChanges);

        if (company is null)
        {
            throw new CompanyNotFoundException(companyId);
        }

        var employeeEntity = await _repository.Employee.GetEmployeeAsync(companyId, id, employeeTrackChanges);

        if (employeeEntity is null)
        {
            throw new EmployeeNotFoundException(id);
        }

        //thus changing the state of the employeeEntity object to Modified.
        _mapper.Map(employeeForUpdate, employeeEntity);

        await _repository.SaveAsync();
    }

    public async Task<(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)> GetEmployeeForPatchAsync
        (Guid companyId, Guid id, bool companyTrackChanges, bool employeeTrackChanges)
    {
        var company = await _repository.Company.GetCompanyAsync(companyId, companyTrackChanges);

        if (company is null)
        {
            throw new CompanyNotFoundException(companyId);
        }

        var employeeEntity = await _repository.Employee.GetEmployeeAsync(companyId, id, employeeTrackChanges);

        if (employeeEntity is null)
        {
            throw new EmployeeNotFoundException(companyId);
        }

        var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employeeEntity);

        return (employeeToPatch, employeeEntity);
    }

    public async Task SaveChangesForPatchAsync(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)
    {
        _mapper.Map(employeeToPatch, employeeEntity);
        await _repository.SaveAsync();
    }
}