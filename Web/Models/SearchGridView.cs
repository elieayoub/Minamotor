using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class SearchGridView
    {
        public int Status { get; set; }

        public string MakeID { get; set; }

        public string ModelID { get; set; }

        public string VehicleTrimID { get; set; }

        public long MinYear { get; set; }

        public long MaxYear { get; set; }

        public long MinPrice { get; set; }

        public long MaxPrice { get; set; }
    }
}