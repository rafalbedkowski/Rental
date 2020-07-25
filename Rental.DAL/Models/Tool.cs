using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rental.DAL.Models
{
    public class Tool
    {
        public Tool()
        {
            PurchaseDate = DateTime.Now;
            Transaction = new HashSet<Transaction>();
        }
        [Key]
        public Guid ToolId { get; set; }

        [StringLength(50, MinimumLength = 3, ErrorMessage = "Nazwa musi zawierać minimum 3 znaki")]
        public string Name { get; set; }
        public string Sn { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal PurchasesValue { get; set; }
        public string DocumentNumber { get; set; }
        public int Warranty { get; set; }
        public decimal RentalPrice { get; set; }
        public bool Destroyed { get; set; }
        public DateTime? DestroyedDate { get; set; }
        public virtual User DestroyedCustomer { get; set; }
        public bool Lost { get; set; }
        public DateTime? LostDate { get; set; }
        public virtual User LostCustomer { get; set; }
        public string Description { get; set; }
        public virtual Company Producer { get; set; }
        public virtual ICollection<Transaction> Transaction { get; set; }
    }
}
