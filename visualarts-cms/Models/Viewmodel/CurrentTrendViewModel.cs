using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace visualarts_cms.Models.Viewmodel
{
    public class CurrentTrendViewModel : BaseViewModel
    {
        public int CurrentTrendId { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedDateStr { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
        public HttpPostedFileBase AudioFile { get; set; }
        public string ImagePath { get; set; }
        public string EmbedUrl { get; set; }
        [AllowHtml]
        public string Content { get; set; }
        public string AudioPath { get; set; }
        public string Title { get; set; }
    }
}