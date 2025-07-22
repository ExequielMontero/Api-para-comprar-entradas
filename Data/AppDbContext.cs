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

            modelBuilder.Entity<Venta>()
                .HasKey(ue => new { ue.UserId, ue.EventId });

            modelBuilder.Entity<Venta>()
                .HasOne(ue => ue.User)
                .WithMany(u => u.UserEvents)
                .HasForeignKey(ue => ue.UserId);

            modelBuilder.Entity<Venta>()
                .HasOne(ue => ue.Event)
                .WithMany(e => e.UserEvents)
                .HasForeignKey(ue => ue.EventId);

            modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.User)
             .IsUnique();

            modelBuilder.Entity<Usuario>()
          .HasIndex(u => u.Email)
           .IsUnique();

            modelBuilder.Entity<Usuario>(builder =>
            {
                 builder
                .Property(u => u.Id)
                .HasColumnType("uuid");  // Para PostgreSQL (opcional, incluso podés no poner nada)
                builder
                .Property(u => u.EstaVerificado)
                .HasConversion(
                    v => v ? 1 : 0,    // de bool a int
                    v => v == 1        // de int a bool
                )
                .HasColumnType("integer");

            });
        }


        public DbSet<Usuario> User { get; set; }
        public DbSet<Evento> Events { get; set; }
        public DbSet<Venta> UserEvent { get; set; }
    }
}
