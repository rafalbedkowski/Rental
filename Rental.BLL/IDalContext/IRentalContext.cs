using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Rental.DAL.Models;

namespace Rental.BLL.IDalContext
{
    public interface IRentalContext : IDisposable
    {
        DbSet<User> Users { get; set; }
        DbSet<Company> Company { get; set; }
        DbSet<Tool> Tools { get; set; }
        DbSet<Transaction> Transactions { get; set; }
        DbEntityEntry Entry(object entity);
        Database Database { get; }
        void SaveChanges();
    }
}