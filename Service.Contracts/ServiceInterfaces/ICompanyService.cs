using Shared.DataTransferObjects;

namespace Service.Contracts.ServiceInterfaces;

public interface ICompanyService
{
    IEnumerable<CompanyDto> GetAllCompanies(bool trackChanges);

    CompanyDto GetCompany(Guid companyId, bool trackChanges);

    public IEnumerable<CompanyDto> GetByIds(IEnumerable<Guid> ids, bool trackChanges);

    CompanyDto CreateCompany(CompanyForCreationDto? company);

    (IEnumerable<CompanyDto> companies, string ids) CreateCompanyCollection
        (IEnumerable<CompanyForCreationDto> companyCollection);

    void DeleteCompany(Guid companyId, bool trackChanges);
}