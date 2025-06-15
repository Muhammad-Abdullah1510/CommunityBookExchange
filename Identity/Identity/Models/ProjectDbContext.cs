using Microsoft.EntityFrameworkCore;

namespace Identity.Models
{
    public class ProjectDbContext:DbContext 
    {
        public DbSet<Request> Requests { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<UserName> UserNames { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Project;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>().ToTable("Books");
            modelBuilder.Entity<Request>().ToTable("Requests");
            modelBuilder.Entity<UserName>().ToTable("UserNames");
        }
    }
}
