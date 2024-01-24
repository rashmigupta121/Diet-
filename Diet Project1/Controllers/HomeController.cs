using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Diet_Project1.Controllers
{
    public class HomeController : Controller
    {
        public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ConnectionString);
        Diet_Project1.Models.DataService db = new Diet_Project1.Models.DataService();
        Diet_Project1.Models.studentDataService st = new Diet_Project1.Models.studentDataService();
        public ActionResult login()
        {
            return View();
        }

        #region checklogin

        [HttpPost]
        public JsonResult admin_Login(string username, string password, string url)
        {
            Models.common_response Response = db.login(username, password);

            if (Response.parameter.ToString() != "Admin")
            {
                url = null;
            }

            if (Response.success == true)
            {
                if (url != null && url.ToString() != "")
                {
                    Response.message = (HttpUtility.HtmlDecode(url));
                }
                else
                {
                    if (Response.parameter.ToString() == "Admin")
                    {
                        Response.message = "/admin/dashboard";
                    }
                    else if (Response.parameter.ToString() == "diet")
                    {
                        Response.message = "/diet/dashboard";
                    }
                    else if (Response.parameter.ToString() == "college")
                    {
                        Response.message = "/college/dashboard";
                    }
                    else if (Response.parameter.ToString() == "student")
                    {
                        Response.message = "/student/dashboard";
                    }
                    else if (Response.parameter.ToString() == "school")
                    {
                        Response.message = "/school/dashboard";
                    }

                }
                Session["adminname"] = Response.parameter.ToString();
                Session["username"] = Response.user_id.ToString();
                Session["name"] = Response.name.ToString();
                // Session["emp_reportingManger"] = Response.report_manager.ToString();
                //st.IsApplicationOpen();
            }

            if (Response.parameter.ToString() == "student") 
            {
                st.IsApplicationOpen();
            }
           return Json(Response);
        }

        public ActionResult logout()
        {
            if (Session["username"] != null)
            {
                Session["adminname"] = null;
                Session["username"] = null;
                Session["name"] = null;
            }

            // Add cache-control headers to prevent caching
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            Response.Cache.SetValidUntilExpires(true);
            Response.AppendHeader("Pragma", "no-cache");

            return RedirectToAction("logoutRedirect");
        }

        public ActionResult logoutRedirect()
        {
            return RedirectToAction("login");
        }


        #endregion

    }
}
