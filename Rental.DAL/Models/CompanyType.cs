using System.ComponentModel.DataAnnotations;

namespace Rental.DAL.Models
{
    public enum CompanyType
    {
        [Display(Name = "Producent")]
        Producer,
        [Display(Name = "Klient")]
        Customer
    }
}