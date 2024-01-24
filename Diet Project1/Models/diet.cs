using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Diet_Project1.Models
{
    public class diet
    {
        public int diet_id { get; set; }
        public string diet_name { get; set; }

        public string user_name { get; set; }
        public string password { get; set; }
        public string director_name { get; set; }
        public string email { get; set; }
        public string address { get; set; }
       
        public string mobile { get; set; }

        public string district { get; set; }
        public string district_id { get; set; }

        public string district_name { get; set; }
        public string block { get; set; }
        public string block_id { get; set; }
        public string block_name { get; set; }
        public string website_url { get; set; }
        public List<BindDistrict> BindDistrict { get; set; }

        public List<BindBlock> BindBlock { get; set; }
    }
}