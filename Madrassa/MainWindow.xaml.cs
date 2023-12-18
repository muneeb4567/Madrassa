
using CsvHelper;
using CsvHelper.Configuration;
using Madrassa.Classes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Formats.Asn1;
using System.IO;
using System.Linq;
using System.Printing.IndexedProperties;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using GDIImage = System.Drawing.Image;


namespace Madrassa
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public string connectionString = @"Data Source=DESKTOP-FCU0PEP\SQLEXPRESS;Initial Catalog=Madrassams;Integrated Security=True";
        public string a;
        students std = new students();
       
        public MainWindow()
        {
            InitializeComponent();

            std.studentsget();
            std.getallclass();
            std.getallexams();
            std.getallsubjects();
            //std.getallresutl();
            std.getallsubmarks();
            std.getfees();
            
            examsgb.ItemsSource = std.exams;
            booksdg.ItemsSource = std.Subjects;
            marksdg.ItemsSource = std.allsubjectmarks;
            classesdg.ItemsSource = std.allClasses;
            
            datagfees.ItemsSource = std.stdfees;
            datagridstdp.ItemsSource = std.studentall;
            getcombodata();
            getexams();
            getcombosubject();
        }
        
        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 1. Retrieve student data from the database.
                

                // 2. Create a method to generate the file content.
                string csvContent = GenerateCsvContent(std.studentall);

                // 3. Save the content to a file and offer download.
                string filePath = "StudentData.csv"; // File path where the CSV will be saved
                File.WriteAllText(filePath, csvContent);

                // 4. Prompt the user to save or open the file.
                MessageBox.Show("Student data has been exported to StudentData.csv. Click OK to open the file location.");
                System.Diagnostics.Process.Start("explorer.exe", "/select, " + filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private string GenerateCsvContent(IEnumerable<studentdata> students)
        {
            using (var writer = new StringWriter())
            using (var csv = new CsvWriter(writer, new CsvConfiguration(System.Globalization.CultureInfo.CurrentCulture)))
            {
                csv.WriteRecords(students);
                return writer.ToString();
            }
        }
       

        private void datagridstdp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void images_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.Image clickedImage = (System.Windows.Controls.Image)sender;

            // Retrieve the image source
            BitmapSource imageSource = (BitmapSource)clickedImage.Source;
            addimage largeImageWindow = new addimage(imageSource);
            largeImageWindow.Topmost = true;
            largeImageWindow.Show();
        }
        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "Search by Name or Father's Name")
            {
                textBox.Text = "";
            }
        }
        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Search by Name or Father's Name";
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            datagridstdp.ItemsSource = null;
            std.studentsget();
            datagridstdp.ItemsSource = std.studentall;
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get selected items (rows)
                List<studentdata> selectedStudents = datagridstdp.SelectedItems.Cast<studentdata>().ToList();

                // Delete selected students from the database
                foreach (studentdata student in selectedStudents)
                {
                    try
                    {
                        SqlConnection sqlConnection = new SqlConnection(connectionString);
                        sqlConnection.Open();
                        string query = "delete from addmissionform where id='" + student.Id + "'";
                        SqlCommand sc = new SqlCommand(query, sqlConnection);
                        sc.ExecuteNonQuery();
                        students stds = new students();
                        stds.studentsget();
                        datagridstdp.ItemsSource = null;
                        datagridstdp.ItemsSource = stds.studentall;
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                }

                // Remove selected students from the collection

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting students: {ex.Message}");
            }
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string searchText = searchBox.Text.Trim().ToLower();

            students stds = new students();
            stds.studentsget();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                // If the search box is empty, display all students
                datagridstdp.ItemsSource = stds.studentall;
            }
            else
            {
                // Filter students based on the search criteria
                var filteredStudents = stds.studentall.Where(student =>
            student.name.ToLower().Contains(searchText) || // Search by name
            student.fname.ToLower().Contains(searchText) || // Search by father's name
                                                            // student.FatherContact.ToLower().Contains(searchText) || // Search by father's contact number
                                                            //  student.StudentContact.ToLower().Contains(searchText) || // Search by student's contact number
            student.Id.ToString().Contains(searchText) || // Search by ID (assuming ID is an integer)
            student.sarfname.ToLower().Contains(searchText) // Search by guardian name
        ).ToList();
                // Display the filtered students in the DataGrid
                datagridstdp.ItemsSource = filteredStudents;
            }
        }
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try { 
                studentdata std = new studentdata();
                std.name = stdname.Text;
                std.fname = fathername.Text;
                std.sarfname = sarfrastname.Text;
                std.sarfcon = decimal.Parse(sarfrastnum.Text);
                std.sarfcnic = decimal.Parse(sarfrastcnic.Text);
                std.stdcnic = decimal.Parse(stdcnic.Text);
                std.sarfaresh = sarfrastreshta.Text;
                std.fees = int.Parse(fees.Text);
                std.dateofbirth = DateTime.Parse(dateofb.Text);
                std.dateofdahila = DateTime.Parse(dateofadd.Text);
                std.darga = darja.Text;
                std.address = textaddress.Text;
                std.nughait1 = jadeedkadeem.Text;
                std.nughait2 = moqeemgh.Text;
                std.stdnum = decimal.Parse(stdnum.Text);
                std.image = File.ReadAllBytes(a);

                students stds = new students();
                stds.addstd(std);

                stdname.Text=null;
                fathername.Text=null;
                sarfrastname.Text = null ;
                sarfrastnum.Text=null;
                sarfrastcnic.Text = null;
                stdcnic.Text = null;
                sarfrastreshta.Text = null;
                fees.Text = null;
                dateofb.Text = null;
                dateofadd.Text = null;
                darja.Text = null;
                textaddress.Text = null;
                jadeedkadeem.Text = null;
                moqeemgh.Text = null;
                stdnum.Text = null;
                image.Source = null;
            }catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        private void Button_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            bool? b = ofd.ShowDialog();
            if (b == true)
            {
                a = ofd.FileName;
                image.Source = new BitmapImage(new Uri(ofd.FileName));

            }
        }
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {

        }

        private void year_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void refresh_Click(object sender, RoutedEventArgs e)
        {
            datagfees.ItemsSource = null;
            students stds = new students();
            stds.getfees();
            datagfees.ItemsSource= stds.stdfees;
        }

        private void btnclasscreate_Click(object sender, RoutedEventArgs e)
        {
            if (txtclass.Text != "")
            {
                classes clas = new classes();
                clas.classname = txtclass.Text;
                students std = new students();
                std.createclass(clas);
                txtclass.Text = null;
                getcombodata();
                std.getallclass();
                classesdg.ItemsSource = std.allClasses;
            }
            else if(txtclass.Text=="") { MessageBox.Show("empty text"); }
        }
        public void getcombodata()
        {
            try
            {
                List<string> allclass = new List<string>();
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
                string sqlQuery = "SELECT * FROM Classes";
                SqlCommand command = new SqlCommand(sqlQuery, sqlConnection);
                SqlDataReader dreader = command.ExecuteReader();
                while (dreader.Read())
                {
                    allclass.Add((string)dreader[1]);

                }
                txtclassid.ItemsSource = allclass;
                classname_exam.ItemsSource = allclass;
                classcombo_search.ItemsSource = allclass;
                classfilter_exam.ItemsSource= allclass;
                darja.ItemsSource = allclass;
                classcombo_fees.ItemsSource = allclass;
                class_feesunpay.ItemsSource = allclass;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        public void getcombosubject()
        {
           try
           {
                            List<string> list = new List<string>();
                            SqlConnection sqlConnection = new SqlConnection(connectionString);
                            sqlConnection.Open(); 
                            string sqlQuery = "SELECT * FROM Subjects1 ";
                            SqlCommand command = new SqlCommand(sqlQuery, sqlConnection);
                            SqlDataReader dreader = command.ExecuteReader();
                            while (dreader.Read())
                            {
                                list.Add((string) dreader[1]);

                            }
                            txtsubjectid.ItemsSource = list;
           }
                        catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        public void getexams()
        {
            try
            {
                List<exam> examlist = new List<exam>();
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
                string sqlQuery = "SELECT * FROM Exams";
                SqlCommand command = new SqlCommand(sqlQuery, sqlConnection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    exam ex = new exam();
                    ex.id = (int)reader[0];
                    ex.examname = (string)reader[1];
                    ex.date = (DateTime)reader[2];
                    examlist.Add(ex);
                }
                examcombo.ItemsSource = examlist;
                examcombo_search.ItemsSource = examlist;
                sqlConnection.Close();
                reader.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void btnsubjectcreate(object sender, RoutedEventArgs e)
        {
            if (txtsubject.Text != "")
            {
                subject sub = new subject();
                sub.subjectname = txtsubject.Text;
                students stds = new students();
                stds.createsubject(sub);
                txtsubject.Text = null;
                getcombosubject();
                stds.getallsubjects();
                booksdg.ItemsSource = stds.Subjects;
            }
            else
            {
                MessageBox.Show("emty txt subject");
            }
        }
        private void btnsubjectmarks_Click(object sender, RoutedEventArgs e)
        {
            if (txtclassid.SelectedItem != null && txtsubjectid.SelectedItem != null && txtmaxmerks.Text != "")
            {
                int a = 0, b = 0;
                try
                {
                    SqlConnection sqlConnection = new SqlConnection(connectionString);
                    sqlConnection.Open();
                    string sqlQuery = "SELECT ClassID FROM Classes where classname=N'" + txtclassid.SelectedItem + "'";
                    string sqlquery = "SELECT SubjectID FROM Subjects1 where Subjectname=N'" + txtsubjectid.SelectedItem + "'";
                    SqlCommand command = new SqlCommand(sqlQuery, sqlConnection);
                    SqlCommand command1 = new SqlCommand(sqlquery, sqlConnection);
                    a = (int)command.ExecuteScalar();
                    b = (int)command1.ExecuteScalar();

                }
                catch (Exception ex) { MessageBox.Show("find" + ex.Message); }
                students std = new students();
                classsubjectmarks clasmarks = new classsubjectmarks();
                clasmarks.subjectid = b;
                clasmarks.classid = a;
                clasmarks.maxmarks = int.Parse(txtmaxmerks.Text);
                std.createsubjectmarks(clasmarks);
                txtclassid.SelectedItem = null;
                txtsubjectid.SelectedItem = null;
                txtmaxmerks.Text = null;
                std.getallsubmarks();
                marksdg.ItemsSource = std.allsubjectmarks;
            }
            else { MessageBox.Show("emty"); }

        }
        private void btnexamcreate_Click(object sender,RoutedEventArgs e)
        {
            if (txtexamname.Text != "" && txtexamdate.SelectedDate!= null)
            {
                exam exam1 = new exam();
                exam1.examname = txtexamname.Text;
                exam1.date = (DateTime)txtexamdate.SelectedDate;
                students std = new students();
                std.createexam(exam1);
                getexams();
                std.getallexams();
                txtexamname.Text = null;
                txtexamdate.SelectedDate = null;
                examsgb.ItemsSource = std.exams;
            }
            else { MessageBox.Show("empty txt"); }
        }
        private void submit_exam_Click(object sender, RoutedEventArgs e)
        {
            var selecteditem = examcombo.SelectedItem as exam;
            if (selecteditem != null && classname_exam.SelectedItem !=null&& subjectid.Text!=null&&studentid_exam.Text!=null&&obtmarks.Text!=null)
            {
               string selectedText = $"{selecteditem.examname} - {selecteditem.date.ToString("MM/dd/yyyy")}";
               
                try
                { 
                    stdexamresult stdr = new stdexamresult();
                    stdr.studentid = int.Parse(studentid_exam.Text.ToString());
                    stdr.exam = selectedText;
                    stdr.@class = classname_exam.Text;
                    stdr.subject=subjectid.Text;
                    stdr.obtmarks =int.Parse(obtmarks.Text);
                    std.insertresult(stdr);

                    studentid_exam.Text = null;
                    subjectid.Text = null;
                    obtmarks.Text = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            

        }
        
        private void studentid_exam_SelectionChanged(object sender, RoutedEventArgs e)
        {
            
                try
                {
                var selecteditem = examcombo.SelectedItem as exam;
                if (selecteditem != null)
                {
                    string selectedText = $"{selecteditem.examname} - {selecteditem.date.ToString("MM/dd/yyyy")}";

                    SqlConnection sc = new SqlConnection(connectionString);
                    sc.Open();
                    string query = "select id,name,darja from addmission1 where id='" + studentid_exam.Text + "' and status='Active'";
                    string query1 = "SELECT stdexamresult.* FROM stdexamresult INNER JOIN addmission1 ON stdexamresult.studentid = addmission1.id WHERE studentid='" + studentid_exam.Text + "' and addmission1.status = 'active' and exam=N'" + selectedText + "'";
                    SqlDataAdapter SDA = new SqlDataAdapter(query, sc);
                    SqlDataAdapter SDA1 = new SqlDataAdapter(query1, sc);
                    DataTable dt = new DataTable();
                    SDA.Fill(dt);
                    namedgrid.ItemsSource = dt.DefaultView;
                    DataTable dt1 = new DataTable();
                    SDA1.Fill(dt1);
                    resultexam.ItemsSource = dt1.DefaultView;
                }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            
            
        }
        private void subjectdelete(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get selected items (rows)
                List<subject> subjectsall = booksdg.SelectedItems.Cast<subject>().ToList();
                // Delete selected students from the database
                foreach (subject book in subjectsall)
                {
                    try
                    {
                        SqlConnection sqlConnection = new SqlConnection(connectionString);
                        sqlConnection.Open();
                        string query = "delete from Subjects1 where SubjectID='" + book.subjectid + "'";
                        SqlCommand sc = new SqlCommand(query, sqlConnection);
                        sc.ExecuteNonQuery();
                        students stds = new students();
                        stds.getallsubjects();
                        booksdg.ItemsSource = stds.Subjects;
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                }
                // Remove selected students from the collection
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting students: {ex.Message}");
            }
        }
        private void deleteclassid_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get selected items (rows)
                List<classes> allclass = classesdg.SelectedItems.Cast<classes>().ToList();
                // Delete selected students from the database
                foreach (classes classall in allclass)
                {
                    try
                    {
                        SqlConnection sqlConnection = new SqlConnection(connectionString);
                        sqlConnection.Open();
                        string query = "delete from Classes where ClassID='" + classall.classid + "'";
                        SqlCommand sc = new SqlCommand(query, sqlConnection);
                        sc.ExecuteNonQuery();
                        students stds = new students();
                        stds.getallclass();
                        classesdg.ItemsSource = stds.allClasses;
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                }
                // Remove selected students from the collection
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting students: {ex.Message}");
            }
        }
        private void examdelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get selected items (rows)
                List<exam> allexams = examsgb.SelectedItems.Cast<exam>().ToList();
                // Delete selected students from the database
                foreach (exam exam1 in allexams)
                {
                    try
                    {
                        SqlConnection sqlConnection = new SqlConnection(connectionString);
                        sqlConnection.Open();
                        string query = "delete from Exams where ExamID='" + exam1.id + "'";
                        SqlCommand sc = new SqlCommand(query, sqlConnection);
                        sc.ExecuteNonQuery();
                        students stds = new students();
                        stds.getallexams();
                        examsgb.ItemsSource = stds.exams;
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                }
                // Remove selected students from the collection
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting students: {ex.Message}");
            }
        }
        private void marksexam_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get selected items (rows)
                List<namessubjectmarks> allsubmerks = marksdg.SelectedItems.Cast<namessubjectmarks>().ToList();
                // Delete selected students from the database
                foreach (namessubjectmarks marks in allsubmerks)
                {
                    try
                    {
                        SqlConnection sqlConnection = new SqlConnection(connectionString);
                        sqlConnection.Open();
                        string query = "delete from ClassSubjectMarks1 where ClassSubjectMarkID='" + marks.id + "'";
                        SqlCommand sc = new SqlCommand(query, sqlConnection);
                        sc.ExecuteNonQuery();
                        students stds = new students();
                        stds.getallsubmarks();
                        marksdg.ItemsSource = stds.allsubjectmarks;
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                }
                // Remove selected students from the collection
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting students: {ex.Message}");
            }
        }

        private void classcombo_search_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }

        private void deletebtn_exam_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlConnection scon = new SqlConnection(connectionString);
                scon.Open();
                string query = "delete from stdexamresult where id='" + deletebtn_examid.Text + "'";
                SqlCommand com = new SqlCommand(query, scon);
                com.ExecuteNonQuery();
                studentid_exam.Text = null;
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private bool DeleteExamFromDatabase(stdmarksnames student)
        {
            try
            {
                // Connect to your database and execute the DELETE query to remove the student.
                // Replace "connectionString" with your actual database connection string.
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Create a SQL command to delete the student based on a unique identifier (e.g., student ID).
                    SqlCommand cmd = new SqlCommand("DELETE FROM StudentExamMarks1 WHERE id = @StudentID", connection);
                    cmd.Parameters.AddWithValue("@StudentID", student.id);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // Deletion successful
                        return true;
                    }
                    else
                    {
                        // Deletion failed
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                // Handle database connection and query errors.
                // You may want to log the exception or display an error message.
                return false;
            }
        }


        private void insertfee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();


                // Check if a record for the same student and date already exists
    string checkQuery = "SELECT COUNT(*) FROM fees WHERE studentid = @stdid AND date = @date";

                SqlCommand checkCommand = new SqlCommand(checkQuery, sqlConnection);
                checkCommand.Parameters.AddWithValue("@stdid", stdid_fees.Text);
                checkCommand.Parameters.AddWithValue("@date", date_fees.SelectedDate);

                int existingRecords = (int)checkCommand.ExecuteScalar();

                if (existingRecords == 0)
                {
                    // No record found for the same student and date, so insert a new record
                    string insertQuery = "INSERT INTO fees (studentid, amount, date) VALUES (@stdid, @amount, @date)";
                    SqlCommand insertCommand = new SqlCommand(insertQuery, sqlConnection);
                    insertCommand.Parameters.AddWithValue("@stdid", stdid_fees.Text);
                    insertCommand.Parameters.AddWithValue("@amount", amount.Text);
                    insertCommand.Parameters.AddWithValue("@date", date_fees.SelectedDate);

                    insertCommand.ExecuteNonQuery();
                    stdid_fees.Text = null;
                    amount.Text = null;
                }
                else
                {
                    MessageBox.Show("Fees for this student and date already exist.");
                }

                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        private void stdid_fees_SelectionChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlConnection sc = new SqlConnection(connectionString);
                sc.Open();
                string query = "select name ,fathername, fees from addmission1 where id='" + stdid_fees.Text + "'";
                SqlDataAdapter SDA = new SqlDataAdapter(query, sc);
                DataTable dt = new DataTable();
                SDA.Fill(dt);
                amountdg_fees.ItemsSource = dt.DefaultView;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnsearch_fees_Click(object sender, RoutedEventArgs e)
        {
            if (idsearch_fees.Text != "")
            {
                try
                {
                    SqlConnection sqlc = new SqlConnection(connectionString);
                    sqlc.Open(); List<fees> allfees = new List<fees>();
                    string query = "select * from fees where studentid ='" + idsearch_fees.Text + "'"; fees f1;
                    SqlCommand sc = new SqlCommand(query, sqlc);
                    SqlDataReader reader = sc.ExecuteReader();
                    while (reader.Read())
                    {
                        f1 = new fees()
                        {
                            id = (int)reader[0],
                            stdid = (int)reader[1],
                            amount = (int)reader[2],
                            date = (DateTime)reader[3]
                        }; allfees.Add(f1);

                    }
                    datagfees.ItemsSource = null;
                    datagfees.ItemsSource = allfees;
                    sqlc.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }else if (idsearch_fees.Text == "")
            {
                MessageBox.Show("empty");
            }
            }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Ensure a row is selected
            if (datagfees.SelectedItem != null)
            {
                // Get the selected student
                fees selectedStudent = (fees)datagfees.SelectedItem;
                MessageBox.Show(selectedStudent.stdid + "click");
                // Perform the delete operation in the database
                //bool deleted = DeleteStudentFromDatabase(selectedStudent);

                //if (deleted)
                //{
                //    // Remove the selected student from the DataGrid's item source
                //    datagfees.Items.Remove(selectedStudent);
                //}
                //else
                //{
                //    // Handle the case when the deletion from the database fails (e.g., show an error message).
                //}
            }
        }

        private bool DeleteStudentFromDatabase(fees student)
        {
            try
            {
                // Connect to your database and execute the DELETE query to remove the student.
                // Replace "connectionString" with your actual database connection string.
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Create a SQL command to delete the student based on a unique identifier (e.g., student ID).
                    SqlCommand cmd = new SqlCommand("DELETE FROM fees WHERE studentid = @StudentID", connection);
                    cmd.Parameters.AddWithValue("@StudentID", student.stdid);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // Deletion successful
                        return true;
                    }
                    else
                    {
                        // Deletion failed
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle database connection and query errors.
                // You may want to log the exception or display an error message.
                return false;
            }
        }

        private void classname_exam_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                List<string> classsub = new List<string>();
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
                string sqlquery = "SELECT  s.SubjectName FROM ClassSubjectMarks1 cs INNER JOIN Classes c ON cs.ClassID = c.ClassID INNER JOIN Subjects1 s ON cs.SubjectID = s.SubjectID WHERE c.ClassName =N'" + classname_exam.SelectedItem + "' ";
                SqlCommand command = new SqlCommand(sqlquery, sqlConnection);
                SqlDataReader dreader = command.ExecuteReader();
                while (dreader.Read())
                {
                    classsub.Add((string)dreader[0]);

                }
                dreader.Close();
                subjectid.ItemsSource = classsub;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlConnection scon = new SqlConnection(connectionString);
                scon.Open();
                string query = "SELECT id,name,fathername FROM addmission1  WHERE darja = N'"+classfilter_exam.SelectedItem+"' AND YEAR(dateofdahila) = '"+yearfilter_exam.Text+"' AND Status='Active'";

                SqlDataAdapter SDA = new SqlDataAdapter(query, scon);
                DataTable dt = new DataTable();
                SDA.Fill(dt);
                filterstd_exam.ItemsSource = dt.DefaultView;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void showresult_Click(object sender, RoutedEventArgs e)
        {

            var selecteditem = examcombo_search.SelectedItem as exam;
            string selectedText = $"{selecteditem.examname} - {selecteditem.date.ToString("MM/dd/yyyy")}";
                Result res = new Result(selectedText);
                res.Show();

        }

        private void btnDelete_fees_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get selected items (rows)
                List<fees> selectedStudentsfees = datagfees.SelectedItems.Cast<fees>().ToList();

                // Delete selected students from the database
                foreach (fees student in selectedStudentsfees)
                {
                    try
                    {
                        SqlConnection sqlConnection = new SqlConnection(connectionString);
                        sqlConnection.Open();
                        string query = "delete from fees where id='" + student.id + "'";
                        SqlCommand sc = new SqlCommand(query, sqlConnection);
                        sc.ExecuteNonQuery();
                        students stds = new students();
                        stds.getfees();
                        datagfees.ItemsSource = null;
                        datagfees.ItemsSource = stds.stdfees;
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                }

                // Remove selected students from the collection

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting students: {ex.Message}");
            }
        }

        private void btnfind_fees_Click(object sender, RoutedEventArgs e)
        {

                try
                {
                    SqlConnection sqlc = new SqlConnection(connectionString);
                    sqlc.Open(); fees f1;List<fees> feeslist = new List<fees>();
                string query = "SELECT * FROM fees WHERE YEAR(date) = " + year_fees.Text + " AND MONTH(date) = " + month_fees.Text + " ";
                SqlCommand sc = new SqlCommand(query, sqlc);
                    SqlDataReader reader = sc.ExecuteReader();
                    while (reader.Read())
                    {
                        f1 = new fees()
                        {
                            id = (int)reader[0],
                            stdid = (int)reader[1],
                            amount = (int)reader[2],
                            date = (DateTime)reader[3]
                        };
                        feeslist.Add(f1);
                    }
                datagfees.ItemsSource = feeslist;
                    sqlc.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            if (readdstd_txt.Text != "")
            {
                try
                {
                    SqlConnection scon = new SqlConnection(connectionString);
                    scon.Open();
                    string query = "select * from addmission1 where id='" + readdstd_txt.Text + "'";
                    SqlDataAdapter SDA = new SqlDataAdapter(query, scon);
                    DataTable dt = new DataTable();
                    SDA.Fill(dt);
                    singalstddg.ItemsSource = null;
                    singalstddg.ItemsSource = dt.DefaultView;

                }catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void addstd_active_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlConnection scon = new SqlConnection(connectionString);
                scon.Open();
                string query = "UPDATE addmission1 SET Status = 'Active'  WHERE id ='" + readdstd_txt.Text + "'  AND Status IN ('Active', 'Inactive')";
                SqlDataAdapter SDA = new SqlDataAdapter(query, scon);
                DataTable dt = new DataTable();
                SDA.Fill(dt);
                singalstddg.ItemsSource = null;
                singalstddg.ItemsSource = dt.DefaultView;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void refresh_fees_Click(object sender, RoutedEventArgs e)
        {
            std.stdfees.Clear();
            std.getfees();
            datagfees.ItemsSource = null;
            datagfees.ItemsSource = std.stdfees;
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlConnection sqlc = new SqlConnection(connectionString);
                sqlc.Open(); fees f1; List<fees> feeslist = new List<fees>();
                string query = "SELECT fees.id, addmission1.id, fees.amount, fees.date FROM addmission1 JOIN fees ON addmission1.id = fees.studentid WHERE addmission1.darja = N'"+classcombo_fees.SelectedItem.ToString()+"'";
                SqlCommand sc = new SqlCommand(query, sqlc);
                SqlDataReader reader = sc.ExecuteReader();
                while (reader.Read())
                {
                    f1 = new fees()
                    {
                        id = (int)reader[0],
                        stdid = (int)reader[1],
                        amount = (int)reader[2],
                        date = (DateTime)reader[3]
                    };
                    feeslist.Add(f1);
                }
                datagfees.ItemsSource = feeslist;
                sqlc.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        
        private void show_formadd_Click(object sender, RoutedEventArgs e)
        {
            if (stdid_formadd.Text != "")
            {
                try
                {
                    studentdata std = new studentdata();
                    SqlConnection scon = new SqlConnection(connectionString); scon.Open();
                    string query = "select * from addmission1 where id='" + stdid_formadd.Text + "'";
                    string query1 = "SELECT studentid, exam, SUM(obtmarks) AS total_marks FROM stdexamresult WHERE studentid = '" + stdid_formadd.Text + "' GROUP BY studentid, exam";
                    SqlCommand sqlc = new SqlCommand(query, scon);
                    SqlDataReader sqlreader = sqlc.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        std = new studentdata()
                        {
                            Id = (int)sqlreader[0],
                            name = (string)sqlreader[1],
                            fname = (string)sqlreader[2],
                            sarfname = (string)sqlreader[3],
                            sarfcon = (decimal)sqlreader[4],
                            stdcnic = (decimal)sqlreader[5],
                            sarfcnic = (decimal)sqlreader[6],
                            darga = (string)sqlreader[11],
                            address = (string)sqlreader[12],
                            stdnum = (decimal)sqlreader[14],
                            image = (byte[])sqlreader[16]
                        };


                    }
                    name_formadd.Text = std.name;
                    fname_formadd.Text = std.fname;
                    address_formadd.Text = std.address;
                    contact_formadd.Text = std.stdnum.ToString();
                    cnic_formadd.Text = std.stdcnic.ToString();
                    fcnic_formadd.Text = std.sarfcnic.ToString();
                    BitmapImage bitmapImage = LoadImageFromByteArray(std.image);
                    image_formadd.Source = bitmapImage;
                    sqlreader.Close();
                    SqlDataAdapter SDA = new SqlDataAdapter(query1, scon);
                    DataTable dt = new DataTable();
                    SDA.Fill(dt);
                    std_formadd.ItemsSource = null;
                    std_formadd.ItemsSource = dt.DefaultView;
                    scon.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }else if (stdid_formadd.Text == "")
            {
                MessageBox.Show("رجسٹریشن نمبر درج نہیں کیا گیا ہے");
            }
           
        }

        private BitmapImage LoadImageFromByteArray(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0)
            {
                // Return a default image or handle the empty case
                return new BitmapImage();
            }

            // Create a MemoryStream from the byte array
            using (MemoryStream stream = new MemoryStream(imageData))
            {
                // Create a BitmapImage and set its source to the MemoryStream
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        private void showfeesfile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 1. Retrieve student data from the database.
                List<fees> feesfilter = new List<fees>();
                SqlConnection scon = new SqlConnection(connectionString);
                scon.Open();
                string query = "SELECT fees.*FROM fees INNER JOIN addmission1 ON fees.studentid = addmission1.id INNER JOIN Classes ON addmission1.darja = Classes.classname WHERE Classes.classname = N'"+class_feesunpay.SelectedItem+"'  AND YEAR(fees.date) = '"+year_feesunpay.Text+"' AND MONTH(fees.date) = '"+month_feesunpay.Text+"'";
                fees f1;
                SqlCommand sc = new SqlCommand(query, scon);
                SqlDataReader reader = sc.ExecuteReader();
                while (reader.Read())
                {
                    f1 = new fees()
                    {
                        id = (int)reader[0],
                        stdid = (int)reader[1],
                        amount = (int)reader[2],
                        date = (DateTime)reader[3]
                    };
                    feesfilter.Add(f1);
                }
                scon.Close();

                // 2. Create a method to generate the file content.

                string csvContent = GenerateCsvContent(feesfilter);
                // 3. Save the content to a file and offer download.
                string month = month_feesunpay.Text; // Format as desired
                string year = year_feesunpay.Text;
                string filePath = $"فیس_{month}-{year}.csv"; // File path where the CSV will be saved
                File.WriteAllText(filePath, csvContent);

                // 4. Prompt the user to save or open the file.
                MessageBox.Show("Student data has been exported to StudentData.csv. Click OK to open the file location.");
                System.Diagnostics.Process.Start("explorer.exe", "/select, " + filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }
        private string GenerateCsvContent(IEnumerable<fees> students)
        {
            using (var writer = new StringWriter())
            using (var csv = new CsvWriter(writer, new CsvConfiguration(System.Globalization.CultureInfo.CurrentCulture)))
            {
                csv.WriteRecords(students);
                return writer.ToString();
            }
        }
    }



}
