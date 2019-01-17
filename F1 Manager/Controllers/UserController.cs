using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Net;
using System.Web.Security;
using F1_Manager.Models;
using MySql.Data.MySqlClient;

namespace F1_Manager.Controllers
{
    public class UserController : Controller
    {
        mysqlcon dc = new mysqlcon();
        
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration(User user)
        {
            bool Status = false;
            string message = "";
            
            // Model Validation 
            if (ModelState.IsValid)
            {
                #region  Password Hashing 
                user.Password = Crypto.Hash(user.Password);
                user.ConfirmPassword = Crypto.Hash(user.ConfirmPassword);
                #endregion

                #region Save to Database

                int result = dc.checkRegistration(user, 300000, 0);
                
                
                if (result == 1)
                {
                    message = "Registration successfull";
                    Status = true;
                }
                else
                {
                    message = "An error occured";
                    Status = false;
                }
                
                
                #endregion
            }
            else
            {
                message = "Invalid Request";
            }

            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View(user);
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLogin login, string ReturnUrl = "")
        {
            string message = "";

            if (ModelState.IsValid)
            {
                int Result = 0;

                UserLogin LoggedInUser = dc.CheckLogin(login.Username, Crypto.Hash(login.Password), ref Result);
                if (string.IsNullOrEmpty(LoggedInUser.Username))
                {
                    Result = 0;
                }
                

                if (Result == 1)
                {
                    int timeout = login.RememberMe ? 525600 : 20; // 525600 min = 1 year
                    var ticket = new FormsAuthenticationTicket(LoggedInUser.Username, login.RememberMe, timeout);
                    string encrypted = FormsAuthentication.Encrypt(ticket);
                    var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                    cookie.Expires = DateTime.Now.AddMinutes(timeout);
                    cookie.HttpOnly = true;
                    Response.Cookies.Add(cookie);
                    login = null;
                    Session["UserLogin"] = LoggedInUser;
                    Session["UserAdmin"] = LoggedInUser.IsAdmin.ToString();

                    if (Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home", new { id = LoggedInUser.Username });
                    }
                }
                else
                {
                    message = "Invalid credential provided";
                }
            }
            else
            {
                message = "No password inserted";
            }
            
            ViewBag.Message = message;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Login", "User");
        }

        [HttpPost]
        public ActionResult PassReset(User TargetUser)
        {
            bool Status = false;
            string Message = "";

            string HashedPassword = Crypto.Hash(TargetUser.Password);
            TargetUser.Password = null;
            int Result = dc.ResetPassword(TargetUser.Username, HashedPassword);

            if (Result == 1)
            {
                Status = true;
                Message = "Password has bin reset";
            }
            else
            {
                Status = true;
                Message = "An error occurred";
            }

            ViewBag.Status = Status;
            ViewBag.Message = Message;
            return View();
        }
        
    }
}