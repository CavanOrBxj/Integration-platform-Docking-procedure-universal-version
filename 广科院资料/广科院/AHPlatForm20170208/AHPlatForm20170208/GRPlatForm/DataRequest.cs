using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GRPlatForm
{
    [Serializable]
    public class DataRequest
    {
        public string QueryType;

        public string StartTime;

        public string EndTime;

        public string DeviceCategory;

        public string DeviceType;
    }


}
