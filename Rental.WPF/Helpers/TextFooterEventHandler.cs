using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.IO;
using iText.IO.Font.Constants;
using iText.Kernel.Events;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Properties;

namespace Rental.WPF.Helpers
{
    public class TextFooterEventHandler : IEventHandler
    {
        protected Document doc;
        public static string FONT = "c:/windows/fonts/calibri.ttf";

        public TextFooterEventHandler(Document doc)
        {
            this.doc = doc;
        }


        public void HandleEvent(Event currentEvent)
        {
            PdfDocumentEvent docEvent = (PdfDocumentEvent)currentEvent;
            Rectangle pageSize = docEvent.GetPage().GetPageSize();
            PdfFont font = null;
            try
            {
                font = PdfFontFactory.CreateFont(FONT, "Cp1250", true);
            }
            catch (IOException e)
            {
                Console.Error.WriteLine(e.Message);
            }

            float coordX = ((pageSize.GetLeft() + doc.GetLeftMargin())
                            + (pageSize.GetRight() - doc.GetRightMargin())) / 2;
            PdfPage page = docEvent.GetPage();
            int pageNumber = docEvent.GetDocument().GetPageNumber(page);
            float footerY = doc.GetBottomMargin();
            Canvas canvas = new Canvas(docEvent.GetPage(), pageSize);
            canvas
                .SetFont(font)
                .SetFontSize(5)
                .ShowTextAligned("Wypożyczalnia narzędzi - Rafał Będkowski   Strona " + pageNumber, coordX, footerY, TextAlignment.CENTER)
                .Close();
        }
    }
}
