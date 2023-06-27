using Microsoft.EntityFrameworkCore;
using ReviewService.Models;

namespace ReviewService.Context
{
    public class ServiceContext:DbContext
    {
        public ServiceContext(DbContextOptions options) : base(options) { }

        public DbSet<Review> Reviews { get; set; }
    }
}
