using Models.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class InventoryListView
    {
        public List<CarView> carViewList { get; set; }

        public SearchGridView searchGridView { get; set; }
    }
}