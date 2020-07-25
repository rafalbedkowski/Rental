using Rental.BLL.IRepository;
using Rental.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Rental.Bll.DalContext;
using Rental.BLL.IDalContext;

namespace Rental.BLL.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IRentalContext _repo;
        private bool _disposed = false;

        public UserRepository(IRentalContext repo) => _repo = repo;

        public void AddUser(User user)
        {
            if (user != null)
                _repo.Users.Add(user);

        }

        public void UpdateUser(User user)
        {
            var userToUpdate = GetById(user.UserId);
            if (userToUpdate != null) _repo.Entry(userToUpdate).CurrentValues.SetValues(user);
        }

        public void RemoveUser(Guid userId)
        {
            var user = GetById(userId);
            if (user != null) _repo.Users.Remove(user);
        }

        public User GetById(Guid userId) => _repo.Users.First(u => u.UserId == userId);

        public ICollection<User> GetAll() => _repo.Users.ToList();

        public ICollection<User> GetAllByType(UserType userType) => _repo.Users.Where(u => u.UserType == userType).ToList();
        public ICollection<Company> GetCompany() => _repo.Company.ToList();

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
