using System.Linq.Expressions;
using System.Security.Claims;
using AutoMapper;
using FluentValidation;
using WashBooking.Application.DTOs.ServiceDTO;
using WashBooking.Application.DTOs.ServiceDTO.Response;
using WashBooking.Application.DTOs.UserProfileDTO.Request;
using WashBooking.Application.DTOs.UserProfileDTO.Response;
using WashBooking.Application.Interfaces;
using WashBooking.Domain.Common;
using WashBooking.Domain.Entities;
using WashBooking.Domain.Interfaces.Persistence;

namespace WashBooking.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<UpdateUserProfileRequest> _updateUserProfileRequestValidator;
    private readonly IValidator<GetPagedRequest> _getPagedRequestValidator;
    private readonly IValidator<ActivateUserRequest> _activateUserRequestValidator;
    private readonly IValidator<UpdateUserRoleRequest> _updateUserRoleRequestValidator;
    
    public UserService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<UpdateUserProfileRequest> updateUserProfileRequestValidator, IValidator<GetPagedRequest> getPagedRequestValidator, IValidator<ActivateUserRequest> activateUserRequestValidator, IValidator<UpdateUserRoleRequest> updateUserRoleRequestValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _updateUserProfileRequestValidator = updateUserProfileRequestValidator;
        _getPagedRequestValidator = getPagedRequestValidator;     
        _activateUserRequestValidator = activateUserRequestValidator;
        _updateUserRoleRequestValidator = updateUserRoleRequestValidator;   
    }
    
    public async Task<Result<UserProfileResponse>> GetUserProfileAsync(ClaimsPrincipal user)
    {
        var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");
        var userProfile = await _unitOfWork.UserProfileRepository.GetByIdAsync(userId);
        if (userProfile is null)
            return Result<UserProfileResponse>.Failure(new Error("UserProfile.GetMe.NotFound", "User profile not found"));
        var userDTO = _mapper.Map<UserProfileResponse>(userProfile);
        return Result<UserProfileResponse>.Success(_mapper.Map<UserProfileResponse>(userDTO));
    }

    public async Task<Result> UpdateAsync(UpdateUserProfileRequest updateUserProfileRequest, ClaimsPrincipal user)
    {
        var validationResult = _updateUserProfileRequestValidator.Validate(updateUserProfileRequest);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new Error("UserProfile.UpdateMe.Validation", e.ErrorMessage))
                .ToList();
            return Result.Failure(errors);
        }
        
        var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");
        var userProfile = await _unitOfWork.UserProfileRepository.GetByIdAsync(userId);
        if (userProfile is null)
        {
            return Result.Failure(new Error("UserProfile.UpdateMe.NotFound", "User profile not found"));
        }        
        
        _mapper.Map(updateUserProfileRequest, userProfile);

        try
        {
            await _unitOfWork.SaveChangesAsync();       
        }
        catch (Exception e)
        {
            return Result.Failure(new Error("UserProfile.UpdateMe.Database.Error", "Update failed. Please try again later."));
        }
        
        return Result.Success();
    }

    public async Task<Result<PagedResult<object>>> GetPaginatedUsersAsync(GetPagedRequest getPagedRequest, ClaimsPrincipal user)
    {
        var validationResult = _getPagedRequestValidator.Validate(getPagedRequest);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new Error("UserProfile.GetPaginated.Validation", e.ErrorMessage))
                .ToList();
            return Result<PagedResult<object>>.Failure(errors);
        }
        
        Expression<Func<UserProfile, bool>>? filter = null;
        if (!string.IsNullOrWhiteSpace(getPagedRequest.Search))
        {
            filter = s => s.FullName.Contains(getPagedRequest.Search) || 
                           s.Phone.Contains(getPagedRequest.Search) || 
                           s.Email.Contains(getPagedRequest.Search);
        }
        
        var userRole = user.FindFirst(ClaimTypes.Role)?.Value;
        var result = await _unitOfWork.UserProfileRepository.GetPagedAsync(getPagedRequest.PageIndex, getPagedRequest.PageSize, filter);
        if (userRole == "Admin")
        {
            var pagedResult = _mapper.Map<PagedResult<AdminUserDetailResponse>>(result);
            var objectResult = new PagedResult<object>
            {
                Items = pagedResult.Items.Cast<object>().ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
            return Result<PagedResult<object>>.Success(objectResult);
        }
        else 
        {
            var pagedResult = _mapper.Map<PagedResult<StaffUserDetailResponse>>(result);
            var objectResult = new PagedResult<object>
            {
                Items = pagedResult.Items.Cast<object>().ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
            return Result<PagedResult<object>>.Success(objectResult);
        }

    }

    public async Task<Result<object>> GetUserByIdAsync(Guid id, ClaimsPrincipal user)
    {
        var userProfile = await _unitOfWork.UserProfileRepository.GetUserByIdAsync(id);
        if (userProfile is null)
        {
            return Result<object>.Failure(new Error("UserProfile.GetById.NotFound", "User profile not found"));
        }
        var userRole = user.FindFirst(ClaimTypes.Role)?.Value;
        if (userRole == "Admin")
        {
            var adminDto = _mapper.Map<AdminUserDetailResponse>(userProfile);
            return Result<object>.Success(adminDto);
        }
        else
        {
            var staffDto = _mapper.Map<StaffUserDetailResponse>(userProfile);
            return Result<object>.Success(staffDto);       
        }
    }

    public async Task<Result> UpdateUserProfileByIdForAdmin(Guid id, UpdateUserProfileRequest updateUserProfileRequest)
    {
        var validationResult = _updateUserProfileRequestValidator.Validate(updateUserProfileRequest);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new Error("UserProfile.UpdateUserProfileByIdForAdmin.Validation", e.ErrorMessage))
                .ToList();
            return Result.Failure(errors);
        }
        
        var userProfile = await _unitOfWork.UserProfileRepository.GetByIdAsync(id);
        if (userProfile is null)
            return Result.Failure(new Error("UserProfile.UpdateUserProfileByIdForAdmin.NotFound", "User profile not found"));
        
        _mapper.Map(updateUserProfileRequest, userProfile);

        try
        {
            await _unitOfWork.SaveChangesAsync();      
        }
        catch (Exception e)
        {
            return Result.Failure(new Error("UserProfile.UpdateUserProfileByIdForAdmin.Database.Error", "Update failed. Please try again later."));       
        }
        
        return Result.Success();
    }

    public async Task<Result> ActivateUserAsync(Guid id, ActivateUserRequest activateUserRequest)
    {
        var validationResult = _activateUserRequestValidator.Validate(activateUserRequest);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new Error("UserProfile.ActivateOrInActivate.Validation", e.ErrorMessage))
                .ToList();
            return Result.Failure(errors);
        }
        
        var userProfile = await _unitOfWork.UserProfileRepository.GetUserByIdAsync(id);
        if (userProfile is null)
            return Result.Failure(new Error("UserProfile.ActivateOrInActivate.NotFound", "User profile not found"));
        //thêm mapper vào đây hoặc map tay cũng đc cho nhanh tùy m, hết pin r ní cút đây
        _mapper.Map(activateUserRequest, userProfile);

        foreach (var account in userProfile.Accounts)
        {
            _mapper.Map(activateUserRequest, account);       
        }

        try
        {
            await _unitOfWork.SaveChangesAsync();      
        }
        catch (Exception e)
        {
            return Result.Failure(new Error("UserProfile.ActivateOrInActivate.Database.Error", "Update failed. Please try again later."));      
        }
        
        return Result.Success();    
    }

    public async Task<Result> ChangeUserRoleAsync(Guid id, UpdateUserRoleRequest updateUserRoleRequest)
    {
        var validationResult = _updateUserRoleRequestValidator.Validate(updateUserRoleRequest);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new Error("UserProfile.ChangeUserRoleAsync.Validation", e.ErrorMessage))
                .ToList();
            return Result.Failure(errors);
        }

        var userProfile = await _unitOfWork.UserProfileRepository.GetUserByIdAsync(id);
        if (userProfile is null)
        {
            return Result.Failure(new Error("UserProfile.ChangeUserRoleAsync.NotFound", "User profile not found"));
        }
        
        if (userProfile.Role == updateUserRoleRequest.Role)
            return Result.Failure(new Error("UserProfile.ChangeUserRoleAsync.Role.Duplicate", "Role is existed"));
        
        if (userProfile.Role.ToString().Equals("Admin"))
            return Result.Failure(new Error("UserProfile.ChangeUserRoleAsync.Role.Admin", "Admin can not change role"));
        
        _mapper.Map(updateUserRoleRequest, userProfile);

        try
        {
            await _unitOfWork.SaveChangesAsync();      
        }
        catch (Exception e)
        {
            return Result.Failure(new Error("UserProfile.ChangeUserRoleAsync.Database.Error", "Update failed. Please try again later."));      
        }
        
        return Result.Success();   
    }
}