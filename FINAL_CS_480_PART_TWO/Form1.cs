using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FINAL_CS_480_PART_TWO
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public readonly string dbfilename = "StudentsCourses.mdf";

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox4_SelectedIndexChanged(this, null);
            this.listBox1.Items.Clear();
            tabPage1.Text = @"Course Information and Registration";
            tabPage2.Text = @"View Drop Swap Student Courses";

            string sql = string.Format(@"
Select Department, CNum ,  CRN
From Courses Order By Department ASC, CNum Asc;
");
            //Semester, Yr, ClassType, MeetDays, MeetTime, ClassSize, SeatsRemaining

            //MessageBox.Show(sql);

            DataAccessTier.Data datatier = new DataAccessTier.Data(dbfilename);

            try
            {
                DataSet ds = datatier.ExecuteNonScalarQuery(sql);

                foreach (DataRow row in ds.Tables["TABLE"].Rows)
                {
                    string dept = row["Department"].ToString();
                    string number = row["CNum"].ToString();
                    string CRN = row["CRN"].ToString();
                    string comp = string.Format("{2}       {0,-4}{1, -4}", dept, number, CRN);

                    this.listBox1.Items.Add(comp);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            string sql2 = string.Format(@"
Select netID From Students Order By netID asc;
");
            
    
            DataAccessTier.Data datatier2 = new DataAccessTier.Data(dbfilename);

            try
            {
                DataSet ds = datatier2.ExecuteNonScalarQuery(sql2);

                foreach (DataRow row in ds.Tables["TABLE"].Rows)
                {

                    string netID = row["netID"].ToString();


                    this.comboBox1.Items.Add(netID);
                    this.comboBox1.SelectedIndex = 0;
                    this.comboBox4.Items.Add(netID);
                    this.comboBox4.SelectedIndex = 0;

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



            string sql3 = string.Format(@"
Select CRN From REGISTRATIONS WHERE netID = '{0}' Order By CRN asc;
", this.comboBox4.SelectedItem.ToString());


            DataAccessTier.Data datatier3 = new DataAccessTier.Data(dbfilename);
            
            try
            {
                DataSet ds = datatier3.ExecuteNonScalarQuery(sql3);
                foreach (DataRow row in ds.Tables["TABLE"].Rows)
                {

                    string netID = row["CRN"].ToString();


                   // this.comboBox2.Items.Add(netID);
                   // this.comboBox2.SelectedIndex = 0;
                    this.comboBox3.Items.Add(netID);
                    this.comboBox3.SelectedIndex = 0;

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            string sql4 = string.Format(@"
Select CRN From REGISTRATIONS WHERE netID = '{0}' Order By CRN asc;
", this.comboBox1.SelectedItem.ToString());


            DataAccessTier.Data datatier4 = new DataAccessTier.Data(dbfilename);

            try
            {
                DataSet ds = datatier4.ExecuteNonScalarQuery(sql3);
                foreach (DataRow row in ds.Tables["TABLE"].Rows)
                {

                    string netID = row["CRN"].ToString();
                    this.comboBox2.Items.Add(netID);
                    this.comboBox2.SelectedIndex = 0;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string sql = string.Format(@"

                Update Courses

                Set SeatsRemaining = ClassSize;
                ALTER TABLE Registrations DISABLE TRIGGER regFromWait;
                DELETE FROM Registrations;

                DELETE FROM Waitlists;

                ALTER TABLE Registrations ENABLE TRIGGER regFromWait;
                ");
            SqlTransaction tx = null;
            SqlConnection db = null;
            int retry = 0;
            while (retry < 3)
            {
                try
                {
                    string filename, connectionInfo;
                    filename = "StudentsCourses.mdf";
                    connectionInfo = String.Format(@"Data Source=(LocalDB)\{0};AttachDbFilename=|DataDirectory|\{1};Integrated Security=True;", "MSSQLLocalDB", filename);
                    db = new SqlConnection(connectionInfo);
                    db.Open();
                    tx = db.BeginTransaction(IsolationLevel.Serializable);
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = db;
                    cmd.CommandText = sql;
                   
                    cmd.Transaction = tx;
                    int rowsModified = cmd.ExecuteNonQuery();

                    int delay = Convert.ToInt32(this.textBox25.Text);
                    System.Threading.Thread.Sleep(delay);
                   
                    tx.Commit();
                    MessageBox.Show("Database Reset Successful");
                    retry = 3;
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205)
                    {
                        System.Threading.Thread.Sleep(500);
                        retry++;
                    }
                    else
                    {
                        MessageBox.Show(ex.Message);
                        retry = 3;
                    }
                }
                catch (Exception ex)
                {
                    if (tx != null)
                    {
                        tx.Rollback();
                    }
                    retry = 3;
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    if (db != null)
                    {
                        db.Close();
                    }
                }
                listBox1_SelectedIndexChanged(this, null);
                comboBox4_SelectedIndexChanged(this, null);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string sql = string.Format(@"
                                            Insert Into Registrations(netID, CRN) Values('jpate201', 10630), ('oconne16', 10630), ('nishimo1', 10630), ('amezhe2', 10630), ('welch9', 10630), ('yrhee7', 10630);
                                            Update Courses Set SeatsRemaining = 0 Where CRN = 10630;
                                            Insert Into Waitlists(netID,CRN) VALUES ('alove5', 10630), ('bhaak2', 10630);
                                       ");
            DataAccessTier.Data datatier = new DataAccessTier.Data(dbfilename);

            try
            {
                int act = datatier.ExecuteActionQuery(sql);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.listBox2.Items.Clear();
            this.listBox3.Items.Clear();
            try
            {


                string comp = this.listBox1.SelectedItem.ToString();
                string[] separatingChars = { "    ", " " };
                string[] words = comp.Split(separatingChars, System.StringSplitOptions.RemoveEmptyEntries);

                int CRN = Convert.ToInt32(words[0]);
                try
                {
                    string sql = string.Format(@"
Select * From Courses Where CRN = {0};", CRN
    );




                    DataAccessTier.Data datatier = new DataAccessTier.Data(dbfilename);

                    DataSet ds = datatier.ExecuteNonScalarQuery(sql);

                    foreach (DataRow row in ds.Tables["TABLE"].Rows)
                    {
                        string meetTime = row["MeetTime"].ToString();
                        string meetDay = row["MeetDays"].ToString();
                        string semester = row["Semester"].ToString();
                        string year = row["Yr"].ToString();
                        string openSeats = row["SeatsRemaining"].ToString();
                        string classSize = row["ClassSize"].ToString();
                        string classType = row["ClassType"].ToString();
                        this.textBox7.Text = meetTime;
                        this.textBox6.Text = meetDay;
                        this.textBox8.Text = semester;
                        this.textBox5.Text = year;
                        this.textBox4.Text = openSeats;
                        this.textBox3.Text = classSize;
                        this.textBox9.Text = classType;
                        this.textBox1.Text = row["CRN"].ToString();





                    }


                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }


                string sql2 = string.Format(@"
Select * From REgistrations Where CRN = {0} Order By netID asc;
", Convert.ToInt32(this.textBox1.Text));
                //Semester, Yr, ClassType, MeetDays, MeetTime, ClassSize, SeatsRemaining

                //MessageBox.Show(sql);

                DataAccessTier.Data datatier2 = new DataAccessTier.Data(dbfilename);

                try
                {
                    DataSet ds = datatier2.ExecuteNonScalarQuery(sql2);

                    foreach (DataRow row in ds.Tables["TABLE"].Rows)
                    {

                        string netID = row["netID"].ToString();


                        this.listBox2.Items.Add(netID);
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }



                string sql3 = string.Format(@"
Select * From Waitlists Where CRN = {0} Order By Waitnum asc;
", Convert.ToInt32(this.textBox1.Text));
                //Semester, Yr, ClassType, MeetDays, MeetTime, ClassSize, SeatsRemaining

                //MessageBox.Show(sql);

                DataAccessTier.Data datatier3 = new DataAccessTier.Data(dbfilename);

                try
                {
                    DataSet ds = datatier3.ExecuteNonScalarQuery(sql3);

                    foreach (DataRow row in ds.Tables["TABLE"].Rows)
                    {

                        string netID = row["netID"].ToString();


                        this.listBox3.Items.Add(netID);
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
            catch (Exception ex)
            {
            }

        }

        private void listBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

          
            string comp = this.listBox5.SelectedItem.ToString();
            string[] separatingChars = { " " };
            string[] words = comp.Split(separatingChars, System.StringSplitOptions.RemoveEmptyEntries);

            int CRN = Convert.ToInt32(words[2]);


            try
            {
                string sql = string.Format(@"
Select * From Courses Where CRN = {0};", CRN
);




                DataAccessTier.Data datatier = new DataAccessTier.Data(dbfilename);

                DataSet ds = datatier.ExecuteNonScalarQuery(sql);

                foreach (DataRow row in ds.Tables["TABLE"].Rows)
                {
                    string meetTime = row["MeetTime"].ToString();
                    string meetDay = row["MeetDays"].ToString();
                    string semester = row["Semester"].ToString();
                    string year = row["Yr"].ToString();
                    string openSeats = row["SeatsRemaining"].ToString();
                    string classSize = row["ClassSize"].ToString();
                    string classType = row["ClassType"].ToString();
                    this.textBox12.Text = meetTime;
                    this.textBox13.Text = meetDay;
                    this.textBox14.Text = semester;
                    this.textBox15.Text = year;
                    this.textBox16.Text = openSeats;
                    this.textBox17.Text = classSize;
                    this.textBox11.Text = classType;

                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            }
            catch (Exception ex)
            {
            }
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            
        }







        private void listBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string comp = this.listBox4.SelectedItem.ToString();
                string[] separatingChars = { " " };
                string[] words = comp.Split(separatingChars, System.StringSplitOptions.RemoveEmptyEntries);

                int CRN = Convert.ToInt32(words[2]);
                try
                {
                    string sql = string.Format(@"
Select * From Courses Where CRN = {0};", CRN
    );


                    DataAccessTier.Data datatier = new DataAccessTier.Data(dbfilename);

                    DataSet ds = datatier.ExecuteNonScalarQuery(sql);

                    foreach (DataRow row in ds.Tables["TABLE"].Rows)
                    {
                        string meetTime = row["MeetTime"].ToString();
                        string meetDay = row["MeetDays"].ToString();
                        string semester = row["Semester"].ToString();
                        string year = row["Yr"].ToString();
                        string openSeats = row["SeatsRemaining"].ToString();
                        string classSize = row["ClassSize"].ToString();
                        string classType = row["ClassType"].ToString();
                        this.textBox19.Text = meetTime;
                        this.textBox20.Text = meetDay;
                        this.textBox21.Text = semester;
                        this.textBox22.Text = year;
                        this.textBox23.Text = openSeats;
                        this.textBox24.Text = classSize;
                        this.textBox18.Text = classType;

                    }


                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            catch (Exception ex)
            {
               
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string sql = string.Format(@"      register14 {1}, '{0}';
                                            
                                       ", this.textBox2.Text.ToString(), this.textBox1.Text.ToString());


            SqlTransaction tx = null;
            SqlConnection db = null;
            int retry = 0;
            while (retry < 3)
            {
                try
                {

                    string filename, connectionInfo;
                    filename = "StudentsCourses.mdf";
                    connectionInfo = String.Format(@"Data Source=(LocalDB)\{0};AttachDbFilename=|DataDirectory|\{1};Integrated Security=True;", "MSSQLLocalDB", filename);
                    db = new SqlConnection(connectionInfo);
                    db.Open();
                    tx = db.BeginTransaction(IsolationLevel.Serializable);
                
                    SqlCommand cmd = new SqlCommand("register14", db);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Transaction = tx;

                    SqlParameter innetID = new SqlParameter();
                    innetID.SqlDbType = System.Data.SqlDbType.NVarChar;
                    innetID.ParameterName = "@nid";
                    innetID.Direction = System.Data.ParameterDirection.Input;
                    innetID.Value = this.textBox2.Text.ToString().Trim();
                    cmd.Parameters.Add(innetID);

                    SqlParameter crn = new SqlParameter();
                    crn.SqlDbType = System.Data.SqlDbType.Int;
                    crn.ParameterName = "@cn";
                    crn.Direction = System.Data.ParameterDirection.Input;
                    crn.Value = Convert.ToInt32(this.textBox1.Text.ToString());
                    cmd.Parameters.Add(crn);

                    SqlParameter returnValue = new SqlParameter("returnVal", SqlDbType.Int);
                    returnValue.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(returnValue);
                    
                    cmd.ExecuteNonQuery();
                    int rowsModified = Convert.ToInt32(returnValue.Value);
                    if (rowsModified == -1)
                    {
                        MessageBox.Show("Invalid netID");
                    }
                    else if (rowsModified == -2)
                    {
                        MessageBox.Show("Invalid CRN");
                    }
                    else if (rowsModified == -3)
                    {
                        MessageBox.Show("Invalid Request: Student Already Enrolled");
                    }
                    else if (rowsModified == -4)
                    {
                        MessageBox.Show("Invalid Request: Class Full");
                    }
                  
                    int delay = Convert.ToInt32(this.textBox25.Text);
                    System.Threading.Thread.Sleep(delay);
                    tx.Commit();
                    if (rowsModified == 11)
                    {
                        MessageBox.Show("Registration Successful");
                    }
                    retry = 3;
                    listBox1_SelectedIndexChanged(this, null);
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205)
                    {
                        System.Threading.Thread.Sleep(500);
                        retry++;
                    }
                    else
                    {
                        retry = 3;
                        MessageBox.Show(ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    if (tx != null)
                    {
                        tx.Rollback();
                    }
                    retry = 3;
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    if (db != null)
                    {
                        db.Close();
                    }
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {

            
            string comp = this.listBox4.SelectedItem.ToString();
            string[] separatingChars = { " " };
            string[] words = comp.Split(separatingChars, System.StringSplitOptions.RemoveEmptyEntries);

            int CRN = Convert.ToInt32(words[2]);
            string sql = string.Format(@"
                                           Delete From Waitlists Where netID = '{0}' AND CRN = {1};
                                       ", this.comboBox4.SelectedItem.ToString(),  CRN);
                
         
            SqlTransaction tx = null;
            SqlConnection db = null;
            int retry = 0;
            while (retry < 3)
            {
                try
                {
                    string filename, connectionInfo;
                    filename = "StudentsCourses.mdf";
                    connectionInfo = String.Format(@"Data Source=(LocalDB)\{0};AttachDbFilename=|DataDirectory|\{1};Integrated Security=True;", "MSSQLLocalDB", filename);
                    db = new SqlConnection(connectionInfo);
                    db.Open();
                    tx = db.BeginTransaction(IsolationLevel.Serializable);
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = db;
                    cmd.CommandText = sql;
                    cmd.Transaction = tx;
                 
                    int rowsModified = cmd.ExecuteNonQuery();

                    int delay = Convert.ToInt32(this.textBox25.Text);
                    System.Threading.Thread.Sleep(delay);
                    tx.Commit();
                    if (rowsModified >= 1)
                    {
                        MessageBox.Show("Waitlist Drop Successful");
                    }
                    else
                    {
                        MessageBox.Show("Waitlist Drop Failed");
                    }
                    retry = 3;
                   
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205)
                    {
                        System.Threading.Thread.Sleep(500);
                        retry++;
                    }
                    else
                    {
                        retry = 3;
                        MessageBox.Show(ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    if (tx != null)
                    {
                        tx.Rollback();
                    }
                    retry = 3;
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    if (db != null)
                    {
                        db.Close();
                    }
                }
            }
            }
            catch(Exception ex)
            {

            }
            comboBox4_SelectedIndexChanged(this, null);

        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {

          
            string comp = this.listBox5.SelectedItem.ToString();
            string[] separatingChars = { " " };
            string[] words = comp.Split(separatingChars, System.StringSplitOptions.RemoveEmptyEntries);

            int CRN = Convert.ToInt32(words[2]);
            string sql = string.Format(@"
                                           Delete From Registrations Where netID = '{0}' AND CRN = {1};
                                           Update Courses Set SeatsRemaining = SeatsRemaining + 1 Where CRN = {1};
                                       ", this.comboBox4.SelectedItem.ToString(), CRN);





            SqlTransaction tx = null;
            SqlConnection db = null;
            int retry = 0;
            while (retry < 3)
            {
                try
                {
                    string filename, connectionInfo;
                    filename = "StudentsCourses.mdf";
                    connectionInfo = String.Format(@"Data Source=(LocalDB)\{0};AttachDbFilename=|DataDirectory|\{1};Integrated Security=True;", "MSSQLLocalDB", filename);
                    db = new SqlConnection(connectionInfo);
                    db.Open();
                    tx = db.BeginTransaction(IsolationLevel.Serializable);
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = db;
                    cmd.CommandText = sql;
                    cmd.Transaction = tx;

                    int rowsModified = cmd.ExecuteNonQuery();

                    int delay = Convert.ToInt32(this.textBox25.Text);
                    System.Threading.Thread.Sleep(delay);
                    tx.Commit();
                    if (rowsModified >= 1)
                    {
                        MessageBox.Show("Course Drop Successful");
                    }
                    else
                    {
                        MessageBox.Show("Course Drop Failed");
                    }
                    retry = 3;
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205)
                    {
                        System.Threading.Thread.Sleep(500);
                        retry++;
                    }
                    else
                    {
                        retry = 3;
                        MessageBox.Show(ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    if (tx != null)
                    {
                        tx.Rollback();
                    }
                    retry = 3;
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    if (db != null)
                    {
                        db.Close();
                    }
                }
            }

            }
            catch(Exception ex)
            {
                
            }
            listBox1_SelectedIndexChanged(this, null);
            comboBox4_SelectedIndexChanged(this, null);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string sql = string.Format(@" wait15 {1}, '{0}';
                                            
                                       ", this.textBox2.Text.ToString(), this.textBox1.Text.ToString());


            SqlTransaction tx = null;
            SqlConnection db = null;
            int retry = 0;
            while (retry < 3)
            {
                try
                {

                    string filename, connectionInfo;
                    filename = "StudentsCourses.mdf";
                    connectionInfo = String.Format(@"Data Source=(LocalDB)\{0};AttachDbFilename=|DataDirectory|\{1};Integrated Security=True;", "MSSQLLocalDB", filename);
                    db = new SqlConnection(connectionInfo);
                    db.Open();
                    tx = db.BeginTransaction(IsolationLevel.Serializable);

                    SqlCommand cmd = new SqlCommand("wait14", db);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Transaction = tx;

                    SqlParameter innetID = new SqlParameter();
                    innetID.SqlDbType = System.Data.SqlDbType.NVarChar;
                    innetID.ParameterName = "@nid";
                    innetID.Direction = System.Data.ParameterDirection.Input;
                    innetID.Value = this.textBox2.Text.ToString().Trim();
                    cmd.Parameters.Add(innetID);

                    SqlParameter crn = new SqlParameter();
                    crn.SqlDbType = System.Data.SqlDbType.Int;
                    crn.ParameterName = "@cn";
                    crn.Direction = System.Data.ParameterDirection.Input;
                    crn.Value = Convert.ToInt32(this.textBox1.Text.ToString());
                    cmd.Parameters.Add(crn);

                    SqlParameter returnValue = new SqlParameter("returnVal", SqlDbType.Int);
                    returnValue.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(returnValue);

                    cmd.ExecuteNonQuery();
                    int rowsModified = Convert.ToInt32(returnValue.Value);
                    if (rowsModified == -1)
                    {
                        MessageBox.Show("Invalid netID");
                    }
                    else if (rowsModified == -2)
                    {
                        MessageBox.Show("Invalid CRN");
                    }
                    else if (rowsModified == -3)
                    {
                        MessageBox.Show("Invalid Request: Cannot Be added to waitlist for class in which you are enrolled");
                    }
                    else if (rowsModified == -4)
                    {
                        MessageBox.Show("Class Not Full: You May Register");
                    }
                    else if (rowsModified == -5)
                    {
                        MessageBox.Show("You are already on the waitlist");
                    }

                    int delay = Convert.ToInt32(this.textBox25.Text);
                    System.Threading.Thread.Sleep(delay);
                    tx.Commit();
                    if (rowsModified == 1)
                    {
                        MessageBox.Show("Waitlisting Successful");
                    }
                    retry = 3;
                    listBox1_SelectedIndexChanged(this, null);
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205)
                    {
                        System.Threading.Thread.Sleep(500);
                        retry++;
                    }
                    else
                    {
                        retry = 3;
                        MessageBox.Show(ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    if (tx != null)
                    {
                        tx.Rollback();
                    }
                    retry = 3;
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    if (db != null)
                    {
                        db.Close();
                    }
                }
            }
        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox27_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string sql4 = string.Format(@"
Select CRN From REGISTRATIONS WHERE netID = '{0}' Order By CRN asc;
", this.comboBox1.SelectedItem.ToString());


            DataAccessTier.Data datatier4 = new DataAccessTier.Data(dbfilename);

            try
            {
                DataSet ds = datatier4.ExecuteNonScalarQuery(sql4);
                foreach (DataRow row in ds.Tables["TABLE"].Rows)
                {

                    string netID = row["CRN"].ToString();
                    this.comboBox2.Items.Add(netID);
                    this.comboBox2.SelectedIndex = 0;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.comboBox2.Items.Clear();
                this.comboBox3.Items.Clear();

                this.listBox4.Items.Clear();
                this.listBox5.Items.Clear();
                string sql2 = string.Format(@"
            Select * From Courses where CRN in (Select CRN From REgistrations Where netID = '{0}');
            ", this.comboBox4.SelectedItem.ToString());
                //Semester, Yr, ClassType, MeetDays, MeetTime, ClassSize, SeatsRemaining

                //MessageBox.Show(sql);

                DataAccessTier.Data datatier2 = new DataAccessTier.Data(dbfilename);

                try
                {
                    DataSet ds = datatier2.ExecuteNonScalarQuery(sql2);

                    foreach (DataRow row in ds.Tables["TABLE"].Rows)
                    {

                        string course = row["Department"].ToString();
                        string course2 = row["CNum"].ToString();
                        string course3 = row["CRN"].ToString();

                        this.listBox5.Items.Add(course + " " + course2 + " " + course3);
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }



                string sql5 = string.Format(@"
            Select * From Courses where CRN in (Select CRN From Waitlists Where netID = '{0}');
            ", this.comboBox4.SelectedItem.ToString());

                DataAccessTier.Data datatier5 = new DataAccessTier.Data(dbfilename);

                try
                {
                    DataSet ds = datatier5.ExecuteNonScalarQuery(sql5);
             

                    foreach (DataRow row in ds.Tables["TABLE"].Rows)
                    {
                        string course = row["Department"].ToString();
                        string course2 = row["CNum"].ToString();
                        string course3 = row["CRN"].ToString();
                        this.listBox4.Items.Add(course + " " + course2 + " " + course3);
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
           




            string sql3 = string.Format(@"
Select CRN From REGISTRATIONS WHERE netID = '{0}' Order By CRN asc;
", this.comboBox4.SelectedItem.ToString());


            DataAccessTier.Data datatier3 = new DataAccessTier.Data(dbfilename);

            try
            {
                DataSet ds = datatier3.ExecuteNonScalarQuery(sql3);
                foreach (DataRow row in ds.Tables["TABLE"].Rows)
                {

                    string netID = row["CRN"].ToString();


                    // this.comboBox2.Items.Add(netID);
                    // this.comboBox2.SelectedIndex = 0;
                    this.comboBox3.Items.Add(netID);
                    this.comboBox3.SelectedIndex = 0;

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            }
            catch (Exception ex)
            {

            }


        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = string.Format(@"

                Update Registrations

                Set netID = '{0}' where netID = '{1}' AND CRN = {2};

Update Registrations

                Set netID = '{1}' where netID = '{0}' AND CRN = {3};
                
                ", this.comboBox4.SelectedItem.ToString(), this.comboBox1.SelectedItem.ToString(), this.comboBox2.SelectedItem.ToString(), this.comboBox3.SelectedItem.ToString());
                SqlTransaction tx = null;
                SqlConnection db = null;
                int retry = 0;
                while (retry < 3)
                {
                    try
                    {
                        string filename, connectionInfo;
                        filename = "StudentsCourses.mdf";
                        connectionInfo = String.Format(@"Data Source=(LocalDB)\{0};AttachDbFilename=|DataDirectory|\{1};Integrated Security=True;", "MSSQLLocalDB", filename);
                        db = new SqlConnection(connectionInfo);
                        db.Open();
                        tx = db.BeginTransaction(IsolationLevel.Serializable);
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = db;
                        cmd.CommandText = sql;
                        cmd.Transaction = tx;
                        int rowsModified = cmd.ExecuteNonQuery();
                        int delay = Convert.ToInt32(this.textBox25.Text);
                        System.Threading.Thread.Sleep(delay);
                        tx.Commit();
                        MessageBox.Show("Swap Successful");
                        retry = 3;
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 1205)
                        {
                            System.Threading.Thread.Sleep(500);
                            retry++;
                        }
                        else
                        {
                            retry = 3;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (tx != null)
                        {
                            tx.Rollback();
                        }
                        retry = 3;
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        if (db != null)
                        {
                            db.Close();
                        }
                    }
                    listBox1_SelectedIndexChanged(this, null);
                    comboBox4_SelectedIndexChanged(this, null);
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
