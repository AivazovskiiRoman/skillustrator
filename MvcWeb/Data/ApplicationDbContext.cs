using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MvcWeb.Models;
using Skillustrator.Api.Entities;

namespace MvcWeb.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Person> People { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<SkillLevel> SkillLevels { get; set; }
        public DbSet<TimePeriod> TimePeriods { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Article>();

            modelBuilder.Entity<Organization>()
                .HasMany<Person>(o => o.People)
                .WithOne(p => p.Organization);

            modelBuilder.Entity<Organization>().Property(o => o.Name).IsRequired();
            modelBuilder.Entity<Person>().Property(p => p.FirstName).IsRequired();
            modelBuilder.Entity<Person>().Property(p => p.LastName).IsRequired();
            modelBuilder.Entity<Skill>().Property(s => s.Name).IsRequired();

            modelBuilder.Entity<SkillTag>()
                .HasOne<Skill>(st => st.Skill)
                .WithMany(s => s.Tags)
                .IsRequired();
            modelBuilder.Entity<SkillTag>()
                .HasOne<Tag>(st => st.Tag)
                .WithMany(t => t.Skills)
                .IsRequired();

            modelBuilder.Entity<Tag>().Property(t => t.Name).IsRequired();
        }
    }
}
