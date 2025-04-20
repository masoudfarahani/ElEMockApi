using ELE.MockApi.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace ELE.MockApi.Core.Db
{
    public class DataBaseContext : DbContext
    {
        public DbSet<MockEndpoint> Endpoints { get; set; }


        public DataBaseContext(DbContextOptions<DataBaseContext> options)
       : base(options)
        {
        }

       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MockEndpoint>().HasKey(c => c.Id);
            modelBuilder.Entity<MockEndpoint>().Property(c => c.Id).ValueGeneratedNever();
            modelBuilder.Entity<MockEndpoint>().Property(c => c.BaseUrl).HasMaxLength(300).IsRequired();
            modelBuilder.Entity<MockEndpoint>().OwnsMany(c => c.Responses, b =>
            {
                b.HasKey(c => c.Id);
                b.Property(c => c.Id).ValueGeneratedNever();
            });



        }
    }
}
