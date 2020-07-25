using System;
using System.Collections.Generic;
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
    public partial class ToolsWindow : Window
    {
        private readonly IToolRepository _db;
        private CollectionViewSource _collectionViewSource;
        private ICollection<Tool> _toolDataContext;

        public IUnityContainer Container { get; set; }

        public ToolsWindow(IUnityContainer container, IToolRepository db)
        {
            Container = container;
            _db = db;
            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
            _toolDataContext = _db.GetAll();
        }

        #region Initialization

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var tools = _db.GetAll();
            if (!tools.Any())
            {
                var result = MessageBox.Show("Czy dodać nowe urządzenie ?", "Wypożyczalnia", MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    AddTool();
                    _toolDataContext = _db.GetAll();
                }
                else this.Close();
            }

            DataContext = _toolDataContext;
            _collectionViewSource = this.FindResource("ToolsListView") as CollectionViewSource;
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void RefreshList()
        {
            var view = (CollectionView)
                CollectionViewSource.GetDefaultView(ToolsListView.ItemsSource);
            view.Refresh();
        }

        #endregion

        #region Button

        private void CloseBtn_Click(object sender, RoutedEventArgs e) => this.Close();
        private void NewBtn_Click(object sender, RoutedEventArgs e) => AddTool();

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            var toolWindow = Container.Resolve<ToolWindow>();
            toolWindow.DataContext = ToolsListView.SelectedItem;
            toolWindow.SetEditingMode(true);
            toolWindow.ShowDialog();
        }

        private void StatBtn_Click(object sender, RoutedEventArgs e)
        {
            var statisticWindow = Container.Resolve<StatisticWindow>();
            statisticWindow.StatisticType = StatisticType.Tool;
            statisticWindow.TransactionGuid = (((Tool)ToolsListView.SelectedItem).ToolId);
            statisticWindow.ShowDialog();
        }

        private void AddTool()
        {
            var toolWidow = Container.Resolve<ToolWindow>();
            toolWidow.ShowDialog();
            SetContext(_db.GetAll());
            RefreshList();
        }

        #endregion

        #region RadioButton

        private void AllBtn_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsInitialized) return;
            SetContext(_toolDataContext);
            SetFilter(sender, ResetListView_Filter);
        }

        private void RentBtn_Checked(object sender, RoutedEventArgs e)
        {
            SetFilter(sender, ResetListView_Filter);
            SetContext(_db.GetForTransactionType(TransactionType.Rent));
        }

        private void StorageBtn_Checked(object sender, RoutedEventArgs e)
        {
            SetFilter(sender, ResetListView_Filter);
            SetContext(_db.GetStorageTools());
        }

        private void DestroyedBtn_Checked(object sender, RoutedEventArgs e) => SetFilter(sender, DestroyedListView_Filter);
        private void LostBtn_Checked(object sender, RoutedEventArgs e) => SetFilter(sender, LostListView_Filter);
        private void CheckedRadioBtn()
        {
            List<RadioButton> radioButtons = ToolGrid.Children.OfType<RadioButton>().ToList();
            RadioButton radioButton = radioButtons.First(r => r.IsChecked == true);
            radioButton.IsChecked = false;
            radioButton.IsChecked = true;
        }

        private void SetContext(object context) => DataContext = context;

        #endregion

        #region Filter

        private void SetFilter(object sender, FilterEventHandler filter)
        {
            SetContext(_db.GetAll());
            if (((RadioButton)sender).IsChecked == true)
            {
                _collectionViewSource.Filter += filter;
                SearchTxt.Text = String.Empty;
            }
            else
                _collectionViewSource.Filter -= filter;
            RefreshList();
        }

        private void DestroyedListView_Filter(object sender, FilterEventArgs e)
        {
            e.Accepted = false;
            var tool = (Tool)e.Item;

            if (tool.Destroyed)
                e.Accepted = true;
        }

        private void LostListView_Filter(object sender, FilterEventArgs e)
        {
            e.Accepted = false;
            var company = (Tool)e.Item;

            if (company.Lost)
                e.Accepted = true;
        }

        private void ToolsFilter_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (((TextBox)sender).Text.Length > 0)
                _collectionViewSource.Filter += ToolFilter;
            else
            {
                _collectionViewSource.Filter -= ToolFilter;
                CheckedRadioBtn();
            }

            RefreshList();
        }

        private void ToolFilter(object sender, FilterEventArgs e) =>
            e.Accepted = !(e?.Item is Tool)
                         || ((Tool)e.Item).Name.ToUpper().Contains(SearchTxt.Text.ToUpper())
                         || ((Tool)e.Item).Sn.ToUpper().Contains(SearchTxt.Text.ToUpper());

        private void ResetListView_Filter(object sender, FilterEventArgs e) => e.Accepted = true;

        #endregion

        #region Repo

        private void RemoveTool(object sender, RoutedEventArgs e)
        {
            if (ToolsListView.SelectedItem == null) return;
            var toolToRemove = (Tool)ToolsListView.SelectedItem;
            _db.RemoveTool(toolToRemove.ToolId);
            try
            {
                _db.SaveChanges();
            }
            catch (Exception exc)
            {
                MessageBox.Show("Błąd usunięcia Firmy : " + exc.Message, "Uwaga", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            DataContext = _db.GetAll();
            RefreshList();
        }

        #endregion
    }
}
