using System;
using System.Collections.Generic;
using Rental.DAL.Models;

namespace Rental.BLL.IRepository
{
    public interface IUserRepository : IDisposable
    {
        void AddUser(User user);
        void UpdateUser(User user);
        void RemoveUser(Guid userId);
        User GetById(Guid userId);
        ICollection<User> GetAll();
        ICollection<User> GetAllByType(UserType userType);
        ICollection<Company> GetCompany();
        void SaveChanges();
    }
}