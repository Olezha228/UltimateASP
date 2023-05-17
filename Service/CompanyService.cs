using AutoMapper;
using Contracts.Manager;
using Entities.Exceptions;
using Entities.Models;
using LoggerService;
using Service.Contracts.ServiceInterfaces;
using Shared.DataTransferObjects.Company;

namespace Service;

internal sealed class CompanyService : ICompanyService
{
    private readonly IRepositoryManager _repository;
    // ReSharper disable once NotAccessedField.Local
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;
    private readonly ServiceHelper _serviceHelper;

    public CompanyService(IRepositoryManager repository,
        ILoggerManager logger,
        IMapper mapper,
        ServiceHelper serviceHelper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
        _serviceHelper = serviceHelper;
    }

    public async Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync(bool trackChanges)
    {
        var companies = await _repository.Company.GetAllCompaniesAsync(trackChanges);

        var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);

        return companiesDto;
    }

    public async Task<CompanyDto> GetCompanyAsync(Guid companyId, bool trackChanges)
    {
        var companyEntity = await _serviceHelper.GetCompanyAndCheckIfItExists(companyId, trackChanges);

        var companyDto = _mapper.Map<CompanyDto>(companyEntity);

        return companyDto;
    }

    public async Task<IEnumerable<CompanyDto>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges)
    {
        // ReSharper disable once PossibleMultipleEnumeration
        _serviceHelper.CheckIfIdsAreNotNull(ids);

        // ReSharper disable once PossibleMultipleEnumeration
        var idsList = ids.ToList();
        var companyEntities = await _repository.Company.GetByIdsAsync(idsList, trackChanges);

        if (idsList.Count != companyEntities.Count())
        {
            throw new CollectionByIdsBadRequestException();
        }
            
        var companiesToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);

        return companiesToReturn;
    }

    public async Task<CompanyDto> CreateCompanyAsync(CompanyForCreationDto? company)
    {
        var companyEntity = _mapper.Map<Company>(company);
         
        _repository.Company.CreateCompany(companyEntity);
        await _repository.SaveAsync();

        var companyToReturn = _mapper.Map<CompanyDto>(companyEntity);

        return companyToReturn;
    }

    public async Task<(IEnumerable<CompanyDto> companies, string ids)> CreateCompanyCollectionAsync
        (IEnumerable<CompanyForCreationDto> companyCollection)
    {
        _serviceHelper.CheckIfCompanyCollectionNotNull(companyCollection);
            
        var companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollection);

        foreach (var company in companyEntities)
        {
            _repository.Company.CreateCompany(company);
        }

        await _repository.SaveAsync();

        var companyCollectionToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);

        var collectionToReturn = companyCollectionToReturn.ToList();
        var ids = string.Join(",", collectionToReturn.Select(c => c.Id));

        return (companies: collectionToReturn, ids);
    }

    public async Task DeleteCompanyAsync(Guid companyId, bool trackChanges)
    {
        var companyEntity = await _serviceHelper.GetCompanyAndCheckIfItExists(companyId, trackChanges);

        _repository.Company.DeleteCompany(companyEntity);
        await _repository.SaveAsync();
    }

    public async Task UpdateCompanyAsync(Guid companyId, CompanyForUpdateDto? companyForUpdate, bool trackChanges)
    {
        var companyEntity = await _serviceHelper.GetCompanyAndCheckIfItExists(companyId, trackChanges);

        _mapper.Map(companyForUpdate, companyEntity);

        await _repository.SaveAsync();
    }
}