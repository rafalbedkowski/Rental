using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class CompaniesWindow : Window
    {

        private readonly ICompanyRepository _db;
        private CollectionViewSource _collectionViewSource;

        public IUnityContainer Container { get; set; }

        public CompaniesWindow(IUnityContainer container, ICompanyRepository db)
        {
            Container = container;
            _db = db;
            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
        }

        #region Initialization

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var companies = _db.GetAll();
            if (!companies.Any())
            {
                var result = MessageBox.Show("Czy dodać nową firmę ?", "Wypożyczalnia", MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    AddCompany();
                    companies = _db.GetAll();
                }
                else this.Close();
            }

            DataContext = companies;
            _collectionViewSource = this.FindResource("CompanyListView") as CollectionViewSource;
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        #endregion

        #region RadioButton

        private void AllBtn_Checked(object sender, RoutedEventArgs e)
        {
            if (IsInitialized)
                SetFilter(sender, CompaniesListView_Filter);
        }

        private void ProducerBtn_Checked(object sender, RoutedEventArgs e) => SetFilter(sender, ProducerListView_Filter);

        private void CustomerBtn_Checked(object sender, RoutedEventArgs e) => SetFilter(sender, CustomerListView_Filter);

        private void CheckedRadioBtn()
        {
            List<RadioButton> radioButtons = MainGrid.Children.OfType<RadioButton>().ToList();
            RadioButton radioButton = radioButtons.First(r => r.IsChecked == true);
            radioButton.IsChecked = false;
            radioButton.IsChecked = true;
        }

        #endregion

        #region Filter

        private void SetFilter(object sender, FilterEventHandler filter)
        {
            if (((RadioButton)sender).IsChecked == true)
            {
                _collectionViewSource.Filter += filter;
                SearchTxt.Text = String.Empty;
            }
            else
                _collectionViewSource.Filter -= filter;
        }

        private void ProducerListView_Filter(object sender, FilterEventArgs e)
        {
            e.Accepted = false;
            var company = (Company)e.Item;

            if (company.CompanyType == CompanyType.Producer)
                e.Accepted = true;
        }

        private void CustomerListView_Filter(object sender, FilterEventArgs e)
        {
            e.Accepted = false;
            var company = (Company)e.Item;

            if (company.CompanyType == CompanyType.Customer)
                e.Accepted = true;
        }

        private void CompaniesFilter_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (((TextBox)sender).Text.Length > 0)
                _collectionViewSource.Filter += CompaniesFilter;
            else
            {
                _collectionViewSource.Filter -= CompaniesFilter;
                CheckedRadioBtn();
            }

            RefreshList();
        }

        private void RefreshList()
        {
            var view = (CollectionView)
                CollectionViewSource.GetDefaultView(CompaniesList.ItemsSource);
            view.Refresh();
        }

        private void CompaniesFilter(object sender, FilterEventArgs e)
        {
            e.Accepted = !(e?.Item is Company) || ((Company)e.Item).CompanyName.ToUpper().Contains(SearchTxt.Text.ToUpper());
        }

        private void CompaniesListView_Filter(object sender, FilterEventArgs e) => e.Accepted = true;

        #endregion

        #region Button

        private void CloseBtn_Click(object sender, RoutedEventArgs e) => this.Close();

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            var companyWidow = Container.Resolve<CompanyWindow>();
            companyWidow.DataContext = CompaniesList.SelectedItem;
            companyWidow.SetEditingMode(true);
            companyWidow.ShowDialog();
        }

        private void NewBtn_Click(object sender, RoutedEventArgs e)
        {
            AddCompany();
        }

        private void StatBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var statisticWindow = Container.Resolve<StatisticWindow>();
            statisticWindow.StatisticType = StatisticType.Company;
            statisticWindow.TransactionGuid = (((Company)CompaniesList.SelectedItem).CompanyId);
            statisticWindow.ShowDialog();
        }

        #endregion

        #region Sorting

        private void Sorting_Click(object sender, RoutedEventArgs e)
        {
            var header = (sender as GridViewColumnHeader);
            var columnNameToSort = header.Name;
            var howToSort = ListSortDirection.Ascending;
            var view = (CollectionView)CollectionViewSource.GetDefaultView(CompaniesList.ItemsSource);

            if (view.SortDescriptions.Any())
            {
                var item = view.SortDescriptions.ElementAt(0);
                howToSort = item.Direction == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
            }

            view.SortDescriptions.Clear();
            view.SortDescriptions.Add(new SortDescription(columnNameToSort, howToSort));
        }

        #endregion

        #region Repo

        private void AddCompany()
        {
            var companyWidow = Container.Resolve<CompanyWindow>();
            companyWidow.ShowDialog();
            DataContext = _db.GetAll();
            RefreshList();
        }

        private void RemoveCompany(object sender, RoutedEventArgs routedEventArgs)
        {
            if (CompaniesList.SelectedItem != null)
            {
                var companyToRemove = (Company)CompaniesList.SelectedItem;
                if (companyToRemove.Users.Any())
                    MessageBox.Show("Nie można usunąć firmy, ponieważ są do niej przypisani użytkownicy.",
                        "Wypożyczalnia", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    _db.RemoveCompany(companyToRemove.CompanyId);
                    try
                    {
                        _db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Błąd usunięcia Firmy : " + e.Message, "Uwaga", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    DataContext = _db.GetAll();
                    RefreshList();
                }

            }
        }
        #endregion

    }
}
