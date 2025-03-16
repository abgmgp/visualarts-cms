using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using visualarts_cms.Models.Viewmodel;

namespace visualarts_cms.Controllers
{
    public class AdminController : BaseController
    {
        public ActionResult Index()
        {
            conn.Open();
            var viewmodel = new DashboardViewModel();

            #region current trend count
            SqlCommand getQueryCT = new SqlCommand("SELECT COUNT(CurrentTrendId) as Count FROM CurrentTrends WHERE IsActive = 1", conn);
            SqlDataReader getQueryCTreader = getQueryCT.ExecuteReader();
            while (getQueryCTreader.Read())
            {
                viewmodel.CurrentTrendCount = Convert.ToInt32(getQueryCTreader["Count"]);
            }
            #endregion

            #region inquiry count
            SqlCommand getQueryInq = new SqlCommand("SELECT COUNT(InquiryId) as Count FROM Inquiries WHERE IsActive = 1", conn);
            SqlDataReader getQueryInqReader = getQueryInq.ExecuteReader();
            while (getQueryInqReader.Read())
            {
                viewmodel.InquiryCount = Convert.ToInt32(getQueryInqReader["Count"]);
            }
            #endregion

            #region trend list
            var currentTrends = new List<CurrentTrendViewModel>();
            SqlCommand getQueryCTList = new SqlCommand("SELECT * FROM CurrentTrends WHERE IsActive = 1", conn);
            SqlDataReader getQueryCTListreader = getQueryCTList.ExecuteReader();
            while (getQueryCTListreader.Read())
            {
                var ct = new CurrentTrendViewModel
                {
                    CurrentTrendId = Convert.ToInt32(getQueryCTListreader["CurrentTrendId"]),
                    Title = getQueryCTListreader["Title"].ToString(),
                    CreatedDateStr = Convert.ToDateTime(getQueryCTListreader["DateCreated"]).ToString("MM/dd/yyyy"),
                    UpdatedDateStr = !Convert.IsDBNull(getQueryCTListreader["UpdatedDate"]) ? Convert.ToDateTime(getQueryCTListreader["UpdatedDate"]).ToString("MM/dd/yyyy") : "",
                    IsActive = Convert.ToBoolean(getQueryCTListreader["IsActive"]),
                    ImagePath = getQueryCTListreader["ImagePath"].ToString(),
                };

                currentTrends.Add(ct);
            }
            viewmodel.CurrentTrendList = currentTrends;
            #endregion


            return View(viewmodel);
        }
    }
}