﻿using Contracts;
using Contracts.Manager;
using Entities.Models;
using LoggerService;
using Service.Contracts.ServiceInterfaces;

namespace Service;

internal sealed class CompanyService : ICompanyService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;

    public CompanyService(IRepositoryManager repository,
        ILoggerManager logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public IEnumerable<Company> GetAll()
    {
        _repository.Company
    }
}