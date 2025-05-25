using Api_entradas.Models;
using System.Collections.Generic;
using System;
using Microsoft.EntityFrameworkCore;

namespace Api_entradas.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> o):base(o){ }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<UserEvent>()
                .HasKey(ue => new { ue.UserId, ue.EventId });

            modelBuilder.Entity<UserEvent>()
                .HasOne(ue => ue.User)
                .WithMany(u => u.UserEvents)
                .HasForeignKey(ue => ue.UserId);

            modelBuilder.Entity<UserEvent>()
                .HasOne(ue => ue.Event)
                .WithMany(e => e.UserEvents)
                .HasForeignKey(ue => ue.EventId);

            modelBuilder.Entity<User>()
            .HasIndex(u => u.Usuario)
             .IsUnique();

            modelBuilder.Entity<User>()
          .HasIndex(u => u.Email)
           .IsUnique();
        }

        public DbSet<User> User { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<UserEvent> UserEvent { get; set; }
    }
}
