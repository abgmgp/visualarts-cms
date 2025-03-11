using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using visualarts_cms.Models.Viewmodel;

namespace visualarts_cms.Controllers
{
    public class AdminCurrentTrendsController : BaseController
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
                    CreatedDateStr = Convert.ToDateTime(reader["DateCreated"]).ToString("MM/dd/yyyy"),
                    UpdatedDateStr = !Convert.IsDBNull(reader["UpdatedDate"]) ? Convert.ToDateTime(reader["UpdatedDate"]).ToString("MM/dd/yyyy") : "",
                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                };

                viewModel.Add(ct);
            }

            return View(viewModel);
        }

        public ActionResult Restore()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(CurrentTrendViewModel viewModel)
        {
            #region Check directories
            if (!Directory.Exists(Server.MapPath("~/Content/Upload/Image")))
            {
                Directory.CreateDirectory(Server.MapPath("~/Content/Upload/Image"));
            }

            if (!Directory.Exists(Server.MapPath("~/Content/Upload/Audio")))
            {
                Directory.CreateDirectory(Server.MapPath("~/Content/Upload/Audio"));
            }
            #endregion

            #region map full image path

            string fullPathImage = null;
            string fullPathAudio = null;

            if(viewModel.ImageFile != null)
            {
                fullPathImage = Server.MapPath("~/Content/Upload/Image" + viewModel.ImageFile.FileName);

            }
            if (viewModel.AudioFile != null)
            {
                fullPathAudio = Server.MapPath("~/Content/Upload/Audio" + viewModel.AudioFile.FileName);
            }
            #endregion

            //write to db
            conn.Open();

            SqlCommand insertQuery = new SqlCommand("INSERT INTO CurrentTrends " +
                "(Title, Content, EmbedUrl, AudioPath, ImagePath, IsActive, DateCreated) " +
                "VALUES (@Title, @Content, @EmbedUrl, @AudioPath, @ImagePath, @IsActive, @DateCreated)", conn);
            insertQuery.Parameters.Add(new SqlParameter("@Title", viewModel.Title));
            insertQuery.Parameters.Add(new SqlParameter("@Content", viewModel.Content));
            insertQuery.Parameters.Add(new SqlParameter("@EmbedUrl", viewModel.EmbedUrl));
            insertQuery.Parameters.Add(new SqlParameter("@AudioPath", viewModel.AudioFile.FileName));
            insertQuery.Parameters.Add(new SqlParameter("@ImagePath", viewModel.ImageFile.FileName));
            insertQuery.Parameters.Add(new SqlParameter("@IsActive", true));
            insertQuery.Parameters.Add(new SqlParameter("@DateCreated", DateTime.Now));

            insertQuery.ExecuteNonQuery();

            return RedirectToAction("Index");
        }
    }
}