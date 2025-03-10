using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace visualarts_cms.Models.Viewmodel
{
    public class CurrentTrendViewModel : BaseViewModel
    {
        public int CurrentTrendId { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string ImagePath { get; set; }
        public string EmbedUrl { get; set; }
        public string Content { get; set; }
        public string AudioPath { get; set; }
    }
}