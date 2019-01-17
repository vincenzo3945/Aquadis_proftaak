using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using F1_Manager.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace F1_Manager.Controllers
{
    public class GroupController : Controller
    {
        mysqlcon db = new mysqlcon();

        [Authorize]
        public ActionResult Index()
        {
            UserLogin loggedinUser = (UserLogin)(Session["UserLogin"]);

            string getUserID = $"SELECT id FROM user WHERE Username =  '{loggedinUser.Username}'";
            int userID = db.getID(getUserID);

            string getGroup = $"SELECT groupID FROM groupuser WHERE userID =  '{userID}'";
            int groupID = db.getID(getGroup);

            if (groupID != 0)
            {
                Group group = new Group();
                group.GroupID = groupID;

                string getGroupName = $"SELECT groupname FROM groups WHERE id = '{groupID}'";
                group.GroupName = db.getString(getGroupName);

                string cmd = $"SELECT userID FROM groupuser Where groupID = '{groupID}'";

                TempData["group"] = groupID;

                MySqlDataReader reader = db.ReadSQL(cmd);

                List<int> userIDList = new List<int>();

                while (reader.Read())
                {
                    int id = int.Parse(reader["userID"].ToString());

                    userIDList.Add(id);
                }

                db.CloseConnection();

                List<User> groupuserList = new List<User>();

                foreach(int id in userIDList)
                {
                    string getUser = $"SELECT Username, Points FROM user WHERE id = '{id}'";
                    MySqlDataReader readuser = db.ReadSQL(getUser);

                    while(readuser.Read())
                    {
                        groupuserList.Add(new User { Username = (string)readuser["Username"], Points = (int)readuser["Points"] });
                    }

                    db.CloseConnection();
                }                

                group.GroupUserList = groupuserList;

                return View(group);
            }
            else
            {
                return RedirectToAction("Create");
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind] Group group)
        {
            bool Status = false;
            string message = "";

            UserLogin user = (UserLogin)(Session["UserLogin"]);

            // Model Validation 
            if (ModelState.IsValid)
            {
                string checkGroupName = $"SELECT groupName FROM groups WHERE groupName = '{group.GroupName}'";
                string groupNameResult = db.getString(checkGroupName);

                if (groupNameResult == null)
                {
                    string getUserID = $"SELECT id FROM user WHERE Username =  '{user.Username}'";
                    int userID = db.getID(getUserID);

                    if (userID != 0)
                    {
                        string checkUserGroup = $"SELECT userID FROM groupuser WHERE userID =  '{userID}'";
                        int usergroupResult = db.getID(checkUserGroup);

                        if (usergroupResult == 0)
                        {
                            string createGroup = $"INSERT into groups (groupname) VALUES ('{group.GroupName}')";
                            db.mysql(createGroup);
                            string getgroupID = $"SELECT id FROM groups WHERE groupname =  '{group.GroupName}'";
                            int groupID = db.getID(getUserID);
                            string adduser = $"INSERT into groupuser (groupID, userID) VALUES ('{groupID}','{userID}')";
                            db.mysql(adduser);

                            message = "Group successfully Created.";
                            Status = true;

                            //TempData["group"] = group;
                            //return RedirectToAction("AddUser", "Group");
                        }

                        else
                        {
                            message = "You are already in a group";
                        }
                    }

                    else
                    {
                        message = "Not logged in";
                    }
                }
                else
                {
                    message = "Group name is already in use.";
                }
            }
            else
            {
                message = "Invalid Request";
            }

            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View(group);
        }

        [HttpGet]
        public ActionResult AddUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddUser([Bind]User user)
        {
            bool Status = false;
            string message = "";
            int groupID = (int)TempData["groupData"];

            
            string getUserID = $"SELECT id FROM user WHERE Username =  '{user.Username}'";
            int userID = db.getID(getUserID);

            if (userID != 0)
            {
                string command = $"INSERT into groupuser (groupID, userID) VALUES ('{groupID}','{userID}')";
                db.mysql(command);
                message = "User successfully added.";
                Status = true;
            }

            else
            {
                message = "User does not exist";
            }

            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View();

        }
        
    }
}

