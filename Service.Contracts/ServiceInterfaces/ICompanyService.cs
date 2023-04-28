using Entities.Models;

namespace Service.Contracts.ServiceInterfaces;

public interface ICompanyService
{
    IEnumerable<Company> GetAllCompanies(bool trackChanges);
}