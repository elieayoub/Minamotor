using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models.Custom
{
    public class VehicleTrimView
    {
        public VehicleTrim vehicleTrim { get; set; }

        public string modelName { get; set; }

        public Guid makeID { get; set; }

        public string makeName { get; set; }
    }
}
