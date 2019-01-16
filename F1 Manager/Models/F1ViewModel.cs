using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace F1_Manager.Models
{
    public class F1ViewModel
    {
        public FormulaOne FormulaOne { get; set; }
        public List<Driver> raceResult { get; set; }
        public int raceFinished { get; set; }
    }
}