﻿using Contracts;
using Contracts.Manager;
using LoggerService;
using Service.Contracts.ServiceInterfaces;

namespace Service;

internal sealed class EmployeeService : IEmployeeService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;

    public EmployeeService(IRepositoryManager repository,
        ILoggerManager logger)
    {
        _repository = repository;
        _logger = logger;
    }
}