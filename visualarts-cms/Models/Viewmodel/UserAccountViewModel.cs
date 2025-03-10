using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace visualarts_cms.Models.Viewmodel
{
    public class UserAccountViewModel : BaseViewModel
    {
        public int UserAccountId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

    }
}