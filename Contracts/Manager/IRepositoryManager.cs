using Contracts.Contracts;

namespace Contracts.Manager;

public interface IRepositoryManager
{
    ICompanyRepository Company { get; }

    IEmployeeRepository Employee { get; }

    void Save();
}