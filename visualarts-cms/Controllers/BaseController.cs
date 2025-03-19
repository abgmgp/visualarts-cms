using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace visualarts_cms.Controllers
{
    public class BaseController : Controller
    {
        public static string connectionString = ConfigurationManager.ConnectionStrings["sqlConnection"].ConnectionString;
        public SqlConnection conn = new SqlConnection(connectionString);

        public bool CheckIfLoggedIn()
        {
            var loggedUser = HttpContext.Session["UserId"];

            if (loggedUser == null)
            {
                return false;
            }

            return true;
        }
    }
}