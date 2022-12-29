using Microsoft.EntityFrameworkCore;
using OpenBank.API.Domain.Entities;

namespace OpenBank.Api.Data;

public class OpenBankApiDbContext : DbContext
{
    public OpenBankApiDbContext(DbContextOptions<OpenBankApiDbContext> options) : base(options) {

    }

    public DbSet<User> Users {get; set;}
    public DbSet<Account> Accounts {get; set;}
    public DbSet<Transfer> Transfers {get; set;}    
}