using Diet_Project1.Models;
using NPOI.POIFS.Crypt.Dsig;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;

namespace Diet_Project1.Controllers
{
    public class dietController : Controller
    {
        public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ConnectionString);
        Diet_Project1.Models.DataService db = new Diet_Project1.Models.DataService();
        Diet_Project1.Models.collegeDataService obj = new Diet_Project1.Models.collegeDataService();
        Diet_Project1.Models.studentDataService st = new Diet_Project1.Models.studentDataService();

        #region Dashboard
        public ActionResult Dashboard()
        {
            if (Session["adminname"] != null)
            {
                var hh = db.total_count();
                return View(hh);
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

        #region Manage College
        public ActionResult CollegeRegistration(string college_id)
        {
            if (Session["adminname"] != null)
            {
                college cg = new college();
                List<BindState> states = db.BindState();
                if (college_id != null && college_id != "")
                {
                    cg = db.getcollegeByiId(Convert.ToInt32(college_id));
                }
                cg.BindState = states;
                //cg.BindCity = cities;
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
        public JsonResult getcities(string state)
        {
            // Assuming you have a method to get cities based on the selected state
            List<BindCity> cities = db.BindCity(state);

            return Json(cities);
        }

        [HttpPost]
        public ActionResult CollegeRegistration(college cg)
        {
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
            cmd.Parameters.AddWithValue("@college_id", cg.college_id);

            if (cg.college_id != 0)
            {
                cmd.Parameters.AddWithValue("@action", "update_college");
                if (con.State == ConnectionState.Closed)
                    con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();

                if (i >= 1)
                {
                    TempData["Message"] = "Data Update Successfully";
                    TempData["para"] = "true";
                    return RedirectToAction("CollegeRegistration");
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
                    return RedirectToAction("CollegeRegistration");
                }
                db.insert_college(cg);
                TempData["Message"] = " Data Submitted Successfully";
                TempData["para"] = "true";
                return RedirectToAction("CollegeRegistration");
            }
            return RedirectToAction("CollegeRegistration");
        }

        public ActionResult CollegeList()
        {
            if (Session["adminname"] != null)
            {

                List<Diet_Project1.Models.college> list = new List<Diet_Project1.Models.college>();
                list = db.getcollege();
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

        public ActionResult delete_college(string college_id)
        {
            if (Session["adminname"] != null)
            {
                db.delete_college(college_id);
                return RedirectToAction("CollegeList");
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
        public ActionResult college_pdfdownload(/*Filter_report filter_report*/)
        {
            DataTable dt = db.download_college_report(/*filter_report*/);

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
            Response.AddHeader("content-disposition", "attachment;filename=College Report" + DateTime.Now.ToString("dd_MM_yyyy") + ".pdf");
            Response.BinaryWrite(pdfBytes);
            Response.End();


            return RedirectToAction("CollegeList");

        }

        //Excel Download
        public ActionResult college_exceldownload(/*Filter_report filter_report*/)
        {
            DataTable dt = db.download_college_report(/*filter_report*/);
            string attachment = "attachment; filename=College Report" + DateTime.Now.ToString("dd_MM_yyyy") + ".xls";
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


            return RedirectToAction("CollegeList");
        }

        public ActionResult ViewCollegeDetails(string college_id)
        {
            if (Session["adminname"] != null)
            {
                college cg = new college();
                cg = db.getcollegeByiId(Convert.ToInt32(college_id));
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

        #region Manage school
        public ActionResult SchoolRegistration(string school_id)
        {
            if (Session["adminname"] != null)
            {
                school cg = new school();
                List<BindDistrict> districts = db.BindDistrict();
                if (school_id != null && school_id != "")
                {
                    cg = db.getschoolByiId(Convert.ToInt32(school_id));
                }
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
            // Assuming you have a method to get cities based on the selected state
            List<BindBlock> blocks = db.BindBlock(district);

            return Json(blocks);
        }

        [HttpPost]
        public ActionResult SchoolRegistration(school cg)
        {
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
            cmd.Parameters.AddWithValue("@school_id", cg.school_id);

            if (cg.school_id != 0)
            {
                cmd.Parameters.AddWithValue("@action", "update_school");
                if (con.State == ConnectionState.Closed)
                    con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();

                if (i >= 1)
                {
                    TempData["Message"] = "Data Update Successfully";
                    TempData["para"] = "true";
                    return RedirectToAction("SchoolRegistration");
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
                    TempData["Message"] = "This school code or mobile number is already exists, please check it and try again";
                    TempData["para"] = "false";
                    return RedirectToAction("SchoolRegistration");
                }
                db.insert_school(cg);
                TempData["Message"] = " Data Submitted Successfully";
                TempData["para"] = "true";
                return RedirectToAction("SchoolRegistration");
            }
            return RedirectToAction("SchoolRegistration");
        }

        public ActionResult SchoolList()
        {
            if (Session["adminname"] != null)
            {
                List<Diet_Project1.Models.school> list = new List<Diet_Project1.Models.school>();
                list = db.getschool();
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

        public ActionResult delete_School(string school_id)
        {
            if (Session["adminname"] != null)
            {
                db.delete_school(school_id);
                return RedirectToAction("SchoolList");
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
        public ActionResult school_pdfdownload(/*Filter_report filter_report*/)
        {
            DataTable dt = db.download_school_report(/*filter_report*/);

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
            Response.AddHeader("content-disposition", "attachment;filename=School Report" + DateTime.Now.ToString("dd_MM_yyyy") + ".pdf");
            Response.BinaryWrite(pdfBytes);
            Response.End();


            return RedirectToAction("SchoolList");

        }

        //Excel Download
        public ActionResult school_exceldownload(/*Filter_report filter_report*/)
        {
            DataTable dt = db.download_school_report(/*filter_report*/);
            string attachment = "attachment; filename=School Report" + DateTime.Now.ToString("dd_MM_yyyy") + ".xls";
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


            return RedirectToAction("SchoolList");
        }

        public ActionResult ViewSchoolDetails(string school_id)
        {
            if (Session["adminname"] != null)
            {
                school cg = new school();
                cg = db.getschoolByiId(Convert.ToInt32(school_id));
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

        // Excel File upload
        [HttpPost]
        public ActionResult UploadExcel(HttpPostedFileBase excelFile)
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;


            if (excelFile != null && excelFile.ContentLength > 0)
            {
                List<Diet_Project1.Models.school> schools = new List<Diet_Project1.Models.school>();

                string fileExtension = Path.GetExtension(excelFile.FileName);

                //var startdate = "";
                //var enddate = "";

                if (fileExtension == ".xlsx")
                {
                    using (var package = new ExcelPackage(excelFile.InputStream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];

                        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                        {
                            schools.Add(new Diet_Project1.Models.school
                            {
                                school_category = worksheet.Cells[row, 1].Value.ToString(),
                                district = worksheet.Cells[row, 2].Value.ToString(),
                                block = worksheet.Cells[row, 3].Value.ToString(),
                                school_name = worksheet.Cells[row, 4].Value.ToString(),
                                school_code = worksheet.Cells[row, 5].Value.ToString(),
                                head_master = worksheet.Cells[row, 6].Value.ToString(),
                                email = worksheet.Cells[row, 7].Value.ToString(),
                                mobile = worksheet.Cells[row, 8].Value.ToString(),
                                address = worksheet.Cells[row, 9].Value.ToString(),
                                internship_capacity = worksheet.Cells[row, 10].Value.ToString(),
                            });
                        }
                    }
                }

                con.Open();
                foreach (var school in schools)
                {
                    string dietIdFromSession = Session["username"].ToString();
                    string dietNameFromSession = Session["name"].ToString();

                    SqlCommand cmd = new SqlCommand("manage_school", con);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@action", "add_school");

                    cmd.Parameters.AddWithValue("@school_category", school.school_category);
                    cmd.Parameters.AddWithValue("@district", school.district);
                    cmd.Parameters.AddWithValue("@block", school.block);
                    cmd.Parameters.AddWithValue("@school_name", school.school_name);
                    cmd.Parameters.AddWithValue("@school_code", school.school_code);
                    cmd.Parameters.AddWithValue("@head_master", school.head_master);
                    cmd.Parameters.AddWithValue("@email", school.email);
                    cmd.Parameters.AddWithValue("@mobile", school.mobile);
                    cmd.Parameters.AddWithValue("@address", school.address);
                    cmd.Parameters.AddWithValue("@internship_capacity", school.internship_capacity);
                    cmd.Parameters.AddWithValue("@diet_id", dietIdFromSession);
                    cmd.Parameters.AddWithValue("@diet_name", dietNameFromSession);
                    //   cmd.Parameters.AddWithValue("@action", "insert");

                    int i = cmd.ExecuteNonQuery();
                }

                con.Close();

                return RedirectToAction("SchoolRegistration"); // Redirect to a success page
            }

            return View("UploadExcel");

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
                    cg = db.getstudentByiId(Convert.ToInt32(student_id));
                }
                else
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("manage_student", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@action", "select_student_id");
                    SqlDataReader sdr = cmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        var ab = Convert.ToInt32(sdr["coun"].ToString());
                        ab = ab + 1;

                        cg.registration_no = "D-STU-" + ab;
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
                db.insert_student(cg);
                TempData["Message"] = " Data Submitted Successfully";
                TempData["para"] = "true";
                return RedirectToAction("StudentRegistration");
            }
            return RedirectToAction("StudentRegistration");
        }

        public ActionResult StudentByCollege()
        {
            if (Session["adminname"] != null)
            {
                List<college> data = db.bindcollege(); // Replace with your actual data retrieval logic

                return Json(data, JsonRequestBehavior.AllowGet);
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
        public ActionResult GetStudentsByCollege(string college_id)
        {
            if (Session["adminname"] != null)
            {
                List<student> students = db.getstudentbycollege(college_id);

                return Json(students);
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
                List<Diet_Project1.Models.student> list = new List<Diet_Project1.Models.student>();
                list = db.getstudent();
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
                db.delete_student(student_id);
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
            DataTable dt = db.download_student_report(/*filter_report*/);

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
            DataTable dt = db.download_student_report(/*filter_report*/);
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
                cg = db.getstudentByiId(Convert.ToInt32(student_id));
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

        public ActionResult ViewCollegeStudentDetails(string student_id)
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

        #region Manage internship details
        public ActionResult AddInternship(string internship_id)
        {
            if (Session["adminname"] != null)
            {
                List<string> batchYears = db.GetBatchYears();
                ViewBag.BatchYears = new SelectList(batchYears);

                internship cg = new internship();
                if (internship_id != null && internship_id != "")
                {
                    cg = db.getinternshipByiId(Convert.ToInt32(internship_id));
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

        [HttpPost]
        public ActionResult AddInternship(internship cg)
        {
            string dietIdFromSession = Session["username"].ToString();
            string dietNameFromSession = Session["name"].ToString();

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

            if (cg.internship_id != 0)
            {
                cmd.Parameters.AddWithValue("@action", "update_internship");
                if (con.State == ConnectionState.Closed)
                    con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();

                if (i >= 1)
                {
                    TempData["Message"] = "Data Update Successfully";
                    TempData["para"] = "true";
                    return RedirectToAction("AddInternship");
                }
            }
            else
            {
                cmd.Parameters.AddWithValue("@action", "duplicate_internship");
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    TempData["Message"] = "Intership for this semester is already exists, please check it and try again";
                    TempData["para"] = "false";
                    return RedirectToAction("AddInternship");
                }
                db.insert_internship(cg);
                TempData["Message"] = " Data Submitted Successfully";
                TempData["para"] = "true";
                return RedirectToAction("AddInternship");
            }
            return RedirectToAction("AddInternship");
        }

        public ActionResult InternshipDetails()
        {
            if (Session["adminname"] != null)
            {
                List<Diet_Project1.Models.internship> list = new List<Diet_Project1.Models.internship>();
                list = db.getinternship();
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

        public ActionResult delete_internship(string internship_id)
        {
            if (Session["adminname"] != null)
            {
                db.delete_internship(internship_id);
                return RedirectToAction("InternshipDetails");
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

        //public ActionResult AppliedStudentList()
        //{
        //    List<Diet_Project1.Models.internship> list = new List<Diet_Project1.Models.internship>();
        //    list = db.getApliedStudentList();
        //    return View(list); 
        //}

        public ActionResult All_Approve_StudentList()
        {
            if (Session["adminname"] != null)
            {
                List<Diet_Project1.Models.internship> list = new List<Diet_Project1.Models.internship>();
                list = db.GetAprrovedStudentList();
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

        public ActionResult LetterGenerate(string user_name , string start_date)
        {

            if (Session["adminname"] != null)
            {
                internship cg = new internship();
                if (user_name != null && user_name != "")
                {
                    cg = db.getstudentByUsername(user_name, start_date);
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
        public ActionResult Internship_Applied_by_DietStudent()
        {
            if (Session["adminname"] != null)
            {
                List<Diet_Project1.Models.internship> list = new List<Diet_Project1.Models.internship>();
                list = db.GetApliedDietStudentList();
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

        public ActionResult Internship_Applied_by_CollegeStudent()
        {
            if (Session["adminname"] != null)
            {
                List<Diet_Project1.Models.internship> list = new List<Diet_Project1.Models.internship>();
                list = db.GetApliedCollegeStudentList();
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
        public ActionResult UpdateStatus(List<internship> dataToSend, string action) // approved and reject
        {
            string status = (action == "Approve") ? "Approved" : "Rejected";

            try
            {
                con.Open();

                foreach (var item in dataToSend)
                {
                    SqlCommand cmd = new SqlCommand("manage_internship", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@action", "update_status_byDiet");
                    cmd.Parameters.AddWithValue("@user_name", item.user_name);
                    cmd.Parameters.AddWithValue("@semester", item.semester);
                    cmd.Parameters.AddWithValue("@start_date", item.start_date);
                    cmd.Parameters.AddWithValue("@status", status);

                    if (status == "Rejected")
                    {
                        db.decrease_schoolCapacity(item.user_name, item.semester);
                    }

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
                list = db.getstudent();
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
            if (Session["adminname"] != null)
            {
                string dietIdFromSession = Session["username"].ToString();
                //    string dietNameFromSession = Session["name"].ToString();

                List<student> cg = new List<student>();

                con.Open();
                SqlCommand cmd = new SqlCommand("manage_student", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "view_studentbySemester");
                cmd.Parameters.AddWithValue("@diet_id", dietIdFromSession);
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
        public ActionResult PromoteStudents(string targetSemester, List<int> studentIds)
        {
            if (Session["adminname"] != null)
            {
                con.Open();

                foreach (var studentId in studentIds)
                {
                    SqlCommand cmd = new SqlCommand("manage_student", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    // Add parameters to the SQL command
                    cmd.Parameters.AddWithValue("@action", "update_semester");
                    cmd.Parameters.AddWithValue("@semester", targetSemester);
                    cmd.Parameters.AddWithValue("@student_id", studentId);

                    // Execute the SQL command
                    cmd.ExecuteNonQuery();

                }

                // You can return a success response if needed
                return Json(new { success = true });
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

        #region manage_directApply_internship
        public ActionResult ApplyInternshipByDiet()
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
        public ActionResult ApplyInternshipByDiet(internship cg)
        {
            if (Session["adminname"] != null)
            {

                SqlCommand cmd = new SqlCommand("manage_internship", con);
                cmd.CommandType = CommandType.StoredProcedure;

                // cmd.Parameters.AddWithValue("@user_name", studentId);

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
                        return RedirectToAction("ApplyInternshipByDiet");
                    }
                    db.internship_apply(cg);
                    TempData["Message"] = " Internship Applied Successfully";
                    TempData["para"] = "true";
                    return RedirectToAction("ApplyInternshipByDiet");
                }
                return RedirectToAction("ApplyInternshipByDiet");
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
            if (Session["adminname"] != null)
            {
                List<student> cg = new List<student>();

                con.Open();
                SqlCommand cmd = new SqlCommand("manage_student", con);
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

        //public ActionResult AppliedByDiet()
        //{
        //    return View();
        //}
        #endregion

        #region manage_attendence_query
        
        public ActionResult View_Query()
        {
            if (Session["adminname"] != null)
            {
                List<attendance_class> cg = db.GetQuery();
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
        public ActionResult UpdateQueryStatus(string query_id, string fromDate, string toDate)
        {
            if (Session["adminname"] != null)
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("manage_query", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@action", "update_query");
                cmd.Parameters.AddWithValue("@query_id", query_id);
                cmd.Parameters.AddWithValue("@fromDate", fromDate);
                cmd.Parameters.AddWithValue("@toDate", toDate);

                cmd.ExecuteNonQuery();
                // You can return a success response if needed
                return Json(new { Success = true, Message = "You are allowed to school" });
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
    }
}