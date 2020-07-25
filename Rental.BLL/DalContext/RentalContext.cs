using Rental.DAL.Models;
using System.Data.Entity;
using System.Threading.Tasks;
using Rental.BLL.IDalContext;


namespace Rental.Bll.DalContext
{
    public class RentalContext : DbContext, IRentalContext
    {
        private readonly IRentalContext _db;

        public RentalContext(IRentalContext db)
        {
            _db = db;
        }
        public RentalContext()
            : base(nameOrConnectionString: "RentalContext")
        {
            Database.SetInitializer<RentalContext>(null);
        }

        public static RentalContext Create()
        {
            return new RentalContext();
        }

        public void SaveChanges() => base.SaveChanges();

        public DbSet<User> Users { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<Tool> Tools { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}