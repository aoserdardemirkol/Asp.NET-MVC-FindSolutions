using ctfw.Models.EntityFramework;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ctfw
{
    public class ViewModel
    {
        public Users users { get; set; }
        public Comments comment { get; set; }
        public Complaints complaint { get; set; }
        public Companys companys { get; set; }
        public List <Users> UsersList { get; set; }
        public List <Complaints> ComplaintsList { get; set; }
        public List<Comments> CommentsList { get; set; }
        public List <Companys> CompanyList { get; set; }
        public IEnumerable<Users> user { get; set; }
        public IEnumerable <Complaints> Complaints { get; set; }
        public IEnumerable <Comments> Comments { get; set; }
        public IEnumerable<Companys> company { get; set; }

    }
}