using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using visualarts_cms.Models.Viewmodel;

namespace visualarts_cms.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            var viewmodel = new InquiryViewModel();
            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult SubmitInquiry(InquiryViewModel viewModel)
        {
            try
            {
                conn.Open();

                SqlCommand insertQuery = new SqlCommand("INSERT INTO Inquiries " +
                    "(Name, Email, Message, IsActive, CreatedDate) " +
                    "VALUES (@Name, @Email, @Message, @IsActive, @CreatedDate)", conn);
                insertQuery.Parameters.Add(new SqlParameter("@Name", viewModel.Name));
                insertQuery.Parameters.Add(new SqlParameter("@Email", viewModel.Email));
                insertQuery.Parameters.Add(new SqlParameter("@Message", viewModel.Message ?? ""));
                insertQuery.Parameters.Add(new SqlParameter("@IsActive", true));
                insertQuery.Parameters.Add(new SqlParameter("@CreatedDate", DateTime.Now));

                insertQuery.ExecuteNonQuery();

                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
    }
}