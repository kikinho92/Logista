using Log.Service.Data;
using Microsoft.EntityFrameworkCore;

namespace Log.Service.Data;

public class LogDbContext : DbContext
{

    public LogDbContext(DbContextOptions<LogDbContext> options) : base(options)
    {  
    }

    public DbSet<LogTrace> LogTrace { get; set; }
    
}