using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using F1_Manager.Models;
using System.ComponentModel.DataAnnotations;


namespace F1_Manager.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Username cant be empty")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password cant be empty")]
        public string Password { get; set; }

        public List<User> UserList { get; set; }

    }
}