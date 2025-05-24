using Inventory_Management_System.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory_Management_System.Domain
{
    public class OrderItem: BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public double Quantity { get; set; } 
        public decimal UnitPrice { get; set; } 
        public decimal TotalPrice { get; set; }    

        [ForeignKey(nameof(Product))]
        public Guid? ProductId { get; set; }
        public Product? Product { get; set; }

        [ForeignKey(nameof(Order))]
        public Guid? OrderId { get; set; }
        public Order? Order { get; set; }

        [ForeignKey(nameof(Status))]
        public Guid? StatusId { get; set; }
        public Status? Status { get; set; }
        public string? Notes { get; set; }
    }
}
