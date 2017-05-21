using Microsoft.EntityFrameworkCore;
using Skillustrator.Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mvc2017.Database
{
    public class MvcContext : DbContext
    {
        public DbSet<Person> People { get; set; }
        public DbSet<Skill> Skills { get; set; }

        public MvcContext(DbContextOptions<MvcContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Person>();
            modelBuilder.Entity<Skill>();
        }
    }
}
