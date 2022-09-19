using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace WpfTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();           
        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            FlowDocument doc = new FlowDocument();
            AddDocument(Viewer.Document, doc);


            PrintDialog printDialog = new PrintDialog();
            //var doc = Viewer.Document;
            double pageHeight = doc.PageHeight;
            double pageWidth = doc.PageWidth;
            Thickness pagePadding = doc.PagePadding;
            double columnGap = doc.ColumnGap;
            double columnWidth = doc.ColumnWidth;
            doc.PageHeight = printDialog.PrintableAreaHeight;
            doc.PageWidth = printDialog.PrintableAreaWidth;
            doc.PagePadding = new Thickness(10);

            //doc.ColumnWidth = doc.PageWidth - doc.PagePadding.Left - doc.PagePadding.Right;

            if (printDialog.ShowDialog() == true)
            {

                printDialog.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, "Напечатай меня?");

                doc.PageHeight = pageHeight;
                doc.PageWidth = pageWidth;
                doc.PagePadding = pagePadding;
                doc.ColumnGap = columnGap;
                doc.ColumnWidth = columnWidth;


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
            MemoryStream stream = new MemoryStream();
            System.Windows.Markup.XamlWriter.Save(range, stream);
            range.Save(stream, DataFormats.XamlPackage);
            TextRange range2 = new TextRange(to.ContentEnd, to.ContentEnd);
            range2.Load(stream, DataFormats.XamlPackage);
        }
    }
}
