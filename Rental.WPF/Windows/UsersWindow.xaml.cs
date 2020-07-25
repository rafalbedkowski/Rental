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
    public partial class UsersWindow : Window
    {
        public IUnityContainer Container { get; set; }
        private readonly IUserRepository _db;
        private CollectionViewSource _collectionViewSource;
        private ICollection<User> _userDataContext;


        public UsersWindow(IUnityContainer container, IUserRepository db)
        {
            Container = container;
            _db = db;
            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
        }

        #region Initialization

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var users = _db.GetAll();

            if (!users.Any())
            {
                var result = MessageBox.Show("Czy dodać nowego uzytkownika ?", "Wypożyczalnia", MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    AddUser();
                }
                else this.Close();
            }

            _userDataContext = _db.GetAll();
            DataContext = _userDataContext;
            _collectionViewSource = this.FindResource("UserListView") as CollectionViewSource;
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        #endregion

        #region RadioButtons

        private void AllBtn_Checked(object sender, RoutedEventArgs e)
        {
            if (IsInitialized)
                SetFilter(sender, UserListView_Filter);
        }

        private void CustomerBtn_Checked(object sender, RoutedEventArgs e) => SetFilter(sender, CustomerListView_Filter);

        private void AppUserBtn_Checked(object sender, RoutedEventArgs e) => SetFilter(sender, AppUserListView_Filter);


        #endregion

        #region Filters

        private void CustomerListView_Filter(object sender, FilterEventArgs e)
        {
            e.Accepted = false;
            var user = (User)e.Item;

            if (user.UserType == UserType.Customer)
                e.Accepted = true;
        }

        private void AppUserListView_Filter(object sender, FilterEventArgs e)
        {
            e.Accepted = false;
            var user = (User)e.Item;

            if (user.UserType == UserType.AppUser)
                e.Accepted = true;
        }

        private void UserFilter_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (((TextBox)sender).Text.Length > 0)
                _collectionViewSource.Filter += UserFilter;
            else
            {
                _collectionViewSource.Filter -= UserFilter;
                CheckedRadioBtn();
            }

            RefreshList();
        }

        private void CheckedRadioBtn()
        {
            List<RadioButton> radioButtons = MainGrid.Children.OfType<RadioButton>().ToList();
            RadioButton radioButton = radioButtons.First(r => r.IsChecked == true);
            radioButton.IsChecked = false;
            radioButton.IsChecked = true;
        }

        private void RefreshList()
        {
            var view = (CollectionView)
                CollectionViewSource.GetDefaultView(UserList.ItemsSource);
            view.Refresh();
        }

        private void UserFilter(object sender, FilterEventArgs e)
        {
            e.Accepted = !(e?.Item is User) || ((User)e.Item).LastName.ToUpper().Contains(SearchTxt.Text.ToUpper());
        }

        private void UserListView_Filter(object sender, FilterEventArgs e) => e.Accepted = true;

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

        #endregion

        #region Button

        private void CloseBtn_Click(object sender, RoutedEventArgs e) => this.Close();

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            var userWindow = Container.Resolve<UserWindow>();
            userWindow.DataContext = UserList.SelectedItem;
            userWindow.SetEditingMode(true);
            userWindow.ShowDialog();
            RefreshList();
        }

        private void StatBtn_Click(object sender, RoutedEventArgs e)
        {
            var statisticWindow = Container.Resolve<StatisticWindow>();
            statisticWindow.StatisticType = StatisticType.User;
            statisticWindow.TransactionGuid = (((User)UserList.SelectedItem).UserId);
            statisticWindow.ShowDialog();
        }

        private void NewBtn_Click(object sender, RoutedEventArgs e) => AddUser();

        #endregion

        #region Sorting

        private void Sorting_Click(object sender, RoutedEventArgs e)
        {
            var header = (sender as GridViewColumnHeader);
            var columnNameToSort = header.Name;

            var howToSort = ListSortDirection.Ascending;
            var view = (CollectionView)CollectionViewSource.GetDefaultView(UserList.ItemsSource);

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

        private void RemoveCompany(object sender, RoutedEventArgs routedEventArgs)
        {
            if (UserList.SelectedItem != null)
            {
                var userToRemove = (User)UserList.SelectedItem;
                _db.RemoveUser(userToRemove.UserId);
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
        private void AddUser()
        {
            var userWindow = Container.Resolve<UserWindow>();
            userWindow.SetEditingMode(false);
            userWindow.ShowDialog();
            DataContext = _db.GetAll();
            RefreshList();
        }

        #endregion
    }
}
