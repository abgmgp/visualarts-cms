﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
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
            var loggedIn = CheckIfLoggedIn();
            if (!loggedIn)
            {
                TempData["NotLoggedIn"] = "Access denied. Please login first.";
                return RedirectToAction("Index", "Login");
            }

            if (TempData.ContainsKey("MessageIndex"))
            {
                var placeholder = TempData["MessageIndex"].ToString();
                if (!String.IsNullOrEmpty(placeholder))
                {
                    ViewBag.MessageIndex = placeholder;
                }
            }

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
            var loggedIn = CheckIfLoggedIn();
            if (!loggedIn)
            {
                TempData["NotLoggedIn"] = "Access denied. Please login first.";
                return RedirectToAction("Index", "Login");
            }

            conn.Open();

            var viewModel = new List<CurrentTrendViewModel>();
            SqlCommand getQuery = new SqlCommand("SELECT * FROM CurrentTrends WHERE IsActive = 0", conn);
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

        #region create
        [HttpGet]
        public ActionResult Create()
        {
            var loggedIn = CheckIfLoggedIn();
            if (!loggedIn)
            {
                TempData["NotLoggedIn"] = "Access denied. Please login first.";
                return RedirectToAction("Index", "Login");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Create(CurrentTrendViewModel viewModel)
        {
            try
            {
                #region Check directories
                if (!Directory.Exists(Server.MapPath("~/Content/Upload/Image")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Content/Upload/Image/"));
                }

                if (!Directory.Exists(Server.MapPath("~/Content/Upload/Audio/")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Content/Upload/Audio/"));
                }
                #endregion

                #region map full image path

                string[] imageFormat = {".jpg", ".jpeg", ".png", ".gif"};
                var audioFormat = ".mp3";

                if (viewModel.ImageFile != null)
                {
                    string fileExtension = Path.GetExtension(viewModel.ImageFile.FileName).ToLower();

                    if (!imageFormat.Contains(fileExtension))
                    {
                        ViewBag.MessageCreate = "Incorrect image format. Only jpg/jpeg, png and gif files are allowed.";
                        return View("Create");
                    }
                    var fullPathImage = Server.MapPath("~/Content/Upload/Image/" + viewModel.ImageFile.FileName);
                    viewModel.ImageFile.SaveAs(fullPathImage);

                }
                if (viewModel.AudioFile != null)
                {
                    string audioExtension = Path.GetExtension(viewModel.AudioFile.FileName).ToLower();

                    if (audioFormat != audioExtension)
                    {
                        ViewBag.MessageCreate = "Incorrect audio format. Only mp3 files are allowed.";
                        return View("Create");
                    }
                    var fullPathAudio = Server.MapPath("~/Content/Upload/Audio/" + viewModel.AudioFile.FileName);
                    viewModel.AudioFile.SaveAs(fullPathAudio);
                }
                #endregion

                //write to db
                conn.Open();

                SqlCommand insertQuery = new SqlCommand("INSERT INTO CurrentTrends " +
                    "(Title, Content, EmbedUrl, AudioPath, ImagePath, IsActive, DateCreated) " +
                    "VALUES (@Title, @Content, @EmbedUrl, @AudioPath, @ImagePath, @IsActive, @DateCreated)", conn);
                insertQuery.Parameters.Add(new SqlParameter("@Title", viewModel.Title));
                insertQuery.Parameters.Add(new SqlParameter("@Content", viewModel.Content));
                insertQuery.Parameters.Add(new SqlParameter("@EmbedUrl", viewModel.EmbedUrl ?? ""));
                insertQuery.Parameters.Add(new SqlParameter("@AudioPath", viewModel.AudioFile != null ? viewModel.AudioFile.FileName : ""));
                insertQuery.Parameters.Add(new SqlParameter("@ImagePath", viewModel.ImageFile != null ? viewModel.ImageFile.FileName : ""));
                insertQuery.Parameters.Add(new SqlParameter("@IsActive", true));
                insertQuery.Parameters.Add(new SqlParameter("@DateCreated", DateTime.Now));

                insertQuery.ExecuteNonQuery();

                TempData["MessageIndex"] = "Record saved successfully!";
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["MessageIndex"] = "Error when saving. Please try again later.";
                return RedirectToAction("Index");
            }
        }
        #endregion create

        #region edit
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var loggedIn = CheckIfLoggedIn();
            if (!loggedIn)
            {
                TempData["NotLoggedIn"] = "Access denied. Please login first.";
                return RedirectToAction("Index", "Login");
            }

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

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(CurrentTrendViewModel viewModel)
        {
            try
            {
                conn.Open();

                #region Check directories
                if (!Directory.Exists(Server.MapPath("~/Content/Upload/Image")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Content/Upload/Image/"));
                }

                if (!Directory.Exists(Server.MapPath("~/Content/Upload/Audio/")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Content/Upload/Audio/"));
                }
                #endregion

                #region check if image/audio path same as current db record

                var currentDbRecord = new CurrentTrendViewModel();
                SqlCommand getQuery = new SqlCommand("SELECT * FROM CurrentTrends WHERE CurrentTrendId = " + viewModel.CurrentTrendId, conn);
                SqlDataReader reader = getQuery.ExecuteReader();

                while (reader.Read())
                {
                    currentDbRecord = new CurrentTrendViewModel
                    {
                        CurrentTrendId = Convert.ToInt32(reader["CurrentTrendId"]),
                        Title = reader["Title"].ToString(),
                        Content = reader["Content"].ToString(),
                        ImagePath = reader["ImagePath"].ToString(),
                        AudioPath = reader["AudioPath"].ToString(),
                        EmbedUrl = reader["EmbedUrl"].ToString(),
                    };
                }
                #endregion

                #region map full image path

                string[] imageFormat = { ".jpg", ".jpeg", ".png", ".gif" };
                var audioFormat = ".mp3";

                if (viewModel.ImageFile != null)
                {
                    string fileExtension = Path.GetExtension(viewModel.ImageFile.FileName).ToLower();

                    if (!imageFormat.Contains(fileExtension))
                    {
                        ViewBag.MessageEdit = "Incorrect image format. Only jpg/jpeg, png and gif files are allowed.";
                        viewModel.ImagePath = currentDbRecord.ImagePath;
                        viewModel.AudioPath = currentDbRecord.AudioPath;
                        return View("Edit", viewModel);
                    }
                    if (viewModel.ImageFile.FileName != currentDbRecord.ImagePath)
                    {
                        var fullPathImageOld = Server.MapPath("~/Content/Upload/Image/" + currentDbRecord.ImagePath);
                        if (System.IO.File.Exists(fullPathImageOld))
                        {
                            System.IO.File.Delete(fullPathImageOld);
                        }
                    }
                    var fullPathImage = Server.MapPath("~/Content/Upload/Image/" + viewModel.ImageFile.FileName);
                    viewModel.ImageFile.SaveAs(fullPathImage);

                }
                if (viewModel.AudioFile != null)
                {
                    string audioExtension = Path.GetExtension(viewModel.AudioFile.FileName).ToLower();

                    if (audioFormat != audioExtension)
                    {
                        ViewBag.MessageEdit = "Incorrect audio format. Only mp3 files are allowed.";
                        viewModel.ImagePath = currentDbRecord.ImagePath;
                        viewModel.AudioPath = currentDbRecord.AudioPath;
                        return View("Edit", viewModel);
                    }
                    if (viewModel.AudioFile.FileName != currentDbRecord.ImagePath)
                    {
                        var fullPathAudioOld = Server.MapPath("~/Content/Upload/Audio/" + currentDbRecord.AudioPath);
                        if (System.IO.File.Exists(fullPathAudioOld))
                        {
                            System.IO.File.Delete(fullPathAudioOld);
                        }
                    }
                    var fullPathAudio = Server.MapPath("~/Content/Upload/Audio/" + viewModel.AudioFile.FileName);
                    viewModel.AudioFile.SaveAs(fullPathAudio);
                }
                #endregion

                SqlCommand insertQuery = new SqlCommand("UPDATE CurrentTrends " +
                    "SET Title = @Title, Content = @Content, EmbedUrl = @EmbedUrl, AudioPath = @AudioPath, ImagePath = @ImagePath, UpdatedDate = @DateUpdated " +
                    "WHERE CurrentTrendId = @CurrentTrendId", conn);
                insertQuery.Parameters.Add(new SqlParameter("@Title", viewModel.Title));
                insertQuery.Parameters.Add(new SqlParameter("@Content", viewModel.Content));
                insertQuery.Parameters.Add(new SqlParameter("@EmbedUrl", viewModel.EmbedUrl ?? ""));
                insertQuery.Parameters.Add(new SqlParameter("@AudioPath", viewModel.AudioFile != null ? viewModel.AudioFile.FileName : currentDbRecord.AudioPath));
                insertQuery.Parameters.Add(new SqlParameter("@ImagePath", viewModel.ImageFile != null ? viewModel.ImageFile.FileName : currentDbRecord.ImagePath));
                insertQuery.Parameters.Add(new SqlParameter("@DateUpdated", DateTime.Now));
                insertQuery.Parameters.Add(new SqlParameter("@CurrentTrendId", viewModel.CurrentTrendId));

                insertQuery.ExecuteNonQuery();

                TempData["MessageIndex"] = "Updated successfully!";
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["MessageIndex"] = "Error when updating!";
                return RedirectToAction("Index");
            }
        }
        #endregion edit

        #region view
        [HttpGet]
        public ActionResult View(int id)
        {
            var loggedIn = CheckIfLoggedIn();
            if (!loggedIn)
            {
                TempData["NotLoggedIn"] = "Access denied. Please login first.";
                return RedirectToAction("Index", "Login");
            }

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

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult ViewRestore(int id)
        {
            var loggedIn = CheckIfLoggedIn();
            if (!loggedIn)
            {
                TempData["NotLoggedIn"] = "Access denied. Please login first.";
                return RedirectToAction("Index", "Login");
            }

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

            return View(viewModel);
        }
        #endregion view

        #region delete 
        [HttpGet]
        public ActionResult ShowModalDelete(int id)
        {
            var viewModel = new CurrentTrendViewModel();

            conn.Open();
            SqlCommand getQuery = new SqlCommand("SELECT * FROM CurrentTrends WHERE CurrentTrendId = @CurrentTrendId", conn);
            getQuery.Parameters.Add(new SqlParameter("CurrentTrendId", id));
            SqlDataReader courseReader = getQuery.ExecuteReader();

            while (courseReader.Read())
            {
                viewModel.CurrentTrendId = Convert.ToInt32(courseReader["CurrentTrendId"]);
            }

            return PartialView("_deleteModal", viewModel);
        }

        [HttpPost]
        public ActionResult Delete(CurrentTrendViewModel viewModel)
        {
            conn.Open();

            SqlCommand updateQuery = new SqlCommand("UPDATE CurrentTrends " +
                "SET IsActive = 0 WHERE CurrentTrendId = @CurrentTrendId", conn);
            updateQuery.Parameters.Add(new SqlParameter("CurrentTrendId", viewModel.CurrentTrendId));

            updateQuery.ExecuteNonQuery();

            return RedirectToAction("Index");
        }
        #endregion

        #region restore
        [HttpGet]
        public ActionResult ShowModalRestore(int id)
        {
            var viewModel = new CurrentTrendViewModel();

            conn.Open();
            SqlCommand getQuery = new SqlCommand("SELECT * FROM CurrentTrends WHERE CurrentTrendId = @CurrentTrendId", conn);
            getQuery.Parameters.Add(new SqlParameter("CurrentTrendId", id));
            SqlDataReader courseReader = getQuery.ExecuteReader();

            while (courseReader.Read())
            {
                viewModel.CurrentTrendId = Convert.ToInt32(courseReader["CurrentTrendId"]);
            }

            return PartialView("_restoreModal", viewModel);
        }

        [HttpPost]
        public ActionResult Restore(CurrentTrendViewModel viewModel)
        {
            conn.Open();

            SqlCommand updateQuery = new SqlCommand("UPDATE CurrentTrends " +
                "SET IsActive = 1 WHERE CurrentTrendId = @CurrentTrendId", conn);
            updateQuery.Parameters.Add(new SqlParameter("CurrentTrendId", viewModel.CurrentTrendId));

            updateQuery.ExecuteNonQuery();

            return RedirectToAction("Restore");
        }
        #endregion
    }
}