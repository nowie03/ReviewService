using Microsoft.EntityFrameworkCore;
using ReviewService.Models;

namespace ReviewService.Context
{
    public class ServiceContext : DbContext
    {
        public ServiceContext(DbContextOptions options) : base(options) { }

        public DbSet<Review> Reviews { get; set; }



        public DbSet<ConsumedMessage> ConsumedMessages { get; set; }

        override
        protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ConsumedMessage>().HasIndex(message => message.MessageId).IsUnique();


        }

    }
}
