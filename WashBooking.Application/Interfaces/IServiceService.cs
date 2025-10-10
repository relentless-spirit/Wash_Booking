using WashBooking.Application.DTOs.ServiceDTO;
using WashBooking.Application.DTOs.ServiceDTO.Response;
using WashBooking.Domain.Common;
using WashBooking.Domain.Entities;

namespace WashBooking.Application.Interfaces;

public interface IServiceService
{
    Task<Result<PagedResult<GetPagedResponse>>> GetPaginatedServicesAsync(GetPagedRequest getPagedRequest);
    Task<Result<IEnumerable<GetAllResponse>>> GetServicesAsync();
    Task<Result<Domain.Entities.Service>> GetServiceByIdAsync(Guid id);
    Task<Result> AddServiceAsync(CreateServiceRequest createServiceRequest );
    Task<Result> UpdateServiceAsync(Guid id, UpdateServiceRequest updateServiceRequest);
    Task<Result> DeleteServiceAsync(Guid id);
}