using Rental.BLL.IRepository;
using Rental.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Rental.BLL.IDalContext;

namespace Rental.BLL.Repository
{
    public class ToolRepository : IToolRepository
    {
        private readonly IRentalContext _repo;
        private bool _disposed = false;

        public ToolRepository(IRentalContext repo) => _repo = repo;

        public void AddTool(Tool tool)
        {
            if (tool != null)
                _repo.Tools.Add(tool);
        }

        public void UpdateTool(Tool tool)
        {
            var toolToUpdate = GetById(tool.ToolId);
            if (toolToUpdate != null) _repo.Entry(toolToUpdate).CurrentValues.SetValues(tool);
        }

        public void RemoveTool(Guid toolId)
        {
            var tool = GetById(toolId);
            if (tool != null) _repo.Tools.Remove(tool);
        }

        public Tool GetById(Guid toolId) => _repo.Tools.First(t => t.ToolId == toolId);

        public ICollection<Tool> GetAll() => _repo.Tools.ToList();

        public ICollection<Tool> GetToolsForTransaction(TransactionType transactionType)
        {
            var tools = _repo.Tools
                   .Where(t => t.Transaction.OrderByDescending(d => d.TransactionNumber)
                       .FirstOrDefault()
                       .TransactionType == transactionType)
                   .ToList();
            if (transactionType == TransactionType.Return)
                tools.AddRange(GetNewTools());
            return tools;
        }

        public ICollection<Tool> GetForTransactionType(TransactionType transactionType) =>
            _repo.Transactions
                .Where(t => t.TransactionType == transactionType)
                .GroupBy(t => t.Tool.Name)
                .Select(d => d.OrderByDescending(y => y.TransactionDate)
                    .FirstOrDefault()).Select(t => t.Tool).ToList();

        public ICollection<Tool> GetNewTools() => _repo.Tools.Where(t => !t.Transaction.Any()).ToList();

        public ICollection<Tool> GetStorageTools()
        {
            var tool = new List<Tool>();
            var storageTools = GetForTransactionType(TransactionType.Return);
            var newTools = GetNewTools();

            tool.AddRange(storageTools);
            tool.AddRange(newTools);

            return tool;
        }

        public ICollection<Company> GetProducers() =>
            _repo.Company.Where(c => c.CompanyType == CompanyType.Producer).ToList();

        public ICollection<User> GetUsers() => _repo.Users.ToList();

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
    }
}
