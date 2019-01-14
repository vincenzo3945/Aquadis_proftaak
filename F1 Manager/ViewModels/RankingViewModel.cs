using F1_Manager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace F1_Manager.ViewModels
{
    public class RankingViewModel
    {
        public List<Competitor> DriverList { get; set; }
        public List<User> UserList { get; set; }
    }
}