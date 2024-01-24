using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Web.Mvc;

namespace Diet_Project1.Models
{
    public class school
    {
        public int school_id { get; set; }
        public string school_category { get; set; }
        public string district { get; set; }
        public string district_id { get; set; }
        public string district_name { get; set; }
        public string block { get; set; }
        public string block_id { get; set; }
        public string block_name { get; set; }
        public string school_name { get; set; }
        public string user_name { get; set; }
        public string password { get; set; }
        public string school_code { get; set; }
        public string head_master { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public string address { get; set; }
        public string diet_name { get; set; }
        public string internship_capacity { get; set; }
        public List<BindDistrict> BindDistrict { get; set;}
        public List<BindBlock> BindBlock { get; set; }
    }

    public class attendance_class
    {
        public int id { get; set; }

        public string school_name { get; set; }
        public string school_id { get; set; }

        public string student_name { get; set; }
        public string student_id { get; set; }
        public string attendance { get; set; }

        public string query_id { get; set; }
        public string query { get; set; }
        public string query_date { get; set; }
        public string semester { get; set; }

        public string fromDate { get; set; }
        public string toDate { get; set; }
        public string lastUpdate { get; set; }
        public bool status { get; set; }
        public DateTime? date { get; set; }
    }

    public class Reports
    {
        public class Filter_report
        {
            public string semester { get; set; }
            public string date { get; set; }
            public string student_id { get; set; }
        }
    }
    public class BindDistrict 
    {
        public string district_id { get; set; }
        public string district_name { get; set; }
    }

    public class BindBlock
    {
        public string district_id { get; set; }
        public string block_id { get; set; }
        public string block_name { get; set; }
    }

    public class SchoolInfo
    {
        public int school_id { get; set; }
        public string block_name { get; set; }
        public string school_name { get; set; }
        public string school_code { get; set; }
    }

    public class schoolDataService
    {
        public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ConnectionString);


        public List<student> GetAprrovedStudentList()
        {
            string schoolIdFromSession = HttpContext.Current.Session["username"].ToString();
            //   string dietNameFromSession = HttpContext.Current.Session["name"].ToString();

            List<student> cg = new List<student>();
            con.Open();
            SqlCommand cmd = new SqlCommand("manage_school", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "view_ApprovedStudentList");
            cmd.Parameters.AddWithValue("@user_name", schoolIdFromSession);
            SqlDataReader sdr = cmd.ExecuteReader();
            student pro;


            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    pro = new student();
                    pro.name = sdr["name"].ToString();
                    pro.user_name = sdr["user_name"].ToString();
                    pro.father_name = sdr["father_name"].ToString();
                    pro.semester = sdr["semester"].ToString();
                    pro.city = sdr["city"].ToString();
                    pro.mobile = sdr["mobile"].ToString();
                    pro.email = sdr["email"].ToString();
                    pro.status = sdr["status"].ToString();
                    pro.start_date = sdr["start_date"].ToString();
                    pro.end_date = sdr["end_date"].ToString();
                    pro.last_update = Convert.ToDateTime(sdr["applied_date"]).ToShortDateString();
                    cg.Add(pro);
                }
            }
            con.Close();


            return cg;
        }

        public List<internship> getinternshipdetails()
        {
            string schoolIdFromSession = HttpContext.Current.Session["username"].ToString();

            List<internship> cg = new List<internship>();

            con.Open();
            SqlCommand cmd = new SqlCommand("manage_internship_details", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "view_internshipBySchool");
            cmd.Parameters.AddWithValue("@school_id", schoolIdFromSession);
            SqlDataReader sdr = cmd.ExecuteReader();
            internship pro;

            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    pro = new internship();
                    pro.internship_id = Convert.ToInt32(sdr["internship_id"]);
                    pro.batch = sdr["batch"].ToString();
                    pro.school_category = sdr["school_category"].ToString();
                    pro.semester = sdr["semester"].ToString();
                    pro.school_category = sdr["school_category"].ToString();
                    pro.start_date = sdr["start_date"].ToString();
                    pro.end_date = sdr["end_date"].ToString();
                    pro.school_capacity = sdr["school_capacity"].ToString();

                    cg.Add(pro);
                }
            }
            con.Close();


            return cg;
        }

        public bool internshipDatematch(DateTime date, string student_id)
        {
            string schoolIdFromSession = HttpContext.Current.Session["username"].ToString();

            internship cg = new internship();

            if (con.State == ConnectionState.Closed)
                con.Open();
            SqlCommand cmd = new SqlCommand("manage_attendance", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "checkdate_forattendence");
            cmd.Parameters.AddWithValue("@school_id", schoolIdFromSession);
            cmd.Parameters.AddWithValue("@date", date);
            cmd.Parameters.AddWithValue("@student_id", student_id);
            SqlDataAdapter adp = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();
            {
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;

                }
            }
            
        }


        #region manage_query
        public bool insert_query(Diet_Project1.Models.attendance_class cg)
        {
            string schoolIdFromSession = HttpContext.Current.Session["username"].ToString();
            string schoolNameFromSession = HttpContext.Current.Session["name"].ToString();
            con.Open();

            SqlCommand cmd = new SqlCommand("manage_query", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@action", "insert_query");
            cmd.Parameters.AddWithValue("@school_id", schoolIdFromSession);
            cmd.Parameters.AddWithValue("@school_name", schoolNameFromSession);
            cmd.Parameters.AddWithValue("@query", cg.query);
            //cmd.Parameters.AddWithValue("@date", cg.query_date);

            if (con.State == ConnectionState.Closed)
                con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();

            if (i >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<attendance_class> SemesterWiseStudentListInSchool2()
        {
            string schoolIdFromSession = HttpContext.Current.Session["username"].ToString();

            List<attendance_class> cg = new List<attendance_class>();
            con.Open();
            SqlCommand cmd = new SqlCommand("manage_attendance", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "view_attendance");
            cmd.Parameters.AddWithValue("@school_id", schoolIdFromSession);
          //  cmd.Parameters.AddWithValue("@student_id", student_name);
            SqlDataReader sdr = cmd.ExecuteReader();
            attendance_class pro;


            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    pro = new attendance_class();
                    //  pro.student_name = sdr["student_name"].ToString();
                    //pro.school_name = sdr["school_name"].ToString();
                    pro.date = sdr.GetDateTime(sdr.GetOrdinal("date"));
                    pro.attendance = sdr["attendance"].ToString();
                    cg.Add(pro);
                }
            }
            con.Close();

            return (cg);
        }

        public DataTable download_studentAttendance_report(string semester, string date)
        {
            DataTable dt = new DataTable();

            string conditions = "";
            if (semester != null || date != null)
            {
                if (semester != null && semester != "")
                {
                    if (conditions != "")
                    {
                        conditions = conditions + " and semester='" + semester.ToString().Replace("'", "").ToLower() + "'";
                    }
                    else
                    {
                        conditions = conditions + " semester ='" + semester.ToString().Replace("'", "").ToLower() + "'";
                    }

                }

                if (date != null && date != "")
                {
                    if (conditions != "")
                    {
                        conditions = conditions + " and date='" + date.ToString().Replace("'", "").ToLower() + "'";
                    }
                    else
                    {
                        conditions = conditions + " date ='" + date.ToString().Replace("'", "").ToLower() + "'";
                    }

                }



                if (conditions != "")
                {
                    conditions = "where" + conditions + " ORDER BY student_name ASC";
                }
            }


            string sql = "select date as Date, student_name as Student_Name, attendance as Attendance from attendance " + conditions + "";
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(sql, con);

                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    dt.Load(sdr); // Load the result into the DataTable
                }
            }
            catch
            {

            }
            con.Close();
            return dt;
        }

        public DataTable download_studentAttendance_report2(string semester, string student)
        {
            DataTable dt = new DataTable();

            string conditions = "";
            if (semester != null || student != null)
            {
                if (semester != null && semester != "")
                {
                    if (conditions != "")
                    {
                        conditions = conditions + " and semester='" + semester.ToString().Replace("'", "").ToLower() + "'";
                    }
                    else
                    {
                        conditions = conditions + " semester ='" + semester.ToString().Replace("'", "").ToLower() + "'";
                    }

                }

                if (student != null && student != "")
                {
                    if (conditions != "")
                    {
                        conditions = conditions + " and student_id='" + student.ToString().Replace("'", "").ToLower() + "'";
                    }
                    else
                    {
                        conditions = conditions + " student_id ='" + student.ToString().Replace("'", "").ToLower() + "'";
                    }

                }



                if (conditions != "")
                {
                    conditions = "where" + conditions + " ORDER BY date ASC";
                }
            }


            string sql = "select date as Date, student_name as Student_Name, attendance as Attendance from attendance " + conditions + "";
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(sql, con);

                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    dt.Load(sdr); // Load the result into the DataTable
                }
            }
            catch
            {

            }
            con.Close();
            return dt;
        }
        #endregion
    }

}