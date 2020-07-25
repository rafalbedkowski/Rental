using System;
using System.Collections.Generic;
using Rental.DAL.Models;

namespace Rental.BLL.IRepository
{
    public interface IToolRepository : IDisposable
    {
        void AddTool(Tool tool);
        void UpdateTool(Tool tool);
        void RemoveTool(Guid toolId);
        Tool GetById(Guid toolId);
        ICollection<Tool> GetAll();
        ICollection<Tool> GetForTransactionType(TransactionType transactionType);
        ICollection<Tool> GetToolsForTransaction(TransactionType transactionType);
        ICollection<Tool> GetStorageTools();
        ICollection<Company> GetProducers();
        ICollection<User> GetUsers();
        void SaveChanges();
    }
}