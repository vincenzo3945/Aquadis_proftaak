using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace F1_Manager.Models
{
    public class Driver
    {
        public int DriverID { get; set; }
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public int Points { get; set; }
        public string Team { get; set; }
    }
}