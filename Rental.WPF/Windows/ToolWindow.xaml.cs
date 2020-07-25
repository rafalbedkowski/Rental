using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Rental.BLL.IRepository;
using Rental.DAL.Models;
using Unity;

namespace Rental.WPF
{
    public partial class ToolWindow : Window
    {
        private IToolRepository _db;
        private bool _isEdited;
        private bool _isEditingMode;
        private ICollection<Company> _producers;
        private readonly ICollection<User> _users;

        public IUnityContainer Container { get; set; }

        public ToolWindow(IUnityContainer container, IToolRepository db)
        {
            InitializeComponent();
            Container = container;
            _db = db;
            _producers = _db.GetProducers();
            _users = _db.GetUsers();
            SelectProducer.ItemsSource = _db.GetProducers();
            SelectDestroyed.ItemsSource = _users;
            SelectLost.ItemsSource = _users;
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

        private void CheckBox_Click(object sender, RoutedEventArgs e) => SetEdited(true);
        private void ComboBox_OnDropDownOpened(object sender, EventArgs e) => SetEdited(true);
        private void SetEdited(bool status) => _isEdited = status;
        private void TextBox_Changed(object sender, TextChangedEventArgs e)
        {
            if (this.IsLoaded)
                SetEdited(true);
        }
        public void SetEditingMode(bool status) => _isEditingMode = status;

        #endregion

        #region Button

        private void CancelBtn_Click(object sender, RoutedEventArgs e) => this.Close();

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_isEditingMode)
                UpdateTool();
            else AddTool();
        }

        #endregion

        #region Repo

        private void AddTool()
        {
            var purchaseDate = DatePickerPurchaseDate.SelectedDate?.Date ?? DateTime.Now;
            decimal.TryParse(TextBoxPurchaseValue.Text, out decimal purchaseValue);
            decimal.TryParse(TextBoxPrice.Text, out decimal rentalPrice);
            int.TryParse(TextBoxWarranty.Text, out int warranty);

            var tool = new Tool()
            {
                ToolId = Guid.NewGuid(),
                Name = TextBoxName.Text,
                Sn = TextBoxSn.Text,
                PurchaseDate = purchaseDate.Add(DateTime.Now.TimeOfDay),
                PurchasesValue = purchaseValue,
                DocumentNumber = TextBoxDocumentNumber.Text,
                Warranty = warranty,
                RentalPrice = rentalPrice,
                Destroyed = (bool)Destroyed.IsChecked,
                DestroyedDate = DestroyedDate.SelectedDate?.Date.Add(DateTime.Now.TimeOfDay),
                DestroyedCustomer = (User)SelectDestroyed.SelectedItem,
                Lost = (bool)Lost.IsChecked,
                LostDate = DestroyedDate.SelectedDate?.Date.Add(DateTime.Now.TimeOfDay),
                LostCustomer = (User)SelectLost.SelectedItem,
                Description = new TextRange(TextBoxDescription.Document.ContentStart, TextBoxDescription.Document.ContentEnd).Text,
                Producer = (Company)SelectProducer.SelectedItem
            };

            _db.AddTool(tool);
            try
            {
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                MessageBox.Show("Błąd zapisu narzędzi: " + e.Message, "Uwaga", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            SetEdited(false);
            this.Close();
        }

        private void UpdateTool()
        {
            var tool = (Tool)this.DataContext;
            _db.UpdateTool(tool);
            try
            {
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                MessageBox.Show("Błąd aktualizacji narzędzi : " + e.Message, "Uwaga", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            SetEdited(false);
            this.Close();
        }

        #endregion
    }
}
