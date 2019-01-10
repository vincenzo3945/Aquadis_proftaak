using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Threading.Tasks;
using F1_Manager.Models;

namespace F1_Manager.Controllers
{
    public class GameController : ApiController
    {
        mysqlcon db = new mysqlcon();

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