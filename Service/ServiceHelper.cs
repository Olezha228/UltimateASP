using Entities.Exceptions;
using Entities.Models;
using Shared.DataTransferObjects.Company;
using Contracts.Manager;

namespace Service;

public class ServiceHelper : IServiceHelper
{
    private readonly IRepositoryManager _repository;

    public ServiceHelper(IRepositoryManager repository)
    {
        _repository = repository;
    }

    public async Task<Company> GetCompanyAndCheckIfItExists(Guid id, bool trackChanges)
    {
        var companyEntity = await _repository.Company.GetCompanyAsync(id, trackChanges);

        if (companyEntity is null)
        {
            throw new CompanyNotFoundException(id);
        }

        return companyEntity;
    }

    public async Task<Employee> GetEmployeeFromCompanyAndCheckIfItExists(Guid companyId, Guid id, bool trackChanges)
    {
        var employeeFromDb = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges);

        if (employeeFromDb is null)
        {
            throw new EmployeeNotFoundException(id);
        }

        return employeeFromDb;
    }

    public void CheckIfCompanyCollectionNotNull(IEnumerable<CompanyForCreationDto> companyCollection)
    {
        if (companyCollection is null)
        {
            throw new CompanyCollectionBadRequest();
        }
    }

    public void CheckIfIdsAreNotNull(IEnumerable<Guid> ids)
    {
        if (ids is null)
        {
            throw new IdParametersBadRequestException();
        }
    }
}