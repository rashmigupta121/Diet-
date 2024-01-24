using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;

namespace Diet_Project1.Models
{
    public class college
    {
        public int college_id { get; set; }
        public string college_name { get; set; }
        public string principal_name { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string user_name { get; set; }
        public string password { get; set; }
        public string mobile { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string state_id { get; set; }
        public string state_name { get; set; }
        public string website_url { get; set; }
        public string city_id { get; set; }
        public string city_name { get; set; }
        public string diet_name { get; set; }


        public List<BindState> BindState { get; set; }

        public List<BindCity> BindCity { get; set; }

    }
    public class collegeinfo
    {
        public string college_id { get; set; }
        public string college_name { get; set; }
    }
        public class BindState
    {
        public string state_id { get; set; }

        public string state_name { get; set; }

        public string country_id { get; set; }
    }
    public class BindCity
    {
        public string state_id { get; set; }
        public string city_id { get; set; }

        public string city_name { get; set; }
    }

    public class collegeDataService
    {
        public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ConnectionString);

        #region Manage student

        public bool insert_student(Diet_Project1.Models.student cg)
        {
            string collegeIdFromSession = HttpContext.Current.Session["username"].ToString();
            string collegeNameFromSession = HttpContext.Current.Session["name"].ToString();

            SqlCommand cmd = new SqlCommand("manage_college_student", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@batch", cg.batch);
            cmd.Parameters.AddWithValue("@registration_no", cg.registration_no);
            cmd.Parameters.AddWithValue("@name", cg.name);
            cmd.Parameters.AddWithValue("@father_name", cg.father_name);
            cmd.Parameters.AddWithValue("@mobile", cg.mobile);
            cmd.Parameters.AddWithValue("@mother_name", cg.mother_name);
            cmd.Parameters.AddWithValue("@gender", cg.gender);
            cmd.Parameters.AddWithValue("@dob", cg.dob);
            cmd.Parameters.AddWithValue("@country", cg.country);
            cmd.Parameters.AddWithValue("@state", cg.state);
            cmd.Parameters.AddWithValue("@city", cg.city);
            cmd.Parameters.AddWithValue("@pin_code", cg.pin_code);
            cmd.Parameters.AddWithValue("@address", cg.address);
            cmd.Parameters.AddWithValue("@course", cg.course);
            cmd.Parameters.AddWithValue("@semester", cg.semester);
            cmd.Parameters.AddWithValue("@email", cg.email);
            cmd.Parameters.AddWithValue("@student_image", cg.student_image);
            cmd.Parameters.AddWithValue("@student_id", cg.student_id);
            cmd.Parameters.AddWithValue("@college_id", collegeIdFromSession);
            cmd.Parameters.AddWithValue("@college_name", collegeNameFromSession);
            cmd.Parameters.AddWithValue("@status", cg.status);

            cmd.Parameters.AddWithValue("@Action", "add_student");

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

        public List<student> getstudent()
        {
            string collegeIdFromSession = HttpContext.Current.Session["username"].ToString();
            string collegeNameFromSession = HttpContext.Current.Session["name"].ToString();

            List<student> cg = new List<student>();

            con.Open();
            SqlCommand cmd = new SqlCommand("manage_college_student", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "view_student");
            cmd.Parameters.AddWithValue("@college_id", collegeIdFromSession);
            SqlDataReader sdr = cmd.ExecuteReader();
            student pro;

            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    pro = new student();
                    pro.student_id = sdr["student_id"].ToString();
                    pro.batch = sdr["batch"].ToString();
                    pro.registration_no = sdr["registration_no"].ToString();
                    pro.name = sdr["name"].ToString();
                    pro.user_name = sdr["user_name"].ToString();
                    pro.password = sdr["password"].ToString();
                    pro.father_name = sdr["father_name"].ToString();
                    pro.mother_name = sdr["mother_name"].ToString();
                    pro.gender = sdr["gender"].ToString();
                    pro.dob = sdr["dob"].ToString();
                    pro.country_id = sdr["country"].ToString();
                    pro.country = sdr["country_name"].ToString();
                    pro.state_id = sdr["state"].ToString();
                    pro.state = sdr["state_name"].ToString();
                    pro.city = sdr["city"].ToString();
                    pro.city_name = sdr["city"].ToString();
                    pro.pin_code = sdr["pin_code"].ToString();
                    pro.address = sdr["address"].ToString();
                    pro.course = sdr["course"].ToString();
                    pro.semester = sdr["semester"].ToString();
                    pro.email = sdr["email"].ToString();
                    pro.mobile = sdr["mobile"].ToString();
                    pro.student_image = sdr["student_image"].ToString();

                    cg.Add(pro);
                }
            }
            con.Close();


            return cg;
        }

        public student getstudentByiId(int? student_id)
        {
            student pro = new student();

            con.Open();
            SqlCommand cmd = new SqlCommand("manage_college_student", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Action", "edit_student");
            cmd.Parameters.AddWithValue("@student_id", student_id);

            SqlDataReader sdr = cmd.ExecuteReader();

            if (sdr.HasRows)
            {
                sdr.Read();
                pro.student_id = sdr["student_id"].ToString();
                pro.batch = sdr["batch"].ToString();
                pro.registration_no = sdr["registration_no"].ToString();
                pro.name = sdr["name"].ToString();
                pro.user_name = sdr["user_name"].ToString();
                pro.password = sdr["password"].ToString();
                pro.father_name = sdr["father_name"].ToString();
                pro.mother_name = sdr["mother_name"].ToString();
                pro.gender = sdr["gender"].ToString();
                pro.dob = sdr["dob"].ToString();
                pro.country_id = sdr["country"].ToString();
                pro.country = sdr["country_name"].ToString();
                pro.state_id = sdr["state"].ToString();
                pro.state = sdr["state_name"].ToString();
                pro.city = sdr["city"].ToString();
                pro.city_name = sdr["city"].ToString();
                pro.pin_code = sdr["pin_code"].ToString();
                pro.address = sdr["address"].ToString();
                pro.course = sdr["course"].ToString();
                pro.semester = sdr["semester"].ToString();
                pro.email = sdr["email"].ToString();
                pro.mobile = sdr["mobile"].ToString();
                pro.student_image = sdr["student_image"].ToString();
                pro.college_name = sdr["college_name"].ToString();
            }
            con.Close();


            return pro;
        }

        public bool delete_student(string student_id)
        {

            if (con.State == ConnectionState.Closed)
                con.Open();

            SqlCommand cmd = new SqlCommand("manage_college_student", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Action", "delete_student");
            cmd.Parameters.AddWithValue("@student_id", student_id);

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

        public DataTable download_student_report(/*Diet_Project1.Models.Reports.Filter_report filter_report*/)
        {
            string collegeIdFromSession = HttpContext.Current.Session["username"].ToString();
            string collegeNameFromSession = HttpContext.Current.Session["name"].ToString();

            DataTable dt = new DataTable();


            //string sql = "Select batch,registration_no,name,father_name,gender,dob,mobile,email from Student where status = '1' " + conditions + "";
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("manage_college_student", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "pdf-excel");
                cmd.Parameters.AddWithValue("@college_id", collegeIdFromSession);

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

        #region Manage Student_applied_Internships
        public List<internship> getApliedCollegeStudent()
        {
            string CollegeIdFromSession = HttpContext.Current.Session["username"].ToString();
           // string CollegeNameFromSession = HttpContext.Current.Session["name"].ToString();

            List<internship> cg = new List<internship>();
            con.Open();
            SqlCommand cmd = new SqlCommand("manage_internship", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "view_internshipByCollege");
            cmd.Parameters.AddWithValue("@college_id", CollegeIdFromSession);
            SqlDataReader sdr = cmd.ExecuteReader();
            internship pro;


            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    pro = new internship();
                    pro.student_name = sdr["student_name"].ToString();
                    pro.user_name = sdr["user_name"].ToString();
                    pro.school_category = sdr["school_category"].ToString();
                    pro.semester = sdr["semester"].ToString();
                    pro.block = sdr["block"].ToString();
                    pro.school_name = sdr["school_name"].ToString();
                    pro.school_code = sdr["school_code"].ToString();
                    pro.start_date = sdr["start_date"].ToString();
                    pro.end_date = sdr["end_date"].ToString();
                    pro.status = sdr["status"].ToString();
                    pro.status_by_college = sdr["status_by_college"].ToString();
                    pro.last_update = Convert.ToDateTime(sdr["applied_date"]).ToShortDateString();

                    cg.Add(pro);
                }
            }
            con.Close();


            return cg;
        }

        #endregion

    }

}