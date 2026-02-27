using FieldServiceManagement.Core.DTOs;

namespace FieldServiceManagement.Core.Interfaces;

/// <summary>Defines operations for managing customers.</summary>
public interface ICustomerService
{
    Task<IEnumerable<CustomerDto>> GetAllAsync();
    Task<CustomerDto?> GetByIdAsync(Guid id);
    Task<CustomerDto> CreateAsync(CustomerCreateDto dto);
    Task<CustomerDto?> UpdateAsync(Guid id, CustomerCreateDto dto);
}
