using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models.Custom
{
    public class CarView
    {
        public Car car { get; set; }

        public string makeName { get; set; }

        public string modelName { get; set; }

        public string vehicleTrimName { get; set; }

        public string currencySymbol { get; set; }

        public string transmissionName { get; set; }

        public string conditionName { get; set; }

        public string statusName { get; set; }
    }
}
