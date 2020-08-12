using System;
using System.Collections.Generic;
using System.Data;
using Rental.DAL.Models;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.Sql;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.Win32;
using Rental.BLL.IDalContext;
using User = Rental.DAL.Models.User;


namespace Rental.Bll.DalContext
{
    public class RentalContext : DbContext, IRentalContext, IDisposable
    {
        private readonly IRentalContext _db;

        public RentalContext(IRentalContext db)
        {
            _db = db;
        }

        public RentalContext() : base("Name=RentalContext")
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<Tool> Tools { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public void SaveChanges() => base.SaveChanges();
    }
}