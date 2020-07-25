using Rental.BLL.IRepository;
using Rental.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Rental.Bll.DalContext;
using Rental.BLL.IDalContext;

namespace Rental.BLL.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly IRentalContext _repo;
        private bool _disposed = false;

        public CompanyRepository(IRentalContext repo) => _repo = repo;

        public void AddCompany(Company company)
        {
            if (company != null)
                _repo.Company.Add(company);
        }

        public void RemoveCompany(Guid companyId)
        {
            var company = GetById(companyId);
            if (company != null) _repo.Company.Remove(company);
        }

        public void UpdateCompany(Company company)
        {
            var companyToUpdate = GetById(company.CompanyId);
            if (companyToUpdate != null) _repo.Entry(companyToUpdate).CurrentValues.SetValues(company);
        }

        public Company GetById(Guid companyId) => _repo.Company.First(c => c.CompanyId == companyId);

        public ICollection<Company> GetAll() => _repo.Company.ToList();

        public ICollection<Company> GetByType(CompanyType companyType) =>
             _repo.Company.Where(c => c.CompanyType == companyType).ToList();

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
