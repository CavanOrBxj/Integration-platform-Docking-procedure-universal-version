using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace GRPlatForm
{
    [Serializable]
    public class EBMStateResponse
    {
        public string MsgID;

        public Coverage Coverage;
    }

    //[Serializable]
    //public class Coverage
    //{
    //    [XmlElement]
    //    public List<Area> Area;
    //}

    //public class Area
    //{
    //    public string AreaName;

    //    public string AreaCode;

    //}
}
