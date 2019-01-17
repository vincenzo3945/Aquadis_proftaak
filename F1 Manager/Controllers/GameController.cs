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

        public ActionResult RemoveDriver(int driverID, decimal cost)
        {
            UserLogin loggedInUser = (UserLogin)Session["UserLogin"];

            string command = $"DELETE FROM userdriver WHERE UserID = '{loggedInUser.UserID}' AND DriverID = '{driverID}'";
            db.mysql(command);

            loggedInUser.Balance = loggedInUser.Balance + cost;

            string addFunds = $"UPDATE user SET Balance = '{loggedInUser.Balance}' WHERE id = '{loggedInUser.UserID}'";
            db.mysql(addFunds);

            return RedirectToAction("MyTeam");
        }

        [HttpGet]
        public ActionResult MyTeam()
        {
            UserLogin loggedInUser = (UserLogin)Session["UserLogin"];
            
            List<Driver> driverList = db.getMyTeam(loggedInUser.UserID);
            GameViewModel GVM = new GameViewModel();
            GVM.DriverList = driverList;

            return View(GVM);
        }

        [HttpGet]
        public async Task<ActionResult> NextRace()
        {
            F1ViewModel model = new F1ViewModel();
            //F1ViewModel model = FormulaOne
            //For documentation go to:
            //https://developer.sportradar.com/docs/read/racing/Formula_1_v2#competitor-profile

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(
                    "http://api.sportradar.us/formula1/trial/v2/en/sport_events/sr:stage:324771/summary.json?api_key=cbrg93g2tbafunheua84ay5h");


                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();

                var formulaOne = FormulaOne.FromJson(responseBody);
                
                model.FormulaOne = formulaOne;
                                
            }

            string getIsFinished = $"SELECT IsFinished FROM race WHERE RaceID = '{1}'";
            model.raceFinished = db.getID(getIsFinished);

            if (model.raceFinished == 1)
            {
                int raceID = 1;
                model.raceResult = db.getRaceResult(raceID);
                model.raceResult = model.raceResult.OrderBy(P => P.Position).ToList();
            }


            return View(model);
        }

        public ActionResult EndRace(int raceID)
        {
            string command = $"UPDATE race SET IsFinished = 1 WHERE RaceID = '{raceID}'";
            db.mysql(command);

            List<Driver> driverResults = db.getRaceResult(raceID);

            foreach (Driver driver in driverResults)
            {
                List<User> usersWithDriver = db.getUsersFromDriver(driver.DriverID);

                if (usersWithDriver != null)
                {
                    foreach (User user in usersWithDriver)
                    {
                        string getUserPoints = $"SELECT Points FROM user WHERE id = '{user.UserID}'";
                        int userPoints = db.getID(getUserPoints);

                        userPoints += driver.Points;

                        string updateUserPoints = $"UPDATE user SET Points = '{userPoints}' WHERE id = '{user.UserID}'";
                        db.mysql(updateUserPoints);
                    }
                }

                usersWithDriver.Clear();
            }

            return RedirectToAction("NextRace");
        }
        
    }
}