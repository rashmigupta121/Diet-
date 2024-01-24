using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Diet_Project1.Models
{
    public class common_response
    {
        public string message { get; set; }
        public string parameter { get; set; }
        public Boolean success { get; set; }
        public string name { get; set; }
        public string roll_id { get; set; }
        public string user_id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }

    public class ErrorResponse
    {
        public string Message { get; set; }
        public string Status { get; set; }

    }

    public class Dashboard
    {
        public int total_diet { get; set; }
        public int total_college { get; set; }
        public int total_schools { get; set; }
        public int total_student { get; set; }

        public int total_diet_college { get; set; } 
        public int total_diet_school { get; set; }
        public int total_diet_student { get; set; }
        public int total_internship { get; set; }

        public int total_college_student { get; set; }
    }
}