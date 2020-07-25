using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rental.DAL.Models;

namespace Rental.BLL.IRepository
{
    public interface ICompanyRepository : IDisposable
    {
        void AddCompany(Company company);
        void RemoveCompany(Guid companyId);
        void UpdateCompany(Company company);
        Company GetById(Guid companyId);
        ICollection<Company> GetAll();
        ICollection<Company> GetByType(CompanyType companyType);
        void SaveChanges();
    }
}