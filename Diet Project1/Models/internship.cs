using NPOI.OpenXmlFormats.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Diet_Project1.Models
{
    public class internship
    {
        public int internship_id { get; set; }
        public string student_name { get; set; }
        public string user_name { get; set; }
        public string school_category { get; set; }
        public string district { get; set; }
        public string district_id { get; set; }
        public string district_name { get; set; }
        public string block { get; set; }
        public string block_id { get; set; }
        public string block_name { get; set; }
        public string school_name { get; set; }
        public string school_code { get; set; }
        public string head_master { get; set; }
        public string address { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public string diet_id { get; set; }
        public string diet_name { get; set; }
        public string college_name { get; set; }
        public string batch { get; set; }
        public string semester { get; set; }
        public string panel_open { get; set; }
        public string apply_date { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public string status { get; set; }
        public string school_capacity { get; set; }
        public string status_by_college { get; set; }
        public int generate_letter { get; set; }
        public bool IsLinkDisabled { get; set; }

        public string last_update { get; set; }
        public List<BindDistrict> BindDistrict { get; set; }
        public List<BindBlock> BindBlock { get; set; }
    }

    public class internshipApi 
    {
        public int internship_id { get; set; }
        public string school_category { get; set; }
        public string batch { get; set; }
        public string semester { get; set; }
        public string panel_open { get; set; }
        public string apply_date { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
    }


}