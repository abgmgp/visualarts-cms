using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace visualarts_cms.Models.Viewmodel
{
    public class InquiryViewModel : BaseViewModel
    {
        public int InquiryId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
    }
}