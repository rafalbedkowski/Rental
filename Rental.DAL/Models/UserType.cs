using System.ComponentModel.DataAnnotations;

namespace Rental.DAL.Models
{
    public enum UserType
    {
        [Display(Name = "Klient")]
        Customer,
        [Display(Name = "Użytkownik")]
        AppUser
    }
}