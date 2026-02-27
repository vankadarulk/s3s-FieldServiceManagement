using FieldServiceManagement.API.Data;
using FieldServiceManagement.Core.DTOs;
using FieldServiceManagement.Core.Interfaces;
using FieldServiceManagement.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace FieldServiceManagement.API.Services;

public class CustomerService : ICustomerService
{
    private readonly AppDbContext _context;

    public CustomerService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CustomerDto>> GetAllAsync()
    {
        return await _context.Customers.Select(c => MapToDto(c)).ToListAsync();
    }

    public async Task<CustomerDto?> GetByIdAsync(Guid id)
    {
        var customer = await _context.Customers.FindAsync(id);
        return customer == null ? null : MapToDto(customer);
    }

    public async Task<CustomerDto> CreateAsync(CustomerCreateDto dto)
    {
        var customer = new Customer
        {
            Name = dto.Name,
            Email = dto.Email,
            Phone = dto.Phone,
            Address = dto.Address
        };
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
        return MapToDto(customer);
    }

    public async Task<CustomerDto?> UpdateAsync(Guid id, CustomerCreateDto dto)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return null;
        customer.Name = dto.Name;
        customer.Email = dto.Email;
        customer.Phone = dto.Phone;
        customer.Address = dto.Address;
        customer.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return MapToDto(customer);
    }

    private static CustomerDto MapToDto(Customer customer) => new()
    {
        Id = customer.Id,
        Name = customer.Name,
        Email = customer.Email,
        Phone = customer.Phone,
        Address = customer.Address,
        CreatedAt = customer.CreatedAt,
        UpdatedAt = customer.UpdatedAt
    };
}
