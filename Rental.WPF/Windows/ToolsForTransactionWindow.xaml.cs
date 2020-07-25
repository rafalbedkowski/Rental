using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Rental.BLL.IRepository;
using Rental.DAL.Models;
using Unity;

namespace Rental.WPF.Windows
{
    public partial class ToolsForTransactionWindow : Window
    {
        private IToolRepository _db;
        private CollectionViewSource _collectionViewSource;

        public IUnityContainer Container { get; set; }
        public TransactionType TransactionType = TransactionType.Rent;
        public List<Tool> SelectedTools = new List<Tool>();
        public List<Tool> TempList = new List<Tool>();

        public ToolsForTransactionWindow(IUnityContainer container, IToolRepository db)
        {
            InitializeComponent();
            Container = container;
            _db = db;
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
        }

        #region Initialization

        private void ToolsForTransactionWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            _collectionViewSource = this.FindResource("ListView") as CollectionViewSource;
            var toolsList = TransactionType == TransactionType.Rent ? _db.GetToolsForTransaction(TransactionType.Return) : _db.GetToolsForTransaction(TransactionType.Rent);

            if (toolsList.Count == 0)
            {
                switch (TransactionType)
                {
                    case TransactionType.Rent:
                        MessageBox.Show("Nie ma nic do wypożyczenia", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    case TransactionType.Return:
                        MessageBox.Show("Nie ma nic do zwrotu", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                }

                this.Close();
            }

            if (TempList.Any())
            {
                foreach (var tool in TempList)
                {
                    toolsList.Remove(tool);
                }
            }

            this.DataContext = toolsList;
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        #endregion

        #region Button

        private void GetSelectedBtn_Click(object sender, RoutedEventArgs e)
        {
            SelectedTools = ListView.SelectedItems.Cast<Tool>().ToList();
            this.Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e) => this.Close();


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
            e.Accepted = !(e?.Item is Tool)
                         || ((Tool)e.Item).Name.ToUpper().Contains(SearchTxt.Text.ToUpper())
                         || ((Tool)e.Item).Sn.ToUpper().Contains(SearchTxt.Text.ToUpper());

        #endregion

        #region Sort

        private void Sorting_Click(object sender, RoutedEventArgs e)
        {
            var header = (sender as GridViewColumnHeader);
            if (header == null) return;
            var columnNameToSort = header.Name;
            var howToSort = ListSortDirection.Ascending;
            var view = (CollectionView)CollectionViewSource.GetDefaultView(ListView.ItemsSource);

            if (view.SortDescriptions.Any())
            {
                var item = view.SortDescriptions.ElementAt(0);
                howToSort = item.Direction == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
            }

            view.SortDescriptions.Clear();
            view.SortDescriptions.Add(new SortDescription(columnNameToSort, howToSort));
        }

        #endregion
    }
}
