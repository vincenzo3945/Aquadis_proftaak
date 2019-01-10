using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace F1_Manager.Models
{
    public class Group
    {
        public string GroupName { get; set; }
        public List<User> GroupUserList { get; set; }
    }
}