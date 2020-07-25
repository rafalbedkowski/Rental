using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Rental.BLL.IRepository;
using Rental.DAL.Models;
using Unity;

namespace Rental.WPF.Windows
{
    public partial class CompanyWindow : Window
    {
        private readonly ICompanyRepository _db;
        private bool _isEdited;
        private bool _isEditingMode;

        public IUnityContainer Container { get; set; }

        public CompanyWindow(IUnityContainer container, ICompanyRepository db)
        {
            Container = container;
            _db = db;
            InitializeComponent();
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

        #region Edit

        private void SetEdited(bool status) => _isEdited = status;

        public void SetEditingMode(bool status) => _isEditingMode = status;

        private void TextBox_Changed(object sender, TextChangedEventArgs e)
        {
            if (this.IsLoaded)
                SetEdited(true);
        }

        private void RadioBtn_Click(object sender, RoutedEventArgs e) => SetEdited(true);

        #endregion

        #region Buttons

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_isEditingMode)
                UpdateCompany();
            else AddCompany();
        }

        #endregion

        #region Repo

        private void AddCompany()
        {
            var company = new Company
            {
                CompanyName = CompanyNameInput.Text,
                CompanyId = Guid.NewGuid(),
                PostCode = PostCodeInput.Text,
                City = CityInput.Text,
                Address = AddressInput.Text,
                Email = EmailInput.Text,
                Phone = PhoneInput.Text,
                CompanyType = CompanyCustomer.IsChecked != null && (bool)CompanyCustomer.IsChecked ? CompanyType.Customer : CompanyType.Producer
            };

            _db.AddCompany(company);
            try
            {
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                MessageBox.Show("Błąd usunięcia Firmy : " + e.Message, "Uwaga", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            SetEdited(false);
            this.Close();
        }

        private void UpdateCompany()
        {
            var company = (Company)this.DataContext;
            _db.UpdateCompany(company);
            try
            {
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                MessageBox.Show("Błąd aktualizacji Firmy : " + e.Message, "Uwaga", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            SetEdited(false);
            this.Close();
        }

        #endregion
    }
}
