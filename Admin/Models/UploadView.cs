using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin.Models
{
    public class UploadView
    {
        public bool Success { get; set; }

        public string UploadedFilePath { get; set; }

        public string ErrorMessage { get; set; }
    }
}