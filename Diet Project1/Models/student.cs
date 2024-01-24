using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Antlr.Runtime;

namespace Diet_Project1.Models
{
    public class student
    {
        public string student_id { get; set; }
        public string batch { get; set; }
        public string registration_no { get; set; }
        public string name { get; set; }
        public string student_name { get; set; }
        public string user_name { get; set; }
        public string password { get; set; }
        public string father_name { get; set; }
        public string mother_name { get; set; }
        public string gender { get; set; }
        public string dob { get; set; }
        public string mobile { get; set; }
        public string email { get; set; }
        public string student_image { get; set; }
        public HttpPostedFileBase image_url { get; set; }
        public DateTime date { get; set; }
        public string status { get; set; }
        public string country { get; set; }
        public string country_id { get; set; }
        public string state { get; set; }

        public string state_id { get; set; }
        public string state_name { get; set; }

        public string city_name { get; set; }
        public string city { get; set; }
        public string pin_code { get; set; }
        public string address { get; set; }
        public string course { get; set; }
        public string semester { get; set; }

        public string start_date { get; set; }
        public string end_date { get; set; }
        public string last_update { get; set; }

        public string college_id { get; set; }
        public string college_name { get; set; }
        public List<BindCountry> BindCountry { get; set; }
        public List<BindState> BindState { get; set; }

        public List<BindCity> BindCity { get; set; }

    }
    public class BindCountry
    {
        public string country_id { get; set; }
        public string country_name { get; set; }
    }

    public class studentDataService
    {
        public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ConnectionString);

        #region manage dashboard

        public bool popup_show()
        {
            string studentIdFromSession = HttpContext.Current.Session["username"].ToString();

            con.Open();
            SqlCommand cmd = new SqlCommand("manage_popup", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "popup_view");
            cmd.Parameters.AddWithValue("@student_id", studentIdFromSession);

            SqlDataReader sdr = cmd.ExecuteReader();

            if (sdr.HasRows)
            {
                sdr.Read();
                return true;
            }
            con.Close();
            return false;
        }

        #endregion

        #region Manage internship_apply

        public bool IsApplicationOpen()
        {
            string studentId = HttpContext.Current.Session["username"].ToString();

            // Assuming "manage_student" stored procedure returns one record         
            con.Open();

            SqlCommand cmd = new SqlCommand("manage_student", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "View_internship");
                cmd.Parameters.AddWithValue("@user_name", studentId);

                SqlDataReader sdr = cmd.ExecuteReader();
            // Initialize the session variable to false
                    HttpContext.Current.Session["IsApplicationOpen"] = false;
                    while (sdr.Read())
                    {
                        // Close the SqlDataReader after reading values
                        DateTime panelOpen = Convert.ToDateTime(sdr["panel_open"]);
                        DateTime applyDate = Convert.ToDateTime(sdr["apply_date"]);

                        DateTime currentDate = DateTime.Now.Date;
                        // Adjust these dates based on your actual application period
                        if (currentDate >= panelOpen && currentDate <= applyDate)
                        {
                            HttpContext.Current.Session["IsApplicationOpen"] = true;
                        }
                    }
                    sdr.Close();


            // Return the value based on the condition
            return (bool)HttpContext.Current.Session["IsApplicationOpen"];
        }


        public bool internship_apply(Diet_Project1.Models.internship cg)
        {
            string studentIdFromSession = HttpContext.Current.Session["username"].ToString();
            string studentNameFromSession = HttpContext.Current.Session["name"].ToString();

            SqlCommand cmd = new SqlCommand("manage_internship", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@student_name", studentNameFromSession);
            cmd.Parameters.AddWithValue("@user_name", studentIdFromSession);
            cmd.Parameters.AddWithValue("@school_category", cg.school_category);
            cmd.Parameters.AddWithValue("@district", cg.district);
            cmd.Parameters.AddWithValue("@block", cg.block);
            cmd.Parameters.AddWithValue("@school_name", cg.school_name);
            cmd.Parameters.AddWithValue("@school_code", cg.school_code);
            cmd.Parameters.AddWithValue("@head_master", cg.head_master);
            cmd.Parameters.AddWithValue("@email", cg.email);
            cmd.Parameters.AddWithValue("@mobile", cg.mobile);
            cmd.Parameters.AddWithValue("@address", cg.address);
            cmd.Parameters.AddWithValue("@internship_id", cg.internship_id);

            cmd.Parameters.AddWithValue("@action", "apply_internship");

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

        public List<school> Bindschool(string block)
        {
            string studentIdFromSession = HttpContext.Current.Session["username"].ToString();
            //string studentNameFromSession = HttpContext.Current.Session["name"].ToString();

            List<school> cg = new List<school>();

            con.Open();
            SqlCommand cmd = new SqlCommand("manage_internship", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "view_school_for_intership");
            cmd.Parameters.AddWithValue("@block", block);
            cmd.Parameters.AddWithValue("@user_name", studentIdFromSession);
            SqlDataReader sdr = cmd.ExecuteReader();
            school pro;

            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    pro = new school();
                    pro.school_id = Convert.ToInt32(sdr["school_id"]);
                    pro.school_category = sdr["school_category"].ToString();
                    pro.district_name = sdr["district"].ToString();
                    pro.district = sdr["district"].ToString();
                    pro.block = sdr["block"].ToString();
                    pro.block_name = sdr["block"].ToString();
                    pro.school_name = sdr["school_name"].ToString();
                    pro.school_code = sdr["school_code"].ToString();
                    pro.head_master = sdr["head_master"].ToString();
                    pro.email = sdr["email"].ToString();
                    pro.mobile = sdr["mobile"].ToString();
                    pro.address = sdr["address"].ToString();
                    pro.internship_capacity = sdr["internship_capacity"].ToString();

                    cg.Add(pro);
                }
            }
            con.Close();


            return cg;
        }

        public List<school> getschoolDetails(string school_code)
        {
            List<school> cg = new List<school>();

            con.Open();
            SqlCommand cmd = new SqlCommand("manage_internship", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "view_detailsBySchool");
            cmd.Parameters.AddWithValue("@school_code", school_code);
            SqlDataReader sdr = cmd.ExecuteReader();
            school pro;

            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    pro = new school();
                    pro.school_id = Convert.ToInt32(sdr["school_id"]);
                    pro.school_category = sdr["school_category"].ToString();
                    pro.district_id = sdr["district"].ToString();
                    pro.district = sdr["user_name"].ToString();
                    pro.block = sdr["block"].ToString();
                    pro.school_name = sdr["school_name"].ToString();
                    pro.school_code = sdr["school_code"].ToString();
                    pro.head_master = sdr["head_master"].ToString();
                    pro.email = sdr["email"].ToString();
                    pro.mobile = sdr["mobile"].ToString();
                    pro.address = sdr["address"].ToString();
                    pro.internship_capacity = sdr["internship_capacity"].ToString();

                    cg.Add(pro);
                }
            }
            con.Close();


            return cg;
        }

        public student ViewStudentProfile()
        {
            string studentIdFromSession = HttpContext.Current.Session["username"].ToString();
          //  string dietNameFromSession = HttpContext.Current.Session["name"].ToString();

            student pro = new student();

            con.Open();
            SqlCommand cmd = new SqlCommand("manage_student", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "view_student_profile");
            cmd.Parameters.AddWithValue("@user_name", studentIdFromSession);

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
                pro.pin_code = sdr["pin_code"].ToString();
                pro.address = sdr["address"].ToString();
                pro.course = sdr["course"].ToString();
                pro.semester = sdr["semester"].ToString();
                pro.email = sdr["email"].ToString();
                pro.mobile = sdr["mobile"].ToString();
                pro.student_image = sdr["student_image"].ToString();
            }
            con.Close();


            return pro;
        }

        public List<attendance_class> GetAttendance(string semester) 
        {
            string studentIdFromSession = HttpContext.Current.Session["username"].ToString();

            List<attendance_class> cg = new List<attendance_class>();
            con.Open();
            SqlCommand cmd = new SqlCommand("manage_attendance", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "ViewAttendanceby_student");
            cmd.Parameters.AddWithValue("@student_id", studentIdFromSession);
          //  cmd.Parameters.AddWithValue("@date", date);
            cmd.Parameters.AddWithValue("@semester", semester);
            SqlDataReader sdr = cmd.ExecuteReader();
            attendance_class pro;


            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    pro = new attendance_class();
                    pro.student_name = sdr["student_name"].ToString();
                    pro.school_name = sdr["school_name"].ToString();
                    pro.date = sdr.GetDateTime(sdr.GetOrdinal("date"));
                    pro.attendance = sdr["attendance"].ToString();
                    cg.Add(pro);
                }
            }
            con.Close();

            return (cg);
        }
        #endregion

        //Apply intership funtion for Api
        public bool internship_applybyApi(string username, string studentname)
        {
           // internship cg = new internship();
            var httpRequest = HttpContext.Current.Request;
            // var curent_date = DateTime.Now.ToString("dd/MM/yyyy");
            var cg = new internship()
            {
                internship_id = Convert.ToInt32(httpRequest.Form.Get("internship_id")),
                student_name = httpRequest.Form.Get("student_name"),
                user_name = httpRequest.Form.Get("user_name"),
                school_category = httpRequest.Form.Get("school_category"),
                district = httpRequest.Form.Get("district"),
                block = httpRequest.Form.Get("block"),
                school_name = httpRequest.Form.Get("school_name"),
                school_code = httpRequest.Form.Get("school_code"),
                head_master = httpRequest.Form.Get("head_master"),
                email = httpRequest.Form.Get("email"),
                mobile = httpRequest.Form.Get("mobile"),
                address = httpRequest.Form.Get("address"),

            };

            SqlCommand cmd = new SqlCommand("manage_internship", con);
            cmd.CommandType = CommandType.StoredProcedure;

            // Set values for the internship object
            cg.student_name = studentname;
            cg.user_name = username;

            cmd.Parameters.AddWithValue("@student_name", cg.student_name);
            cmd.Parameters.AddWithValue("@user_name", cg.user_name);
            cmd.Parameters.AddWithValue("@school_category", cg.school_category);
            cmd.Parameters.AddWithValue("@district", cg.district);
            cmd.Parameters.AddWithValue("@block", cg.block);
            cmd.Parameters.AddWithValue("@school_name", cg.school_name);
            cmd.Parameters.AddWithValue("@school_code", cg.school_code);
            cmd.Parameters.AddWithValue("@head_master", cg.head_master);
            cmd.Parameters.AddWithValue("@email", cg.email);
            cmd.Parameters.AddWithValue("@mobile", cg.mobile);
            cmd.Parameters.AddWithValue("@address", cg.address);
            cmd.Parameters.AddWithValue("@internship_id", cg.internship_id);

            cmd.Parameters.AddWithValue("@action", "apply_internship");

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

    }

}