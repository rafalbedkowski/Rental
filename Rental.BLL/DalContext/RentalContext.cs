using System;
using Rental.DAL.Models;
using System.Data.Entity;
using System.Net.Http;
using System.Threading.Tasks;
using Rental.BLL.IDalContext;
using Rental.BLL.Migrations;


namespace Rental.Bll.DalContext
{
    public class RentalContext : DbContext, IRentalContext, IDisposable
    {
        private readonly IRentalContext _db;

        public RentalContext(IRentalContext db)
        {
            _db = db;
        }

        public RentalContext()
            : base(nameOrConnectionString: "RentalContext")
        {
            try
            {
                this.Database.Connection.Open();
                this.Database.Connection.Close();
            }
            catch
            {
                Database.SetInitializer<RentalContext>(new DropCreateDatabaseAlways<RentalContext>());
            }
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<Tool> Tools { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public void SaveChanges() => base.SaveChanges();
    }
}