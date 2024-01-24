using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Configuration;

namespace Diet_Project1.Models
{
    public class adminDataService
    {
        public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ConnectionString);

        #region Manage diet
        public bool insert_diet(Diet_Project1.Models.diet cg)
        {
            SqlCommand cmd = new SqlCommand("manage_diet", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@diet_name", cg.diet_name);
            cmd.Parameters.AddWithValue("@password", cg.password);
            cmd.Parameters.AddWithValue("@director_name", cg.director_name);
            cmd.Parameters.AddWithValue("@address", cg.address);
            cmd.Parameters.AddWithValue("@district", cg.district);
            cmd.Parameters.AddWithValue("@block", cg.block);
            cmd.Parameters.AddWithValue("@mobile", cg.mobile);
            cmd.Parameters.AddWithValue("@email", cg.email);
            cmd.Parameters.AddWithValue("@website_url", cg.website_url);
            cmd.Parameters.AddWithValue("@diet_id", cg.diet_id);

            cmd.Parameters.AddWithValue("@action", "add_diet");

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

        public List<diet> getdiet()
        {
            List<diet> cg = new List<diet>();

            con.Open();
            SqlCommand cmd = new SqlCommand("manage_diet", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "view_diet");
            SqlDataReader sdr = cmd.ExecuteReader();
            diet pro;

            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    pro = new diet();
                    pro.diet_id = Convert.ToInt32(sdr["diet_id"]);
                    pro.diet_name = sdr["diet_name"].ToString();
                    pro.user_name = sdr["user_name"].ToString();
                    pro.password = sdr["password"].ToString();
                    pro.director_name = sdr["director_name"].ToString();
                    pro.address = sdr["address"].ToString();
                    pro.district_name = sdr["district"].ToString();
                    pro.district = sdr["district"].ToString();
                    pro.block = sdr["block"].ToString();
                    pro.block_name = sdr["block"].ToString();
                    pro.email = sdr["email"].ToString();
                    pro.mobile = sdr["mobile"].ToString();
                    pro.website_url = sdr["website_url"].ToString();

                    cg.Add(pro);
                }
            }
            con.Close();


            return cg;
        }

        public diet getdietByiId(int? diet_id)
        {
            diet pro = new diet();

            con.Open();
            SqlCommand cmd = new SqlCommand("manage_diet", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Action", "edit_diet");
            cmd.Parameters.AddWithValue("@diet_id", diet_id);

            SqlDataReader sdr = cmd.ExecuteReader();

            if (sdr.HasRows)
            {
                sdr.Read();
                pro.diet_id = Convert.ToInt32(sdr["diet_id"]);
                pro.diet_name = sdr["diet_name"].ToString();
                pro.user_name = sdr["user_name"].ToString();
                pro.password = sdr["password"].ToString();
                pro.director_name = sdr["director_name"].ToString();
                pro.address = sdr["address"].ToString();
                pro.district_name = sdr["district"].ToString();
                pro.district = sdr["district"].ToString();
                pro.block = sdr["block"].ToString();
                pro.block_name = sdr["block"].ToString();
                pro.email = sdr["email"].ToString();
                pro.mobile = sdr["mobile"].ToString();
                pro.website_url = sdr["website_url"].ToString();

            }
            con.Close();


            return pro;
        }

        public bool delete_diet(string diet_id,string user_name)
        {

            if (con.State == ConnectionState.Closed)
                con.Open();

            SqlCommand cmd = new SqlCommand("manage_diet", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Action", "delete_diet");
            cmd.Parameters.AddWithValue("@diet_id", diet_id);
            cmd.Parameters.AddWithValue("@user_name", user_name);

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

        public DataTable download_diet_report(/*Diet_Project1.Models.Reports.Filter_report filter_report*/)
        {
            DataTable dt = new DataTable();

            string conditions = "";
            //if (filter_report.BookCtg_Name != null)
            //{
            //    if (filter_report.BookCtg_Name != null && filter_report.BookCtg_Name != "")
            //    {
            //        if (conditions != "")
            //        {
            //            conditions = conditions + " and BookCtg_Name='" + filter_report.BookCtg_Name.ToString().Replace("'", "").ToLower() + "'";
            //        }
            //        else
            //        {
            //            conditions = conditions + " BookCtg_Name ='" + filter_report.BookCtg_Name.ToString().Replace("'", "").ToLower() + "'";
            //        }

            //    }

            //    if (conditions != "")
            //    {
            //        conditions = "where " + conditions + "";
            //    }
            //}


            string sql = "Select diet_id,diet_name,director_name,address,mobile,email from diet where status = '1' " + conditions + "";
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