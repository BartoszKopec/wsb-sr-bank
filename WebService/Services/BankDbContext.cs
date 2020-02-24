using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebService.Models;

namespace WebService.Services
{
    public class BankDbContext : DbContext
    {


        public BankDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("Users").HasKey(user=> user.Id);
        }

        private void M()
        {
            
        }

        public DbSet<User> Users { get; set; }
    }
}
