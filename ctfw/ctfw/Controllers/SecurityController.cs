using ctfw.Models.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ctfw.Controllers
{
    public class SecurityController : Controller
    {
        ctfwEntities db = new ctfwEntities();
        [AllowAnonymous]
        public ActionResult Login()
        {
            if (String.IsNullOrEmpty(HttpContext.User.Identity.Name))
            {
                FormsAuthentication.SignOut();
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(Users users)
        {
            var userindb = db.Users.FirstOrDefault(x => x.username == users.username && x.password == users.password);
            
            if (userindb != null)
            {
                HttpCookie cookie = new HttpCookie("login_user_id", userindb.id.ToString());
                Response.Cookies.Add(cookie);

                FormsAuthentication.SetAuthCookie(userindb.username, false);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["msg"] = "false";
                return View();
            }
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(Users users)
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
            return RedirectToAction("Login", "Security");
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(Users users)
        {
            var model = db.Users.Where(x => x.email == users.email).FirstOrDefault();
            if (model != null)
            {
                Guid random = Guid.NewGuid();
                model.password = random.ToString().Substring(0, 8);
                db.SaveChanges();
                SmtpClient client = new SmtpClient("smtp.yandex.ru", 587);
                client.EnableSsl = true;
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("ctfw@yandex.com", "Parola Sıfırlama");
                mail.To.Add(model.email);
                mail.IsBodyHtml = true;
                mail.Subject = "Parola Sıfırlama Talebi";
                mail.Body += "Merhaba " + model.username + "<br/> Kullanıcı Adınız: " + model.username + "<br/> Yeni Parolanız: " + model.password;
                NetworkCredential net = new NetworkCredential("ctfw@yandex.com", "ctfw.mail");
                client.Credentials = net;
                client.Send(mail);

                TempData["msg"] = "true";
            }
            else
                TempData["msg"] = "false";
            
            return RedirectToAction("Login", "Security");
        }

        public ActionResult Logout()
        {
            // Cookie Silme
            Request.Cookies.Remove("login_user_id");

            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}