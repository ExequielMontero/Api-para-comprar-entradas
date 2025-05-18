using Api_entradas.Models;
using System.Collections.Generic;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Api_entradas.Data
{
    public class AppDbContext: IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> o):base(o){ }

        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
    }
}
