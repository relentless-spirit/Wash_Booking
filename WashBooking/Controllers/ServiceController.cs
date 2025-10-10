using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WashBooking.Application.DTOs.ServiceDTO;
using WashBooking.Application.Interfaces;

namespace WashBooking.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ServiceController : ControllerBase
{
    private readonly IServiceService  _serviceService;
    
    public ServiceController(IServiceService serviceService)
    {
        _serviceService = serviceService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetPagedServices([FromQuery]GetPagedRequest getPagedRequest)
    {
        var result = await _serviceService.GetPaginatedServicesAsync(getPagedRequest);
        if (result.IsFailure)
        {
            if (result.Error.Code.Contains("Validation"))
            {
                return UnprocessableEntity(result.Errors);
            }

            return BadRequest(result.Error);
        }  
        return Ok(result.Value);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetServiceById(Guid id)
    {
        var result = await _serviceService.GetServiceByIdAsync(id);
        if (result.IsFailure)
        {
            if (result.Error.Code.Contains("NotFound"))
            {
                return NotFound(result.Error);
            }
            return BadRequest(result.Error);
        }
        return Ok(result.Value);
    }
    
    [HttpGet("dropdown")]
    public async Task<IActionResult> GetServices()
    {
        var result = await _serviceService.GetServicesAsync();
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }
        return Ok(result.Value);
    }
    
    [HttpPost]
    /*[Authorize]*/
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateServiceRequest createServiceRequest)
    {
        var result = await _serviceService.AddServiceAsync(createServiceRequest);
        if (result.IsFailure)
        {
            if (result.Error.Code.Contains("Validation"))
            {
                return UnprocessableEntity(result.Errors);
            }

            return BadRequest(result.Error);
        }

        return Ok(new { code = "Service.Add.Success", message = "Service added successfully." });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateServiceRequest updateServiceRequest)
    {
        var result = await _serviceService.UpdateServiceAsync(id, updateServiceRequest);
        if (result.IsFailure)
        {
            if (result.Error.Code.Contains("Validation"))
            {
                return UnprocessableEntity(result.Errors);
            }

            if (result.Error.Code.Contains("NotFound"))
            {
                return NotFound(result.Error);           
            }
            
            return BadRequest(result.Error);
        }

        return Ok(new { code = "Service.Update.Success", message = "Service updated successfully." });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(Guid id)
    {
        var result = await _serviceService.DeleteServiceAsync(id);
        if (result.IsFailure)
        {
            if (result.Error.Code.Contains("NotFound"))
            {
                return NotFound(result.Error);
            }
            return BadRequest(result.Error);
        }
        return Ok(new { code = "Service.Delete.Success", message = "Service deleted successfully." });   
    }
}