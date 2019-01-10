using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Security;
using F1_Manager.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Net.Http;

namespace F1_Manager.Controllers
{
    public class LeaderboardController : Controller
    {
        mysqlcon dc = new mysqlcon();

        public ActionResult UserRanking()
        {
            List<User> rankingList = new List<User>(); 
            string cmd = $"SELECT Username,Points FROM user";

            MySqlDataReader reader = dc.ReadSQL(cmd);

            while(reader.Read())
            {
                rankingList.Add(new User { Username = reader["Username"].ToString(), Points = int.Parse(reader["Points"].ToString())});
            }

            dc.CloseConnection();
            
            return View(rankingList.ToList());
        }
    }
}