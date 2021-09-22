using ctfw.Models.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ctfw.Controllers
{
    public class NotificationController : Controller
    {
        // GET: Notification
        ctfwEntities db = new ctfwEntities();
        ViewModel model = new ViewModel();

        [Authorize]
        public PartialViewResult Notification()
        {
            var useridholder = 0;
            if (Request.Cookies["login_user_id"] != null)
            {
                ViewData["userid"] = Request.Cookies["login_user_id"].Value;
                useridholder = Convert.ToInt32(ViewData["userid"].ToString());

                model.ComplaintsList = db.Complaints.Where(x => x.user_id == useridholder).ToList();
                model.CommentsList = db.Comments.ToList();
            }
            return PartialView(model);
        }

        [Authorize]
        [HttpGet]
        public ActionResult AllNotification()
        {
            var useridholder = 0;
            if (Request.Cookies["login_user_id"] != null)
            {
                ViewData["userid"] = Request.Cookies["login_user_id"].Value;
                useridholder = Convert.ToInt32(ViewData["userid"].ToString());

                model.ComplaintsList = db.Complaints.Where(x => x.user_id == useridholder).ToList();
                model.CommentsList = db.Comments.ToList();
                model.CommentsList.Reverse();
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult AllNotification(Comments comments)
        {
            var com = db.Comments.Find(comments.id);
            com.notification = true;
            db.SaveChanges();
            return RedirectToAction("SikayetDetay/" + com.complaint_id.ToString(), "Home");
        }



        [Authorize]
        public PartialViewResult CompanyNotification()
        {
            var useridholder = 0;
            if (Request.Cookies["login_user_id"] != null)
            {
                ViewData["userid"] = Request.Cookies["login_user_id"].Value;
                useridholder = Convert.ToInt32(ViewData["userid"].ToString());

                var user = db.Users.FirstOrDefault(x => x.id == useridholder);
                var comp = db.Companys.FirstOrDefault(x => x.company == user.username);

                model.ComplaintsList = db.Complaints.Where(x => x.company_id == comp.id).ToList();
            }
            return PartialView(model);
        }

        [Authorize]
        [HttpGet]
        public ActionResult CompanyAllNotification()
        {
            var useridholder = 0;
            if (Request.Cookies["login_user_id"] != null)
            {
                ViewData["userid"] = Request.Cookies["login_user_id"].Value;
                useridholder = Convert.ToInt32(ViewData["userid"].ToString());

                var user = db.Users.FirstOrDefault(x => x.id == useridholder);
                var comp = db.Companys.FirstOrDefault(x => x.company == user.username);

                model.ComplaintsList = db.Complaints.Where(x => x.company_id == comp.id).ToList();
                model.ComplaintsList.Reverse();
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult CompanyAllNotification(Complaints complaints)
        {
            var com = db.Complaints.Find(complaints.id);
            com.notification = true;
            db.SaveChanges();
            return RedirectToAction("SikayetDetay/" + com.id.ToString(), "Home");
        }
    }
}