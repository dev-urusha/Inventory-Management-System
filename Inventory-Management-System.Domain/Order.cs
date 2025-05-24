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
    public class Order: BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }

        [ForeignKey(nameof(Customer))]
        public Guid? CustomerId { get; set; }
        public Customer? Customer { get; set; }

        [ForeignKey(nameof(Status))]
        public Guid? StatusId { get; set; }
        public Status? Status { get; set; }

        public string? Notes { get; set; }
    }
}
