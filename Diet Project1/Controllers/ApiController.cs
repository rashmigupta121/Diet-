using Diet_Project1.Models;
using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;


namespace Diet_Project1.Controllers
{
    #region manage student_panel_Api
    public class LoginApiController : ApiController
    {
        public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ConnectionString);
        Diet_Project1.Models.DataService db = new Diet_Project1.Models.DataService();
        
        [HttpPost]
        public IHttpActionResult Check_login()
        {
            var httpRequest = HttpContext.Current.Request;
            var emp = new Diet_Project1.Models.common_response
            {
                user_id = httpRequest.Form.Get("username"),
                password = httpRequest.Form.Get("password"),
            };
            Models.common_response Response = db.login(emp.user_id, emp.password);

            if (Response.success == true)
            {
                return Ok(new
                {
                    Status = "Success",
                    Message = "Login successful!",
                    Username = Response.username,
                    Password = Response.password,
                    Name = Response.name,
                    RollId = Response.roll_id
                });
            }
            else
            {
                return Ok(new
                {
                    Status = "Error",
                    Message = "Login failed. " + Response.message,
                });
            }

        }
    }


    public class StudentProfileApiController : ApiController 
    {
        public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ConnectionString);
        Diet_Project1.Models.studentDataService st = new Diet_Project1.Models.studentDataService();

        [HttpGet]
        public IHttpActionResult StudentProfile(string username)
        {
            if (username == null || username == "")
            {
                var errorResponse = new ErrorResponse
                {
                    Status = "Error",
                    Message = "Please provide a valid username as an argument."
                };

                return Content(HttpStatusCode.BadRequest, errorResponse);
            }
            student pro = new student();

            con.Open();
            SqlCommand cmd = new SqlCommand("manage_student", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "view_student_profile");
            cmd.Parameters.AddWithValue("@user_name", username);

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

                con.Close();
                return Ok(new
                {
                    Status = "Success",
                    StudentProfile = pro
                });
            }
            else
            {
                var notFoundResponse = new ErrorResponse
                {
                    Status = "Error",
                    Message = "No student profile found for the provided username."
                };

                return Content(HttpStatusCode.NotFound, notFoundResponse);
            }
        }
    }

    public class GetInternshipDetailsApiController : ApiController
    {
        public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ConnectionString);

        [HttpGet]
        public IHttpActionResult InternshipDetails(string username)
        {
            if (username == null || username == "")
            {
                var errorResponse = new ErrorResponse
                {
                    Status = "Error",
                    Message = "Please provide a valid username as an argument."
                };

                return Content(HttpStatusCode.BadRequest, errorResponse);
            }

            List<internshipApi> internships = new List<internshipApi>();
            con.Open();
            SqlCommand cmd = new SqlCommand("manage_student", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "View_internship");
            cmd.Parameters.AddWithValue("@user_name", username);

            SqlDataReader sdr = cmd.ExecuteReader();

                while (sdr.Read())
                {
                    internshipApi pro = new internshipApi
                    {
                        internship_id = Convert.ToInt32(sdr["internship_id"]),
                        batch = sdr["batch"].ToString(),
                        school_category = sdr["school_category"].ToString(),
                        semester = sdr["semester"].ToString(),
                        panel_open = sdr["panel_open"].ToString().Substring(0, Math.Min(10, sdr["panel_open"].ToString().Length)),
                        apply_date = sdr["apply_date"].ToString().Substring(0, Math.Min(10, sdr["apply_date"].ToString().Length)),
                        start_date = sdr["start_date"].ToString(),
                        end_date = sdr["end_date"].ToString(),
                        //school_capacity = sdr["school_capacity"].ToString()
                    };

                    internships.Add(pro);
                }
            con.Close();


            if (internships.Count > 0)
            {
                return Ok(new
                {
                    Status = "Success",
                    InternshipDetails = internships
                });
            }
            else
            {
                var notFoundResponse = new ErrorResponse
                {
                    Status = "Error",
                    Message = "No intership details found for the provided username."
                };

                return Content(HttpStatusCode.NotFound, notFoundResponse);
            }
        }
    }


    public class GetDistrictApiController : ApiController 
    {
        Diet_Project1.Models.DataService db = new Diet_Project1.Models.DataService();

        [HttpGet]
        public IHttpActionResult District()
        {
            List<BindDistrict> districts = db.BindDistrict();
            if (districts.Count == 0)
            {
                var notFoundResponse = new ErrorResponse
                {
                    Status = "Error",
                    Message = "No districts found."
                };

                return Content(HttpStatusCode.NotFound, notFoundResponse);
            }

            return Ok(new
            {
                Status = "Success",
                district = districts
            });
        }
    }

    public class GetBlockApiController : ApiController
    {
        Diet_Project1.Models.DataService db = new Diet_Project1.Models.DataService();

        [HttpGet]
        public IHttpActionResult Block(string district)
        {
            if (district == null || district == "")
            {
                var errorResponse = new ErrorResponse
                {
                    Status = "Error",
                    Message = "Please provide a valid district as an argument."
                };

                return Content(HttpStatusCode.BadRequest, errorResponse);
            }
            List<BindBlock> blocks = db.BindBlock(district);

            if (blocks.Count == 0)
            {
                var notFoundResponse = new ErrorResponse
                {
                    Status = "Error",
                    Message = "No blocks found for the provided district."
                };

                return Content(HttpStatusCode.NotFound, notFoundResponse);
            }

            return Ok(new
            {
                Status = "Success",
                block = blocks
            });
        }
    }

    public class GetSchoolApiController : ApiController
    {
        public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ConnectionString);

        [HttpGet]
        public IHttpActionResult Block(string block, string username)
        {
            if (username == null || username == "")
            {
                if (block == null || block == "")
                {
                    var errorResponse = new ErrorResponse
                    {
                        Status = "Error",
                        Message = "Please provide a valid username and block as an argument."
                    };

                    return Content(HttpStatusCode.BadRequest, errorResponse);
                }
            }

            List<SchoolInfo> cg = new List<SchoolInfo>();

            con.Open();
            SqlCommand cmd = new SqlCommand("manage_internship", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "view_school_for_intership");
            cmd.Parameters.AddWithValue("@block", block);
            cmd.Parameters.AddWithValue("@user_name", username);
            SqlDataReader sdr = cmd.ExecuteReader();
            SchoolInfo pro;

            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    pro = new SchoolInfo();
                    pro.school_id = Convert.ToInt32(sdr["school_id"]);
                    pro.block_name = sdr["block"].ToString();
                    pro.school_name = sdr["school_name"].ToString();
                    pro.school_code = sdr["school_code"].ToString();
                    
                    cg.Add(pro);
                }
            }
            con.Close();


            if (cg.Count > 0)
            {
                return Ok(new
                {
                    Status = "Success",
                    Schoolname = cg
                });
            }
            else
            {
                var notFoundResponse = new ErrorResponse
                {
                    Status = "Error",
                    Message = "No school found for the provided block."
                };

                return Content(HttpStatusCode.NotFound, notFoundResponse);
            }
        }
    }

    public class GetSchoolDetailsApiController : ApiController
    {
        public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ConnectionString);

        [HttpGet]
        public IHttpActionResult SchoolDetails(string school_id)
        {
            if (school_id == null || school_id == "")
            {
                    var errorResponse = new ErrorResponse
                    {
                        Status = "Error",
                        Message = "Please provide a valid school_id as an argument."
                    };

                    return Content(HttpStatusCode.BadRequest, errorResponse);
            }

            List<school> cg = new List<school>();

            con.Open();
            SqlCommand cmd = new SqlCommand("manage_school", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "edit_school");
            cmd.Parameters.AddWithValue("@school_id", school_id);
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


            if (cg.Count > 0)
            {
                return Ok(new
                {
                    Status = "Success",
                    Schooldetails = cg
                });
            }
            else
            {
                var notFoundResponse = new ErrorResponse
                {
                    Status = "Error",
                    Message = "No school details found for the provided School Id."
                };

                return Content(HttpStatusCode.NotFound, notFoundResponse);
            };
        }
    }


    public class ApplyInternshipApiController : ApiController
    {
        public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ConnectionString);
        Diet_Project1.Models.studentDataService st = new Diet_Project1.Models.studentDataService();

        [HttpPost]
        public IHttpActionResult ApplyInternship(string username, string studentname)
        {
            if (username == null || username == "")
            {
                if (studentname == null || studentname == "")
                {
                    var errorResponse = new ErrorResponse
                    {
                        Status = "Error",
                        Message = "Please provide a valid username and studentname as an argument."
                    };

                    return Content(HttpStatusCode.BadRequest, errorResponse);
                }
            }

            internship cg = new internship();

            SqlCommand cmd = new SqlCommand("manage_internship", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@user_name", username);

            if (cg.internship_id == 0)
            {
                cmd.Parameters.AddWithValue("@action", "duplicate_record");
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    return Ok(new
                    {
                        Status = "Error",
                        Message = "You have already applied for this internship. Thank you for your application!"
                    });
                }
                else
                {
                    var success = st.internship_applybyApi(username, studentname);


                    if (success)
                    {
                        return Ok(new
                        {
                            Status = "Success",
                            Message = "Internship Applied Successfully"
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            Status = "Error",
                            Message = "Failed to apply for internship. Please try again later."
                        });
                    }
                }
            }
            return Ok();
        }
    }

    public class AttendanceApiController : ApiController
    {
        public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ConnectionString);

        [HttpGet]
        public IHttpActionResult GetAttendanceData(string semester, string username)
        {
            if (username == null || username == "")
            {
                if (semester == null || semester == "")
                {
                    var errorResponse = new ErrorResponse
                    {
                        Status = "Error",
                        Message = "Please provide a valid username and semester as an argument."
                    };

                    return Content(HttpStatusCode.BadRequest, errorResponse);
                }
            }

            List<attendance_class> cg = new List<attendance_class>();
            con.Open();
            SqlCommand cmd = new SqlCommand("manage_attendance", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "ViewAttendanceby_student");
            cmd.Parameters.AddWithValue("@student_id", username);
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

            if (cg.Count > 0)
            {
                return Ok(new
                {
                    Status = "Success",
                    Attendance = cg
                });
            }
            else
            {
                var notFoundResponse = new ErrorResponse
                {
                    Status = "Error",
                    Message = "No attendance details found for the provided Username and semester."
                };

                return Content(HttpStatusCode.NotFound, notFoundResponse);
            };
        }
    }

    #endregion

    #region manage diet_panel_Api

    public class CollegeListApiController : ApiController
    {
        public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ConnectionString);

        [HttpGet]
        public IHttpActionResult ViewCollegeList(string username)
        {
            if (username == null || username == "")
            {
                var errorResponse = new ErrorResponse
                {
                    Status = "Error",
                    Message = "Please provide a valid username as an argument."
                };

                return Content(HttpStatusCode.BadRequest, errorResponse);
            }
            List<college> cg = new List<college>();

            con.Open();
            SqlCommand cmd = new SqlCommand("manage_college", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "view_college");
            cmd.Parameters.AddWithValue("@diet_id", username);
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

            if (cg.Count > 0)
            {
                return Ok(new
                {
                    Status = "Success",
                    Collegelists = cg
                });
            }
            else
            {
                var notFoundResponse = new ErrorResponse
                {
                    Status = "Error",
                    Message = "No College list found for the provided username."
                };

                return Content(HttpStatusCode.NotFound, notFoundResponse);
            }
        }
    }

    public class GetCollegebyIdApiController : ApiController
    {
        public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ConnectionString);
        Diet_Project1.Models.DataService db = new Diet_Project1.Models.DataService();

        [HttpGet]
        public IHttpActionResult GetCollege(string college_id)
        {
            if (college_id == null || college_id == "")
            {
                var errorResponse = new ErrorResponse
                {
                    Status = "Error",
                    Message = "Please provide a valid college_id as an argument."
                };

                return Content(HttpStatusCode.BadRequest, errorResponse);
            }
            college cg = new college();
            cg = db.getcollegeByiId(Convert.ToInt32(college_id));

            if (cg == null || cg.college_id == 0)
            {
                var notFoundResponse = new ErrorResponse
                {
                    Status = "Error",
                    Message = "No college found."
                };

                return Content(HttpStatusCode.NotFound, notFoundResponse);
            }

            return Ok(new
            {
                Status = "Success",
                collegedetails = cg
            });
        }
    }

    public class SchoolListApiController : ApiController
    {
        public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ConnectionString);

        [HttpGet]
        public IHttpActionResult ViewSchoolList(string username)
        {
            if (username == null || username == "")
            {
                var errorResponse = new ErrorResponse
                {
                    Status = "Error",
                    Message = "Please provide a valid username as an argument."
                };

                return Content(HttpStatusCode.BadRequest, errorResponse);
            }

            List<school> cg = new List<school>();

            con.Open();
            SqlCommand cmd = new SqlCommand("manage_school", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "view_school");
            cmd.Parameters.AddWithValue("@diet_id", username);
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

            if (cg.Count > 0)
            {
                return Ok(new
                {
                    Status = "Success",
                    Schoollists = cg
                });
            }
            else
            {
                var notFoundResponse = new ErrorResponse
                {
                    Status = "Error",
                    Message = "No School list found for the provided username."
                };

                return Content(HttpStatusCode.NotFound, notFoundResponse);
            }
        }
    }

    public class GetSchoolbyIdApiController : ApiController
    {
        public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ConnectionString);
        Diet_Project1.Models.DataService db = new Diet_Project1.Models.DataService();

        [HttpGet]
        public IHttpActionResult GetSchool(string school_id)
        {
            if (school_id == null || school_id == "")
            {
                var errorResponse = new ErrorResponse
                {
                    Status = "Error",
                    Message = "Please provide a valid school_id as an argument."
                };

                return Content(HttpStatusCode.BadRequest, errorResponse);
            }
            school cg = new school();
            cg = db.getschoolByiId(Convert.ToInt32(school_id));

            if (cg == null || cg.school_id == 0)
            {
                var notFoundResponse = new ErrorResponse
                {
                    Status = "Error",
                    Message = "No school found."
                };

                return Content(HttpStatusCode.NotFound, notFoundResponse);
            }

            return Ok(new
            {
                Status = "Success",
                schooldetails = cg
            });
        }
    }

    public class StudentListApiController : ApiController
    {
        public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ConnectionString);

        [HttpGet]
        public IHttpActionResult ViewStudentList(string username)
        {
            if (username == null || username == "")
            {
                var errorResponse = new ErrorResponse
                {
                    Status = "Error",
                    Message = "Please provide a valid username as an argument."
                };

                return Content(HttpStatusCode.BadRequest, errorResponse);
            }

            List<student> cg = new List<student>();

            con.Open();
            SqlCommand cmd = new SqlCommand("manage_student", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "view_student");
            cmd.Parameters.AddWithValue("@diet_id", username);
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

            if (cg.Count > 0)
            {
                return Ok(new
                {
                    Status = "Success",
                    Studentlists = cg
                });
            }
            else
            {
                var notFoundResponse = new ErrorResponse
                {
                    Status = "Error",
                    Message = "No Student list found for the provided username."
                };

                return Content(HttpStatusCode.NotFound, notFoundResponse);
            }
        }
    }

    public class GetStudentbyIdApiController : ApiController
    {
        public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ConnectionString);
        Diet_Project1.Models.DataService db = new Diet_Project1.Models.DataService();

        [HttpGet]
        public IHttpActionResult GetStudent(string student_id)
        {
            if (student_id == null || student_id == "")
            {
                var errorResponse = new ErrorResponse
                {
                    Status = "Error",
                    Message = "Please provide a valid student_id as an argument."
                };

                return Content(HttpStatusCode.BadRequest, errorResponse);
            }
            student cg = new student();
            cg = db.getstudentByiId(Convert.ToInt32(student_id));

            if (cg == null || cg.student_id == null || cg.student_id == "0")
            {
                var notFoundResponse = new ErrorResponse
                {
                    Status = "Error",
                    Message = "No student found."
                };

                return Content(HttpStatusCode.NotFound, notFoundResponse);
            }

            return Ok(new
            {
                Status = "Success",
                studentdetails = cg
            });
        }
    }

    public class CollegeNameApiController : ApiController
    {
        public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ConnectionString);

        [HttpGet]
        public IHttpActionResult ViewCollegeName(string username)
        {
            if (username == null || username == "")
            {
                var errorResponse = new ErrorResponse
                {
                    Status = "Error",
                    Message = "Please provide a valid username as an argument."
                };

                return Content(HttpStatusCode.BadRequest, errorResponse);
            }

            List<collegeinfo> cg = new List<collegeinfo>();

            con.Open();
            SqlCommand cmd = new SqlCommand("manage_college", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "view_college");
            cmd.Parameters.AddWithValue("@diet_id", username);
            SqlDataReader sdr = cmd.ExecuteReader();
            collegeinfo pro;

            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    pro = new collegeinfo();
                    pro.college_id = sdr["user_name"].ToString();
                    pro.college_name = sdr["college_name"].ToString();
                    cg.Add(pro);
                }
            }
            con.Close();

            if (cg.Count > 0)
            {
                return Ok(new
                {
                    Status = "Success",
                    Collegelists = cg
                });
            }
            else
            {
                var notFoundResponse = new ErrorResponse
                {
                    Status = "Error",
                    Message = "No Colleges found for the provided username."
                };

                return Content(HttpStatusCode.NotFound, notFoundResponse);
            }
        }
    }

    public class GetStudentsByCollegeApiController : ApiController
    {
        public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ConnectionString);
        Diet_Project1.Models.DataService db = new Diet_Project1.Models.DataService();

        [HttpGet]
        public IHttpActionResult getstudentbycollegeid(string college_id)
        {
            if (college_id == null || college_id == "")
            {
                var errorResponse = new ErrorResponse
                {
                    Status = "Error",
                    Message = "Please provide a valid student_id as an argument."
                };

                return Content(HttpStatusCode.BadRequest, errorResponse);
            }
            List<student> cg = new List<student>();
            student pro;

            con.Open();
            SqlCommand cmd = new SqlCommand("manage_college_student", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@college_id", college_id);

            //if (college_id == "")
            //{
            //    cmd.Parameters.AddWithValue("@diet_id", dietIdFromSession);
            //    cmd.Parameters.AddWithValue("@Action", "view_all_student_by_diet");
            //}
            //else
            //{
                cmd.Parameters.AddWithValue("@Action", "view_student");
            //}

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

            if (cg.Count > 0)
            {
                return Ok(new
                {
                    Status = "Success",
                   Studentlists = cg
                });
            }
            else
            {
                var notFoundResponse = new ErrorResponse
                {
                    Status = "Error",
                    Message = "No Student list found for the provided college."
                };

                return Content(HttpStatusCode.NotFound, notFoundResponse);
            }
        }
    }

    public class GetCollegeStudentbyIdApiController : ApiController
    {
        public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ConnectionString);
        Diet_Project1.Models.DataService db = new Diet_Project1.Models.DataService();
        Diet_Project1.Models.collegeDataService obj = new Diet_Project1.Models.collegeDataService();

        [HttpGet]
        public IHttpActionResult GetCollegeStudent(string student_id)
        {
            if (student_id == null || student_id == "")
            {
                var errorResponse = new ErrorResponse
                {
                    Status = "Error",
                    Message = "Please provide a valid student_id as an argument."
                };

                return Content(HttpStatusCode.BadRequest, errorResponse);
            }
            student cg = new student();
            cg = obj.getstudentByiId(Convert.ToInt32(student_id));

            if (cg == null || cg.student_id == null || cg.student_id == "0")
            {
                var notFoundResponse = new ErrorResponse
                {
                    Status = "Error",
                    Message = "No student found."
                };

                return Content(HttpStatusCode.NotFound, notFoundResponse);
            }

            return Ok(new
            {
                Status = "Success",
                studentdetails = cg
            });
        }
    }

    public class InternshipListApiController : ApiController
    {
        public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ConnectionString);

        [HttpGet]
        public IHttpActionResult ViewInternshipList(string username)
        {
            if (username == null || username == "")
            {
                var errorResponse = new ErrorResponse
                {
                    Status = "Error",
                    Message = "Please provide a valid username as an argument."
                };

                return Content(HttpStatusCode.BadRequest, errorResponse);
            }
            List<internship> cg = new List<internship>();

            con.Open();
            SqlCommand cmd = new SqlCommand("manage_internship_details", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "view_internship");
            cmd.Parameters.AddWithValue("@diet_id", username);
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

            if (cg.Count > 0)
            {
                return Ok(new
                {
                    Status = "Success",
                    Internshiplists = cg
                });
            }
            else
            {
                var notFoundResponse = new ErrorResponse
                {
                    Status = "Error",
                    Message = "No Internship list found for the provided username."
                };

                return Content(HttpStatusCode.NotFound, notFoundResponse);
            }
        }
    }

    public class DietAppliedStudentApiController : ApiController
    {
        public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ConnectionString);

        [HttpGet]
        public IHttpActionResult AppliedStudentList(string username)
        {
            if (username == null || username == "")
            {
                var errorResponse = new ErrorResponse
                {
                    Status = "Error",
                    Message = "Please provide a valid username as an argument."
                };

                return Content(HttpStatusCode.BadRequest, errorResponse);
            }
            List<internship> cg = new List<internship>();
            con.Open();
            SqlCommand cmd = new SqlCommand("manage_internship", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "view_directly_Applied_student_ByDiet");
            cmd.Parameters.AddWithValue("@diet_id", username);
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

            if (cg.Count > 0)
            {
                return Ok(new
                {
                    Status = "Success",
                    ApplyStudentlists = cg
                });
            }
            else
            {
                var notFoundResponse = new ErrorResponse
                {
                    Status = "Error",
                    Message = "No Student list found for the provided username."
                };

                return Content(HttpStatusCode.NotFound, notFoundResponse);
            }
        }
    }

    public class CollegeAppliedStudentApiController : ApiController
    {
        public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ConnectionString);

        [HttpGet]
        public IHttpActionResult CollegeAppliedStudentList(string username)
        {
            if (username == null || username == "")
            {
                var errorResponse = new ErrorResponse
                {
                    Status = "Error",
                    Message = "Please provide a valid username as an argument."
                };

                return Content(HttpStatusCode.BadRequest, errorResponse);
            }
            List<internship> cg = new List<internship>();
            con.Open();
            SqlCommand cmd = new SqlCommand("manage_internship", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "view_college_student_Internship_ByDiet");
            cmd.Parameters.AddWithValue("@diet_id", username);
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


            if (cg.Count > 0)
            {
                return Ok(new
                {
                    Status = "Success",
                    ApplyStudentlists = cg
                });
            }
            else
            {
                var notFoundResponse = new ErrorResponse
                {
                    Status = "Error",
                    Message = "No Student list found for the provided username."
                };

                return Content(HttpStatusCode.NotFound, notFoundResponse);
            }
        }
    }

    #endregion
}

