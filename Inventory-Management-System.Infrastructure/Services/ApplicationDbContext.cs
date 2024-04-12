using Inventory_Management_System.Domain;
using Inventory_Management_System.Domain.Common;
using Inventory_Management_System.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory_Management_System.Infrastructure.Services
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Status> Statuses => Set<Status>();
        public DbSet<UsersOTP> UsersOTPs => Set<UsersOTP>();


        public virtual DbSet<T> GetDbSet<T>() where T : BaseEntity
        {
            return this.Set<T>();
        }
    }
}
