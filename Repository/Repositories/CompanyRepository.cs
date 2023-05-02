using Contracts.Contracts;
using Entities.Models;

namespace Repository.Repositories;

public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
{
    public CompanyRepository(RepositoryContext repositoryContext)
        : base(repositoryContext)
    {
    }

    public IEnumerable<Company> GetAllCompanies(bool trackChanges) =>
        FindAll(trackChanges)
            .OrderBy(c => c.Name)
            .ToList();

    public Company? GetCompany(Guid companyId, bool trackChanges) =>
        FindByCondition(c => c.Id.Equals(companyId), trackChanges)
            .SingleOrDefault();

    public void CreateCompany(Company company) => Create(company);

    public IEnumerable<Company> GetByIds(IEnumerable<Guid> ids, bool trackChanges)
    {
        return FindAll(trackChanges)
            .Where(company => ids.Contains(company.Id))
            .OrderBy(company => company.Name)
            .ToList();
    }
}