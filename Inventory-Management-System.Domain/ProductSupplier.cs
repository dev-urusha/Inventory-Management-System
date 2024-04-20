using Inventory_Management_System.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory_Management_System.Domain
{
    public class ProductSupplier: BaseEntity
    {
        public Guid Id { get; set; }
        public string? DistributorName { get; set; }
        public string? BrandName { get; set; }
        public string? Email { get; set; }
        public int? Phone { get; set; }
        public string? AddressLineOne { get; set; }
        public string? AddressLineTwo { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
    }
}
