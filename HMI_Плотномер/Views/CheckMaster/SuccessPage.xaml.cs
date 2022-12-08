using IDensity.Services.CheckServices;
using IDensity.ViewModels.MasrerSettings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IDensity.Views.CheckMaster
{
    /// <summary>
    /// Interaction logic for SuccessPage.xaml
    /// </summary>
    public partial class SuccessPage : Page
    {
        private readonly CheckMasterVm _checkMasterVm;

        public SuccessPage(CheckMasterVm checkMasterVm)
        {
            InitializeComponent();
            _checkMasterVm = checkMasterVm;
            DataContext = checkMasterVm;           

        }

        

       

        

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            //FlowDocument doc = new FlowDocument();
            //AddDocument(Document, doc);
            //foreach (var block in Document.Blocks)
            //{
            //    AddBlock(block, doc);
            //}

            PrintDialog printDialog = new PrintDialog();
            var doc = pageViewer.Document;
            double pageHeight = doc.PageHeight;
            double pageWidth = doc.PageWidth;
            Thickness pagePadding = doc.PagePadding;
            double columnGap = doc.ColumnGap;
            double columnWidth = doc.ColumnWidth;
            doc.PageHeight = printDialog.PrintableAreaHeight;
            doc.PageWidth = printDialog.PrintableAreaWidth;
            doc.PagePadding = new Thickness(10);
            doc.ColumnWidth = doc.PageWidth - doc.PagePadding.Left - doc.PagePadding.Right;

            if (printDialog.ShowDialog() == true)
            {

                printDialog.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, "Напечатай меня?");

                doc.PageHeight = pageHeight;
                doc.PageWidth = pageWidth;
                doc.PagePadding = pagePadding;
                doc.ColumnGap = columnGap;
                doc.ColumnWidth = columnWidth;
                pageViewer.IncreaseZoom();

            }

        }

        /// <summary>
        /// Adds one flowdocument to another.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        public static void AddDocument(FlowDocument from, FlowDocument to)
        {
            TextRange range = new TextRange(from.ContentStart, from.ContentEnd);
            using (MemoryStream stream = new MemoryStream())
            {
                System.Windows.Markup.XamlWriter.Save(range, stream);
                range.Save(stream, DataFormats.XamlPackage);
                TextRange textRange2 = new TextRange(to.ContentEnd, to.ContentEnd);
                textRange2.Load(stream, DataFormats.XamlPackage);
            }
        }

        /// <summary>
        /// Adds a block to a flowdocument.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        public static void AddBlock(Block from, FlowDocument to)
        {
            if (from != null)
            {
                TextRange range = new TextRange(from.ContentStart, from.ContentEnd);
                MemoryStream stream = new MemoryStream();
                System.Windows.Markup.XamlWriter.Save(range, stream);
                range.Save(stream, DataFormats.XamlPackage);
                TextRange textRange2 = new TextRange(to.ContentEnd, to.ContentEnd);
                textRange2.Load(stream, DataFormats.XamlPackage);
            }
        }
    }
}
