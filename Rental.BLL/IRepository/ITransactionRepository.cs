using System;
using System.Collections.Generic;
using Rental.DAL.Models;

namespace Rental.BLL.IRepository
{
    public interface ITransactionRepository : IDisposable
    {
        void AddTransaction(Transaction transaction);
        Transaction GetById(Guid transactionId);
        void UpdateTransaction(Transaction transaction);
        void RemoveTransaction(Guid transactionId);
        ICollection<User> GetUsersByType(UserType userType);
        int GetTransactionNumber();
        decimal GetPriceForRent(Guid toolId, DateTime returnDate);
        ICollection<Transaction> GetAll();
        ICollection<Transaction> GetByType(TransactionType type);
        ICollection<Transaction> GetTransactionForUser(Guid userId);
        ICollection<Transaction> GetTransactionForCompany(Guid companyId);
        ICollection<Transaction> GetTransactionForTool(Guid toolId);
        ICollection<Tool> GetToolByTransactionNumber(int transactionNumber);
        ICollection<Transaction> GetTransactionsByNumber(int transactionNumber);
        void SaveChanges();
    }
}