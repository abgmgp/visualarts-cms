using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using visualarts_cms.Models.Viewmodel;

namespace visualarts_cms.Controllers
{
    public class LoginController : BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoginUser(UserAccountViewModel viewModel)
        {
            try
            {
                conn.Open();

                if(viewModel.Password == viewModel.ConfirmPassword)
                {
                    SqlCommand checkCredentialsQuery = new SqlCommand("SELECT COUNT(*) FROM UserAccounts WHERE Username = @username", conn);
                    checkCredentialsQuery.Parameters.AddWithValue("@username", viewModel.Username);

                    int userCount = (int)checkCredentialsQuery.ExecuteScalar();

                    if (userCount > 0)
                    {
                        SqlCommand getUserInfoQuery = new SqlCommand("SELECT UserAccountId,Password FROM UserAccounts WHERE Username = @username", conn);
                        getUserInfoQuery.Parameters.AddWithValue("@username", viewModel.Username);

                        SqlDataReader reader = getUserInfoQuery.ExecuteReader();

                        if (reader.Read())
                        {
                            int userId = (int)reader["UserAccountId"];
                            string hashPassFromDb = (string)reader["Password"];

                            reader.Close();

                            bool hashPassGen = Crypto.VerifyHashedPassword(hashPassFromDb, viewModel.Password);

                            if (hashPassGen)
                            {
                                HttpContext.Session["UserId"] = userId;
                                return RedirectToAction("Index", "Admin");
                            }
                            else
                            {
                                ViewBag.Message = "Incorrect password. Please try again.";
                                return View("Index");
                            }

                        }
                    }

                    ViewBag.Message = "No username of " + viewModel.Username + " found. Please try again.";
                    return View("Index");
                }
                ViewBag.Message = "Passwords do not match. Please try again.";
                return View("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewBag.Message = "Error occured when logging in. Please contact system administrator for support.";
                return View("Index");
            }
            finally
            {
                conn.Close();
            }
        }

        [HttpGet]
        public void Logout()
        {
            Session.Abandon();
            return ;
        }
    }

}