using System.Linq.Expressions;
using AutoMapper;
using FluentValidation;
using WashBooking.Application.DTOs.ServiceDTO;
using WashBooking.Application.DTOs.ServiceDTO.Response;
using WashBooking.Application.Interfaces;
using WashBooking.Domain.Common;
using WashBooking.Domain.Entities;
using WashBooking.Domain.Interfaces.Persistence;

namespace WashBooking.Application.Services;

public class ServiceService : IServiceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateServiceRequest>  _createServiceRequestValidator;
    private readonly IValidator<UpdateServiceRequest> _updateServiceRequestValidator;
    private readonly IValidator<GetPagedRequest> _getPagedRequestValidator;
    private readonly IMapper _mapper;

    public ServiceService(IUnitOfWork unitOfWork, IValidator<CreateServiceRequest> createServiceRequestValidator, IMapper mapper, IValidator<GetPagedRequest> getPagedRequestValidator, IValidator<UpdateServiceRequest> updateServiceRequestValidator)
    {
        _unitOfWork = unitOfWork;
        _createServiceRequestValidator = createServiceRequestValidator;
        _mapper = mapper;
        _getPagedRequestValidator = getPagedRequestValidator;
        _updateServiceRequestValidator = updateServiceRequestValidator;
    }
    
    public async Task<Result<PagedResult<GetPagedResponse>>> GetPaginatedServicesAsync(GetPagedRequest getPagedRequest)
    {
        var validationResult = await _getPagedRequestValidator.ValidateAsync(getPagedRequest);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new Error("Service.GetPaged.Validation", e.ErrorMessage))
                .ToList();
            return Result<PagedResult<GetPagedResponse>>.Failure(errors);
        }
        Expression<Func<Service, bool>>? filter = null;
        if (!string.IsNullOrWhiteSpace(getPagedRequest.Search))
        {
            filter = s => s.Name.Contains(getPagedRequest.Search) || 
                          (s.Description != null && s.Description.Contains(getPagedRequest.Search));
        }

        var result =
            await _unitOfWork.ServiceRepository.GetPagedAsync(getPagedRequest.PageIndex, getPagedRequest.PageSize,
                filter);
        var pagedResult = _mapper.Map<PagedResult<GetPagedResponse>>(result);
        return Result<PagedResult<GetPagedResponse>>.Success(pagedResult);
    }

    public async Task<Result<IEnumerable<GetAllResponse>>> GetServicesAsync()
    {
        var services = await _unitOfWork.ServiceRepository.GetAllAsync();
        var servicesDto = _mapper.Map<IEnumerable<GetAllResponse>>(services);
        return Result<IEnumerable<GetAllResponse>>.Success(servicesDto);
    }

    public async Task<Result<Service>> GetServiceByIdAsync(Guid id)
    {
        var service = await _unitOfWork.ServiceRepository.GetByIdAsync(id);
        if (service is null)
        {
            return Result<Service>.Failure(new Error("Service.GetById.NotFound", "Service not found"));
        }
        return Result<Service>.Success(service);
    }
    
    public async Task<Result> AddServiceAsync(CreateServiceRequest createServiceRequest)
    {
        var validationResult = _createServiceRequestValidator.Validate(createServiceRequest);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new Error("Service.Add.Validation", e.ErrorMessage))
                .ToList();
            return Result.Failure(errors);
        }
        var service = _mapper.Map<Service>(createServiceRequest);
        await _unitOfWork.ServiceRepository.AddAsync(service);
        try
        {
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception e)
        {
            return Result.Failure(new Error("Service.Add.Database.Error", "Service create failed. Please try again later."));
        }
        return Result.Success();
    }

    public async Task<Result> UpdateServiceAsync(Guid id, UpdateServiceRequest updateServiceRequest)
    {
        var oldService = await _unitOfWork.ServiceRepository.GetByIdAsync(id);
        if (oldService is null)
        {
            return Result.Failure(new Error("Service.Update.NotFound", "Service not found"));       
        }
        var validationResult = _updateServiceRequestValidator.Validate(updateServiceRequest);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new Error("Service.Update.Validation", e.ErrorMessage))
                .ToList();
            return Result.Failure(errors);
        }

        _mapper.Map(updateServiceRequest, oldService);
        oldService.UpdatedAt = DateTime.UtcNow;
        try
        {
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception e)
        {
            return Result.Failure(new Error("Service.Update.Database.Error", "Service update failed. Please try again later."));
        }
        return Result.Success();
    }

    public async Task<Result> DeleteServiceAsync(Guid id)
    {
        var oldService = await _unitOfWork.ServiceRepository.GetByIdAsync(id);
        if (oldService is null)
        {
            return Result.Failure(new Error("Service.Delete.NotFound", "Service not found"));       
        }

        _unitOfWork.ServiceRepository.Remove(oldService);
        try
        {
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception e)
        {
            return Result.Failure(new Error("Service.Delete.Database.Error", "Service delete failed. Please try again later."));
        }
        return Result.Success();
    }
}