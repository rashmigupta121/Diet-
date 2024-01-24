using Diet_Project1.Models;
using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Diet_Project1.Controllers
{
    public class studentController : Controller
    {
        public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ConnectionString);
        Diet_Project1.Models.DataService db = new Diet_Project1.Models.DataService();
        Diet_Project1.Models.studentDataService obj = new Diet_Project1.Models.studentDataService();
        public ActionResult Dashboard()
        {
            return View();
        }
        [HttpPost]
        public JsonResult getInternshipDetails()
        {
            try
            {
                string studentId = Session["username"] as string;
                string studentName = Session["name"] as string;

                bool showPopup = obj.popup_show();

                if (!showPopup)
                {
                    List<internship> internships = new List<internship>();
                        con.Open();

                        using (SqlCommand cmd = new SqlCommand("manage_student", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@action", "View_internship");
                            cmd.Parameters.AddWithValue("@user_name", studentId);

                            using (SqlDataReader sdr = cmd.ExecuteReader())
                            {
                                while (sdr.Read())
                                {
                                    internship pro = new internship
                                    {
                                        internship_id = Convert.ToInt32(sdr["internship_id"]),
                                        batch = sdr["batch"].ToString(),
                                        school_category = sdr["school_category"].ToString(),
                                        semester = sdr["semester"].ToString(),
                                        panel_open = sdr["panel_open"].ToString().Substring(0, Math.Min(10, sdr["panel_open"].ToString().Length)),
                                        apply_date = sdr["apply_date"].ToString().Substring(0, Math.Min(10, sdr["apply_date"].ToString().Length)),
                                        start_date = sdr["start_date"].ToString(),
                                        end_date = sdr["end_date"].ToString(),
                                        school_capacity = sdr["school_capacity"].ToString()
                                    };

                                    internships.Add(pro);
                                }
                            }
                        }
                    return Json(internships);
                }
                else
                {
                    // Handle the case where the popup should be shown
                    return Json(new { message = "Popup should be shown" });
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here, log the error, or return an appropriate response
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult popup_status(string panel_open, string apply_date)
        {
            string studentIdFromSession = Session["username"].ToString();

            SqlCommand cmd = new SqlCommand("manage_popup", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@student_id", studentIdFromSession);
            cmd.Parameters.AddWithValue("@panel_open", panel_open);
            cmd.Parameters.AddWithValue("@apply_date", apply_date);

            cmd.Parameters.AddWithValue("@action", "insert");

            if (con.State == ConnectionState.Closed)
                con.Open();
             cmd.ExecuteNonQuery();
            con.Close();

             return View();
           
        }
        public ActionResult ApplyInternship()
        {
            if (Session["adminname"] != null)
            {
                string studentId = Session["username"] as string;
                string studentName = Session["name"] as string;

                internship cg = new internship
                {
                    student_name = studentName,
                };

                List<BindDistrict> districts = db.BindDistrict();

                cg.BindDistrict = districts;
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
        public JsonResult getblocks(string district)
        {
            List<BindBlock> blocks = db.BindBlock(district);
            return Json(blocks);
        }

        [HttpPost]
        public JsonResult getschools(string block)
        {
            List<school> schools = obj.Bindschool(block);
            return Json(schools);
        }

        [HttpPost]
        public JsonResult getschool_details(string school_code)
        {
            List<school> details = obj.getschoolDetails(school_code);
            return Json(details);
        }

        [HttpPost]
        public ActionResult ApplyInternship(internship cg)
        {
            if (Session["adminname"] != null)
            {
                string studentId = Session["username"] as string;
                // string studentName = Session["name"] as string;

                SqlCommand cmd = new SqlCommand("manage_internship", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@user_name", studentId);

                if (cg.internship_id == 0)
                {
                    cmd.Parameters.AddWithValue("@action", "duplicate_record");
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        TempData["Message"] = "You have already applied for this internship. Thank you for your application!";
                        TempData["para"] = "false";
                        return RedirectToAction("ApplyInternship");
                    }
                    obj.internship_apply(cg);
                    TempData["Message"] = " Internship Applied Successfully";
                    TempData["para"] = "true";
                    return RedirectToAction("ApplyInternship");
                }
                return RedirectToAction("ApplyInternship");
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


        public ActionResult StudentProfile()
        {
            if (Session["adminname"] != null)
            {
                student cg = obj.ViewStudentProfile();
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

        public ActionResult ViewAttendance()
        {
            if (Session["adminname"] != null)
            {
                return View();
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

        
        public ActionResult GetAttendanceData(string semester)
        {
            if (Session["adminname"] != null)
            {
                List<attendance_class> cg = obj.GetAttendance(semester);
                return Json(cg, JsonRequestBehavior.AllowGet);
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