using iTextSharp.text.pdf;
using Madrassa.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.Metadata;
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

namespace Madrassa
{
    /// <summary>
    /// Interaction logic for Result.xaml
    /// </summary>
    public partial class Result : Window
    {
        string examn;
        List<studentresult> studentResults = new List<studentresult>();
        public string connectionString = @"Data Source=DESKTOP-FCU0PEP\SQLEXPRESS;Initial Catalog=Madrassams;Integrated Security=True";
        public Result(string exam2)
        {
            InitializeComponent();
            this.examn = exam2; Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            List<studentresult> studentResults = GetStudentResultsFromDatabase();

            // Bind the list of StudentResult objects to the DataGrid
            resultexam.ItemsSource = studentResults;
        }
        public List<studentresult> GetStudentResultsFromDatabase()
            {
               

                // Assuming conn is a SqlConnection object already initialized with the connection string
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT studentid, subject, obtmarks FROM stdexamresult";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            studentresult result = new studentresult()
                            {
                                Student = (int)reader[0],
                                Subject = (string)reader[1],
                                Marks = (int)reader[2]
                            };
                            studentResults.Add(result);
                        }
                    }
                }

                return studentResults;
            }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }
       
        
    }
    public class studentresult
    {
        public int Student { get; set; }
        public string Subject { get; set; }
        public int Marks { get; set; }
    }
}
