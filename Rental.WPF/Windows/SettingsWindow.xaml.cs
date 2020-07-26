using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Newtonsoft.Json;
using Rental.WPF.Helpers;
using Path = System.IO.Path;

namespace Rental.WPF.Windows
{
    public partial class SettingsWindow : Window
    {
        private bool _isEdited = false;
        private const string SettingsFileName = "settings.json";
        private const string AppDirectory = "Rental";

        public SettingsWindow()
        {
            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
        }

        #region Initialization

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ReadFromFile();
            SetEdited(false);
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

        #endregion

        #region Button

        private void CancelBtn_Click(object sender, RoutedEventArgs e) => this.Close();
        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveToFile();
            SetEdited(false);
            this.Close();
        }
        private void LogoBtn_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() != true) return;
            var fileUri = new Uri(openFileDialog.FileName);
            LogoUrl.Source = new BitmapImage(fileUri);
        }

        #endregion

        #region FileOperation

        private void SaveToFile()
        {
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppDirectory, SettingsFileName);
            var logoUrl = LogoUrl.Source != null ? LogoUrl.Source.ToString() : "";
            var settings = new SettingsViewModel
            {
                CompanyName = CompanyName.Text,
                PostalCode = PostalCode.Text,
                City = City.Text,
                Address = Address.Text,
                Email = Email.Text,
                Phone = Phone.Text,
                Conditions = Conditions.Text,
                LogoUrl = logoUrl
            };

            try
            {
                using (var sw = File.CreateText(filePath))
                {
                    var serializer = new JsonSerializer();
                    serializer.Serialize(sw, settings);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Błąd zapisu do pliku konfiguracyjnego !" + "Erro : " + e.Message, "Uwaga !", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void ReadFromFile()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), SettingsFileName);
            var fileExist = File.Exists(filePath);

            if (!fileExist) return;
            using (var file = File.OpenText(filePath))
            {
                var serializer = new JsonSerializer();
                var model =
                    (SettingsViewModel)serializer.Deserialize(file, typeof(SettingsViewModel));
                DataContext = model;
            }
        }

        #endregion

        #region Edit

        private void TextBox_Changed(object sender, TextChangedEventArgs e)
        {
            if (this.IsLoaded)
                SetEdited(true);
        }

        public void SetEdited(bool edited) => _isEdited = edited;

        #endregion

    }
}
