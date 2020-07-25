using Rental.BLL.IRepository;
using Rental.DAL.Models;
using System;
using System.Linq;
using Rental.BLL.IDalContext;
using System.Collections.Generic;
using System.Data.Entity;

namespace Rental.BLL.Repository
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly IRentalContext _repo;
        private bool _disposed = false;

        public TransactionRepository(IRentalContext repo) => _repo = repo;

        public void AddTransaction(Transaction transaction)
        {
            if (transaction != null) _repo.Transactions.Add(transaction);
        }

        public Transaction GetById(Guid transactionId)
            => _repo.Transactions.First(t => t.TransactionId == transactionId);

        public ICollection<Transaction> GetAll()
            => _repo.Transactions
                .OrderBy(d => d.RowVersion)
                .ThenBy(n => n.TransactionNumber)
                .ToList();

        public ICollection<Transaction> GetByType(TransactionType type) =>
            _repo.Transactions.Where(t => t.TransactionType == type)
                .OrderBy(d => d.TransactionDate)
                .ToList();

        public void UpdateTransaction(Transaction transaction)
        {
            var transactionToUpdate = GetById(transaction.TransactionId);
            if (transactionToUpdate != null) _repo.Entry(transactionToUpdate).CurrentValues.SetValues(transaction);
        }

        public void RemoveTransaction(Guid transactionId)
        {
            var transaction = GetById(transactionId);
            if (transaction != null) _repo.Transactions.Remove(transaction);
        }

        public decimal GetPriceForRent(Guid toolId, DateTime returnDate)
        {
            var transaction = GetLastRentTransaction(toolId);
            var days = (int)(returnDate - transaction.TransactionDate).TotalDays;
            if (days < 0) return 0;
            return days == 0 ? transaction.Tool.RentalPrice : days * transaction.Tool.RentalPrice;
        }

        public ICollection<User> GetUsersByType(UserType userType) => _repo.Users.Where(u => u.UserType == userType).ToList();

        public int GetTransactionNumber() =>
            _repo.Transactions.Any() ? _repo.Transactions.Max(n => n.TransactionNumber) + 1 : 0;

        public ICollection<Transaction> GetTransactionForUser(Guid userId) =>
            _repo.Transactions.Where(u => u.Customer.UserId == userId).OrderBy(d => d.TransactionDate).ToList();

        public ICollection<Transaction> GetTransactionForCompany(Guid companyId) => _repo.Transactions.Where(c => c.Customer.Company.CompanyId == companyId).OrderBy(d => d.TransactionDate).ToList();

        public ICollection<Transaction> GetTransactionForTool(Guid toolId) => _repo.Transactions.Where(t => t.Tool.ToolId == toolId).OrderBy(d => d.TransactionDate).ToList();

        public ICollection<Tool> GetToolByTransactionNumber(int transactionNumber) =>
            _repo.Transactions.Where(t => t.TransactionNumber == transactionNumber).Select(t => t.Tool).ToList();

        public ICollection<Transaction> GetTransactionsByNumber(int transactionNumber) =>
            _repo.Transactions.Where(t => t.TransactionNumber == transactionNumber).ToList();

        public void SaveChanges() => _repo.SaveChanges();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                    _repo.Dispose();
            }
            _disposed = true;
        }

        private Transaction GetLastRentTransaction(Guid toolId)
        {
            return _repo.Transactions.Where(t => t.TransactionType == TransactionType.Rent)
                .Where(t => t.Tool.ToolId == toolId).OrderBy(d => d.RowVersion).ToList().Last();
        }
    }
}
