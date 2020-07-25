using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rental.DAL.Models
{
    public class Transaction
    {
        public Transaction()
        {
            TransactionDate = DateTime.Now;
        }
        [Key]
        public Guid TransactionId { get; set; }

        [Required]
        public TransactionType TransactionType { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; }

        [Required]
        public int TransactionNumber { get; set; }

        public decimal PriceForRent { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        [Required]
        public virtual Tool Tool { get; set; }

        [Required]
        public virtual User AppUser { get; set; }

        [Required]
        public virtual User Customer { get; set; }
    }
}
