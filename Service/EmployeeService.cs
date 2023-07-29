using AutoMapper;
using Contracts.Contracts;
using Contracts.Manager;
using Entities.Exceptions;
using Entities.LinkModels;
using Entities.Models;
using LoggerService;
using Service.Contracts.ServiceInterfaces;
using Shared.DataTransferObjects.Employee;
using Shared.RequestFeatures;

namespace Service;

internal sealed class EmployeeService : IEmployeeService
{
    private readonly IRepositoryManager _repository;
    // ReSharper disable once NotAccessedField.Local
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;
    private readonly IEmployeeLinks _employeeLinks;
    private readonly ServiceHelper _serviceHelper;

    public EmployeeService(IRepositoryManager repository, ILoggerManager logger,
        IMapper mapper, IEmployeeLinks employeeLinks, ServiceHelper serviceHelper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
        _employeeLinks = employeeLinks;
        _serviceHelper = serviceHelper;
    }

    public async Task<(LinkResponse linkResponse, MetaData metaData)> GetEmployeesAsync
        (Guid companyId, LinkParameters linkParameters, bool trackChanges)
    {
        if (!linkParameters.EmployeeParameters.ValidAgeRange)
        {
            throw new MaxAgeRangeBadRequestException();
        }

        _ = await _serviceHelper.GetCompanyAndCheckIfItExists(companyId, trackChanges);

        var employeesWithMetaData = await _repository.Employee
            .GetEmployeesAsync(companyId, linkParameters.EmployeeParameters, trackChanges);

        var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesWithMetaData);

        var links = _employeeLinks.TryGenerateLinks(employeesDto,
            linkParameters.EmployeeParameters.Fields,
            companyId, linkParameters.Context);

        return (linkResponse: links, metaData: employeesWithMetaData.MetaData);
    }

    public async Task<EmployeeDto> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges)
    {
        _ = await _serviceHelper.GetCompanyAndCheckIfItExists(companyId, trackChanges);

        var employeeFromDb = await _serviceHelper.GetEmployeeFromCompanyAndCheckIfItExists(companyId, id, trackChanges);

        var employeeDto = _mapper.Map<EmployeeDto>(employeeFromDb);

        return employeeDto;
    }

    public async Task<EmployeeDto?> CreateEmployeeForCompanyAsync(Guid companyId, EmployeeForCreationDto employeeForCreation, bool trackChanges)
    {
        _ = await _serviceHelper.GetCompanyAndCheckIfItExists(companyId, trackChanges);

        var employeeEntity = _mapper.Map<Employee>(employeeForCreation);

        _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);

        await _repository.SaveAsync();

        var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);

        return employeeToReturn;
    }

    public async Task DeleteEmployeeForCompanyAsync(Guid companyId, Guid id, bool trackChanges)
    {
        _ = await _serviceHelper.GetCompanyAndCheckIfItExists(companyId, trackChanges);

        var employeeForCompany = await _serviceHelper.GetEmployeeFromCompanyAndCheckIfItExists(companyId, id, trackChanges);

        _repository.Employee.DeleteEmployee(employeeForCompany);
        await _repository.SaveAsync();
    }

    public async Task UpdateEmployeeForCompanyAsync(Guid companyId, Guid id, EmployeeForUpdateDto?
            employeeForUpdate,
        bool companyTrackChanges, bool employeeTrackChanges)
    {
        _ = await _serviceHelper.GetCompanyAndCheckIfItExists(companyId, companyTrackChanges);

        var employeeEntity = await _serviceHelper.GetEmployeeFromCompanyAndCheckIfItExists(companyId, id, employeeTrackChanges);

        //thus changing the state of the employeeEntity object to Modified.
        _mapper.Map(employeeForUpdate, employeeEntity);

        await _repository.SaveAsync();
    }

    public async Task<(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)> GetEmployeeForPatchAsync
        (Guid companyId, Guid id, bool companyTrackChanges, bool employeeTrackChanges)
    {
        _ = await _serviceHelper.GetCompanyAndCheckIfItExists(companyId, companyTrackChanges);

        var employeeEntity = await _serviceHelper.GetEmployeeFromCompanyAndCheckIfItExists(companyId, id, employeeTrackChanges);

        var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employeeEntity);

        return (employeeToPatch, employeeEntity);
    }

    public async Task SaveChangesForPatchAsync(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)
    {
        _mapper.Map(employeeToPatch, employeeEntity);
        await _repository.SaveAsync();
    }
}