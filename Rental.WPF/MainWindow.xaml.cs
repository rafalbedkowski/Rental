using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using Microsoft.Win32;
using Rental.BLL.IRepository;
using Rental.DAL.Models;
using Rental.WPF.Windows;
using Unity;

namespace Rental.WPF
{
    public partial class MainWindow : Window
    {
        private readonly ITransactionRepository _db;
        private readonly ICollection<Transaction> _transaction;

        public IUnityContainer Container { get; set; }
        public decimal Amount { get; set; }

        public MainWindow(IUnityContainer container, ITransactionRepository db)
        {
            Container = container;
            _db = db;

            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
            _transaction = _db.GetAll();
        }

        #region Initialize

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Transaction.ItemsSource = _transaction;
            CalculateValue();
        }

        private void CalculateValue()
        {
            var itemSource = Transaction.ItemsSource as IEnumerable<Transaction>;
            Amount = 0;
            foreach (var item in itemSource)
            {
                Amount += item.PriceForRent;
            }

            Price.Text = Convert.ToString(Amount, CultureInfo.InvariantCulture) + " zł";
        }


        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void ExitProgram(object sender, CancelEventArgs e)
        {
            var decision = MessageBox.Show("Czy chcesz wyjść z programu ?", "Wypożyczalnia", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (decision == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        #endregion

        #region MenuActions

        private void ToolsList(object sender, RoutedEventArgs e) => ShowWindow<ToolsWindow>();
        private void CompaniesList(object sender, RoutedEventArgs e) => ShowWindow<CompaniesWindow>();
        private void UsersList(object sender, RoutedEventArgs e) => ShowWindow<UsersWindow>();
        private void About(object sender, RoutedEventArgs e)
        {
            var about = new AboutWindow();
            about.ShowDialog();
        }

        private void ShowWindow<T>() where T : Window
        {
            var window = Container.Resolve<T>();
            window.ShowDialog();
        }

        #endregion

        #region Button

        private void ExitProgramBtn_Click(object sender, RoutedEventArgs e) => this.Close();
        private void RentBtn_Click(object sender, RoutedEventArgs e) => RadioBtnAction(TransactionType.Rent);
        private void ReturnBtn_Click(object sender, RoutedEventArgs e) => RadioBtnAction(TransactionType.Return);

        private void TransactionListLoad(TransactionType transactionType)
        {
            Transaction.ItemsSource = _db.GetByType(transactionType);
            CalculateValue();
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            var transaction = (Transaction)Transaction.SelectedItem;
            if (transaction == null) return;
            var transactionWidow = Container.Resolve<TransactionWindow>();
            transactionWidow.DataContext = transaction;
            transactionWidow.SetEditedMode(true);
            transactionWidow.ToolsListView.ItemsSource =
                _db.GetToolByTransactionNumber(((Transaction)Transaction.SelectedItem).TransactionNumber);
            transactionWidow.TransactionType = ((Transaction)Transaction.SelectedItem).TransactionType;
            transactionWidow.ShowDialog();
            LoadDataToCheckBtn();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }

        #endregion

        #region RadioCheck
        private void CheckRent_Checked(object sender, RoutedEventArgs e) => TransactionListLoad(TransactionType.Rent);
        private void CheckReturn_Checked(object sender, RoutedEventArgs e) => TransactionListLoad(TransactionType.Return);
        private void RadioBtnAction(TransactionType transactionType)
        {
            var transactionWindow = Container.Resolve<TransactionWindow>();
            transactionWindow.TransactionType = transactionType;
            transactionWindow.ShowDialog();
            LoadDataToCheckBtn();
        }

        private void LoadDataToCheckBtn()
        {
            if (CheckAll.IsChecked != null && (bool)CheckAll.IsChecked)
            {
                Transaction.ItemsSource = _db.GetAll();
                CalculateValue();
            }
            else if (CheckRent.IsChecked != null && (bool)CheckRent.IsChecked)
                TransactionListLoad(TransactionType.Rent);
            else if (CheckReturn.IsChecked != null && (bool)CheckReturn.IsChecked)
                TransactionListLoad(TransactionType.Return);
            CalculateValue();
        }

        private void CheckAll_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsInitialized) return;
            Transaction.ItemsSource = _db.GetAll();
            CalculateValue();
        }

        #endregion

        #region Repo

        private void RemoveTransaction(object sender, RoutedEventArgs e)
        {
            var transaction = (Transaction)Transaction.SelectedItem;
            _db.RemoveTransaction(transaction.TransactionId);
            _db.SaveChanges();
            LoadDataToCheckBtn();
        }

        #endregion
    }
}
