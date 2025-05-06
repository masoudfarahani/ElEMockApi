using ELE.MockApi.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace ELE.MockApi.Core.Db
{
    public class DataBaseContext : DbContext
    {
        public DbSet<MockEndpoint> Endpoints { get; set; }
        public DbSet<ApiCallLog> CallLogs { get; set; }
        public DbSet<Log> Logs{ get; set; }
      
      


        public DataBaseContext(DbContextOptions<DataBaseContext> options)
       : base(options)
        {
        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MockEndpoint>().HasKey(c => c.Id);
            modelBuilder.Entity<MockEndpoint>().Property(c => c.Id).ValueGeneratedNever();
            modelBuilder.Entity<MockEndpoint>().Property(propertyExpression: c => c.BaseUrl).HasMaxLength(300).IsRequired();
            modelBuilder.Entity<MockEndpoint>().OwnsMany(c => c.Responses, b =>
            {
                b.HasKey(c => c.Id);
                b.Property(c => c.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<ApiCallLog>().HasKey(c => c.Id);
            modelBuilder.Entity<ApiCallLog>().Property(c => c.Id).ValueGeneratedNever();
            modelBuilder.Entity<ApiCallLog>().Property(c => c.Url).HasMaxLength(300).IsRequired();

            modelBuilder.Entity<Log>().HasKey(c => c.Id);
            modelBuilder.Entity<Log>().Property(c => c.Id).ValueGeneratedNever();
            modelBuilder.Entity<Log>().Property(c => c.Content).HasMaxLength(4000).IsRequired();

        }
    }
}
