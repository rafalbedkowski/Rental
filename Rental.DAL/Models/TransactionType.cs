using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Rental.DAL.Models
{
    public enum TransactionType
    {
        [Display(Name = "Zakup")]
        Purchase,
        [Display(Name = "Wypożyczenie")]
        Rent,
        [Display(Name = "Zwrot")]
        Return,
        [Display(Name = "Uszkodzony")]
        Destroyed,
        [Display(Name = "Zagubiony")]
        Loss,
        [Display(Name = "Sprzedany")]
        Sale
    }
}