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
    public class collegeController : Controller
    {
        public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ConnectionString);
        Diet_Project1.Models.DataService db = new Diet_Project1.Models.DataService();
        Diet_Project1.Models.collegeDataService obj = new Diet_Project1.Models.collegeDataService();
        Diet_Project1.Models.studentDataService st = new Diet_Project1.Models.studentDataService();

        #region Dashboard
        public ActionResult Dashboard()
        {
            var hh = db.total_count();
            return View(hh);
        }
        #endregion

        #region Manage Student
        public ActionResult StudentRegistration(string student_id)
        {
            if (Session["adminname"] != null)
            {
                List<string> batchYears = db.GetBatchYears();
            ViewBag.BatchYears = new SelectList(batchYears);

            student cg = new student();
            List<BindCountry> countries = db.BindCountry();

            if (student_id != null && student_id != "")
            {
                cg = obj.getstudentByiId(Convert.ToInt32(student_id));
            }
            else
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("manage_college_student", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "select_student_id");
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    sdr.Read();
                    var ab = Convert.ToInt32(sdr["coun"].ToString());
                    ab = ab + 1;

                    cg.registration_no = "C-STU-" + ab;
                }
            }
                cg.BindCountry = countries;
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

        [HttpPost]
        public JsonResult getstates(string country)
        {
            // Assuming you have a method to get cities based on the selected state
            List<BindState> states = db.BindStatebyCountry(country);

            return Json(states);
        }

        [HttpPost]
        public ActionResult StudentRegistration(student cg)
        {
            var path = System.IO.Path.Combine(Server.MapPath("/student_image/"));

            HttpPostedFileBase file1 = Request.Files["image_url"];

            string uploadpayslipss1;

            if (file1 != null && file1.FileName.ToString() != "")
            {
                uploadpayslipss1 = DateTime.Now.ToString("ddMMyy") + System.Guid.NewGuid() + "." + file1.FileName.Split('.')[1];
                file1.SaveAs(path + uploadpayslipss1);
                cg.student_image = "/student_image/" + uploadpayslipss1;
            }


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
            cmd.Parameters.AddWithValue("@status", cg.status);

            if (cg.student_id != null && cg.student_id != "")
            {
                cmd.Parameters.AddWithValue("@action", "update_student");
                if (con.State == ConnectionState.Closed)
                    con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();

                if (i >= 1)
                {
                    TempData["Message"] = "Data Update Successfully";
                    TempData["para"] = "true";
                    return RedirectToAction("StudentRegistration");
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
                    return RedirectToAction("StudentRegistration");
                }
                obj.insert_student(cg);
                TempData["Message"] = " Data Submitted Successfully";
                TempData["para"] = "true";
                return RedirectToAction("StudentRegistration");
            }
            return RedirectToAction("StudentRegistration");
        }

        public ActionResult StudentList()
        {
            if (Session["adminname"] != null)
            {
                List<Diet_Project1.Models.student> list = new List<Diet_Project1.Models.student>();
                list = obj.getstudent();
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

        public ActionResult delete_Student(string student_id)
        {
            if (Session["adminname"] != null)
            {
                obj.delete_student(student_id);
                return RedirectToAction("StudentList");
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
        public ActionResult student_pdfdownload(/*Filter_report filter_report*/)
        {
            DataTable dt = obj.download_student_report(/*filter_report*/);

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
            Response.AddHeader("content-disposition", "attachment;filename=Student Report" + DateTime.Now.ToString("dd_MM_yyyy") + ".pdf");
            Response.BinaryWrite(pdfBytes);
            Response.End();


            return RedirectToAction("StudentList");

        }

        //Excel Download
        public ActionResult student_exceldownload(/*Filter_report filter_report*/)
        {
            DataTable dt = obj.download_student_report(/*filter_report*/);
            string attachment = "attachment; filename=Student Report" + DateTime.Now.ToString("dd_MM_yyyy") + ".xls";
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


            return RedirectToAction("StudentList");
        }

        public ActionResult ViewStudentDetails(string student_id)
        {
            if (Session["adminname"] != null)
            {
                student cg = new student();
                cg = obj.getstudentByiId(Convert.ToInt32(student_id));
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
        #endregion

        #region Manage Student_applied_Internships
        public ActionResult AppliedStudentList()
        {
            if (Session["adminname"] != null)
            {
                List<Diet_Project1.Models.internship> list = new List<Diet_Project1.Models.internship>();
                list = obj.getApliedCollegeStudent();
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

        [HttpPost]
        public ActionResult UpdateStatus(List<internship> dataToSend, string action)
        {
            string status = (action == "Approve") ? "Approved" : "Rejected";
            try
            {
                con.Open();

                foreach (var item in dataToSend)
                {
                SqlCommand cmd = new SqlCommand("manage_internship", con);
                cmd.CommandType = CommandType.StoredProcedure;
                // Assuming "manage_internship" is the correct action for updating the status
                cmd.Parameters.AddWithValue("@action", "update_status_byCollege");
                cmd.Parameters.AddWithValue("@user_name", item.user_name);
                cmd.Parameters.AddWithValue("@semester", item.semester);
                cmd.Parameters.AddWithValue("@start_date", item.start_date);
                cmd.Parameters.AddWithValue("@status_by_college", status);

                    int i = cmd.ExecuteNonQuery();

                    if (i < 1)
                    {
                        // Handle failure for individual rows if needed
                        return Json(new { success = false, message = "Failed to update status for some rows" });
                    }
                }


                return Json(new { success = true, message = "Status updated successfully" });
            }
            catch (Exception ex)
            {
                // Handle exception if needed
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }
        #endregion

        #region manaage_RenewalStudents
        public ActionResult RenewalStudents()
        {
            if (Session["adminname"] != null)
            {
                List<Diet_Project1.Models.student> list = new List<Diet_Project1.Models.student>();
                list = obj.getstudent();
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

        [HttpPost]
        public ActionResult FilterStudentsBySemester(string semester)  //filter Students by semester
        {
            string CollegeIdFromSession = Session["username"].ToString();
            //    string dietNameFromSession = Session["name"].ToString();

            List<student> cg = new List<student>();

            con.Open();
            SqlCommand cmd = new SqlCommand("manage_college_student", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "view_studentbySemester");
            cmd.Parameters.AddWithValue("@college_id", CollegeIdFromSession);
            cmd.Parameters.AddWithValue("@semester", semester);
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

                    cg.Add(pro);
                }
            }
            con.Close();
            return Json(cg, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PromoteStudents(string targetSemester, List<int> studentIds)
        {
            con.Open();

            foreach (var studentId in studentIds)
            {
                SqlCommand cmd = new SqlCommand("manage_college_student", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "update_semester");
                cmd.Parameters.AddWithValue("@semester", targetSemester);
                cmd.Parameters.AddWithValue("@student_id", studentId);
                cmd.ExecuteNonQuery();
            }
            return Json(new { success = true });
        }
        #endregion

        #region manage_directApply_internship
        public ActionResult ApplyInternshipByCollege()
        {
            if (Session["adminname"] != null)
            {
                internship cg = new internship();

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
        public ActionResult ApplyInternshipByCollege(internship cg)
        {
            if (Session["adminname"] != null)
            {
                SqlCommand cmd = new SqlCommand("manage_internship", con);
                cmd.CommandType = CommandType.StoredProcedure;

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
                        return RedirectToAction("ApplyInternshipByCollege");
                    }
                    db.internship_apply(cg);
                    TempData["Message"] = " Internship Applied Successfully";
                    TempData["para"] = "true";
                    return RedirectToAction("ApplyInternshipByCollege");
                }
                return RedirectToAction("ApplyInternshipByCollege");
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
        public ActionResult GetStudentName(string username)  // Change parameter name to 'username'
        {
            List<student> cg = new List<student>();

            con.Open();
            SqlCommand cmd = new SqlCommand("manage_college_student", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "ViewstudentByUsername");
            cmd.Parameters.AddWithValue("@user_name", username);  // Use the 'username' parameter
            SqlDataReader sdr = cmd.ExecuteReader();
            student pro;

            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    pro = new student();
                    pro.student_name = sdr["name"].ToString();
                    pro.user_name = sdr["user_name"].ToString();
                    cg.Add(pro);
                }
            }
            con.Close();
            return Json(cg);
        }

        [HttpPost]
        public JsonResult getschools(string block)
        {

            List<school> cg = new List<school>();

            con.Open();
            SqlCommand cmd = new SqlCommand("manage_internship", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "view_school_for_intershipbyDiet");
            cmd.Parameters.AddWithValue("@block", block);
            //cmd.Parameters.AddWithValue("@user_name", studentIdFromSession);
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
                    //pro.district = sdr["Name"].ToString();
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


            return Json(cg);
        }

        [HttpPost]
        public JsonResult getschool_details(string school_code)
        {
            List<school> details = st.getschoolDetails(school_code);
            return Json(details);
        }

      
        #endregion

    }
}