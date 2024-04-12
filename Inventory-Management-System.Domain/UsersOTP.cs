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
    public class UsersOTP : BaseEntity
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string GeneratedOTP { get; set; }

        public DateTime ExpirationTime { get; set; }

        [ForeignKey(nameof(User))]
        [Required]
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
