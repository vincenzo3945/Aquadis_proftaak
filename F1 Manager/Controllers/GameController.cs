using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Threading.Tasks;
using F1_Manager.Models;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using F1_Manager.ViewModels;

namespace F1_Manager.Controllers
{
    public class GameController : Controller
    {
        mysqlcon db = new mysqlcon();

        [HttpGet]
        public ActionResult Index()
        {

            List<Driver> driverList = new List<Driver>();

            string command = $"SELECT * FROM driver";
            MySqlDataReader reader = db.ReadSQL(command);

            while (reader.Read())
            {
                driverList.Add(new Driver { DriverID = (int)reader["DriverID"], Name = (string)reader["FirstName"], Team = (string)reader["Team"], Cost = (decimal)reader["Cost"] });
            }
            db.CloseConnection();

            GameViewModel GVM = new GameViewModel();
            GVM.DriverList = driverList;

            return View(GVM);
        }
        
        public ActionResult BuyDriver(int driverID, decimal cost)
        {
            UserLogin loggedInUser = (UserLogin)Session["UserLogin"];
            bool status = false;
            string message;

            if (cost <= loggedInUser.Balance)
            {
                string checkUserDriver = $"SELECT DriverID FROM userdriver WHERE UserID = '{loggedInUser}' AND DriverID = '{driverID}'";
                int hasDriver = db.getID(checkUserDriver);

                if (hasDriver == 0)
                {
                    string buyDriver = $"INSERT INTO userdriver (UserID, DriverID) VALUES('{loggedInUser.UserID}', '{driverID}')";
                    db.mysql(buyDriver);

                    loggedInUser.Balance = loggedInUser.Balance - cost;

                    string updateBalance = $"UPDATE user SET Balance = '{loggedInUser.Balance}' WHERE id = '{loggedInUser.UserID}'";
                    db.mysql(updateBalance);

                    status = true;
                    message = "Driver successfully bought!";
                }
                else
                {
                    message = "Already bought this driver!";
                }
            }
            else
            {
                message = "Not enough funds to buy this driver!";
            }

            ViewBag.Status = status;
            ViewBag.Message = message;

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult MyTeam()
        {
            UserLogin loggedInUser = (UserLogin)Session["UserLogin"];
            //List<Driver> driverList = new List<Driver>();

            //string getDriverIDs = $"SELECT * FROM driver INNER JOIN userdriver ON driver.DriverID = userdriver.DriverID WHERE userdriver.UserID = '{loggedInUser.UserID}";
            //MySqlDataReader reader = db.ReadSQL(getDriverIDs);

            //while (reader.Read())
            //{
            //    driverList.Add(new Driver { DriverID = (int)reader["DriverID"], Name = (string)reader["FirstName"], Team = (string)reader["Team"], Cost = (decimal)reader["Cost"], Points = (int)reader["Points"] });            
            //}

            //db.CloseConnection();
            List<Driver> driverList = db.getMyTeam(loggedInUser.UserID);
            GameViewModel GVM = new GameViewModel();
            GVM.DriverList = driverList;

            return View(GVM);
        }

        [HttpPost]
        public void AddTeam(string username, string json)
        {
            string St = "Username: ";
            int pUFrom = St.IndexOf("Username: ") + "Username: ".Length;
            int pUTo = St.LastIndexOf(",");

            string result = St.Substring(pUFrom, pUTo - pUFrom);

            string getUserID = $"SELECT id FROM user WHERE Username = '{username}'";
            int userID = db.getID(getUserID);


            /*using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(
                    "http://api.sportradar.us/formula1/trial/v2/en/sport_events/sr:stage:324771/summary.json?api_key=cbrg93g2tbafunheua84ay5h");



                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();

                var formulaOne = FormulaOne.FromJson(responseBody);

                foreach (var competitor in formulaOne.Stage.Competitors)
                {
                    DriverList.Add(new Competitor { Name = competitor.Name, CountryCode = competitor.CountryCode, Nationality = competitor.Nationality, Gender = competitor.Gender, Points = competitor.Points, PreviousPoints = 0 });
                }

                
            }*/
        }

        
    }
}