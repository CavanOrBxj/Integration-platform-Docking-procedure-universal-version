using System.Threading;
using System.Collections.Generic;
using System.Data;

namespace GRPlatForm
{
    public class SingletonInfo
    {
        private static SingletonInfo _singleton;


        public string Longitude;//经度
        public string Latitude;//纬度
        public string CurrentURL;//当前平台对县平台的url

        public string CheckEBMStatusFlag;

     //   public Dictionary<string, string> AreacodeDic;

        private SingletonInfo()                                                                 
        {
            Longitude = "";
            Latitude = "";
            CurrentURL = "";
            CheckEBMStatusFlag = "";
        }
        public static SingletonInfo GetInstance()
        {
            if (_singleton == null)
            {
                Interlocked.CompareExchange(ref _singleton, new SingletonInfo(), null);
            }
            return _singleton;
        }
    }
}