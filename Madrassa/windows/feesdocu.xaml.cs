using Madrassa.Classes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Xceed.Wpf.AvalonDock.Themes;

namespace Madrassa.windows
{
    /// <summary>
    /// Interaction logic for feesdocu.xaml
    /// </summary>
    public partial class feesdocu : Window
    {
        public feesdocu(string select,string year, string month)
        {
            InitializeComponent();
            GenerateFixedDocument(select,year,month);
        }
        public string connectionString = @"Data Source=DESKTOP-FCU0PEP\SQLEXPRESS;Initial Catalog=Madrassams;Integrated Security=True";
        private void GenerateFixedDocument(string select, string year, string month)
        {
            List<feesclass> feesfilter = new List<feesclass>();
            try
            {
                // 1. Retrieve student data from the database.

                SqlConnection scon = new SqlConnection(connectionString);
                scon.Open();
                string query = "SELECT fees.*, addmission1.name FROM fees INNER JOIN addmission1 ON fees.studentid = addmission1.id INNER JOIN Classes ON addmission1.darja = Classes.classname  WHERE Classes.classname = N'" + select + "' AND YEAR(fees.date) = '" + year+ "' AND MONTH(fees.date) = '" + month + "'";
                feesclass f1;
                SqlCommand sc = new SqlCommand(query, scon);
                SqlDataReader reader = sc.ExecuteReader();
                while (reader.Read())
                {
                    f1 = new feesclass()
                    {
                        id = (int)reader[0],
                        stdid = (int)reader[1],
                        amount = (int)reader[2],
                        date = (DateTime)reader[3],
                        Name = (string)reader[4]
                    };
                    feesfilter.Add(f1);
                }
                
                scon.Close();

                // 2. Create a method to generate the file content.

           
            // Create a new FixedDocument
            FixedDocument fixedDocument = new FixedDocument();

            // Sample collection of invoices (replace it with your actual collection)


            // Define positions for each invoice on the page
            double startX = 50;
            double startY = 50;
            double invoiceWidth = 300;
            double invoiceHeight = 150;
            double spacing = 20; // Spacing between invoices

            // Create a new FixedPage
            FixedPage fixedPage = new FixedPage();
            fixedPage.Width = 800; // Adjust page width
            fixedPage.Height = 600; // Adjust page height

            // Add invoices to the fixed page
            foreach (var invoice in feesfilter)
            {
                // Create a new TextBlock for the invoice details
                TextBlock textBlock = new TextBlock();
                textBlock.Text = $"Invoice Number: {invoice.stdid}\n" +
                                  $"Date: {invoice.date}\n" +
                                  $"Customer Name: {invoice.Name}\n" +
                                  $"Total Amount: {invoice.amount:C}\n";
                textBlock.Margin = new Thickness(startX, startY, 0, 0);

                // Add the TextBlock to the FixedPage
                fixedPage.Children.Add(textBlock);

                // Update position for the next invoice
                startX += invoiceWidth + spacing;

                // If the next invoice exceeds the page width, move to the next row
                if (startX + invoiceWidth > fixedPage.Width)
                {
                    startX = 50; // Reset X position
                    startY += invoiceHeight + spacing; // Move to the next row
                }

                // If the next invoice exceeds the page height, create a new page
                if (startY + invoiceHeight > fixedPage.Height)
                {
                    // Add the current fixedPage to the FixedDocument
                    PageContent pageContent = new PageContent();
                    ((IAddChild)pageContent).AddChild(fixedPage);
                    fixedDocument.Pages.Add(pageContent);

                    // Create a new FixedPage for the next page
                    fixedPage = new FixedPage();
                    fixedPage.Width = 800; // Adjust page width
                    fixedPage.Height = 600; // Adjust page height

                    // Reset positions for the new page
                    startX = 50;
                    startY = 50;
                }
            }

            // Add the last fixedPage to the FixedDocument
            PageContent lastPageContent = new PageContent();
            ((IAddChild)lastPageContent).AddChild(fixedPage);
            fixedDocument.Pages.Add(lastPageContent);
            
            // Set the document to the DocumentViewer
            documentViewer.Document = fixedDocument;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("An error occurred: " + ex.Message);
            }
        }
    }
}
