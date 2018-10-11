using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GRPlatForm
{
    [Serializable]
    public class TermBRDReport
    {
        public string RPTStartTime;

        public string RPTEndTime;

        public Term Term;
    }

    public class Term
    {
        public string MsgID;

        public string DeviceID;

        public string BRDTime;

        public string ResultCode;

        public string ResultDesc;
    }
}
