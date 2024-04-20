using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventory_Management_System.Domain;
using Inventory_Management_System.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Management_System.Infrastructure.Interfaces
{
    public interface IApplicationDbContext
    {
        public DbSet<User> Users { get; }
        public DbSet<Role> Roles { get; }
        public DbSet<Status> Statuses { get; }
        public DbSet<UsersOTP> UsersOTPs { get; }
        public DbSet<StockKeepingUnit> StockKeepingUnits { get; }
        public DbSet<ProductCategory> ProductCategories { get; }
        public DbSet<ProductSupplier> ProductSuppliers { get; }
        public DbSet<Product> Products { get; }


        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        DbSet<T> GetDbSet<T>() where T : BaseEntity;
    }
}
