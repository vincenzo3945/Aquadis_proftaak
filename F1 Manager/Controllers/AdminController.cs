using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using F1_Manager.Models;
using F1_Manager.ViewModels;

namespace F1_Manager.Controllers
{
    public class AdminController : Controller
    {
        mysqlcon dc = new mysqlcon();

        // GET: Admin
        [HttpGet]
        public ActionResult AdminPage()
        {
            //Get al users for change of user password
            List<User> AllUsers = dc.GetAllUsers();

            ResetPasswordViewModel RPVM = new ResetPasswordViewModel();

            if (AllUsers.Any())
            {
                RPVM.UserList = AllUsers;
            }

            return View(RPVM);
        }

        [HttpPost]
        public ActionResult ChangePassword(ResetPasswordViewModel Data)
        {
            TempData["Message"] = "Something went wrong";
            //Get al users for change of user password
            int Result = dc.ResetPassword(Data.Username, Crypto.Hash(Data.Password));

            if (Result == 1)
            {
                TempData["Message"] = "Password changed";
            }

            return RedirectToAction("AdminPage");
        }

        public ActionResult RefreshData()
        {
            //code for refres of api/race data(random generate)

            return RedirectToAction("AdminPage");
        }
    }
}