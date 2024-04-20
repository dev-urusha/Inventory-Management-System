using Inventory_Management_System.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory_Management_System.Domain
{
    public class Product : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal PricePerQuantity { get; set; }
        public decimal Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalQuantities { get; set; }

        [ForeignKey(nameof(StockKeepingUnit))]
        public Guid? StockKeepingUnitId { get; set; }
        public StockKeepingUnit? StockKeepingUnit { get; set; }

        [ForeignKey(nameof(Category))]
        public Guid? CategoryId { get; set; }
        public ProductCategory? Category { get; set; }

        [ForeignKey(nameof(Supplier))]
        public Guid? SupplierId { get; set; }
        public ProductSupplier? Supplier { get; set; }
    }
}
