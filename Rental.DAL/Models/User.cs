using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rental.DAL.Models
{
    public class User
    {
        public User()
        {
            Transaction = new HashSet<Transaction>();
        }

        [Key]
        public Guid UserId { get; set; }

        [StringLength(50, MinimumLength = 3, ErrorMessage = "Imię musi mieć minimum 3 znaki")]
        [Required]
        public string FirstName { get; set; }

        [StringLength(50, MinimumLength = 3, ErrorMessage = "Nazwisko musi mieć minimum 3 znaki")]
        public string LastName { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [StringLength(50, MinimumLength = 6, ErrorMessage = "Login musi mieć minimum 6 znaków")]
        public string LoginName { get; set; }

        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Hasło musi mieć minimum 6 znaków")]
        public string Password { get; set; }

        [Required]
        public UserType UserType { get; set; }

        public virtual Company Company { get; set; }
        public virtual ICollection<Transaction> Transaction { get; set; }

        [NotMapped]
        public string FullName => $"{LastName} {FirstName}";
    }
}
