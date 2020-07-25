using System.ComponentModel.DataAnnotations;

namespace Rental.DAL.Models
{
    public enum StatisticType
    {
        [Display(Name = "Użytkownik")]
        User,
        [Display(Name = "Firma")]
        Company,
        [Display(Name = "Narzędzia")]
        Tool
    }
}