using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Printing;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Events;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.Win32;
using Newtonsoft.Json;
using Rental.BLL.IRepository;
using Rental.DAL.Models;
using Border = iText.Layout.Borders.Border;
using DateTime = System.DateTime;
using HorizontalAlignment = iText.Layout.Properties.HorizontalAlignment;
using Image = iText.Layout.Element.Image;
using IOException = iText.IO.IOException;
using TextAlignment = iText.Layout.Properties.TextAlignment;
using Transaction = Rental.DAL.Models.Transaction;
using VerticalAlignment = iText.Layout.Properties.VerticalAlignment;

namespace Rental.WPF.Helpers
{
    class PrintTransaction
    {
        private readonly Transaction _transaction;
        private readonly IList<Tool> _tools;
        private const string SettingsFileName = "settings.json";
        private const string reportDirectory = "ReportDirectory";

        private SettingsViewModel _settings = new SettingsViewModel();
        private readonly ITransactionRepository _db;
        private decimal _suma = 0;

        public static string FONT = "c:/windows/fonts/Calibri.ttf";

        public PrintTransaction(Transaction transaction, IList<Tool> tools, ITransactionRepository db)
        {
            _transaction = transaction;
            _tools = tools;
            _db = db;
            ReadSettings();
        }

        public bool Print()
        {
            var fileName = "Transaction" + _transaction.TransactionNumber + ".pdf";
            var filePath = Path.Combine(CheckDirectory(), fileName);
            Image logo = new Image(ImageDataFactory.Create(_settings.LogoUrl))
                .SetWidth(200)
                .SetMaxWidth(400)
                .SetHorizontalAlignment(HorizontalAlignment.LEFT);
            try
            {
                createPdf(filePath, logo);
            }
            catch (Exception e)
            {
                MessageBox.Show("Błąd tworzenia pliku PDF : " + e.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            PrintFile(filePath);
            return true;
        }

        #region Helpers

        private string CheckDirectory()
        {
            var currentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var path = Path.Combine(currentDirectory, reportDirectory);

            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
            catch (Exception e)
            {
                MessageBox.Show("Błąd katalogu wydruków : " + e.Message, "Uwaga", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return path;
        }

        private void ReadSettings()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), SettingsFileName);
            var fileExist = File.Exists(filePath);

            if (!fileExist) return;
            using (var file = File.OpenText(filePath))
            {
                var serializer = new JsonSerializer();
                var model =
                    (SettingsViewModel)serializer.Deserialize(file, typeof(SettingsViewModel));
                _settings = model;
            }
        }

        private void PrintFile(string Filepath)
        {
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo()
            {
                CreateNoWindow = true,
                Verb = "print",
                FileName = Filepath //put the correct path here
            };
            p.Start();
        }

        public virtual void createPdf(String dest, Image logo)
        {
            PdfWriter writer = new PdfWriter(dest);
            PdfDocument pdf = new PdfDocument(writer);
            PdfFont font = PdfFontFactory.CreateFont(FONT, "Cp1250", true);
            Document document = new Document(pdf);
            pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, new TextFooterEventHandler(document));

            #region RentalHeaderTable

            Table rentalHeaderTable = new Table(UnitValue.CreatePercentArray(2))
                .UseAllAvailableWidth();

            Cell logoImage = new Cell()
                .Add(logo)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetBorder(Border.NO_BORDER);
            Cell transactionData = new Cell()
                .Add(new Paragraph("Data wystawienie : " + DateTime.Now).SetTextAlignment(TextAlignment.RIGHT).SetFontSize(10))
                .Add(new Paragraph("Data transakcji : " + _transaction.TransactionDate).SetTextAlignment(TextAlignment.RIGHT).SetFontSize(10))
                .Add(new Paragraph("Miejsce wystawienia : " + _settings.City).SetTextAlignment(TextAlignment.RIGHT).SetFontSize(10))
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetBorder(Border.NO_BORDER);

            rentalHeaderTable.AddCell(logoImage);
            rentalHeaderTable.AddCell(transactionData);

            #endregion

            #region UsersDataHeaderTable

            Table usersDataHeaderTable = new Table(UnitValue.CreatePercentArray(2))
                .UseAllAvailableWidth();

            Cell OwnerHeader = new Cell()
                .Add(new Paragraph("Wypożyczalnia :"))
                .SetTextAlignment(TextAlignment.LEFT)
                .SetBorder(Border.NO_BORDER);


            Cell CustomerHeader = new Cell()
                .Add(new Paragraph("Wypożyczający :"))
                .SetTextAlignment(TextAlignment.LEFT)
                .SetBorder(Border.NO_BORDER);

            usersDataHeaderTable.AddCell(OwnerHeader);
            usersDataHeaderTable.AddCell(CustomerHeader);

            #endregion

            #region UserDataHeaderTable

            Table userDataHeaderTable = new Table(UnitValue.CreatePercentArray(2))
                .UseAllAvailableWidth()
                .SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE)
                .SetHorizontalBorderSpacing(10);

            Cell OwnerData = new Cell()
                .SetTextAlignment(TextAlignment.LEFT)
                .SetPaddingLeft(5)
                .SetBorderRadius(new BorderRadius(4))
                .Add(new Paragraph(_settings.CompanyName))
                .Add(new Paragraph(_settings.PostalCode + " " + _settings.City))
                .Add(new Paragraph(_settings.Address))
                .Add(new Paragraph("Email : " + _settings.Email))
                .Add(new Paragraph("Telefon : " + _settings.Phone));

            Cell CustomerData = new Cell()
                .SetTextAlignment(TextAlignment.LEFT)
                .SetPaddingLeft(5)
                .SetBorderRadius(new BorderRadius(4))
                .Add(new Paragraph(_transaction.Customer.Company.CompanyName))
                .Add(new Paragraph(_transaction.Customer.FullName))
                .Add(new Paragraph(_transaction.Customer.Company.PostCode + " " + _transaction.Customer.Company.City))
                .Add(new Paragraph("Email : " + _transaction.Customer.Company.Address))
                .Add(new Paragraph("Telefon : " + _transaction.Customer.Phone));

            Paragraph transactionNumber = new Paragraph(_transaction.TransactionType == TransactionType.Rent ? "Wypozyczenie nr : " + _transaction.TransactionNumber.ToString() : "Zwrot nr : " + _transaction.TransactionNumber.ToString());
            transactionNumber
                .SetFontSize(20)
                .SetBold()
                .SetMarginTop(10)
                .SetTextAlignment(TextAlignment.CENTER);

            userDataHeaderTable.SetHorizontalAlignment(HorizontalAlignment.CENTER);
            userDataHeaderTable.AddCell(OwnerData);
            userDataHeaderTable.AddCell(CustomerData);

            #endregion

            #region TransactionToolsTable

            float[] columnWidths = { 1, 4, 4, 4, 2 };
            Table transactionToolsTable = new Table(UnitValue.CreatePercentArray(columnWidths)).UseAllAvailableWidth()
                .SetMarginTop(10);

            transactionToolsTable.AddHeaderCell("Lp.");
            transactionToolsTable.AddHeaderCell("Nazwa");
            transactionToolsTable.AddHeaderCell("Producent");
            transactionToolsTable.AddHeaderCell("SN");
            transactionToolsTable.AddHeaderCell("Cena r-d");
            transactionToolsTable.GetHeader()
                .SetBackgroundColor(DeviceGray.GRAY)
                .SetFontColor(DeviceRgb.WHITE)
                .SetBold();

            for (int i = 0; i < _tools.Count; i++)
            {
                transactionToolsTable.AddCell((i + 1) + ".");
                transactionToolsTable.AddCell(_tools[i].Name);
                if (_tools[i].Producer != null)
                    transactionToolsTable.AddCell(_tools[i].Producer.CompanyName);
                else
                    transactionToolsTable.AddCell(new Paragraph(""));
                transactionToolsTable.AddCell(_tools[i].Sn);
                transactionToolsTable.AddCell(_tools[i].RentalPrice + " zł");
            }

            #endregion

            #region RentalPrice

            Table value = new Table(1)
                .SetHorizontalAlignment(HorizontalAlignment.RIGHT);
            Cell cell = new Cell()
                .SetBorder(Border.NO_BORDER);
            Paragraph sum = new Paragraph();

            if (_transaction.TransactionType == TransactionType.Return)
            {
                foreach (var tool in _tools)
                {
                    var price = _db.GetPriceForRent(tool.ToolId, _transaction.TransactionDate);
                    cell.Add(new Paragraph(tool.Name + " dni: " + (price / tool.RentalPrice) + " x cena: " + _transaction.PriceForRent + " = wartość: " + price + " zł").SetBorder(Border.NO_BORDER))
                        .SetTextAlignment(TextAlignment.RIGHT)
                        .SetBorder(Border.NO_BORDER)
                        .SetFontSize(10);
                    _suma += price;
                    value.AddCell(cell);
                }

                sum.Add("RAZEM : " + _suma + " zł")
                    .SetFontSize(15)
                    .SetBold()
                    .SetTextAlignment(TextAlignment.RIGHT);
            }

            #endregion

            #region Condidions

            Paragraph conditionsHeader = new Paragraph()
                .Add("Warunki wypożyczenie :")
                .SetMarginTop(20)
                .SetBold();

            Paragraph conditions = new Paragraph()
                .Add(_settings.Conditions)
                .SetMarginTop(10)
                .SetItalic();

            #endregion

            #region Signature

            Table signatureTable = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            signatureTable.SetMarginTop(50);
            signatureTable.SetBorder(Border.NO_BORDER);

            Cell User = new Cell()
                .Add(new Paragraph(_transaction.AppUser.FullName))
                .Add(new Paragraph("............................................................"))
                .Add(new Paragraph("data i podpis wydającego"))
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(10)
                .SetBorder(Border.NO_BORDER);

            Cell Customer = new Cell()
                .Add(new Paragraph(_transaction.Customer.FullName))
                .Add(new Paragraph("............................................................"))
                .Add(new Paragraph("data i podpis wypożyczającego"))
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(10)
                .SetBorder(Border.NO_BORDER);

            signatureTable.AddCell(User);
            signatureTable.AddCell(Customer);

            #endregion

            #region CreateDocument

            document.SetFont(font);
            document.Add(rentalHeaderTable);
            document.Add(usersDataHeaderTable);
            document.Add(userDataHeaderTable);
            document.Add(transactionNumber);
            document.Add(transactionToolsTable);
            document.Add(value);
            document.Add(sum);
            document.Add(conditionsHeader);
            document.Add(conditions);
            document.Add(signatureTable);
            document.Close();

            #endregion
        }

        #endregion

    }
}
