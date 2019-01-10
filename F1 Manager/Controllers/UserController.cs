using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Net;
using System.Web.Security;
using F1_Manager.Models;

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

                int result = dc.checkRegistration(user, 0, 0);


                //string command = $"INSERT into user (Username, Password, Balance, IsAdmin) VALUES ('{user.Username}','{user.Password}','{0.00}','{0}')";
                //dc.mysql(command);
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
                int Result = dc.CheckLogin(login.Username, Crypto.Hash(login.Password));

                /*
                string uNameQuery =  $"SELECT Username FROM user WHERE Username= '{login.Username}'";
                string pWordQuery =  $"SELECT Password FROM user WHERE Password= '{Crypto.Hash(login.Password)}'";

                string uNameResult = dc.getString(uNameQuery);
                string pWordResult = dc.getString(pWordQuery);*/

                if (Result == 1)
                {
                    int timeout = login.RememberMe ? 525600 : 20; // 525600 min = 1 year
                    var ticket = new FormsAuthenticationTicket(login.Username, login.RememberMe, timeout);
                    string encrypted = FormsAuthentication.Encrypt(ticket);
                    var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                    cookie.Expires = DateTime.Now.AddMinutes(timeout);
                    cookie.HttpOnly = true;
                    Response.Cookies.Add(cookie);
                    
                    Session["UserLogin"] = new UserLogin()
                    { Username = login.Username};

                    if (Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home", new { id = login.Username });
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