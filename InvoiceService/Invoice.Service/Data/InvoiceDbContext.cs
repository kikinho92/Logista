using Invoice.Service.Data;
using Microsoft.EntityFrameworkCore;

namespace Invoice.Service.Data;

public class InvoiceDbContext : DbContext
{

    public InvoiceDbContext(DbContextOptions<InvoiceDbContext> options) : base(options)
    {  
    }

    public DbSet<Invoice> Invoice { get; set; }
    
}