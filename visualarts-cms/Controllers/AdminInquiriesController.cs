using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using visualarts_cms.Models.Viewmodel;

namespace visualarts_cms.Controllers
{
    public class AdminInquiriesController : BaseController
    {
        public ActionResult Index()
        {
            conn.Open();

            var viewModel = new List<InquiryViewModel>();
            SqlCommand getQuery = new SqlCommand("SELECT * FROM Inquiries WHERE IsActive = 1", conn);
            SqlDataReader reader = getQuery.ExecuteReader();

            while (reader.Read())
            {
                var ct = new InquiryViewModel
                {
                    InquiryId = Convert.ToInt32(reader["InquiryId"]),
                    Name = reader["Name"].ToString(),
                    Email = reader["Email"].ToString(),
                    Message = reader["Message"].ToString(),
                    CreatedDateStr = Convert.ToDateTime(reader["CreatedDate"]).ToString("MM/dd/yyyy"),
                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                };

                viewModel.Add(ct);
            }
            return View(viewModel);
        }

        public ActionResult History()
        {
            conn.Open();

            var viewModel = new List<InquiryViewModel>();
            SqlCommand getQuery = new SqlCommand("SELECT * FROM Inquiries WHERE IsActive = 0", conn);
            SqlDataReader reader = getQuery.ExecuteReader();

            while (reader.Read())
            {
                var ct = new InquiryViewModel
                {
                    InquiryId = Convert.ToInt32(reader["InquiryId"]),
                    Name = reader["Name"].ToString(),
                    Email = reader["Email"].ToString(),
                    Message = reader["Message"].ToString(),
                    CreatedDateStr = Convert.ToDateTime(reader["CreatedDate"]).ToString("MM/dd/yyyy"),
                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                };

                viewModel.Add(ct);
            }
            return View(viewModel);
        }

        [HttpGet]
        public ActionResult View(int id)
        {
            conn.Open();

            var viewModel = new InquiryViewModel();
            SqlCommand getQuery = new SqlCommand("SELECT * FROM Inquiries WHERE InquiryId = " + id, conn);
            SqlDataReader reader = getQuery.ExecuteReader();

            while (reader.Read())
            {
                viewModel = new InquiryViewModel
                {
                    InquiryId = Convert.ToInt32(reader["InquiryId"]),
                    Name = reader["Name"].ToString(),
                    Email = reader["Email"].ToString(),
                    Message = reader["Message"].ToString(),
                    CreatedDateStr = Convert.ToDateTime(reader["CreatedDate"]).ToString("MM/dd/yyyy"),
                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                };
            }

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult ViewHistory(int id)
        {
            conn.Open();

            var viewModel = new InquiryViewModel();
            SqlCommand getQuery = new SqlCommand("SELECT * FROM Inquiries WHERE InquiryId = " + id, conn);
            SqlDataReader reader = getQuery.ExecuteReader();

            while (reader.Read())
            {
                viewModel = new InquiryViewModel
                {
                    InquiryId = Convert.ToInt32(reader["InquiryId"]),
                    Name = reader["Name"].ToString(),
                    Email = reader["Email"].ToString(),
                    Message = reader["Message"].ToString(),
                    CreatedDateStr = Convert.ToDateTime(reader["CreatedDate"]).ToString("MM/dd/yyyy"),
                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                };
            }

            return View(viewModel);
        }

        #region delete 
        [HttpGet]
        public ActionResult ShowModalDelete(int id)
        {
            var viewModel = new InquiryViewModel();

            conn.Open();
            SqlCommand getQuery = new SqlCommand("SELECT * FROM Inquiries WHERE InquiryId = @InquiryId", conn);
            getQuery.Parameters.Add(new SqlParameter("InquiryId", id));
            SqlDataReader courseReader = getQuery.ExecuteReader();

            while (courseReader.Read())
            {
                viewModel.InquiryId = Convert.ToInt32(courseReader["InquiryId"]);
            }

            return PartialView("_deleteModal", viewModel);
        }

        [HttpPost]
        public ActionResult Delete(InquiryViewModel viewModel)
        {
            conn.Open();

            SqlCommand updateQuery = new SqlCommand("UPDATE Inquiries " +
                "SET IsActive = 0 WHERE InquiryId = @InquiryId", conn);
            updateQuery.Parameters.Add(new SqlParameter("InquiryId", viewModel.InquiryId));

            updateQuery.ExecuteNonQuery();

            return RedirectToAction("Index");
        }
        #endregion
    }
}