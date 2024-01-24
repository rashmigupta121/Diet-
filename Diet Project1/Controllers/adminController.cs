using Diet_Project1.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace Diet_Project1.Controllers
{
    public class adminController : Controller
    {
        public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ConnectionString);
        Diet_Project1.Models.adminDataService obj = new Diet_Project1.Models.adminDataService();
        Diet_Project1.Models.DataService db = new Diet_Project1.Models.DataService();

        #region Dashboard
        public ActionResult Dashboard()
        {
            var hh = db.total_count();
            return View(hh);
        }
        #endregion


        #region Manage diet
        public ActionResult DietRegistration(string diet_id)
        {
            if (Session["adminname"] != null)
            {
                diet cg = new diet();
                List<BindDistrict> districts = db.BindDistrict();
                List<BindBlock> blocks = new List<BindBlock>();
                if (diet_id != null && diet_id != "")
                {
                    cg = obj.getdietByiId(Convert.ToInt32(diet_id));
                }
                cg.BindDistrict = districts;
                cg.BindBlock = blocks;
                return View(cg);

            }
            else
            {
                Models.common_response Response = db.adminssioncheck("");
                if (Response.success == false || Response.parameter != "admin")
                {
                    string url = Request.Url.PathAndQuery;
                    return Redirect("/Home/login?url=" + HttpUtility.UrlEncode(url) + "");
                }

            }

            return View("login");

        }
        [HttpPost]
        public ActionResult DietRegistration(diet cg)
        {
            SqlCommand cmd = new SqlCommand("manage_diet", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@diet_name", cg.diet_name);
         //   cmd.Parameters.AddWithValue("@user_name", cg.user_name);
         //  cmd.Parameters.AddWithValue("@password", cg.password);
            cmd.Parameters.AddWithValue("@director_name", cg.director_name);
            cmd.Parameters.AddWithValue("@address", cg.address);
            cmd.Parameters.AddWithValue("@district", cg.district);
            cmd.Parameters.AddWithValue("@block", cg.block);
            cmd.Parameters.AddWithValue("@mobile", cg.mobile);
            cmd.Parameters.AddWithValue("@email", cg.email);
            cmd.Parameters.AddWithValue("@website_url", cg.website_url);
            cmd.Parameters.AddWithValue("@diet_id", cg.diet_id);

            if (cg.diet_id != 0)
            {
                cmd.Parameters.AddWithValue("@action", "update_diet");
                if (con.State == ConnectionState.Closed)
                    con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();

                if (i >= 1)
                {
                    TempData["Message"] = "Data Update Successfully";
                    TempData["para"] = "true";
                    return RedirectToAction("DietRegistration");
                }
            }
            else
            {
                cmd.Parameters.AddWithValue("@action", "duplicate_record");
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    TempData["Message"] = "This mobile number is already exists, please check it and try again";
                    TempData["para"] = "false";
                    return RedirectToAction("DietRegistration");
                }
                obj.insert_diet(cg);
                TempData["Message"] = " Data Submitted Successfully";
                TempData["para"] = "true";
                return RedirectToAction("DietRegistration");
            }
            return RedirectToAction("DietRegistration");
        }

        public ActionResult DietList()
        {
            if (Session["adminname"] != null)
            {
                List<Diet_Project1.Models.diet> list = new List<Diet_Project1.Models.diet>();
                list = obj.getdiet();
                return View(list);
            }
            else
            {
                Models.common_response Response = db.adminssioncheck("");
                if (Response.success == false || Response.parameter != "admin")
                {
                    string url = Request.Url.PathAndQuery;
                    return Redirect("/Home/login?url=" + HttpUtility.UrlEncode(url) + "");
                }

            }

            return View("login");
        }

        public ActionResult ViewDietDetails(string diet_id) 
        {
            if (Session["adminname"] != null)
            {
                diet cg = new diet();
                if (diet_id != null && diet_id != "")
                {
                    cg = obj.getdietByiId(Convert.ToInt32(diet_id));
                }
                return View(cg);
            }
            else
            {
                Models.common_response Response = db.adminssioncheck("");
                if (Response.success == false || Response.parameter != "admin")
                {
                    string url = Request.Url.PathAndQuery;
                    return Redirect("/Home/login?url=" + HttpUtility.UrlEncode(url) + "");
                }

            }

            return View("login");
        }

        public ActionResult delete_diet(string diet_id,string user_name)
        {
            if (Session["adminname"] != null)
            {
                obj.delete_diet(diet_id, user_name);
                return RedirectToAction("DietList");
            }
            else
            {
                Models.common_response Response = db.adminssioncheck("");
                if (Response.success == false || Response.parameter != "admin")
                {
                    string url = Request.Url.PathAndQuery;
                    return Redirect("/Home/login?url=" + HttpUtility.UrlEncode(url) + "");
                }

            }

            return View("login");
        }

        //Pdf Download
        public ActionResult diet_pdfdownload(/*Filter_report filter_report*/)
        {
            DataTable dt = obj.download_diet_report(/*filter_report*/);

            string html = " <html> <head> <meta charset='utf-8'> <meta http-equiv='X-UA-Compatible' content='IE=edge'><style> body *{font-family:'Montserrat';} table { width: 100%; border-collapse: collapse; border: 2px solid #3498db;margin-top: 20px; }  td, th {  border: 1px solid #3498db; padding: 10px; text-align: center; font-size: 15px;}  th {background-color: #3498db; color: #fff;padding: 10px 0;text-align: center; text-transform: capitalize; font-size: 18px; border: 1px solid white;}  </style>  <link href='https://fonts.googleapis.com/css2?family=Montserrat:wght@400;500;600;700;800&display=swap' rel='stylesheet'> </head> <body><div> <label style='text-align:center; color:Black; margin:5px; font-weight:bolder; font-size:30px;'>Diet (Gautam Budh Nagar)</label></div><hr> <div>" +
                " <table> <thead>";

            var col = 0;
            foreach (DataColumn dc in dt.Columns)
            {
                col++;
                if (col == 20)
                {
                    break;
                }
                html = html + " <th>" + dc.ColumnName + "</th>";
            }
            html = html + "  </thead> <tbody>";
            int i;
            foreach (DataRow dr in dt.Rows)
            {
                html = html + " <tr> ";
                for (i = 0; i < dt.Columns.Count; i++)
                {
                    html = html + "<td>" + dr[i].ToString() + "</td>";
                }
                html = html + " </tr>";
            }
            html = html + "  </tbody> </table> </div></body></html> ";


            var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
            htmlToPdf.Orientation = NReco.PdfGenerator.PageOrientation.Landscape;
            var pdfBytes = htmlToPdf.GeneratePdf(html);

            Response.Clear();
            Response.ContentType = "application/force-download";
            Response.AddHeader("content-disposition", "attachment;filename=diet Report" + DateTime.Now.ToString("dd_MM_yyyy") + ".pdf");
            Response.BinaryWrite(pdfBytes);
            Response.End();


            return RedirectToAction("DietList");

        }

        //Excel Download
        public ActionResult diet_exceldownload(/*Filter_report filter_report*/)
        {
            DataTable dt = obj.download_diet_report(/*filter_report*/);
            string attachment = "attachment; filename=diet Report" + DateTime.Now.ToString("dd_MM_yyyy") + ".xls";
            Response.ClearContent();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/vnd.ms-excel";
            string tab = "";
            string fullhtml = "";
            int datecolumn_index = 0;
            int datecolumn_dob_index = 0;
            int col = 0;

            foreach (DataColumn dc in dt.Columns)
            {
                col++;
                if (col == 20)
                {
                    break;
                }

                fullhtml = fullhtml + "<th style='border:1px solid black'>" + dc.ColumnName + "</th>";

            }
            fullhtml = "<thead><tr>" + fullhtml + "</tr></thead><tbody>";
            int i;

            string bodyhtml = "";
            foreach (DataRow dr in dt.Rows)
            {
                bodyhtml = bodyhtml + "<tr>";
                for (i = 0; i < dt.Columns.Count; i++)
                {
                    if (i == datecolumn_index || datecolumn_dob_index == i)
                    {
                        try
                        {
                            bodyhtml = bodyhtml + "<td  style='border:1px solid black;vertical-align: top;text-align: left;'>" + Convert.ToDateTime(tab + dr[i].ToString().Replace("<p>", "").Replace("</p>", "").Replace("\n", "").Replace("\t", "")).ToString("yyyy-MM-dd") + "</td>";
                        }
                        catch
                        {
                            bodyhtml = bodyhtml + "<td  style='border:1px solid black;vertical-align: top;text-align: left;'>" + (tab + dr[i].ToString().Replace("<p>", "").Replace("</p>", "").Replace("\n", "").Replace("\t", "")) + "</td>";
                        }

                    }
                    else
                    {
                        bodyhtml = bodyhtml + "<td  style='border:1px solid black;vertical-align: top;text-align: left;'>" + (tab + dr[i].ToString().Replace("<p>", "").Replace("</p>", "").Replace("\n", "").Replace("\t", "")) + "</td>";
                    }
                }
                bodyhtml = bodyhtml + "</tr>";
            }
            fullhtml = "<table style='border-collapse: collapse;'>" + fullhtml + bodyhtml + "</body></table>";
            Response.Write(fullhtml);
            Response.End();


            return RedirectToAction("DietList");
        }
        #endregion

        public ActionResult CollegeList()
        {
            if (Session["adminname"] != null)
            {
                List<college> cg = new List<college>();

                con.Open();
                SqlCommand cmd = new SqlCommand("manage_admin_panel", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "view_college");
            //    cmd.Parameters.AddWithValue("@diet_id", dietIdFromSession);
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
                        pro.state = sdr["Name"].ToString();
                        pro.email = sdr["email"].ToString();
                        pro.mobile = sdr["mobile"].ToString();
                        pro.website_url = sdr["website_url"].ToString();
                        pro.diet_name = sdr["diet_name"].ToString();

                        cg.Add(pro);
                    }
                }
                con.Close();
               
                return View (cg);

            }
            else
            {
                Models.common_response Response = db.adminssioncheck("");
                if (Response.success == false || Response.parameter != "admin")
                {
                    string url = Request.Url.PathAndQuery;
                    return Redirect("/Home/login?url=" + HttpUtility.UrlEncode(url) + "");
                }

            }

            return View("login");
        }

        public ActionResult ViewCollegeDetails()
        {
            return View();
        }


        public ActionResult SchoolList()
        {
            if (Session["adminname"] != null)
            {
                List<school> cg = new List<school>();

                con.Open();
                SqlCommand cmd = new SqlCommand("manage_admin_panel", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "view_school");
                //cmd.Parameters.AddWithValue("@diet_id", dietIdFromSession);
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
                        pro.diet_name = sdr["diet_name"].ToString();

                        cg.Add(pro);
                    }
                }
                con.Close();


                return View (cg);
            }
            else
            {
                Models.common_response Response = db.adminssioncheck("");
                if (Response.success == false || Response.parameter != "admin")
                {
                    string url = Request.Url.PathAndQuery;
                    return Redirect("/Home/login?url=" + HttpUtility.UrlEncode(url) + "");
                }
            }

            return View("login");
        }

        public ActionResult StudentList()
        {
            if (Session["adminname"] != null)
            {
                List<student> cg = new List<student>();

                con.Open();
                SqlCommand cmd = new SqlCommand("manage_admin_panel", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "view_student");
               // cmd.Parameters.AddWithValue("@diet_id", dietIdFromSession);
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
                        pro.pin_code = sdr["pin_code"].ToString();
                        pro.address = sdr["address"].ToString();
                        pro.course = sdr["course"].ToString();
                        pro.semester = sdr["semester"].ToString();
                        pro.email = sdr["email"].ToString();
                        pro.mobile = sdr["mobile"].ToString();
                        pro.student_image = sdr["student_image"].ToString();
                       // pro.diet_name = sdr["diet_name"].ToString();

                        cg.Add(pro);
                    }
                }
                con.Close();


                return View(cg);
            }
            else
            {
                Models.common_response Response = db.adminssioncheck("");
                if (Response.success == false || Response.parameter != "admin")
                {
                    string url = Request.Url.PathAndQuery;
                    return Redirect("/Home/login?url=" + HttpUtility.UrlEncode(url) + "");
                }
            }
            return View("login");
        }
    }
}