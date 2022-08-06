#pragma warning disable CS8618
using Microsoft.EntityFrameworkCore;
namespace LoginRegistration.Models;

public class LoginRegContext : DbContext 
{ 
    public LoginRegContext(DbContextOptions options) : base(options) { }

    public DbSet<User> Users { get; set; } 
}
