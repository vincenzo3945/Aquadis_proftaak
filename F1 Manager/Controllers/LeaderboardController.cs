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
using F1_Manager.ViewModels;
using System.Threading.Tasks;

namespace F1_Manager.Controllers
{
    public class LeaderboardController : Controller
    {
        mysqlcon dc = new mysqlcon();

        public async Task<ActionResult> Ranking()
        {
            RankingViewModel rankingModel = new RankingViewModel();
            DriverController drivercontroller = new DriverController();


            List<User> userList = new List<User>();
            string cmd = $"SELECT Username,Points FROM user";

            MySqlDataReader reader = dc.ReadSQL(cmd);

            while (reader.Read())
            {
                userList.Add(new User { Username = reader["Username"].ToString(), Points = int.Parse(reader["Points"].ToString()) });
            }

            dc.CloseConnection();

            rankingModel.UserList = userList;
            rankingModel.DriverList = await drivercontroller.DriverRanking();

            rankingModel.UserList = rankingModel.UserList.OrderByDescending(P => P.Points).ToList();

            return View(rankingModel);
        }
    }
}