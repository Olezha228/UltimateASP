using Shared.DataTransferObjects;
using Shared.DataTransferObjects.Company;

namespace Service.Contracts.ServiceInterfaces;

public interface ICompanyService
{
    IEnumerable<CompanyDto> GetAllCompanies(bool trackChanges);

    CompanyDto GetCompany(Guid companyId, bool trackChanges);

    IEnumerable<CompanyDto> GetByIds(IEnumerable<Guid> ids, bool trackChanges);

    CompanyDto CreateCompany(CompanyForCreationDto? company);

    (IEnumerable<CompanyDto> companies, string ids) CreateCompanyCollection
        (IEnumerable<CompanyForCreationDto> companyCollection);

    void DeleteCompany(Guid companyId, bool trackChanges);

    void UpdateCompany(Guid companyId, CompanyForUpdateDto? companyForUpdate, bool
        trackChanges);
}