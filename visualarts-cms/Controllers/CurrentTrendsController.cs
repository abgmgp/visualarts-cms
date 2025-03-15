using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using visualarts_cms.Models.Viewmodel;

namespace visualarts_cms.Controllers
{
    public class CurrentTrendsController : BaseController
    {
        public ActionResult Index()
        {
            conn.Open();

            var viewModel = new List<CurrentTrendViewModel>();
            SqlCommand getQuery = new SqlCommand("SELECT * FROM CurrentTrends WHERE IsActive = 1", conn);
            SqlDataReader reader = getQuery.ExecuteReader();

            while (reader.Read())
            {
                var ct = new CurrentTrendViewModel
                {
                    CurrentTrendId = Convert.ToInt32(reader["CurrentTrendId"]),
                    Title = reader["Title"].ToString(),
                    ImagePath = reader["ImagePath"].ToString(),
                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                };

                viewModel.Add(ct);
            }

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Read(int id)
        {
            conn.Open();

            var viewModel = new CurrentTrendViewModel();
            SqlCommand getQuery = new SqlCommand("SELECT * FROM CurrentTrends WHERE CurrentTrendId = " + id, conn);
            SqlDataReader reader = getQuery.ExecuteReader();

            while (reader.Read())
            {
                viewModel = new CurrentTrendViewModel
                {
                    CurrentTrendId = Convert.ToInt32(reader["CurrentTrendId"]),
                    Title = reader["Title"].ToString(),
                    Content = reader["Content"].ToString(),
                    ImagePath = reader["ImagePath"].ToString(),
                    AudioPath = reader["AudioPath"].ToString(),
                    EmbedUrl = reader["EmbedUrl"].ToString(),
                };
            }

            viewModel.EmbedUrl = viewModel.EmbedUrl.Replace("watch?v=", "embed/");
            return View(viewModel);
        }
    }
}