using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Collections;
using System.Drawing;
using NPOI.SS.Formula.Functions;
using System.Web.Mvc;

namespace Diet_Project1.Models
{
    public class DataService
    {
        public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ConnectionString);

        #region login
        public Models.common_response login(string username, string password)
        {
            Models.common_response Response = new Models.common_response();

            #region Validation

            if (username == null || username == "")
            {
                Response.message = "Invalid Username.";
                return Response;
            }

            if (password == null || password == "")
            {
                Response.message = "Invalid password (must include atleast 8 charaters,uppercase and lowercase alphabhet, one number and one special charater).";
                return Response;
            }

            username = username.Replace("'", "''").Trim();
            password = password.Replace("'", "''").Trim();


            #endregion;

            #region Check User
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("manage_user", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "verify_user");
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@Password", password);
                //   cmd.Parameters.AddWithValue("@roll_id", roll_id);
                SqlDataReader sda = cmd.ExecuteReader();


                if (sda.HasRows)
                {
                    sda.Read();
                    Response.success = true;
                    Response.parameter = sda["roll_id"].ToString();
                    Response.name = sda["name"].ToString();
                    Response.user_id = sda["username"].ToString();
                    Response.roll_id = sda["roll_id"].ToString();
                   Response.username = sda["username"].ToString();
                    Response.password = sda["password"].ToString();
                    //     Response.report_manager = sda["reporting_manager"].ToString();

                    return Response;

                }
                else { Response.message = "Invalid username or password!."; }

                //if (Response.success == true)
                //{
                //    if (token != null && token != "")
                //    {
                //        con.Open();
                //        SqlCommand cmd1 = new SqlCommand("sp_employeeManage", con);
                //        cmd1.CommandType = CommandType.StoredProcedure;
                //        cmd1.Parameters.AddWithValue("@Action", "update_Token");
                //        cmd1.Parameters.AddWithValue("@device_token", token);
                //        cmd1.Parameters.AddWithValue("@Username", username);
                //        cmd1.Parameters.AddWithValue("@Password", password);
                //        int i = cmd1.ExecuteNonQuery();

                //        con.Close();
                //        return Response;
                //    }
                //    return Response;
                //}

                #region Login the User
                //{
                //    string refrence = System.Guid.NewGuid().ToString() + System.Guid.NewGuid().ToString() + System.Guid.NewGuid().ToString() + System.Guid.NewGuid().ToString();
                //    string agencyid = dt.Rows[0]["id"].ToString();
                //    string createdon = DateTime.UtcNow.ToString();
                //    string expire_on = DateTime.UtcNow.AddDays(1).ToString();
                //    string ip = HttpContext.Current.Request.UserHostAddress;
                //    string ip_city = "";


                //    string login_querystring = " delete from tbl_admin_current_login where admin_id='" + agencyid + "' and expire_on<='" + createdon + "'  insert into tbl_admin_current_login(username, admin_id,created_on,ip_address,ip_city,expire_on) values('" + username + "', '" + agencyid + "','" + createdon + "','" + ip + "','" + ip_city + "','" + expire_on.ToString() + "')";

                //    SqlHelper.ExecuteNonQuery(CommandType.Text, login_querystring);

                //}
                #endregion;



            }
            #endregion;

            return Response;
        }
        #endregion

        #region check Session

        public Models.common_response adminssioncheck(string viewdashboaradds)
        {
            Models.common_response response = new Models.common_response();

            if (HttpContext.Current.Session["adminname"] != null)
            {
                response.success = true;
                response.parameter = HttpContext.Current.Session["adminname"].ToString();
            }

            return response;
        }


        #endregion

        #region register
        //public bool insert_user(Diet_Project1.Models.register user)
        //{

        //    SqlCommand cmd = new SqlCommand("manage_user", con);
        //    cmd.CommandType = CommandType.StoredProcedure;

        //    cmd.Parameters.AddWithValue("@name", user.name);
        //    cmd.Parameters.AddWithValue("@email", user.email);
        //    cmd.Parameters.AddWithValue("@mobile_no", user.mobile);
        //    cmd.Parameters.AddWithValue("@password", user.password);
        //    cmd.Parameters.AddWithValue("@username", user.username);
        //    // cmd.Parameters.AddWithValue("@user_id", user.user_id);


        //    cmd.Parameters.AddWithValue("@Action", "add_user");


        //    if (con.State == ConnectionState.Closed)
        //        con.Open();
        //    int i = cmd.ExecuteNonQuery();
        //    con.Close();

        //    if (i >= 1)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        #endregion

        #region manage_dashboard
        public Dashboard total_count()
        {
            string dietIdFromSession = HttpContext.Current.Session["username"].ToString();
            string dietNameFromSession = HttpContext.Current.Session["name"].ToString();

            Dashboard cg = new Dashboard();
            con.Open();
            SqlCommand cmd = new SqlCommand("manage_dashboard", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@diet_id", dietIdFromSession);

            SqlDataReader dr = cmd.ExecuteReader();
            // int count = 0;
            if (dr.HasRows)
            {
                dr.Read();
                cg.total_diet = Convert.ToInt32(dr["total_diet"]);
                cg.total_college = Convert.ToInt32(dr["total_college"]);
                cg.total_schools = Convert.ToInt32(dr["total_schools"]);
                cg.total_student = Convert.ToInt32(dr["total_student"]);

                cg.total_diet_college = Convert.ToInt32(dr["total_diet_college"]);
                cg.total_diet_school = Convert.ToInt32(dr["total_school"]);
                cg.total_diet_student = Convert.ToInt32(dr["total_diet_student"]);
                cg.total_internship = Convert.ToInt32(dr["total_internships"]);
                cg.total_college_student = Convert.ToInt32(dr["total_college_student"]);
            }
            return cg;
        }
        #endregion

        #region Manage College
        public List<BindState> BindState()       //Bind State only for uttar pardesh
        {
            List<BindState> Ctg = new List<BindState>();
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from StateMaster where name = 'Uttar Pradesh'", con);
            // cmd.CommandType = System.Data.CommandType.Text;
            //  cmd.Parameters.AddWithValue("@action", "ViewBook_Ctg");
            SqlDataReader sdr = cmd.ExecuteReader();
            BindState pro;
            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    pro = new BindState();
                    pro.state_id = sdr["ID"].ToString();
                    pro.state_name = sdr["Name"].ToString();
                    Ctg.Add(pro);
                }
            }
            con.Close();
            return Ctg;
        }

        public bool insert_college(Diet_Project1.Models.college cg)
        {
            string dietIdFromSession = HttpContext.Current.Session["username"].ToString();
            string dietNameFromSession = HttpContext.Current.Session["name"].ToString();

            SqlCommand cmd = new SqlCommand("manage_college", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@college_name", cg.college_name);
            cmd.Parameters.AddWithValue("@principal_name", cg.principal_name);
            cmd.Parameters.AddWithValue("@address", cg.address);
            cmd.Parameters.AddWithValue("@city", cg.city);
            cmd.Parameters.AddWithValue("@state", cg.state);
            cmd.Parameters.AddWithValue("@mobile", cg.mobile);
            cmd.Parameters.AddWithValue("@email", cg.email);
            cmd.Parameters.AddWithValue("@website_url", cg.website_url);
            cmd.Parameters.AddWithValue("@diet_id", dietIdFromSession);
            cmd.Parameters.AddWithValue("@diet_name", dietNameFromSession);
            cmd.Parameters.AddWithValue("@college_id", cg.college_id);

            cmd.Parameters.AddWithValue("@action", "add_college");

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

        public List<college> getcollege()
        {
            string dietIdFromSession = HttpContext.Current.Session["username"].ToString();
            string dietNameFromSession = HttpContext.Current.Session["name"].ToString();

            List<college> cg = new List<college>();

            con.Open();
            SqlCommand cmd = new SqlCommand("manage_college", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "view_college");
            cmd.Parameters.AddWithValue("@diet_id", dietIdFromSession);
            SqlDataReader sdr = cmd.ExecuteReader();
            college pro;

            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    pro = new college();
                    pro.college_id = Convert.ToInt32(sdr["college_id"]);
                    pro.college_name = sdr["college_name"].ToString();
                    pro.user_name = sdr["user_name"].ToString();
                    pro.password = sdr["password"].ToString();
                    pro.principal_name = sdr["principal_name"].ToString();
                    pro.address = sdr["address"].ToString();
                    // pro.city_id = sdr["ID"].ToString();
                    pro.state_id = sdr["state"].ToString();
                    pro.city = sdr["city"].ToString();
                    pro.city_name = sdr["city"].ToString();
                    pro.state = sdr["Name"].ToString();
                    pro.email = sdr["email"].ToString();
                    pro.mobile = sdr["mobile"].ToString();
                    pro.website_url = sdr["website_url"].ToString();
                    cg.Add(pro);
                }
            }
            con.Close();


            return cg;
        }

        public college getcollegeByiId(int? college_id)
        {
            college pro = new college();

            con.Open();
            SqlCommand cmd = new SqlCommand("manage_college", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Action", "edit_college");
            cmd.Parameters.AddWithValue("@college_id", college_id);

            SqlDataReader sdr = cmd.ExecuteReader();

            if (sdr.HasRows)
            {
                sdr.Read();
                pro.college_id = Convert.ToInt32(sdr["college_id"]);
                pro.college_name = sdr["college_name"].ToString();
                pro.user_name = sdr["user_name"].ToString();
                pro.password = sdr["password"].ToString();
                pro.principal_name = sdr["principal_name"].ToString();
                pro.address = sdr["address"].ToString();
                //   pro.city_id = sdr["ID"].ToString();
                pro.state_id = sdr["state"].ToString();
                pro.city = sdr["city"].ToString();
                pro.city_name = sdr["city"].ToString();
                pro.state = sdr["Name"].ToString();
                pro.email = sdr["email"].ToString();
                pro.mobile = sdr["mobile"].ToString();
                pro.website_url = sdr["website_url"].ToString();

            }
            con.Close();


            return pro;
        }

        public bool delete_college(string college_id)
        {

            if (con.State == ConnectionState.Closed)
                con.Open();

            SqlCommand cmd = new SqlCommand("manage_college", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Action", "delete_college");
            cmd.Parameters.AddWithValue("@college_id", college_id);

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

        public DataTable download_college_report(/*Diet_Project1.Models.Reports.Filter_report filter_report*/)
        {
            string dietIdFromSession = HttpContext.Current.Session["username"].ToString();
            string dietNameFromSession = HttpContext.Current.Session["name"].ToString();
            DataTable dt = new DataTable();

           // string sql = "Select college_id,college_name,principal_name,address,mobile,email from College where status = '1' And diet_id = ' + conditions + '";
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("manage_college", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "pdf_excel");
                cmd.Parameters.AddWithValue("@diet_id", dietIdFromSession);

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

        #region Manage school

        public bool insert_school(Diet_Project1.Models.school cg)
        {
            string dietIdFromSession = HttpContext.Current.Session["username"].ToString();
            string dietNameFromSession = HttpContext.Current.Session["name"].ToString();

            SqlCommand cmd = new SqlCommand("manage_school", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@school_category", cg.school_category);
            cmd.Parameters.AddWithValue("@district", cg.district);
            cmd.Parameters.AddWithValue("@block", cg.block);
            cmd.Parameters.AddWithValue("@school_name", cg.school_name);
            cmd.Parameters.AddWithValue("@school_code", cg.school_code);
            cmd.Parameters.AddWithValue("@head_master", cg.head_master);
            cmd.Parameters.AddWithValue("@email", cg.email);
            cmd.Parameters.AddWithValue("@mobile", cg.mobile);
            cmd.Parameters.AddWithValue("@address", cg.address);
            cmd.Parameters.AddWithValue("@internship_capacity", cg.internship_capacity);
            cmd.Parameters.AddWithValue("@diet_id", dietIdFromSession);
            cmd.Parameters.AddWithValue("@diet_name",dietNameFromSession);
            cmd.Parameters.AddWithValue("@school_id", cg.school_id);

            cmd.Parameters.AddWithValue("@Action", "add_school");

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

        public List<school> getschool()
        {
            string dietIdFromSession = HttpContext.Current.Session["username"].ToString();
           // string dietNameFromSession = HttpContext.Current.Session["name"].ToString();

            List<school> cg = new List<school>();

            con.Open();
            SqlCommand cmd = new SqlCommand("manage_school", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "view_school");
            cmd.Parameters.AddWithValue("@diet_id", dietIdFromSession);
            SqlDataReader sdr = cmd.ExecuteReader();
            school pro;

            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    pro = new school();
                    pro.school_id = Convert.ToInt32(sdr["school_id"]);
                    pro.school_category = sdr["school_category"].ToString();
                    pro.district = sdr["district"].ToString();
                    pro.district_name = sdr["district"].ToString();
                    pro.user_name = sdr["user_name"].ToString();
                    pro.password = sdr["password"].ToString();
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

        public school getschoolByiId(int? school_id)
        {
            school pro = new school();

            con.Open();
            SqlCommand cmd = new SqlCommand("manage_school", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Action", "edit_school");
            cmd.Parameters.AddWithValue("@school_id", school_id);

            SqlDataReader sdr = cmd.ExecuteReader();

            if (sdr.HasRows)
            {
                sdr.Read();
                pro.school_id = Convert.ToInt32(sdr["school_id"]);
                pro.school_category = sdr["school_category"].ToString();
                pro.district = sdr["district"].ToString();
                pro.district_name = sdr["district"].ToString();
                pro.user_name = sdr["user_name"].ToString();
                pro.password = sdr["password"].ToString();
                pro.block = sdr["block"].ToString();
                pro.block_name = sdr["block"].ToString();
                pro.school_name = sdr["school_name"].ToString();
                pro.school_code = sdr["school_code"].ToString();
                pro.head_master = sdr["head_master"].ToString();
                pro.email = sdr["email"].ToString();
                pro.mobile = sdr["mobile"].ToString();
                pro.address = sdr["address"].ToString();
                pro.internship_capacity = sdr["internship_capacity"].ToString();

            }
            con.Close();


            return pro;
        }

        public bool delete_school(string school_id)
        {

            if (con.State == ConnectionState.Closed)
                con.Open();

            SqlCommand cmd = new SqlCommand("manage_school", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Action", "delete_school");
            cmd.Parameters.AddWithValue("@school_id", school_id);

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

        public DataTable download_school_report(/*Diet_Project1.Models.Reports.Filter_report filter_report*/)
        {
            string dietIdFromSession = HttpContext.Current.Session["username"].ToString();
            string dietNameFromSession = HttpContext.Current.Session["name"].ToString();

            DataTable dt = new DataTable();

           // string sql = "Select school_id,school_name,school_code,address,mobile,email,internship_capacity from School where status = '1' " + conditions + "";
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("manage_school", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "pdf_excel");
                cmd.Parameters.AddWithValue("@diet_id", dietIdFromSession);

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

        public bool decrease_schoolCapacity(string user_name, string semester) 
        {
            if (con.State == ConnectionState.Closed)
                con.Open();

            SqlCommand cmd = new SqlCommand("manage_internship", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@action", "decrease_school_capacity");
                cmd.Parameters.AddWithValue("@user_name", user_name);
            cmd.Parameters.AddWithValue("@semester", semester);
            int i = cmd.ExecuteNonQuery();
            con.Close();

            return i > 0;
        }

        #endregion

        #region Manage student
        public List<string> GetBatchYears()
        {
            List<string> batchYears = new List<string>();

            int currentYear = DateTime.Now.Year;
            int futureYears = 5; // You can adjust this value based on your needs

            for (int i = 0; i < futureYears; i++)
            {
                int startYear = currentYear + i;
                int endYear = (startYear % 100) + 1; // Take only the last two digits of the year
                string batchYear = $"{startYear}-{endYear:D2}";

                batchYears.Add(batchYear);
            }

            return batchYears;
        }

        public bool insert_student(Diet_Project1.Models.student cg)
        {
            string dietIdFromSession = HttpContext.Current.Session["username"].ToString();
            string dietNameFromSession = HttpContext.Current.Session["name"].ToString();

            SqlCommand cmd = new SqlCommand("manage_student", con);
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
            cmd.Parameters.AddWithValue("@diet_id", dietIdFromSession);
            cmd.Parameters.AddWithValue("@diet_name", dietNameFromSession);
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
            string dietIdFromSession = HttpContext.Current.Session["username"].ToString();
            string dietNameFromSession = HttpContext.Current.Session["name"].ToString();

            List<student> cg = new List<student>();

            con.Open();
            SqlCommand cmd = new SqlCommand("manage_student", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "view_student");
            cmd.Parameters.AddWithValue("@diet_id", dietIdFromSession);
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
                    pro.state_name = sdr["state_name"].ToString();
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
            SqlCommand cmd = new SqlCommand("manage_student", con);
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
                pro.state_name = sdr["state_name"].ToString();
                pro.city = sdr["city"].ToString();
                pro.city_name = sdr["city"].ToString();
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

        public bool delete_student(string student_id)
        {

            if (con.State == ConnectionState.Closed)
                con.Open();

            SqlCommand cmd = new SqlCommand("manage_student", con);
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
            string dietIdFromSession = HttpContext.Current.Session["username"].ToString();
            string dietNameFromSession = HttpContext.Current.Session["name"].ToString();

            DataTable dt = new DataTable();


            //string sql = "Select batch,registration_no,name,father_name,gender,dob,mobile,email from Student where status = '1' " + conditions + "";
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("manage_student", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "pdf-excel");
                cmd.Parameters.AddWithValue("@diet_id", dietIdFromSession);

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

        #region Manage intership_details
        public bool insert_internship(Diet_Project1.Models.internship cg)
        {
            string dietIdFromSession = HttpContext.Current.Session["username"].ToString();
            string dietNameFromSession = HttpContext.Current.Session["name"].ToString();

            SqlCommand cmd = new SqlCommand("manage_internship_details", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@school_category", cg.school_category);
            cmd.Parameters.AddWithValue("@batch", cg.batch);
            cmd.Parameters.AddWithValue("@semester", cg.semester);
            cmd.Parameters.AddWithValue("@panel_open", DateTime.Parse(cg.panel_open).ToString("yyyy-MM-dd 00:00:00"));
            cmd.Parameters.AddWithValue("@apply_date", DateTime.Parse(cg.apply_date).ToString("yyyy-MM-dd 23:59:00"));
            cmd.Parameters.AddWithValue("@start_date", cg.start_date);
            cmd.Parameters.AddWithValue("@end_date", cg.end_date);
            cmd.Parameters.AddWithValue("@school_capacity", cg.school_capacity);
            cmd.Parameters.AddWithValue("@diet_id", dietIdFromSession);
            cmd.Parameters.AddWithValue("@diet_name", dietNameFromSession);
            cmd.Parameters.AddWithValue("@internship_id", cg.internship_id);

            cmd.Parameters.AddWithValue("@Action", "add_internship");

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

        public List<internship> getinternship()
        {
            string dietIdFromSession = HttpContext.Current.Session["username"].ToString();
            string dietNameFromSession = HttpContext.Current.Session["name"].ToString();

            List<internship> cg = new List<internship>();

            con.Open();
            SqlCommand cmd = new SqlCommand("manage_internship_details", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "view_internship");
            cmd.Parameters.AddWithValue("@diet_id", dietIdFromSession);
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
                    // Get the first 10 characters of panel_open
                    pro.panel_open = sdr["panel_open"].ToString().Substring(0, Math.Min(10, sdr["panel_open"].ToString().Length));

                    // Get the first 10 characters of apply_date
                    pro.apply_date = sdr["apply_date"].ToString().Substring(0, Math.Min(10, sdr["apply_date"].ToString().Length));

                    pro.start_date = sdr["start_date"].ToString();
                    pro.end_date = sdr["end_date"].ToString();
                    pro.school_capacity = sdr["school_capacity"].ToString();

                    cg.Add(pro);
                }
            }
            con.Close();


            return cg;
        }

        public internship getinternshipByiId(int? internship_id)
        {
            internship pro = new internship();

            con.Open();
            SqlCommand cmd = new SqlCommand("manage_internship_details", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@action", "edit_internship");
            cmd.Parameters.AddWithValue("@internship_id", internship_id);

            SqlDataReader sdr = cmd.ExecuteReader();

            if (sdr.HasRows)
            {
                sdr.Read();
                pro.internship_id = Convert.ToInt32(sdr["internship_id"]);
                pro.school_category = sdr["school_category"].ToString();
                pro.batch = sdr["batch"].ToString();
                pro.semester = sdr["semester"].ToString();
                pro.school_category = sdr["school_category"].ToString();
                // Get the first 10 characters of panel_open
                pro.panel_open = sdr["panel_open"].ToString().Substring(0, Math.Min(10, sdr["panel_open"].ToString().Length));

                // Get the first 10 characters of apply_date
                pro.apply_date = sdr["apply_date"].ToString().Substring(0, Math.Min(10, sdr["apply_date"].ToString().Length));

                pro.start_date = sdr["start_date"].ToString();
                pro.end_date = sdr["end_date"].ToString();
                pro.school_capacity = sdr["school_capacity"].ToString();

            }
            con.Close();


            return pro;
        }

        public bool delete_internship(string internship_id)
        {

            if (con.State == ConnectionState.Closed)
                con.Open();

            SqlCommand cmd = new SqlCommand("manage_internship_details", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Action", "delete_internship");
            cmd.Parameters.AddWithValue("@internship_id", internship_id);

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

        #endregion

        #region Manage Student_applied_Internships
        //public List<internship> getApliedStudentList()
        //{
        //    string dietIdFromSession = HttpContext.Current.Session["username"].ToString();
        //    string dietNameFromSession = HttpContext.Current.Session["name"].ToString();

        //    List<internship> cg = new List<internship>();
        //    con.Open();
        //    SqlCommand cmd = new SqlCommand("manage_internship", con);
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    cmd.Parameters.AddWithValue("@action", "view_internshipByDiet");
        //    cmd.Parameters.AddWithValue("@diet_id", dietIdFromSession);
        //    SqlDataReader sdr = cmd.ExecuteReader();
        //    internship pro;


        //    if (sdr.HasRows)
        //    {
        //        while (sdr.Read())
        //        {
        //            pro = new internship();
        //            pro.student_name = sdr["student_name"].ToString();
        //            pro.user_name = sdr["user_name"].ToString();
        //            pro.school_category = sdr["school_category"].ToString();
        //            pro.semester = sdr["semester"].ToString();
        //            pro.block = sdr["block"].ToString();
        //            pro.school_name = sdr["school_name"].ToString();
        //            pro.school_code = sdr["school_code"].ToString();
        //            pro.status = sdr["status"].ToString();
        //            pro.status_by_college = sdr["status_by_college"].ToString();

        //            cg.Add(pro);
        //        }
        //    }
        //    con.Close();


        //    return cg;
        //}

        public List<internship> GetAprrovedStudentList()   
        {
            string dietIdFromSession = HttpContext.Current.Session["username"].ToString();
         //   string dietNameFromSession = HttpContext.Current.Session["name"].ToString();

            List<internship> cg = new List<internship>();
            con.Open();
            SqlCommand cmd = new SqlCommand("manage_internship", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "view_ApprovedStudentList");
            cmd.Parameters.AddWithValue("@diet_id", dietIdFromSession);
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
                    pro.status = sdr["status"].ToString();
                    pro.status_by_college = sdr["status_by_college"].ToString();
                    pro.generate_letter = Convert.ToInt32(sdr["generate_letter"]);
                    pro.start_date = sdr["start_date"].ToString();
                    pro.end_date = sdr["end_date"].ToString();
                    pro.last_update = Convert.ToDateTime(sdr["applied_date"]).ToShortDateString();
                    cg.Add(pro);
                }
            }
            con.Close();


            return cg;
        }

        public List<internship> GetApliedDietStudentList()
        {
            string dietIdFromSession = HttpContext.Current.Session["username"].ToString();
            string dietNameFromSession = HttpContext.Current.Session["name"].ToString();

            List<internship> cg = new List<internship>();
            con.Open();
            SqlCommand cmd = new SqlCommand("manage_internship", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "view_directly_Applied_student_ByDiet");
            cmd.Parameters.AddWithValue("@diet_id", dietIdFromSession);
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
        public List<internship> GetApliedCollegeStudentList()
        {
            string dietIdFromSession = HttpContext.Current.Session["username"].ToString();
            string dietNameFromSession = HttpContext.Current.Session["name"].ToString();

            List<internship> cg = new List<internship>();
            con.Open();
            SqlCommand cmd = new SqlCommand("manage_internship", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "view_college_student_Internship_ByDiet");
            cmd.Parameters.AddWithValue("@diet_id", dietIdFromSession);
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

        public internship getstudentByUsername(string user_name, string start_date)
         {
            internship pro = new internship();

            con.Open();
            SqlCommand cmd = new SqlCommand("manage_student", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@action", "getstudentByUsername");
            cmd.Parameters.AddWithValue("@user_name", user_name);
            cmd.Parameters.AddWithValue("@start_date", start_date);

            SqlDataReader sdr = cmd.ExecuteReader();

            if (sdr.HasRows)
            {
                sdr.Read();
                pro.user_name = sdr["user_name"].ToString();
                pro.student_name = sdr["student_name"].ToString();
                pro.batch = sdr["batch"].ToString();
                pro.semester = sdr["semester"].ToString();
                pro.user_name = sdr["user_name"].ToString();
                pro.start_date = sdr["start_date"].ToString();
                pro.end_date = sdr["end_date"].ToString();
                pro.address = sdr["address"].ToString();
                pro.block = sdr["block"].ToString();
                pro.district_name = sdr["district"].ToString();
                pro.college_name = sdr["college_name"].ToString();
                pro.diet_name = sdr["diet_name"].ToString();
            }
            con.Close();

            return pro;
        }
        #endregion

        #region manage_directApply_internship
        public bool internship_apply(Diet_Project1.Models.internship cg)
        {
            string dietIdFromSession = HttpContext.Current.Session["username"].ToString();
            string dietNameFromSession = HttpContext.Current.Session["name"].ToString();

            SqlCommand cmd = new SqlCommand("manage_internship", con);
            cmd.CommandType = CommandType.StoredProcedure;

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
            //cmd.Parameters.AddWithValue("@internship_capacity", cg.internship_capacity);
            cmd.Parameters.AddWithValue("@diet_id", dietIdFromSession);
            cmd.Parameters.AddWithValue("@diet_name", dietNameFromSession);
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
        #endregion

        #region manage_query

        public List<attendance_class> GetQuery()
        {
            string DietIdFromSession = HttpContext.Current.Session["username"].ToString();
            //   string dietNameFromSession = HttpContext.Current.Session["name"].ToString();

            List<attendance_class> cg = new List<attendance_class>();
            con.Open();
            SqlCommand cmd = new SqlCommand("manage_query", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "view_query");
            cmd.Parameters.AddWithValue("@diet_id", DietIdFromSession);
            SqlDataReader sdr = cmd.ExecuteReader();
            attendance_class pro;


            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    pro = new attendance_class();
                    pro.query_id = sdr["query_id"].ToString();
                    pro.school_id = sdr["school_id"].ToString();
                    pro.school_name = sdr["school_name"].ToString();
                    pro.query = sdr["query"].ToString();
                   // pro.query_date = sdr["date"].ToString();
                    pro.status = (bool)sdr["status"];

                    cg.Add(pro);
                }
            }
            con.Close();


            return cg;
        }

        #endregion

        #region managae all binding
        public List<BindCountry> BindCountry()       //Bind Country
        {
            List<BindCountry> Ctg = new List<BindCountry>();
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from CountryMaster", con);
            SqlDataReader sdr = cmd.ExecuteReader();
            BindCountry pro;
            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    pro = new BindCountry();
                    pro.country_id = sdr["ID"].ToString();
                    pro.country_name = sdr["Name"].ToString();
                    Ctg.Add(pro);
                }
            }
            con.Close();
            return Ctg;
        }

        public List<BindState> BindStatebyCountry(string country)       //Bind State by country id
        {
            List<BindState> Ctg = new List<BindState>();
            con.Open();
            SqlCommand cmd = new SqlCommand("manage_countries", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "getstateByCountryId");
            cmd.Parameters.AddWithValue("@CountryID", country);
            SqlDataReader sdr = cmd.ExecuteReader();
            BindState pro;

            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    pro = new BindState();
                    pro.state_id = sdr["ID"].ToString();
                    pro.state_name = sdr["Name"].ToString();
                    pro.country_id = sdr["CountryID"].ToString();
                    Ctg.Add(pro);
                }
            }
            con.Close();
            return Ctg;
        }

        public List<BindCity> BindCity(string state)       //Bind city by state_id
        {
            List<BindCity> Ctg = new List<BindCity>();
            con.Open();
            SqlCommand cmd = new SqlCommand("manage_countries", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "getcityByStateId");
            cmd.Parameters.AddWithValue("@StateID", state);
            SqlDataReader sdr = cmd.ExecuteReader();
            BindCity pro;

            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    pro = new BindCity();
                    pro.city_id = sdr["ID"].ToString();
                    pro.city_name = sdr["Name"].ToString();
                    pro.state_id = sdr["StateID"].ToString();
                    Ctg.Add(pro);
                }
            }
            con.Close();
            return Ctg;
        }

        public List<BindDistrict> BindDistrict()       //Bind District
        {
            List<BindDistrict> Ctg = new List<BindDistrict>();
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT ID, RTRIM(LTRIM(Name)) AS Name FROM districtMaster WHERE UPPER(LTRIM(RTRIM(Name))) = UPPER(LTRIM(RTRIM('GAUTAM BUDDHA NAGAR')))", con);
            // cmd.CommandType = System.Data.CommandType.Text;
            //  cmd.Parameters.AddWithValue("@action", "ViewBook_Ctg");
            SqlDataReader sdr = cmd.ExecuteReader();
            BindDistrict pro;
            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    pro = new BindDistrict();
                    pro.district_id = sdr["ID"].ToString();
                    pro.district_name = sdr["Name"].ToString();
                    Ctg.Add(pro);
                }
            }
            con.Close();
            return Ctg;
        }

        public List<BindBlock> BindBlock(string district)       //Bind block by district_id
        {
            List<BindBlock> Ctg = new List<BindBlock>();
            con.Open();
            SqlCommand cmd = new SqlCommand("manage_countries", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "getblockByDistrictId");
            cmd.Parameters.AddWithValue("@DistrictID", district);
            SqlDataReader sdr = cmd.ExecuteReader();
            BindBlock pro;

            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    pro = new BindBlock();
                    pro.block_id = sdr["ID"].ToString();
                    pro.block_name = sdr["BlockName"].ToString();
                    pro.district_id = sdr["DistrictName"].ToString();
                    Ctg.Add(pro);
                }
            }
            con.Close();
            return Ctg;
        }

        public List<college> bindcollege()  // bind college by diet_id
        {
            string dietIdFromSession = HttpContext.Current.Session["username"].ToString();
            string dietNameFromSession = HttpContext.Current.Session["name"].ToString();

            List<college> cg = new List<college>();

            con.Open();
            SqlCommand cmd = new SqlCommand("manage_college", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "view_college");
            cmd.Parameters.AddWithValue("@diet_id", dietIdFromSession);
            SqlDataReader sdr = cmd.ExecuteReader();
            college pro;

            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    pro = new college();
                    pro.user_name = sdr["user_name"].ToString();
                    pro.college_name = sdr["college_name"].ToString();
                    cg.Add(pro);
                }
            }
            con.Close();
            return cg;
        }

        public List<student> getstudentbycollege(string college_id)     //student bind by college id
        {
            string dietIdFromSession = HttpContext.Current.Session["username"].ToString();

            List<student> cg = new List<student>();
            student pro;

            con.Open();
            SqlCommand cmd = new SqlCommand("manage_college_student", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@college_id", college_id);

            if(college_id == "")
            {
                cmd.Parameters.AddWithValue("@diet_id", dietIdFromSession);
                cmd.Parameters.AddWithValue("@Action", "view_all_student_by_diet");
            }
            else
            {
                cmd.Parameters.AddWithValue("@Action", "view_student");
            }
           
            SqlDataReader sdr = cmd.ExecuteReader();
            

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
                    pro.pin_code = sdr["pin_code"].ToString();
                    pro.address = sdr["address"].ToString();
                    pro.course = sdr["course"].ToString();
                    pro.semester = sdr["semester"].ToString();
                    pro.email = sdr["email"].ToString();
                    pro.mobile = sdr["mobile"].ToString();
                    pro.student_image = sdr["student_image"].ToString();
                    pro.college_name = sdr["college_name"].ToString();

                    cg.Add(pro);
                }
            }
            con.Close();
            return cg;
        }
        #endregion
    }
}