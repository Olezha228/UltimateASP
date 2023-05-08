using Contracts.Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
using Shared.RequestFeatures;

namespace Repository.Repositories;

public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
{
    public EmployeeRepository(RepositoryContext repositoryContext)
        : base(repositoryContext)
    {
    }

    public async Task<PagedList<Employee>> GetEmployeesAsync(Guid companyId,
        EmployeeRequestParameters employeeParameters, bool trackChanges)
    {
        var employees = await FindByCondition(e => e.CompanyId.Equals(companyId),
                trackChanges)
            .FilterEmployees(employeeParameters.MinAge, employeeParameters.MaxAge)
            .Search(searchTerm: employeeParameters.SearchTerm)
            .Sort(employeeParameters.OrderBy)
            .ToListAsync();

        var count = await FindByCondition(e => e.CompanyId.Equals(companyId), trackChanges)
            .CountAsync();

        return new PagedList<Employee>(employees, count,
            employeeParameters.PageNumber, employeeParameters.PageSize);
    }


    public async Task<Employee?> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges) =>
        await FindByCondition(e => e.CompanyId.Equals(companyId) && e.Id.Equals(id), trackChanges).SingleOrDefaultAsync();

    public void CreateEmployeeForCompany(Guid companyId, Employee employee)
    {
        employee.CompanyId = companyId;
        Create(employee);
    }

    public void DeleteEmployee(Employee employee) => Delete(employee);
}