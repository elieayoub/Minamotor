using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models.Custom
{
    public class WebCarListView
    {
        public string carList { get; set; }

        public string listName { get; set; }

        public Guid listID { get; set; }
        
        public string makeName { get; set; }

        public string modelName { get; set; }

        public string vehicleTrimName { get; set; }

        public string currencySymbol { get; set; }

        public string transmissionName { get; set; }

        public string conditionName { get; set; }

        public string statusName { get; set; }

        public Guid carID { get; set; }

        public string smallImage { get; set; }

        public string largeImage { get; set; }

        public List<CarImage> carImage { get; set; }

        public int condition { get; set; }

        public int status { get; set; }

        public int transmission { get; set; }

        public int? year { get; set; }
    }
}
