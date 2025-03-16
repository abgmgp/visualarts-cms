using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace visualarts_cms.Models.Viewmodel
{
    public class DashboardViewModel
    {
        public int CurrentTrendCount { get; set; }
        public int InquiryCount { get; set; }
        public List<CurrentTrendViewModel> CurrentTrendList { get;set; }
    }
}