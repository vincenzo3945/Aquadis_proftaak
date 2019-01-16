using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using F1_Manager.Models;
using System.Web.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace F1_Manager.Controllers
{
    public class DriverController : ApiController
    {
        public async Task<List<Competitor>> DriverRanking()
        {
            List<Competitor> DriverList = new List<Competitor>();
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

                foreach (var competitor in formulaOne.Stage.Competitors)
                {
                    DriverList.Add(new Competitor { Name = competitor.Name, Nationality = competitor.Nationality, Team = competitor.Team, Points = competitor.Points, Result = competitor.Result });
                }

                return DriverList;
            }
        }
    }
}