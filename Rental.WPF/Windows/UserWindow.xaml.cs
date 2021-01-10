using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Rental.BLL.IRepository;
using Rental.DAL.Models;
using Unity;

namespace Rental.WPF.Windows
{
    public partial class UserWindow : Window
    {
        private readonly IUserRepository _db;
        private bool _isEdited;
        private bool _isEditingMode;

        public IUnityContainer Container { get; set; }

        public UserWindow(IUnityContainer container, IUserRepository db)
        {
            Container = container;
            _db = db;
            InitializeComponent();
            CompanySelect.ItemsSource = _db.GetCompany();
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
        }

        #region Initialization
        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void ClosingWindow(object sender, CancelEventArgs e)
        {
            if (!_isEdited) return;
            var result = MessageBox.Show("Masz nie zapisane dane. Czy mimo to chcesz wyjść ?", "Wypożyczalnia",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No) e.Cancel = true;
        }

        #endregion

        #region RadioButton

        private void RadioBtn_Click(object sender, RoutedEventArgs e) => _isEdited = true;

        #endregion

        #region Buttons
        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_isEditingMode)
                UpdateUser();
            else AddUser();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e) => this.Close();

        #endregion

        #region Repo

        private void AddUser()
        {
            var user = new User
            {
                UserId = Guid.NewGuid(),
                FirstName = FirstNameInput.Text,
                LastName = LastNameInput.Text,
                Email = EmailInput.Text,
                Phone = PhoneInput.Text,
                Company = (Company)CompanySelect.SelectedItem,
                UserType = UserRadio.IsChecked != null && (bool)UserRadio.IsChecked ? UserType.AppUser : UserType.Customer
            };

            _db.AddUser(user);

            try
            {
                _db.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                DbExceptionValidate("Błąd dodania użytkownika", e);
            }
            catch (Exception e)
            {
                MessageBox.Show("Błąd dodania użytkownika : " + e.Message, "Uwaga", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            SetEdited(false);
            this.Close();
        }

        private void UpdateUser()
        {
            var user = (User)this.DataContext;
            _db.UpdateUser(user);
            try
            {
                _db.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                DbExceptionValidate("Błąd aktualizacji użytkownika", e);
            }
            catch (Exception e)
            {
                MessageBox.Show("Błąd aktualizacji użytkownika : " + e.Message, "Uwaga", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            SetEdited(false);
            this.Close();
        }

        private void DbExceptionValidate(string title, DbEntityValidationException e)
        {
            var outputLines = new List<string>();
            var i = 0;
            var message = "";

            foreach (var eve in e.EntityValidationErrors)
            {
                outputLines.AddRange(eve.ValidationErrors.Select(ve =>
                    $"{++i}- Property: \"{ve.PropertyName}\", Error: \"{ve.ErrorMessage}\""));
                switch (eve.Entry.State)
                {
                    case EntityState.Added:
                        eve.Entry.State = EntityState.Detached;
                        break;
                    case EntityState.Modified:
                        eve.Entry.CurrentValues.SetValues(eve.Entry.OriginalValues);
                        eve.Entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Deleted:
                        eve.Entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Detached:
                        break;
                    case EntityState.Unchanged:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (outputLines.Any())
            {
                foreach (var outputLine in outputLines)
                {
                    message += $"\n{outputLine}";
                }
            }

            MessageBox.Show($"{title} : " + message, "Uwaga", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        #endregion

        #region Edit

        private void SetEdited(bool status) => _isEdited = status;

        public void SetEditingMode(bool status) => _isEditingMode = status;

        private void CompanySelect_OnDropDownOpened(object sender, EventArgs e) => _isEdited = true;

        private void TextBox_Changed(object sender, TextChangedEventArgs e)
        {
            if (this.IsLoaded)
                SetEdited(true);
        }

        #endregion
    }
}
