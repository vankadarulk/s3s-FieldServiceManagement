using FieldServiceManagement.Core.Enums;
using FieldServiceManagement.Core.Models;

namespace FieldServiceManagement.API.Data;

public static class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        context.Database.EnsureCreated();

        if (context.Users.Any())
            return;

        var adminId = Guid.NewGuid();
        var tech1Id = Guid.NewGuid();
        var tech2Id = Guid.NewGuid();
        var customer1Id = Guid.NewGuid();
        var customer2Id = Guid.NewGuid();

        var admin = new User
        {
            Id = adminId,
            FirstName = "Admin",
            LastName = "User",
            Email = "admin@fieldservice.com",
            Role = UserRole.Admin,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            IsActive = true
        };

        var tech1 = new User
        {
            Id = tech1Id,
            FirstName = "John",
            LastName = "Smith",
            Email = "john.smith@fieldservice.com",
            Role = UserRole.Technician,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Tech@123"),
            PhoneNumber = "555-0101",
            IsActive = true
        };

        var tech2 = new User
        {
            Id = tech2Id,
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane.doe@fieldservice.com",
            Role = UserRole.Technician,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Tech@123"),
            PhoneNumber = "555-0102",
            IsActive = true
        };

        context.Users.AddRange(admin, tech1, tech2);

        var customer1 = new Customer
        {
            Id = customer1Id,
            Name = "Acme Corporation",
            Email = "contact@acme.com",
            Phone = "555-1000",
            Address = new Address { Street = "123 Main St", City = "Springfield", State = "IL", ZipCode = "62701" }
        };

        var customer2 = new Customer
        {
            Id = customer2Id,
            Name = "TechCorp Inc",
            Email = "info@techcorp.com",
            Phone = "555-2000",
            Address = new Address { Street = "456 Oak Ave", City = "Shelbyville", State = "IL", ZipCode = "62565" }
        };

        context.Customers.AddRange(customer1, customer2);

        var job1 = new Job
        {
            Id = Guid.NewGuid(),
            Title = "HVAC Maintenance",
            Description = "Routine HVAC maintenance and filter replacement.",
            Status = JobStatus.Assigned,
            Priority = JobPriority.Medium,
            AssignedTechnicianId = tech1Id,
            CustomerId = customer1Id,
            ScheduledDate = DateTime.UtcNow.AddDays(2),
            Location = new Address { Street = "123 Main St", City = "Springfield", State = "IL", ZipCode = "62701" }
        };

        var job2 = new Job
        {
            Id = Guid.NewGuid(),
            Title = "Electrical Inspection",
            Description = "Annual electrical safety inspection.",
            Status = JobStatus.Created,
            Priority = JobPriority.High,
            CustomerId = customer2Id,
            ScheduledDate = DateTime.UtcNow.AddDays(5)
        };

        context.Jobs.AddRange(job1, job2);
        context.SaveChanges();
    }
}
