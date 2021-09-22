using ctfw.Models.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using System.Web.Security;
using System.Net.Mail;
using System.Net;
using System.IO;
using System.Data.Entity.Validation;

namespace ctfw.Controllers
{
    public class HomeController : Controller
    {
        ctfwEntities db = new ctfwEntities();
        ViewModel model = new ViewModel();

        [AllowAnonymous]
        public ActionResult Index()
        {
            if (Request.Cookies["login_user_id"] != null)
            {
                ViewData["userid"] = Request.Cookies["login_user_id"].Value;
            }

            model.ComplaintsList = db.Complaints.ToList();
            model.ComplaintsList.Reverse();
            model.UsersList = db.Users.ToList();
            model.CompanyList = db.Companys.ToList();

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Sikayetler(string ara, int page=1)
        {
            if (Request.Cookies["login_user_id"] != null)
                ViewData["userid"] = Request.Cookies["login_user_id"].Value;

            var pages = db.Complaints.Where(x => x.complaint_header.Contains(ara) || x.complaint_text.Contains(ara) || ara == null).ToList();
            pages.Reverse();
            var pagess = pages.ToPagedList(page, 15);

            return View(pagess);
        }

        [AllowAnonymous]
        public ActionResult Markalar(string ara, int page = 1)
        {
            if (Request.Cookies["login_user_id"] != null)
                ViewData["userid"] = Request.Cookies["login_user_id"].Value;

            var pages = db.Companys.Where(x => x.company.Contains(ara) || ara == null).ToList();
            pages.Reverse();
            var pagess = pages.ToPagedList(page, 15);

            return View(pagess);
        }

        [AllowAnonymous]
        [HttpGet]
        public new ActionResult Profile(int id)
        {
            if (Request.Cookies["login_user_id"] != null)
                ViewData["userid"] = Request.Cookies["login_user_id"].Value;

            model.user = db.Users.Where(x => x.id == id).ToList();
            model.Complaints = db.Complaints.Where(x => x.user_id == id).ToList();
            model.Comments = db.Comments.Where(x => x.user_id == id).ToList();

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult MarkaDetay(int id)
        {
            if (Request.Cookies["login_user_id"] != null)
                ViewData["userid"] = Request.Cookies["login_user_id"].Value;


            model.company= db.Companys.Where(x => x.id == id).ToList();
            model.Complaints = db.Complaints.Where(x => x.company_id == id).ToList();

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult SikayetDetay(int id)
        {
            ViewData["deger"] = id;

            if (Request.Cookies["login_user_id"] != null)
                ViewData["userid"] = Request.Cookies["login_user_id"].Value;

            model.Complaints = db.Complaints.Where(x => x.id == id).ToList();
            model.Comments = db.Comments.Where(x => x.complaint_id == id).ToList();

            return View(model);
        }

        [Authorize]
        [HttpGet]
        public ActionResult Sikayetyaz()
        {
            if (Request.Cookies["login_user_id"] != null)
                ViewData["userid"] = Request.Cookies["login_user_id"].Value;

            List<SelectListItem> degerler = (from i in db.Companys.ToList()
                                             select new SelectListItem
                                             {
                                                 Text = i.company,
                                                 Value = i.id.ToString()
                                             }).ToList();
            ViewBag.dgr = degerler;

            return View();
        }

        [HttpPost]
        public ActionResult Sikayetyaz(Complaints complaints)
        {
            var usr = db.Users.FirstOrDefault(x => x.username == complaints.username);
            complaints.complaint_releasedate = DateTime.Now;
            complaints.complaint_processstate = "W";
            complaints.notification = false;
            complaints.user_id = usr.id;

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

                string yol = "~/images/" + time + "-" + complaints.username.Trim() + "-" + complaints.user_id + uzanti;
                Request.Files[0].SaveAs(Server.MapPath(yol));

                complaints.complaint_picture = "/images/" + time + "-" + complaints.username.Trim() + "-" + complaints.user_id + uzanti;
            }

            db.Complaints.Add(complaints);
            db.SaveChanges();

            return RedirectToAction("SikayetDetay/" + complaints.id, "Home");
        }

        [HttpPost]
        public ActionResult Answer(Comments comments)
        {
            var com = db.Comments.Find(comments.id);
            var comp = db.Complaints.Find(comments.complaint_id);

            if (com.comment_answer == "N")
            {
                com.comment_answer = "Y";
                comp.complaint_processstate = "Y";
            }
            else
            {
                com.comment_answer = "N";
                comp.complaint_processstate = "W";
            }
            db.SaveChanges();

            return RedirectToAction("SikayetDetay/" + comments.complaint_id.ToString(), "Home");
        }

        [HttpPost]
        public ActionResult Like(Complaints complaints)
        {
            var comp = db.Complaints.Find(complaints.id);
            var com = db.Comments.Find(complaints.user_id);

            if (comp.complaint_processstate == "W")
            {
                comp.complaint_processstate = "Y";
                com.comment_answer = "Y";
            }
            else
            {
                comp.complaint_processstate = "W";
                com.comment_answer = "N";

            }
            db.SaveChanges();
            return RedirectToAction("SikayetDetay/" + complaints.id.ToString(), "Home");
        }

        [HttpPost]
        public ActionResult Comment(int id, string content, string username)
        {
            Comments comments = new Comments();
            comments.comment_date = DateTime.Now;
            comments.comment_answer = "N";
            comments.notification = false;
            comments.comment = content;
            comments.complaint_id = id;
            comments.username = username;
            

            var userindb = db.Users.FirstOrDefault(x => x.username == username);
            comments.user_id = userindb.id;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            else
            {
                if (content == "")
                {
                    return HttpNotFound();
                }
                else
                {
                    db.Comments.Add(comments);
                    db.SaveChanges();
                    return Json(new { result = true }, JsonRequestBehavior.AllowGet);
                }
            }
        }


        [HttpPost]
        public ActionResult RemoveComment(Comments comments)
        {
            var com = db.Comments.Find(comments.id);

            db.Comments.Remove(com);
            db.SaveChanges();

            return RedirectToAction("SikayetDetay/" + comments.complaint_id.ToString(), "Home");
        }

        [HttpPost]
        public ActionResult RemoveComplaint(Complaints complaints)
        {
            var compl = db.Complaints.Find(complaints.id);

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

            return RedirectToAction("Sikayetler");
        }

        [HttpPost]
        public ActionResult UpdateProfile(Users users)
        {
            if (Request.Cookies["login_user_id"] != null)
                ViewData["userid"] = Request.Cookies["login_user_id"].Value;

            var com = db.Users.FirstOrDefault(x => x.username == users.username);

            com.password = users.password;
            com.role = "U";

            db.SaveChanges();

            return RedirectToAction("Profile/" + ViewData["userid"], "Home");
        }
    }
}