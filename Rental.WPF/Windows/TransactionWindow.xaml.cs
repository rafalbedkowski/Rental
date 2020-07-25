using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Rental.BLL.IRepository;
using Rental.DAL.Models;
using Rental.WPF.Helpers;
using Unity;

namespace Rental.WPF.Windows
{
    public partial class TransactionWindow : Window
    {
        private bool _editedMode = false;
        private readonly ITransactionRepository _db;
        private bool _isEdited;
        private readonly ObservableCollection<Tool> _selectedTools = new ObservableCollection<Tool>();

        public IUnityContainer Container { get; set; }
        public TransactionType TransactionType { get; set; }

        public TransactionWindow(IUnityContainer container, ITransactionRepository db)
        {
            InitializeComponent();
            Container = container;
            _db = db;
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
        }

        #region Initialize

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SelectListCustomer.ItemsSource = _db.GetUsersByType(UserType.Customer);
            SelectListUser.ItemsSource = _db.GetUsersByType(UserType.AppUser);
            switch (TransactionType)
            {
                case TransactionType.Rent:
                    this.Title = "Wypożyczenie";
                    break;
                case TransactionType.Return:
                    this.Title = "Zwrot";
                    break;
            }

            if (!_editedMode)
            {
                PrintBtn.Visibility = Visibility.Hidden;
                return;
            }
            foreach (Tool tool in ToolsListView.ItemsSource)
            {
                _selectedTools.Add(tool);
            }
            this.ToolsListView.ItemsSource = _selectedTools;
        }

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

        public void SetEditedMode(bool editedMode) => _editedMode = editedMode;

        #endregion

        #region Edit

        private void SetEdited(bool status) => _isEdited = status;
        private void ComboBox_OnDropDownOpened(object sender, EventArgs e) => SetEdited(true);

        #endregion

        #region Print

        private void PrintTransaction(Transaction transaction, IList<Tool> tools)
        {
            var result = MessageBox.Show("Czy wydrukować potwierdzenie ?", "Potwierdzenie", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                var print = new PrintTransaction(transaction, tools, _db);
                print.Print();
            }
        }

        private void PrintBtn_Click(object sender, RoutedEventArgs e)
        {
            var transaction = this.DataContext as Transaction;
            var tools = ToolsListView.ItemsSource as IList<Tool>;

            PrintTransaction(transaction, tools);
        }

        #endregion

        #region Button

        private void About(object sender, RoutedEventArgs e)
        {
            var about = new AboutWindow();
            about.ShowDialog();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e) => this.Close();

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = _editedMode ? UpdateTransaction() : AddTransaction();

            if (result == null) return;

            try
            {
                _db.SaveChanges();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Błąd zapisu : " + exception.Message, "Błąd", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            PrintTransaction(result.First(), ToolsListView.ItemsSource as IList<Tool>);
            this.Close();

        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var toolWindow = Container.Resolve<ToolsForTransactionWindow>();
            toolWindow.TransactionType = TransactionType == TransactionType.Return ? TransactionType.Return : TransactionType.Rent;

            foreach (var tool in _selectedTools)
            {
                toolWindow.TempList.Add(tool);
            }

            toolWindow.ShowDialog();

            foreach (var toolWindowSelectedTool in toolWindow.SelectedTools)
            {
                _selectedTools.Add(toolWindowSelectedTool);
            }
            this.ToolsListView.ItemsSource = _selectedTools;
        }

        #endregion

        #region Repo

        private ICollection<Transaction> AddTransaction()
        {
            ICollection<Transaction> transactions = new List<Transaction>();

            if ((User)SelectListUser.SelectedItem == null || (User)SelectListCustomer.SelectedItem == null)
            {
                MessageBox.Show("Musisz wybrać Odbiorcę i użytkownika", "Błąd", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return null;
            }
            SetEdited(false);
            var transactionNumber = _editedMode
                ? ((Transaction)this.DataContext).TransactionNumber
                : _db.GetTransactionNumber();

            foreach (var tool in ToolsListView.ItemsSource)
            {
                var price = TransactionType == TransactionType.Return
                    ? _db.GetPriceForRent(((Tool)tool).ToolId, (DateTime)TransactionDate.SelectedDate)
                    : 0;

                if (TransactionDate.SelectedDate == null) continue;
                var transaction = new Transaction
                {
                    TransactionId = Guid.NewGuid(),
                    TransactionDate = ((DateTime)TransactionDate.SelectedDate).Add(DateTime.Now.TimeOfDay),
                    TransactionType = TransactionType,
                    TransactionNumber = transactionNumber,
                    Tool = (Tool)tool,
                    AppUser = (User)SelectListUser.SelectedItem,
                    Customer = (User)SelectListCustomer.SelectedItem,
                    PriceForRent = price
                };

                _db.AddTransaction(transaction);
                transactions.Add(transaction);
            }

            return transactions;
        }

        private ICollection<Transaction> UpdateTransaction()
        {
            var actualTransaction = (Transaction)this.DataContext;
            var transactions = _db.GetTransactionsByNumber(actualTransaction.TransactionNumber);

            foreach (var transaction in transactions)
            {
                _db.RemoveTransaction(transaction.TransactionId);
            }

            try
            {
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                MessageBox.Show("Błąd usunięcia transakcji : " + e.Message, "Uwaga", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return AddTransaction();
        }

        private void RemoveTool(object sender, RoutedEventArgs e)
        {
            var tool = (Tool)ToolsListView.SelectedItem;
            _selectedTools.Remove(tool);
        }

        #endregion
    }
}
