using Inventory_Management_System.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory_Management_System.Domain
{
    public class StockKeepingUnit : BaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
