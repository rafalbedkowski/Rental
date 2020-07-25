using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Rental.BLL.IRepository;
using Rental.DAL.Models;
using Unity;

namespace Rental.WPF.Windows
{
    public partial class StatisticWindow : Window
    {
        private readonly ITransactionRepository _db;
        private CollectionViewSource _collectionViewSource;

        public IUnityContainer Container { get; set; }
        public Guid TransactionGuid { get; set; }
        public StatisticType StatisticType;

        public StatisticWindow(IUnityContainer container, ITransactionRepository db)
        {
            InitializeComponent();
            Container = container;
            _db = db;
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
        }

        #region Initialization

        private void StatisticWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            switch (StatisticType)
            {
                case StatisticType.User:
                    this.DataContext = _db.GetTransactionForUser(TransactionGuid);
                    break;
                case StatisticType.Company:
                    this.DataContext = _db.GetTransactionForCompany(TransactionGuid);
                    break;
                case StatisticType.Tool:
                    this.DataContext = _db.GetTransactionForTool(TransactionGuid);
                    break;
            }
            _collectionViewSource = this.FindResource("StatisticListView") as CollectionViewSource;
            var amount = ((ICollection<Transaction>)DataContext)
                .Sum(item => item.PriceForRent);

            Price.Text = Convert.ToString(amount, CultureInfo.InvariantCulture);
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        #endregion

        #region Button

        private void CloseBtn_Click(object sender, RoutedEventArgs e) => Close();


        #endregion

        #region Filter

        private void ToolsFilter_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (((TextBox)sender).Text.Length > 0)
                _collectionViewSource.Filter += ToolFilter;
            else
                _collectionViewSource.Filter -= ToolFilter;
        }

        private void ToolFilter(object sender, FilterEventArgs e) =>
            e.Accepted = !(e?.Item is Transaction)
                         || ((Transaction)e.Item).Tool.Name.ToUpper().Contains(SearchTxt.Text.ToUpper())
                         || ((Transaction)e.Item).Tool.Sn.ToUpper().Contains(SearchTxt.Text.ToUpper())
                         || ((Transaction)e.Item).Tool.Producer.CompanyName.ToUpper().Contains(SearchTxt.Text.ToUpper());

        #endregion
    }
}
