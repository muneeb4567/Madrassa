using CsvHelper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Madrassa.Classes
{
    public class students
    {
        public string connectionString = @"Data Source=DESKTOP-FCU0PEP\SQLEXPRESS;Initial Catalog=Madrassams;Integrated Security=True";
        public List<studentdata> studentall = new List<studentdata>();
        public List<fees> stdfees = new List<fees>();
        public List<stdmarksnames> exammarks = new List<stdmarksnames>();
        public List<classes> allClasses = new List<classes>();
        public List<subject> Subjects = new List<subject>();
        public List<exam> exams = new List<exam>();
        public List<namessubjectmarks> allsubjectmarks = new List<namessubjectmarks>();

        public void getallsubmarks()
        {
            try
            {
                SqlConnection sqlconnection = new SqlConnection(connectionString);
                sqlconnection.Open(); namessubjectmarks result1 = new namessubjectmarks();
                string query = "SELECT mt.ClassSubjectMarkID,  c.classname , s.SubjectName , mt.MaxMerks FROM ClassSubjectMarks1 mt JOIN Classes c ON mt.ClassID = c.ClassID JOIN Subjects1 s ON mt.SubjectID = s.SubjectID;";
                SqlCommand sc = new SqlCommand(query, sqlconnection);
                SqlDataReader reader = sc.ExecuteReader();
                while (reader.Read())
                {
                    result1 = new namessubjectmarks()
                    {
                        id = (int)reader[0],
                        classid = (string)reader[1],
                        subjectid = (string)reader[2],
                        maxmarks = (int)reader[3]
                    };
                    allsubjectmarks.Add(result1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("DD", ex.Message);
            }
        }
        //public void getallresutl()
        //{
        //    try
        //    {
        //        SqlConnection sqlconnection = new SqlConnection(connectionString);
        //        sqlconnection.Open(); stdmarksnames result1 = new stdmarksnames();
        //        string query = "SELECT sr.MarkID, e.ExamName, c.classname, s.name, sub.SubjectName, sr.MarksObtained FROM  StudentExamMarks1 sr JOIN  Exams e ON sr.ExamID = e.ExamID JOIN  Classes c ON sr.ClassID = c.ClassID JOIN  addmission1 s ON sr.StudentID = s.id JOIN  Subjects1 sub ON sr.SubjectID = sub.SubjectID;";
        //        SqlCommand sc = new SqlCommand(query, sqlconnection);
        //        SqlDataReader reader = sc.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            result1 = new stdmarksnames()
        //            {
        //                id = (int)reader[0],
        //                examname = (string)reader[1],
        //                classid = (string)reader[2],
        //                studentid = (string)reader[3],
        //                subjectid = (string)reader[4],
        //                obtmarks = (int)reader[5]
        //            };
        //            exammarks.Add(result1);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}
        public void getallsubjects()
        {
            try
            {
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open(); subject subs;
                string sqlQuery = "SELECT * FROM Subjects1";
                SqlCommand command = new SqlCommand(sqlQuery, sqlConnection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    subs = new subject()
                    {
                        subjectid = (int)reader[0],
                        subjectname = (string)reader[1]
                    }; Subjects.Add(subs);
                }
                sqlConnection.Close();
                reader.Close();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        public void getallexams()
        {
            try
            {
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open(); exam exam1;
                string sqlQuery = "SELECT * FROM Exams";
                SqlCommand command = new SqlCommand(sqlQuery, sqlConnection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    exam1 = new exam()
                    {
                        id = (int)reader[0],
                        examname = (string)reader[1],
                        date = (DateTime)reader[2]
                    };
                    exams.Add(exam1);
                }
                sqlConnection.Close();
                reader.Close();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        public void getallclass()
        {
            try
            {
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open(); classes class1;
                string sqlQuery = "SELECT * FROM Classes";
                SqlCommand command = new SqlCommand(sqlQuery, sqlConnection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    class1 = new classes()
                    {
                        classid = (int)reader[0],
                        classname = (string)reader[1]
                    };
                    allClasses.Add(class1);
                }
                sqlConnection.Close();
                reader.Close();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        public void addstd(studentdata student)
        {
            try
            {
                SqlConnection scon = new SqlConnection(connectionString);
                scon.Open();
                string query = "insert into addmission1 values(N'" + student.name + "',N'" + student.fname + "',N'" + student.sarfname + "'," + student.sarfcon + "," + student.stdcnic + "," + student.sarfcnic + ",N'" + student.sarfaresh + "'," + student.fees + ",'" + student.dateofbirth + "',N'" + student.dateofdahila + "',N'" + student.darga + "',N'" + student.address + "',N'" + student.nughait1 + "','" + student.stdnum + "',N'" + student.nughait2 + "',@Data,'Active')";
                SqlCommand sc = new SqlCommand(query, scon);
                sc.Parameters.AddWithValue("@Data", student.image);
                sc.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void studentsget()
        {
            try
            {
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();// Create a list for database students
                string sqlQuery = "SELECT * FROM addmission1";
                SqlCommand command = new SqlCommand(sqlQuery, sqlConnection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    studentdata std = new studentdata()
                    {
                        Id = (int)reader[0],
                        name = (string)reader[1],
                        fname = (string)reader[2],
                        sarfname = (string)reader[3],
                        sarfcon = (decimal)reader[4],
                        stdcnic = (decimal)reader[5],
                        sarfcnic = (decimal)reader[6],
                        sarfaresh = (string)reader[7],
                        fees = (int)reader[8],
                        dateofbirth = (DateTime)reader[9],
                        dateofdahila = (DateTime)reader[10],
                        darga = (string)reader[11],
                        address = (string)reader[12],
                        nughait1 = (string)reader[13],
                        stdnum = (decimal)reader[14],
                        nughait2 = (string)reader[15],
                        image = (byte[])reader[16]
                    };
                    studentall.Add(std);
                }

                // Find students in the database that are not in the studentall list
                var missingStudents = studentall.Where(dbStudent => !studentall.Any(s => s.Id == dbStudent.Id));

                // Add missing students to the studentall list
                studentall.AddRange(missingStudents);

                sqlConnection.Close();
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void getfees()
        {
            try
            {
                SqlConnection sqlc = new SqlConnection(connectionString);
                sqlc.Open();
                string query = "select * from fees"; fees f1;
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
                    stdfees.Add(f1);
                }
                sqlc.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void createclass(classes clas)
        {

            try
            {
                SqlConnection scon = new SqlConnection(connectionString);
                scon.Open();

                // Check if a class with the same name already exists
                string checkQuery = "SELECT COUNT(*) FROM Classes WHERE classname = @classname";
                SqlCommand checkCommand = new SqlCommand(checkQuery, scon);
                checkCommand.Parameters.AddWithValue("@classname", clas.classname);

                int count = (int)checkCommand.ExecuteScalar();

                if (count == 0)
                {
                    // The class with the same name doesn't exist, so insert it
                    string insertQuery = "INSERT INTO Classes (classname) VALUES (@classname)";
                    SqlCommand sc = new SqlCommand(insertQuery, scon);
                    sc.Parameters.AddWithValue("@classname", clas.classname);

                    sc.ExecuteNonQuery();
                    MessageBox.Show("کلاس بنائی گئی ہے: " + clas.classname);
                }
                else
                {
                    // A class with the same name already exists
                    MessageBox.Show("یہ کلاس پہلے سے موجود ہے: " + clas.classname);
                }

                scon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void createsubject(subject sub)
        {
            try
            {
                SqlConnection scon = new SqlConnection(connectionString);
                scon.Open();

                // Check if a subject with the same name already exists
                string checkQuery = "SELECT COUNT(*) FROM Subjects1 WHERE subjectname = @subjectname";
                SqlCommand checkCommand = new SqlCommand(checkQuery, scon);
                checkCommand.Parameters.AddWithValue("@subjectname", sub.subjectname);

                int count = (int)checkCommand.ExecuteScalar();

                if (count == 0)
                {
                    // The subject with the same name doesn't exist, so insert it
                    string insertQuery = "INSERT INTO Subjects1 (subjectname) VALUES (@subjectname)";
                    SqlCommand sc = new SqlCommand(insertQuery, scon);
                    sc.Parameters.AddWithValue("@subjectname", sub.subjectname);

                    sc.ExecuteNonQuery();
                    MessageBox.Show(sub.subjectname + " :تخلیق کردہ موضوع");
                }
                else
                {
                    // A subject with the same name already exists
                    MessageBox.Show(sub.subjectname + " :موضوع پہلے سے موجود ہے");
                }

                scon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void createsubjectmarks(classsubjectmarks csmarks)
        {
            try
            {
                SqlConnection scon = new SqlConnection(connectionString);
                scon.Open();

                // Check if a record with the same classid and subjectid exists
                string checkQuery = "SELECT COUNT(*) FROM ClassSubjectMarks1 WHERE classid = @classid AND subjectid = @subjectid";
                SqlCommand checkCommand = new SqlCommand(checkQuery, scon);
                checkCommand.Parameters.AddWithValue("@classid", csmarks.classid);
                checkCommand.Parameters.AddWithValue("@subjectid", csmarks.subjectid);

                int count = (int)checkCommand.ExecuteScalar();

                if (count == 0)
                {
                    // The record doesn't exist, so insert it
                    string insertQuery = "INSERT INTO ClassSubjectMarks1 (classID, SubjectID, MaxMerks) VALUES (@classid, @subjectid, @maxmarks)";
                    SqlCommand sc = new SqlCommand(insertQuery, scon);
                    sc.Parameters.AddWithValue("@classid", csmarks.classid);
                    sc.Parameters.AddWithValue("@subjectid", csmarks.subjectid);
                    sc.Parameters.AddWithValue("@maxmarks", csmarks.maxmarks);

                    sc.ExecuteNonQuery();
                    MessageBox.Show("تخلیق کردہ موضوع");
                }
                else
                {
                    // A record with the same classid and subjectid already exists
                    MessageBox.Show("موضوع پہلے سے موجود ہے");
                }

                scon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }
        public void createexam(exam ex)
        {
            try
            {
                SqlConnection scon = new SqlConnection(connectionString);
                scon.Open();

                // Check if an exam with the same name and date already exists
                string checkQuery = "SELECT COUNT(*) FROM Exams WHERE ExamName = @examname AND ExamDate = @date";
                SqlCommand checkCommand = new SqlCommand(checkQuery, scon);
                checkCommand.Parameters.AddWithValue("@examname", ex.examname);
                checkCommand.Parameters.AddWithValue("@date", ex.date);

                int count = (int)checkCommand.ExecuteScalar();

                if (count == 0)
                {
                    // The exam with the same name and date doesn't exist, so insert it
                    string insertQuery = "INSERT INTO Exams (ExamName, ExamDate) VALUES (@examname, @date)";
                    SqlCommand scom = new SqlCommand(insertQuery, scon);
                    scom.Parameters.AddWithValue("@examname", ex.examname);
                    scom.Parameters.AddWithValue("@date", ex.date);

                    scom.ExecuteNonQuery();
                }
                else
                {
                    // An exam with the same name and date already exists
                    MessageBox.Show("Exam with the same name and date already exists.");
                }

                scon.Close();
            }
            catch (Exception ex1)
            {
                MessageBox.Show(ex1.Message);
            }
        }
        public void insertresult(stdexamresult stdmarks)
        {
            try
            {
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();

                // Check if a record with the same student, exam, class, and subject already exists
                string queryCheck = "SELECT COUNT(*) FROM stdexamresult WHERE studentid = @studentid AND exam = @exam AND class = @class AND subject = @subject";
                SqlCommand sqlCheck = new SqlCommand(queryCheck, sqlConnection);
                sqlCheck.Parameters.AddWithValue("@studentid", stdmarks.studentid);
                sqlCheck.Parameters.AddWithValue("@exam", stdmarks.exam);
                sqlCheck.Parameters.AddWithValue("@class", stdmarks.@class);
                sqlCheck.Parameters.AddWithValue("@subject", stdmarks.subject);

                int existingRecordCount = (int)sqlCheck.ExecuteScalar();

                if (existingRecordCount == 0)
                {
                    // If no existing record is found, insert the new record
                    string queryInsert = "INSERT INTO stdexamresult (studentid, exam, class, subject, obtmarks) VALUES (@studentid, @exam, @class, @subject, @obtmarks)";
                    SqlCommand sqlInsert = new SqlCommand(queryInsert, sqlConnection);
                    sqlInsert.Parameters.AddWithValue("@studentid", stdmarks.studentid);
                    sqlInsert.Parameters.AddWithValue("@exam", stdmarks.exam);
                    sqlInsert.Parameters.AddWithValue("@class", stdmarks.@class);
                    sqlInsert.Parameters.AddWithValue("@subject", stdmarks.subject);
                    sqlInsert.Parameters.AddWithValue("@obtmarks", stdmarks.obtmarks);

                    sqlInsert.ExecuteNonQuery();
                }
                else
                {
                    // Inform the user or handle the case where the record already exists
                    MessageBox.Show("This student's exam result already exists in the database.");
                }

                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }

}
