using Diet_Project1.Models;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml.Table;

namespace Diet_Project1.Controllers
{
    public class schoolController : Controller
    {
        public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ConnectionString);
        Diet_Project1.Models.DataService db = new Diet_Project1.Models.DataService();
        Diet_Project1.Models.schoolDataService ob = new Diet_Project1.Models.schoolDataService();

        public static List<attendance_class> GetStudentAttandanceList = new List<attendance_class>();
        public static string Student_Semester = "";
        public ActionResult Dashboard()
        {
            return View();
        }

        #region manage_approved student list
        public ActionResult ApprovedStudentList()
        {
            if (Session["adminname"] != null)
            {
                List<Diet_Project1.Models.student> list = new List<Diet_Project1.Models.student>();
                list = ob.GetAprrovedStudentList();
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
                    cmd.Parameters.AddWithValue("@action", "update_status_bySchool");
                    cmd.Parameters.AddWithValue("@user_name", item.user_name);
                    cmd.Parameters.AddWithValue("@semester", item.semester);
                    cmd.Parameters.AddWithValue("@start_date", item.start_date);
                    cmd.Parameters.AddWithValue("@status", status);

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

        #region manage intership_details

        public ActionResult Intership_details() 
        {
            if (Session["adminname"] != null)
            {
                List<internship> cg = ob.getinternshipdetails();
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

        #region manage mark attendance
        public ActionResult MarkAttendance()
        {
            if (Session["adminname"] != null)
            {
                List<Diet_Project1.Models.student> list = new List<Diet_Project1.Models.student>();
                list = ob.GetAprrovedStudentList();
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

        public ActionResult GetStudentsBySemester(string semester)
        {
            if (Session["adminname"] != null)
            {
                string schoolIdFromSession = Session["username"].ToString();
                //    string dietNameFromSession = Session["name"].ToString();

                List<student> cg = new List<student>();

                con.Open();
                SqlCommand cmd = new SqlCommand("manage_attendance", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "filter_StudentbySemester");
                cmd.Parameters.AddWithValue("@school_id", schoolIdFromSession);
                cmd.Parameters.AddWithValue("@semester", semester);
                SqlDataReader sdr = cmd.ExecuteReader();
                student pro;

                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        pro = new student();
                        //  pro.student_id = sdr["student_id"].ToString();
                        pro.student_name = sdr["name"].ToString();
                        pro.student_id = sdr["user_name"].ToString();

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
        public JsonResult SubmitAttendance(List<attendance_class> attendanceData)
        {
            string schoolIdFromSession = Session["username"]?.ToString();
            string schoolNameFromSession = Session["name"]?.ToString();

            try
            {
                con.Open();

                foreach (var data in attendanceData)
                {
                    DateTime currentDate = DateTime.Now;
                    // Check if the date is valid using internshipDatematch
                    bool isDateValid = ob.internshipDatematch(data.date ?? currentDate, data.student_id);

                    if (isDateValid)
                    {
                        SqlCommand cmd = new SqlCommand("manage_attendance", con);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@action", "mark_attendance");
                        cmd.Parameters.AddWithValue("@school_id", schoolIdFromSession);
                        cmd.Parameters.AddWithValue("@school_name", schoolNameFromSession);
                        cmd.Parameters.AddWithValue("@student_id", data.student_id);
                        cmd.Parameters.AddWithValue("@student_name", data.student_name);

                        string default_attendance = "Absent";
                        cmd.Parameters.AddWithValue("@attendance", data.attendance ?? default_attendance);

                        // Check if date is null, if true, set it to the current date
                        cmd.Parameters.AddWithValue("@date", data.date ?? currentDate);

                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        // Return an error response if the date is not valid for any data
                        return Json(new { Success = false, Message = "No attendance mark for any student. Error submitting attendance for student name: " + data.student_name + " and date:" + (data.date ?? currentDate) });
                    }
                }

                // Return a success response after processing all attendance data
                return Json(new { Success = true, Message = "Attendance submitted successfully" });
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log, return error response)
                return Json(new { Success = false, Message = "Error submitting attendance: " + ex.Message });
            }
            finally
            {
                con.Close();
            }
        }


        #endregion

        #region Manage_Attendence_List
        public ActionResult AttendanceList()
        {
            return View();
        }
        public ActionResult FilterAttendanceList(string date, string semester)
        {
            if (Session["adminname"] != null)
            {
                string schoolIdFromSession = Session["username"].ToString();

                List<attendance_class> cg = new List<attendance_class>();
                con.Open();
                SqlCommand cmd = new SqlCommand("manage_attendance", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "view_attendance");
                cmd.Parameters.AddWithValue("@school_id", schoolIdFromSession);
                cmd.Parameters.AddWithValue("@date", date);
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

        public ActionResult GetStudentList(string semester)
        {
            if (Session["adminname"] != null)
            {
                string schoolIdFromSession = Session["username"].ToString();

                List<attendance_class> cg = new List<attendance_class>();
                con.Open();
                SqlCommand cmd = new SqlCommand("manage_attendance", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "get_studentbySemester");
                cmd.Parameters.AddWithValue("@school_id", schoolIdFromSession);
                // cmd.Parameters.AddWithValue("@date", date);
                cmd.Parameters.AddWithValue("@semester", semester);
                SqlDataReader sdr = cmd.ExecuteReader();
                attendance_class pro;


                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        pro = new attendance_class();
                        pro.student_name = sdr["student_name"].ToString();
                        pro.student_id = sdr["student_id"].ToString();
                        //  pro.school_name = sdr["school_name"].ToString();
                        //  pro.date = sdr.GetDateTime(sdr.GetOrdinal("date"));
                        //  pro.attendance = sdr["attendance"].ToString();
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

        public ActionResult FilterAttendanceBySemesterAndStudent(string student, string semester)
        {
            if (Session["adminname"] != null)
            {
                string schoolIdFromSession = Session["username"].ToString();

                List<attendance_class> cg = new List<attendance_class>();
                con.Open();
                SqlCommand cmd = new SqlCommand("manage_attendance", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "view_attendance_Student_Base");
                cmd.Parameters.AddWithValue("@school_id", schoolIdFromSession);
                cmd.Parameters.AddWithValue("@student_id", student);
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

        public ActionResult studentAttendance_exceldownload(string selectedSemester, string selectedDate) // date based report
        {
            DataTable dt = ob.download_studentAttendance_report(selectedSemester, selectedDate);

            // Add a Serial Number column to the DataTable as the first column
            DataColumn serialNoColumn = new DataColumn("S.No.", typeof(int));
            dt.Columns.Add(serialNoColumn);

            // Add serial numbers to each row
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i]["S.No."] = i + 1;
            }

            // Create a new Excel package
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                // Add a worksheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Attendance Report");

                // Rearrange the columns to ensure "S.No." appears as the first column
                DataTable reorderedDt = new DataTable();
                reorderedDt.Columns.Add("S.No.", typeof(int));
                reorderedDt.Merge(dt);

                // Load data to the worksheet with headers
                worksheet.Cells["A1"].LoadFromDataTable(reorderedDt, true, TableStyles.None);

                // Set date format for the "date" column (assuming it's the second column)
                int dateColumnIndex = 2; // Adjust this index based on your actual column order
                using (var range = worksheet.Cells[2, dateColumnIndex, worksheet.Dimension.End.Row, dateColumnIndex])
                {
                    range.Style.Numberformat.Format = "yyyy-mm-dd"; // Adjust the date format as needed
                }

                // Set column width
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Clear response content
                Response.ClearContent();

                // Set the content type and header for the Excel file
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=Attendance Report" + DateTime.Now.ToString("dd_MM_yyyy") + ".xlsx");

                // Write the Excel package to the response stream
                Response.BinaryWrite(excelPackage.GetAsByteArray());

                // End the response
                Response.End();
            }

            return RedirectToAction("AttendanceList");
        }

        public ActionResult studentAttendancebyName_exceldownload(string selectedSemester1, string selectedStudent1) //student based report
        {
            DataTable dt = ob.download_studentAttendance_report2(selectedSemester1, selectedStudent1);

            // Add a Serial Number column to the DataTable as the first column
            DataColumn serialNoColumn = new DataColumn("S.No.", typeof(int));
            dt.Columns.Add(serialNoColumn);

            // Add serial numbers to each row
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i]["S.No."] = i + 1;
            }

            // Create a new Excel package
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                // Add a worksheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Attendance Report");

                // Rearrange the columns to ensure "S.No." appears as the first column
                DataTable reorderedDt = new DataTable();
                reorderedDt.Columns.Add("S.No.", typeof(int));
                reorderedDt.Merge(dt);

                // Load data to the worksheet with headers
                worksheet.Cells["A1"].LoadFromDataTable(reorderedDt, true, TableStyles.None);

                // Set date format for the "date" column (assuming it's the second column)
                int dateColumnIndex = 2; // Adjust this index based on your actual column order
                using (var range = worksheet.Cells[2, dateColumnIndex, worksheet.Dimension.End.Row, dateColumnIndex])
                {
                    range.Style.Numberformat.Format = "yyyy-mm-dd"; // Adjust the date format as needed
                }

                // Set column width
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Clear response content
                Response.ClearContent();

                // Set the content type and header for the Excel file
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=Attendance Report" + DateTime.Now.ToString("dd_MM_yyyy") + ".xlsx");

                // Write the Excel package to the response stream
                Response.BinaryWrite(excelPackage.GetAsByteArray());

                // End the response
                Response.End();
            }

            return RedirectToAction("AttendanceList");
        }


        #endregion

        #region manage_AttendanceQuery
        public ActionResult AttendanceQuery()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AttendanceQuery(attendance_class cg)
        {
            if (Session["adminname"] != null)
            {
                ob.insert_query(cg);
                TempData["Message"] = " Query Send Successfully";
                TempData["para"] = "true";
                return RedirectToAction("AttendanceQuery");
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

        public ActionResult CheckQueryStatusAndDateRange()
        {
            if (Session["adminname"] != null)
            {
                string schoolIdFromSession = Session["username"].ToString();

                List<attendance_class> cg = new List<attendance_class>();
                con.Open();
                SqlCommand cmd = new SqlCommand("manage_query", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "select_date_range");
                cmd.Parameters.AddWithValue("@school_id", schoolIdFromSession);
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
                        //   pro.query_date = sdr["date"].ToString();
                        pro.status = (bool)sdr["status"];
                        pro.lastUpdate = ((DateTime)sdr["lastUpdate"]).ToString("yyyy-MM-ddTHH:mm:ss");
                        pro.fromDate = sdr["fromDate"].ToString();
                        pro.toDate = sdr["toDate"].ToString();
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

        #endregion
    }

}