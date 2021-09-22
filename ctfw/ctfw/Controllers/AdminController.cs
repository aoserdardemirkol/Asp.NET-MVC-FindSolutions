using ctfw.Models.EntityFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ctfw.Views.Admin
{
    public class AdminController : Controller
    {
        ctfwEntities db = new ctfwEntities();
        ViewModel model = new ViewModel();
        [Authorize(Roles = "A")]
        // GET: Admin
        public ActionResult Index()
        {
            model.CompanyList = db.Companys.ToList();
            return View(model);
        }
        [Authorize(Roles = "A")]
        public ActionResult Kullanıcılar()
        {
            model.UsersList = db.Users.ToList();
            return View(model);
        }
        [Authorize(Roles = "A")]
        public ActionResult Complaints()
        {
            model.ComplaintsList = db.Complaints.ToList();
            return View(model);
        }
        [Authorize(Roles = "A")]
        public ActionResult Comments()
        {
            model.CommentsList = db.Comments.ToList();
            return View(model);
        }

        public ActionResult AddCompany(ViewModel viewModel)
        {
            var isBrandAlreadyExist = db.Companys.Any(x => x.company == viewModel.users.username);

            var isUsernameAlreadyExist = db.Users.Any(x => x.username == viewModel.users.username);
            var isEmailAlreadyExists = db.Users.Any(x => x.email == viewModel.users.email);

            if (isBrandAlreadyExist || isUsernameAlreadyExist || isEmailAlreadyExists)
                TempData["msg"] = "false";

            else if (isUsernameAlreadyExist)
                TempData["msg"] = "false";
            else
            {
                Companys companys = new Companys();
                companys.company = viewModel.users.username;

                if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0)
                {
                    string dosyaadi = Path.GetFileName(Request.Files[0].FileName);
                    string uzanti = Path.GetExtension(Request.Files[0].FileName);

                    string[] subs = dosyaadi.Split('.');

                    String splitsecond = DateTime.Now.ToString("ss");
                    String minute = DateTime.Now.ToString("mm");
                    String hour = DateTime.Now.ToString("hh");
                    String month = DateTime.Now.ToString("MM");
                    String year = DateTime.Now.ToString("yyyy");

                    String time = year + "-" + month + "-" + hour + "-" + minute + "-" + splitsecond;

                    string yol = "~/images/markalogo/" + time + "-" + viewModel.users.username.Trim() + "-" + uzanti;
                    Request.Files[0].SaveAs(Server.MapPath(yol));

                    companys.company_img = "/images/markalogo/" + time + "-" + viewModel.users.username.Trim() + "-" + uzanti;
                }

                db.Companys.Add(companys);

                viewModel.users.role = "M";
                db.Users.Add(viewModel.users);

                db.SaveChanges();

                TempData["msg"] = "true";
            }
            return RedirectToAction("Index");
        }

        public ActionResult DeleteCompany(int id)
        {
            for (int i = 0; i < db.Complaints.Count(); i++)
            {
                var compl = db.Complaints.FirstOrDefault(x => x.company_id == id);
                if (compl != null)
                {
                    for (int j = 0; j < db.Comments.Count(); j++)
                    {
                        var comm = db.Comments.FirstOrDefault(x => x.complaint_id == compl.id);
                        if (comm != null)
                        {
                            db.Comments.Remove(comm);
                            db.SaveChanges();
                        }
                    }
                    db.Complaints.Remove(compl);
                    db.SaveChanges();
                }
            }

            var com = db.Companys.Find(id);

            var usr = db.Users.FirstOrDefault(x => x.username == com.company);
            db.Users.Remove(usr);

            db.Companys.Remove(com);

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult CompanyDetail(int id)
        {
            var comp = db.Companys.Find(id);
            //model.company = db.Companys.Where(x => x.id == id).ToList();
            return View("CompanyDetail", comp);
        }

        public ActionResult UpdateCompany(Companys companys)
        {
            var comp = db.Companys.Find(companys.id);
            comp.company = companys.company;
            comp.company_text = companys.company_text;

            if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0)
            {
                string dosyaadi = Path.GetFileName(Request.Files[0].FileName);
                string uzanti = Path.GetExtension(Request.Files[0].FileName);

                string[] subs = dosyaadi.Split('.');

                String splitsecond = DateTime.Now.ToString("ss");
                String minute = DateTime.Now.ToString("mm");
                String hour = DateTime.Now.ToString("hh");
                String month = DateTime.Now.ToString("MM");
                String year = DateTime.Now.ToString("yyyy");

                String time = year + "-" + month + "-" + hour + "-" + minute + "-" + splitsecond;

                string yol = "~/images/markalogo/" + time + "-" + companys.company.Trim() + "-" + uzanti;
                Request.Files[0].SaveAs(Server.MapPath(yol));

                comp.company_img = "/images/markalogo/" + time + "-" + companys.company.Trim() + "-" + uzanti;
            }

            db.SaveChanges();

            return RedirectToAction("Index");
        }


        public ActionResult Adduser(Users users)
        {
            var isUsernameAlreadyExist = db.Users.Any(x => x.username == users.username);
            var isEmailAlreadyExists = db.Users.Any(x => x.email == users.email);
            if (isEmailAlreadyExists)
                TempData["msg"] = "false";

            else if (isUsernameAlreadyExist)
                TempData["msg"] = "false";
            else
            {
                users.role = "U";
                db.Users.Add(users);
                db.SaveChanges();
                TempData["msg"] = "true";
            }
            return RedirectToAction("Kullanıcılar");
        }

        public ActionResult DeleteUser(int id)
        {
            for (int i = 0; i < db.Complaints.Count(); i++)
            {
                var compl = db.Complaints.FirstOrDefault(x => x.company_id == id);
                if (compl != null)
                {
                    for (int j = 0; j < db.Comments.Count(); j++)
                    {
                        var comm = db.Comments.FirstOrDefault(x => x.complaint_id == compl.id);
                        if (comm != null)
                        {
                            db.Comments.Remove(comm);
                            db.SaveChanges();
                        }
                    }
                    db.Complaints.Remove(compl);
                    db.SaveChanges();
                }
            }

            var usr = db.Users.FirstOrDefault(x => x.id == id);
            db.Users.Remove(usr);

            db.SaveChanges();
            return RedirectToAction("Kullanıcılar");
        }

        public ActionResult DeleteComplaints(int id)
        {
            var compl = db.Complaints.Find(id);

            for (int j = 0; j < db.Comments.Count(); j++)
            {
                var comm = db.Comments.FirstOrDefault(x => x.complaint_id == compl.id);
                if (comm != null)
                {
                    db.Comments.Remove(comm);
                    db.SaveChanges();
                }
            }

            db.Complaints.Remove(compl);
            db.SaveChanges();

            return RedirectToAction("Complaints");


            //var comp = db.Complaints.Find(id);

            //db.Complaints.Remove(comp);

            //db.SaveChanges();

            //return RedirectToAction("Complaints");
        }

        public ActionResult DeleteComments(int id)
        {
            var com = db.Comments.Find(id);

            db.Comments.Remove(com);

            db.SaveChanges();

            return RedirectToAction("Comments");
        }
    }
}