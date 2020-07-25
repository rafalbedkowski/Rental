using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rental.DAL.Models
{
    public class Company
    {
        public Company()
        {
            Users = new HashSet<User>();
            Tools = new HashSet<Tool>();
        }
        [Key]
        public Guid CompanyId { get; set; }

        [StringLength(50, MinimumLength = 2, ErrorMessage = "Nazwa firmy musi mieć minimum 2 znaki")]
        [Required]
        public string CompanyName { get; set; }

        [DataType(DataType.PostalCode)]
        public string PostCode { get; set; }
        public string City { get; set; }
        public string Address { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Required]
        public CompanyType CompanyType { get; set; }

        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<Tool> Tools { get; set; }
    }
}
