﻿using Shared.DataTransferObjects;

namespace Service.Contracts.ServiceInterfaces;

public interface IEmployeeService
{
    IEnumerable<EmployeeDto> GetEmployees(Guid companyId, bool trackChanges);

    EmployeeDto GetEmployee(Guid companyId, Guid id, bool trackChanges);
}