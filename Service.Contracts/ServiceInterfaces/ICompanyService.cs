using Shared.DataTransferObjects;

namespace Service.Contracts.ServiceInterfaces;

public interface ICompanyService
{
    IEnumerable<CompanyDto> GetAllCompanies(bool trackChanges);

    CompanyDto GetCompany(Guid companyId, bool trackChanges);
}