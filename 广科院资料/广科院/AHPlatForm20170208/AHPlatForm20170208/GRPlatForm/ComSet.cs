using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace GRPlatForm
{
    public class ComSet
    {
        public string PortName
        {
            set;
            get;
        }
        public int BaudRate
        {
            set;
            get;
        }
        public int DataBits
        {
            set;
            get;
        }
        public StopBits StopBits
        {
            set;
            get;
        }
        public Parity Parity
        {
            set;
            get;
        }
    }
}
