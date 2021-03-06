﻿using Apache.NMS;
using GRPlatForm;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;


namespace GRPlatForm
{
    public partial class ServerForm : Form
    {
        private bool RealAudioFlag = false;
        //MQ
        private MQ m_mq = null;
        private static MQ m_mq_checkEBM = null;

        private IMessageConsumer m_consumer; //消费者

        private bool isConn = false; //是否已与MQ服务器正常连接


        Random rdMQFileName = new Random();
        object OMDRequestLock = new object();//OMDRequest业务锁
        public SendInfo send = new SendInfo();
        private string SEBDIDStatusFlag = "";
        private SendFileHttpPost postfile = new SendFileHttpPost();

        public static string sTarPathName = "";//全局变量
        public static string sTmptarFileName = "";//定义处理Tar包临时文件名

        Thread thTar = null;//解压回复线程
        Thread httpthread = null;//HTTP服务

        Thread thFeedBack = null;//回复状态线程

        Thread ccplayerthread = null;//播放CCPLAY线程

        Thread thBackup = null;//周期反馈线程

        private HttpServer httpServer = null;//HttpServer端//
        public static TarHelper tar = null;
        public static Object oLockFile = null;//文件操作锁
        private Object oLockTarFile = null;
        private Object oLockFeedBack = null;

        private Object oLockPlay = null;

        private List<string> xmlfilename = new List<string>();//获取Tar包里面的XML文件名列表（一个签名包，一个请求包）
        public static List<string> lRevFiles;
        private string sUrlAddress = string.Empty;//回复地址
        private bool bDeal = true;//线程处理是否停止处理
        private IniFiles serverini;
        //临时文件夹变量
        public string sSendTarName = "";//发送Tar包名字
        public static string sRevTarPath = "";//接收Tar包存放路径
        public static string sSendTarPath = "";//发送Tar包存放路径
        public static string sSourcePath = "";//需压缩文件路径
        public static string sUnTarPath = "";//Tar包解压缩路径
        public static string sAudioFilesFolder = "";//音频文件存放位置

        public string sServerIP = "";
        public string sServerPort = "";
        private IPAddress iServerIP;
        private int iServerPort = 0;

        public static string sZJPostUrlAddress = "";//总局接收地址
        //private string sYXPostUrlAddress = "";//永新接收地址
        public static mainForm mainFrm;
        //定时反馈执行结果
        List<string> lFeedBack;//反馈列表

        public static string strSourceAreaCode = "";
        public static string strSourceType = "";
        public static string strSourceName = "";
        public static string strSourceID = "";
        public static string strHBRONO = "";  //ini文件中配置的实体编号，与通用反馈中的SRC/EBEID对应，即本平台的ID

        public static string strHBAREACODE = "";  //2016-04-03 电科院区域码对应

        //同步返回处理临时文件夹路径
        public static string strBeUnTarFolder = "";//预处理解压缩
        public static string strBeSendFileMakeFolder = "";//生成XML文件路径
        //心跳包变量
        public static string sHeartSourceFilePath = string.Empty;

        //SRV状态包变量
        public static string SRVSSourceFilePath = string.Empty;
        //SRV信息包变量
        public static string SRVISourceFilePath = string.Empty;

        //平台状态包变量
        public static string TerraceSSourceFilePath = string.Empty;
        //平台信息包变量
        public static string TerradcISourceFilePath = string.Empty;
        //定时心跳
        public static string TimesHeartSourceFilePath = string.Empty;

        public static string sEBMStateResponsePath = string.Empty;
        private DateTime dtLinkTime = new DateTime();//用于判断平台连接状态
        private const int OnOffLineInterval = 300;//离线在线间隔
        /*2016-03-31*/
        private List<string> listAreaCode;  //2016-04-01
        // private string AreaCode;            //2016-04-01
        private string EMBCloseAreaCode = "";//关闭区域逻辑代码
        //private string strAreaFlag = "";     //区域标志, 1代表命令发送到本区域，2代表上一级，3代表上上一级

        private int iAudioDelayTime = 0;//文转语延迟时间
        private int iMediaDelayTime = 0;//音频延迟时间
        private string bCharToAudio = "";  //1文转语，2 音频播放 
        private EBD ebd;

        delegate void SetTextCallback(string text, Color color); //在界面上显示信息委托
        private string AudioFlag = "";//********音频文件是否立即播放标志：1：立即播放 2：根据下发时间播放
        private string TEST = "";//********音频文件是否处于测试状态：test:测试状态，即收到的TAR包内xml的开始、结束时间无论是否过期，开始时间+1，结束时间+30
        private string TextFirst = "";//********文转语是否处于优先级1：文转语优先 2：语音优先
        private string PlayType = "";

        //MQInfo
        private string MQUrl = "";
        private string CloudConsumer = "";
        private string CloudProducer = "";
        private string AudioCloudIP = "";

        //平台使用的PID序号
        private string FileAudioPIDID = "";
        private string ActulAudioCloudIP = "";
        private string TsCmdStoreID = "";//对应的TsCmdStore表中的ID

        public string m_strIP;
        public string m_Port;
        public string m_nAudioPID;
        public string m_nVedioPID;
        public string m_nVedioRat;
        public string m_nAuioRat;
        public string m_EBDID;
        public string m_EBMID;
        public string m_EBRID;
        public static string m_StreamPortURL;
        public static string m_UsbPwsSupport;
        public static string m_nAudioPIDID;
        public ccplayer ccplay;
      //  public static string m_AreaCode;
        public static string m_ccplayURL;

        public static bool MQStartFlag = false;
        //EBM是否人工审核
        private bool EBMVerifyState = false;

        //直播流播放启用ccplayer倒计时
        DateTime ccplayerStopTime = DateTime.Now.AddSeconds(-50);

        System.Timers.Timer t = new System.Timers.Timer(30000);//心跳
        System.Timers.Timer tSrvState = new System.Timers.Timer(600000); //终端状态
        System.Timers.Timer tTerraceState = new System.Timers.Timer(30000); //平台状态
        System.Timers.Timer tSrvInfo = new System.Timers.Timer(180000);  //终端信息
        System.Timers.Timer tTerraceInfrom = new System.Timers.Timer(180000);  //平台信息
        //System.Timers.Timer InfromActiveTime = new System.Timers.Timer(30000); //暂不使用
        System.Timers.Timer Tccplayer = new System.Timers.Timer(1000);
        private int NUMInfrom = 0;
        //MQ指令集合
        private List<Property> m_lstProperty = new List<Property>();
        UserInfo MQUserInfo = new UserInfo();//MQ指令用户信息

       private static FTPHelper ftphelper;

        [DllImport("TTSDLL.dll", EntryPoint = "TTSConvertOut", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void TTSConvertOut([In()] [MarshalAs(UnmanagedType.LPStr)]string szPath, [In()][MarshalAs(UnmanagedType.LPStr)] string szContent);

        public ServerForm()
        {
            try
            {
                InitializeComponent();
                serverini = new IniFiles(@Application.StartupPath + "\\Config.ini");
                dtLinkTime = DateTime.Now.AddSeconds(-1 - OnOffLineInterval);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "PlayM");
            }
        }


        public  void OnlineCheck(bool state)
        {
            this.Invoke((EventHandler)delegate
            {
                this.Text = "在线";
                dtLinkTime = DateTime.Now;//刷新时间
            });

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (btnStart.Text == "启动服务")
            {
                btnStart.Text = "停止服务";
                txtServerPort.Enabled = false;
                if (MQStartFlag)
                    FindUserInfo("admin");
            }
            else
            {
                #region 停止服务
                try
                {
                    if (thTar != null)
                    {
                        thTar.Abort();
                    }
                    if (thFeedBack != null)
                    {
                        thFeedBack.Abort();
                    }
                    if (httpthread != null)
                    {
                        httpthread.Abort();
                        httpthread = null;
                    }
                    if (thBackup != null)
                    {
                        thBackup.Abort();
                        thBackup = null;
                    }
                    httpServer.StopListen();

                    //文转语Stop()
                    MQDLL.StopActiveMQ();
                }
                catch (Exception em)
                {
                    Log.Instance.LogWrite("停止线程错误：" + em.Message);
                }
                btnStart.Text = "启动服务";
                txtServerPort.Enabled = true;

                tTerraceInfrom.Enabled = false;
                tSrvInfo.Enabled = false;
                tTerraceState.Enabled = false;
                tSrvState.Enabled = false;
                t.Enabled = false;

                return;
                #endregion End
            }
            if (txtServerPort.Text.Trim() != "")
            {
                if (int.TryParse(txtServerPort.Text, out iServerPort))
                {
                    if (iServerPort < 1 || iServerPort > 65535)
                    {
                        MessageBox.Show("无效的端口号，请重新输入！");
                        txtServerPort.Focus();
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("非端口号，请重新输入！");
                    txtServerPort.Focus();
                    return;
                }
            }
            else
            {
                MessageBox.Show("服务端口号不能为空！");
                txtServerPort.Focus();
                return;
            }
            bDeal = true;//解析开关
            try
            {
                IPAddress[] ipArr;
                ipArr = Dns.GetHostAddresses(Dns.GetHostName());
                if (!ipArr.Contains(iServerIP))
                {
                    MessageBox.Show("IP设置错误，请重新设置后运行服务！");
                    return;
                }
                httpServer = new HttpServer(iServerIP, iServerPort);
            }
            catch (Exception es)
            {
                MessageBox.Show("可能端口已经使用中，请重新分配端口：" + es.Message);
                return;
            }

            httpthread = new Thread(new ThreadStart(httpServer.listen));
            httpthread.IsBackground = true;
            httpthread.Name = "HttpServer服务";
            httpthread.Start();
            //=================
            thTar = new Thread(DealTar);
            thTar.IsBackground = true;
            thTar.Name = "解压回复线程";
            thTar.Start();
            //=================
            thFeedBack = new Thread(FeedBackDeal);
            thFeedBack.IsBackground = true;
            thFeedBack.Name = "处理反馈线程";
            thFeedBack.Start();
            //=================
            //thBackup = new Thread(AnswerBackUP);
            //thBackup.IsBackground = true;
            //thBackup.Name = "周期状态信息反馈";
            //thBackup.Start();

            ccplayerthread = new Thread(CPPPlayerThread);
            ccplayerthread.Start();
        }

        private void ServerForm_Load(object sender, EventArgs e)  //页面参数初始化
        {
            bDeal = true;//解析开关
            oLockFile = new Object();//文件操作锁
            oLockTarFile = new object();
            oLockFeedBack = new object();//回复处理锁
            oLockPlay = new object();
            tar = new TarHelper();
            strSourceAreaCode = serverini.ReadValue("INFOSET", "SourceAreaCode");
            strSourceID = serverini.ReadValue("INFOSET", "SourceID");
            strSourceName = serverini.ReadValue("INFOSET", "SourceName");
            strSourceType = serverini.ReadValue("INFOSET", "SourceType");
            strHBRONO = serverini.ReadValue("INFOSET", "HBRONO");  //实体编号
            strHBAREACODE = serverini.ReadValue("INFOSET", "HBAREACODE");
            AudioFlag = serverini.ReadValue("INFOSET", "AudioFlag"); //********音频文件是否立即播放标志：1：立即播放 2：根据下发时间播放
            TEST = serverini.ReadValue("INFOSET", "TEST");//********音频文件是否处于测试状态：test:测试状态，即收到的TAR包内xml的开始、结束时间无论是否过期，开始时间+1，结束时间+30
            TextFirst = serverini.ReadValue("INFOSET", "TextFirst");//********文转语是否处于优先级1：文转语优先 2：语音优先
            PlayType = serverini.ReadValue("INFOSET", "PlayType");//********1:推流播放 2:平台播放
            ccplay = new ccplayer();

            m_strIP = serverini.ReadValue("CCPLAY", "ccplay_strIP");
            m_Port = serverini.ReadValue("CCPLAY", "ccplay_Port");
            m_nAudioPID = serverini.ReadValue("CCPLAY", "ccplay_AudioPID");
            m_nVedioPID = serverini.ReadValue("CCPLAY", "ccplay_VedioPID");
            m_nVedioRat = serverini.ReadValue("CCPLAY", "ccplay_VedioRat");
            m_nAuioRat = serverini.ReadValue("CCPLAY", "ccplay_AuioRat");

            m_nAudioPIDID = serverini.ReadValue("CCPLAY", "ccplay_AudioPIDID");

            m_StreamPortURL = serverini.ReadValue("StreamPortURL", "URL");


         //   m_AreaCode = serverini.ReadValue("AREA", "AreaCode");  注释于20181010

            MQStartFlag = serverini.ReadValue("MQInfo", "IsStartFlag").ToString() == "1" ? true : false;//判断是否启用MQ

          //  MessageBox.Show("IsStartFlag:"+serverini.ReadValue("MQInfo", "IsStartFlag").ToString());
            MQUrl = serverini.ReadValue("MQInfo", "ServerUrl");
            CloudConsumer = serverini.ReadValue("MQInfo", "CloudConsumer");
            CloudProducer = serverini.ReadValue("MQInfo", "CloudProducer");
            AudioCloudIP = serverini.ReadValue("MQInfo", "AudioCloudIP");

            FileAudioPIDID = serverini.ReadValue("CCPLAY", "ccplay_FileAuioRat");
            ActulAudioCloudIP = serverini.ReadValue("CCPLAY", "ccplay_AudioPIDID");
            EBMVerifyState = serverini.ReadValue("EBD", "EBMState").ToString() == "False" ? false : true;

            SingletonInfo.GetInstance().Latitude= serverini.ReadValue("LocationInfo", "Latitude");
            SingletonInfo.GetInstance().Longitude = serverini.ReadValue("LocationInfo", "Longitude");

            SingletonInfo.GetInstance().CurrentURL= serverini.ReadValue("platformURL", "URL");

            if (EBMVerifyState)
            {
                btn_Verify.Text = "自动审核状态";
                SingletonInfo.GetInstance().CheckEBMStatusFlag = "1";
            }
            else
            {
                btn_Verify.Text = "人工审核状态";
                SingletonInfo.GetInstance().CheckEBMStatusFlag = "0";
            }

            listAreaCode = new List<string>();  //2016-04-12
            try
            {
                iAudioDelayTime = int.Parse(serverini.ReadValue("INFOSET", "AudioDelayTime"));
            }
            catch
            {
                iAudioDelayTime = 1000;
            }
            try
            {
                iMediaDelayTime = int.Parse(serverini.ReadValue("INFOSET", "MediaDelayTime"));
            }
            catch
            {
                iMediaDelayTime = 1000;
            }
            /* 2016-04-03 */

            mainFrm = (this.ParentForm as mainForm);
            lRevFiles = new List<string>();
            lFeedBack = new List<string>();//反馈列表
            #region 设置处理文件夹路径Tar包存放文件夹路径
            try
            {
                //接收TAR包存放路径
                sRevTarPath = serverini.ReadValue("FolderSet", "RevTarFolder");
                if (!Directory.Exists(sRevTarPath))
                {
                    Directory.CreateDirectory(sRevTarPath);//不存在该路径就创建
                }
                sTarPathName = sRevTarPath + "\\revebm.tar";//存放接收Tar包的路径及文件名。

                //接收到的Tar包解压存放路径
                sUnTarPath = serverini.ReadValue("FolderSet", "UnTarFolder");
                if (!Directory.Exists(sUnTarPath))
                {
                    Directory.CreateDirectory(sUnTarPath);//不存在该路径就创建
                }
                //生成的需发送的XML文件路径
                sSourcePath = serverini.ReadValue("FolderSet", "XmlBuildFolder");
                if (!Directory.Exists(sSourcePath))
                {
                    Directory.CreateDirectory(sSourcePath);//
                }
                //生成的TAR包，将要被发送的位置
                sSendTarPath = serverini.ReadValue("FolderSet", "SndTarFolder");
                if (!Directory.Exists(sSendTarPath))
                {
                    Directory.CreateDirectory(sSendTarPath);
                }
                sAudioFilesFolder = serverini.ReadValue("FolderSet", "AudioFileFolder");
                if (!Directory.Exists(sAudioFilesFolder))
                {
                    Directory.CreateDirectory(sAudioFilesFolder);
                }
                sHeartSourceFilePath = @Application.StartupPath + "\\HeartBeat";
                if (!Directory.Exists(sHeartSourceFilePath))
                {
                    Directory.CreateDirectory(sHeartSourceFilePath);
                }
                SRVSSourceFilePath = @Application.StartupPath + "\\SrvStateBeat";
                if (!Directory.Exists(SRVSSourceFilePath))
                {
                    Directory.CreateDirectory(SRVSSourceFilePath);
                }
                SRVISourceFilePath = @Application.StartupPath + "\\SrvInfromBeat";
                if (!Directory.Exists(SRVISourceFilePath))
                {
                    Directory.CreateDirectory(SRVISourceFilePath);
                }
                TerraceSSourceFilePath = @Application.StartupPath + "\\TerraceStateBeat";
                if (!Directory.Exists(TerraceSSourceFilePath))
                {
                    Directory.CreateDirectory(TerraceSSourceFilePath);
                }
                TerradcISourceFilePath = @Application.StartupPath + "\\TerracdInfromBeat";
                if (!Directory.Exists(TerradcISourceFilePath))
                {
                    Directory.CreateDirectory(TerradcISourceFilePath);
                }
                TimesHeartSourceFilePath = @Application.StartupPath + "\\TerracdInfromBeat";
                if (!Directory.Exists(TimesHeartSourceFilePath))
                {
                    Directory.CreateDirectory(TimesHeartSourceFilePath);
                }
                //反馈应急消息播发状态
                sEBMStateResponsePath = @Application.StartupPath + "\\EBMStateResponse";
                if (!Directory.Exists(sEBMStateResponsePath))
                {
                    Directory.CreateDirectory(sEBMStateResponsePath);
                }
                //预处理文件夹
                strBeUnTarFolder = serverini.ReadValue("FolderSet", "BeUnTarFolder");
                if (!Directory.Exists(strBeUnTarFolder))
                {
                    Directory.CreateDirectory(strBeUnTarFolder);
                }
                strBeSendFileMakeFolder = serverini.ReadValue("FolderSet", "BeXmlFileMakeFolder");
                if (!Directory.Exists(strBeSendFileMakeFolder))
                {
                    Directory.CreateDirectory(strBeSendFileMakeFolder);
                }
                //预处理文件夹
                if (strBeUnTarFolder == "" || strBeSendFileMakeFolder == "")
                {
                    MessageBox.Show("预处理文件夹路径不能为空，请设置好路径！");
                    this.Close();
                }

                if (sRevTarPath == "" || sSendTarPath == "" || sSourcePath == "" || sUnTarPath == "")
                {
                    MessageBox.Show("文件夹路径不能为空，请设置好路径！");
                    this.Close();
                }
            }
            catch (Exception em)
            {
                MessageBox.Show("文件夹设置错误，请重新：" + em.Message);
                this.Close();
            }
            #endregion 文件夹路径设置END

            sServerIP = serverini.ReadValue("INFOSET", "ServerIP");
            txtServerPort.Text = serverini.ReadValue("INFOSET", "ServerPort");
            if (sServerIP != "")
            {
                if (!IPAddress.TryParse(sServerIP, out iServerIP))
                {
                    MessageBox.Show("非有效的IP地址，关闭服务重新配置IP后启动！");
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("服务IP不能为空，关闭服务重新配置IP后启动！");
                this.Close();
            }

            sZJPostUrlAddress = serverini.ReadValue("INFOSET", "BJURL");
            //sYXPostUrlAddress = serverini.ReadValue("INFOSET", "YXURL");
            if (sZJPostUrlAddress == "")
            {
                MessageBox.Show("回馈地址不能为空，请重新设置！");
                this.Close();
            }
            this.Text = "离线";
            if (tim_ClearMemory.Enabled == false)
            {
                tim_ClearMemory.Enabled = true;
            }

            t.Elapsed += new System.Timers.ElapsedEventHandler(HeartUP);
            t.AutoReset = true;

            tSrvState.Elapsed += new System.Timers.ElapsedEventHandler(SrvStateUP);
            tSrvState.AutoReset = true;

            tTerraceState.Elapsed += new System.Timers.ElapsedEventHandler(TerraceStateUP);
            tTerraceState.AutoReset = true;

            tSrvInfo.Elapsed += new System.Timers.ElapsedEventHandler(SrvInfromUP);
            tSrvInfo.AutoReset = true;

            tTerraceInfrom.Elapsed += new System.Timers.ElapsedEventHandler(TerraceInfrom);
            tTerraceInfrom.AutoReset = true;

            Tccplayer.Elapsed += new System.Timers.ElapsedEventHandler(TimerCcplayer);
            Tccplayer.AutoReset = true;

            //信息主动上报
            //InfromActiveTime.Elapsed += new System.Timers.ElapsedEventHandler(InfromActive);
            //InfromActiveTime.AutoReset = true;

            //   InitAeracodeDic();

            ConnectMQServer();
            InitFTPServer();
        }


        private void InfromActive(object source, System.Timers.ElapsedEventArgs e)
        {
            NUMInfrom++;
            if (NUMInfrom > 4)
            {
                NUMInfrom = 1;
            }

            //ServerForm.DeleteFolder(ServerForm.strCurrency);//删除原有XML发送文件的文件夹下的XML
            #region 运维请求反馈
            string strOMDType = ReturnInfrom(NUMInfrom);//
            if (strOMDType != "")
            {
                try
                {
                    XmlDocument xmlStateDoc = new XmlDocument();
                    responseXML rState = new responseXML();
                    rState.SourceAreaCode = ServerForm.strSourceAreaCode;
                    rState.SourceType = ServerForm.strSourceType;
                    rState.SourceName = ServerForm.strSourceName;
                    rState.SourceID = ServerForm.strSourceID;
                    rState.sHBRONO = ServerForm.strHBRONO;
                    Random rdState = new Random();
                    string frdStateName = "10" + rState.sHBRONO + "0000000000000" + rdState.Next(100, 999).ToString();
                    string xmlEBMStateFileName = "\\EBDB_" + frdStateName + ".xml";
                    List<Device> lDev = new List<Device>();
                    lock (OMDRequestLock)
                    {
                        TarOMRequest(xmlStateDoc, rState, strOMDType, frdStateName, xmlEBMStateFileName, lDev);
                    }
                }
                catch (Exception h)
                {
                    Log.Instance.LogWrite("错误510行:" + h.Message);
                }
            }
            #endregion End
        }

        /// <summary>
        /// 随机生成一个要返回的业务类型
        /// </summary>
        /// <param name="Num"></param>
        /// <returns></returns>
        private string ReturnInfrom(int Num)
        {
            switch (Num)
            {
                case 1:
                    return "EBRPSInfo";
                case 2:
                    SetText("EBRDTInfo    NO:6", Color.Orange);
                    DateTime INow = DateTime.Now;
                    XmlDocument xmlHeartDoc = new XmlDocument();
                    responseXML rHeart = new responseXML();
                    rHeart.SourceAreaCode = strSourceAreaCode;
                    rHeart.SourceType = strSourceType;
                    rHeart.SourceName = strSourceName;
                    rHeart.SourceID = strSourceID;
                    rHeart.sHBRONO = strHBRONO;
                    string MediaSql = "";
                    string strSRV_ID = "";
                    string strSRV_CODE = "";
                    ServerForm.DeleteFolder(sHeartSourceFilePath);//删除原有XML发送文件的文件夹下的XML
                    string frdStateName = "";
                    List<Device> lDev = new List<Device>();
                    try
                    {
                        MediaSql = "select top(99) SRV_ID,SRV_CODE,SRV_GOOGLE from SRV";
                        DataTable dtMedia = mainForm.dba.getQueryInfoBySQL(MediaSql);
                        if (dtMedia != null && dtMedia.Rows.Count > 0)
                        {
                            for (int idtM = 0; idtM < dtMedia.Rows.Count; idtM++)
                            {
                                string TrLL = dtMedia.Rows[idtM][2].ToString();
                                Device DV = new Device();
                                if (idtM < 10)
                                {
                                    DV.DeviceID = "0" + idtM;
                                }
                                else { DV.DeviceID = idtM.ToString(); }
                                strSRV_CODE = dtMedia.Rows[idtM][1].ToString();
                                DV.DeviceID = strSRV_ID;
                                DV.DeviceName = strSRV_ID;
                                if (TrLL != "")
                                {
                                    string[] str = TrLL.Split(',');
                                    if (str.Length >= 2)
                                    {
                                        DV.Longitude = str[1];
                                        DV.Latitude = str[0];
                                    }
                                    else
                                    {
                                        DV.Longitude = SingletonInfo.GetInstance().Longitude;
                                        DV.Latitude = SingletonInfo.GetInstance().Latitude;
                                    }
                                }
                                else
                                {
                                    DV.Longitude = SingletonInfo.GetInstance().Longitude;
                                    DV.Latitude = SingletonInfo.GetInstance().Latitude;
                                }
                                lDev.Add(DV);
                            }
                            Random rdState = new Random();
                            frdStateName = "10" + rHeart.sHBRONO + "0000000000000" + rdState.Next(100, 999).ToString();
                            string xmlEBMStateFileName = "\\EBDB_" + frdStateName + ".xml";
                            xmlHeartDoc = rHeart.DeviceInfoResponse(lDev, frdStateName);
                            CreateXML(xmlHeartDoc, sHeartSourceFilePath + xmlEBMStateFileName);
                            ServerForm.mainFrm.GenerateSignatureFile(sHeartSourceFilePath, frdStateName);
                            ServerForm.tar.CreatTar(sHeartSourceFilePath, ServerForm.sSendTarPath, frdStateName);//使用新TAR
                            string sHeartBeatTarName = sSendTarPath + "\\" + "EBDT_" + frdStateName + ".tar";
                            send.address = sZJPostUrlAddress;
                            send.fileNamePath = sHeartBeatTarName;
                            postfile.UploadFilesByPostThread(send);
                        }
                    }
                    catch
                    {
                    }
                    Console.WriteLine((INow - DateTime.Now));
                    return "";
                case 3:
                    return "EBRPSState";
                case 4:
                    SetText("EBRDTState    NO:9", Color.Orange);
                    DateTime SNow = DateTime.Now;
                    XmlDocument xmlHeartDocS = new XmlDocument();
                    responseXML rHeartS = new responseXML();
                    rHeartS.SourceAreaCode = strSourceAreaCode;
                    rHeartS.SourceType = strSourceType;
                    rHeartS.SourceName = strSourceName;
                    rHeartS.SourceID = strSourceID;
                    rHeartS.sHBRONO = strHBRONO;
                    string MediaSqlS = "";
                    string strSRV_IDS = "";
                    string strSRV_CODES = "";
                    ServerForm.DeleteFolder(sHeartSourceFilePath);//删除原有XML发送文件的文件夹下的XML
                    string frdStateNameS = "";
                    List<Device> lDevS = new List<Device>();
                    try
                    {
                        MediaSqlS = "select top(99) SRV_ID,SRV_CODE,SRV_GOOGLE from SRV";
                        DataTable dtMedia = mainForm.dba.getQueryInfoBySQL(MediaSqlS);
                        if (dtMedia != null && dtMedia.Rows.Count > 0)
                        {
                            for (int idtM = 0; idtM < dtMedia.Rows.Count; idtM++)
                            {
                                string TrLL = dtMedia.Rows[idtM][2].ToString();
                                Device DV = new Device();
                                strSRV_IDS = dtMedia.Rows[idtM][0].ToString().Trim();
                                strSRV_CODES = dtMedia.Rows[idtM][1].ToString();
                                if (idtM < 10)
                                {
                                    DV.DeviceID = "0" + idtM;
                                }
                                else { DV.DeviceID = idtM.ToString(); }
                                DV.DeviceName = strSRV_IDS;
                                if (TrLL != "")
                                {
                                    string[] str = TrLL.Split(',');
                                    if (str.Length >= 2)
                                    {
                                        DV.Longitude = str[1];
                                        DV.Latitude = str[0];
                                    }
                                    else
                                    {
                                        DV.Longitude = SingletonInfo.GetInstance().Longitude;
                                        DV.Latitude = SingletonInfo.GetInstance().Latitude;
                                    }
                                }
                                else
                                {
                                    DV.Longitude = SingletonInfo.GetInstance().Longitude;
                                    DV.Latitude = SingletonInfo.GetInstance().Latitude;
                                }
                                lDevS.Add(DV);
                            }
                            Random rdState = new Random();
                            frdStateNameS = "10" + rHeartS.sHBRONO + "0000000000000" + rdState.Next(100, 999).ToString();
                            string xmlEBMStateFileName = "\\EBDB_" + frdStateNameS + ".xml";

                            xmlHeartDocS = rHeartS.DeviceStateResponse(lDevS, frdStateNameS);
                            CreateXML(xmlHeartDocS, sHeartSourceFilePath + xmlEBMStateFileName);
                            ServerForm.mainFrm.GenerateSignatureFile(sHeartSourceFilePath, frdStateNameS);
                            ServerForm.tar.CreatTar(sHeartSourceFilePath, ServerForm.sSendTarPath, frdStateNameS);//使用新TAR
                            string sHeartBeatTarName = sSendTarPath + "\\" + "EBDT_" + frdStateNameS + ".tar";
                            send.address = sZJPostUrlAddress;
                            send.fileNamePath = sHeartBeatTarName;
                            postfile.UploadFilesByPostThread(send);
                        }

                    }
                    catch
                    {
                    }
                    Console.WriteLine((SNow - DateTime.Now));
                    return "";
                default:
                    return "EBRPSInfo";
            }
        }

        public void CPPPlayerThread()
        {
            try
            {
                while (true)
                {
                    if (ccplay.m_bPlayFlag)
                    {
                        ccplay.init("", m_ccplayURL, m_strIP, m_Port, "pipe", "EVENT", m_nAudioPID, m_nVedioPID, m_nVedioRat, m_nAuioRat);
                        ccplay.CreatePipeandEvent("pipename", "eventname");
                        ccplay.CreateCPPPlayer();
                        Thread.Sleep(2000);
                        ccplay.StopCPPPlayer();
                        //string strSql = "delete  from PLAYRECORD";
                        //mainForm.dba.UpdateDbBySQL(strSql);
                        ccplay.m_bPlayFlag = false;
                    }
                    Thread.Sleep(500);
                }

            }
            catch (Exception es)
            {
                Log.Instance.LogWrite(es.Message);
            }
        }

        /// <summary>
        /// 从HttpServer得到的Tar包获取数据并解析；以后tar包弄成List处理
        /// </summary>
        private void DealTar()
        {
            List<string> lDealTarFiles = new List<string>();
            List<string> AudioFileListTmp = new List<string>();//收集的音频文件列表
            List<string> AudioFileList = new List<string>();//收集的音频文件列表

            while (bDeal)
            {
                //没有Tar包不处理
                if (lRevFiles.Count == 0)
                {
                    Thread.Sleep(1000);
                    continue;
                }
                else
                {
                    lock (oLockTarFile)
                    {
                        if (lRevFiles.Count > 0)
                        {
                            lDealTarFiles.AddRange(lRevFiles);
                            lRevFiles.Clear();
                        }
                    }
                }
                this.Invoke((EventHandler)delegate
                {
                    this.Text = "在线";
                    dtLinkTime = DateTime.Now;//刷新时间
                });
                #region 处理Tar包
                if (lDealTarFiles.Count == 0)
                {
                    continue;//没有处理文件包不处理
                }
                try
                {
                    while (lDealTarFiles.Count > 0)
                    {
                        SetText("解压文件：" + lDealTarFiles[0].ToString(), Color.Green);
                        try
                        {
                            #region 解压
                            if (File.Exists(lDealTarFiles[0]))
                            {
                                try
                                {
                                    DeleteFolder(sUnTarPath);
                                    tar.UnpackTarFiles(lDealTarFiles[0], sUnTarPath);
                                    //把压缩包解压到专门存放接收到的XML文件的文件夹下
                                    SetText("解压文件：" + lDealTarFiles[0].ToString() + "成功", Color.Green);
                                }
                                catch (Exception exa)
                                {
                                    SetText("删除解压文件夹：" + sUnTarPath + "文件失败!错误信息：" + exa.Message, Color.Red);
                                }
                            }
                            #endregion 解压
                        }
                        catch (Exception ex)
                        {
                            Log.Instance.LogWrite("解压出错：" + ex.Message);
                        }
                        //处理XML文件
                        try
                        {
                            string[] xmlfilenames = Directory.GetFiles(sUnTarPath, "*.xml");//从解压XML文件夹下获取解压的XML文件名
                            string sTmpFile = string.Empty;
                            string sAnalysisFileName = "";
                            string sSignFileName = "";

                            for (int i = 0; i < xmlfilenames.Length; i++)
                            {
                                sTmpFile = Path.GetFileName(xmlfilenames[i]);
                                if (sTmpFile.ToUpper().IndexOf("EBDB") > -1 && sTmpFile.ToUpper().IndexOf("EBDS_EBDB") < 0)
                                {
                                    sAnalysisFileName = xmlfilenames[i];
                                }
                                else if (sTmpFile.ToUpper().IndexOf("EBDS_EBDB") > -1)//签名文件
                                {
                                    sSignFileName = xmlfilenames[i];//签名文件
                                }
                            }
                            DeleteFolder(sSourcePath);//删除原有XML发送文件的文件夹下的XML
                            if (sSignFileName == "")
                            {
                            }
                            else
                            {
                                //读取xml中的文件,转换为byte字节
                                byte[] xmlArray = File.ReadAllBytes(sAnalysisFileName);

                                #region 签名处理
                                Console.WriteLine("开始验证签名文件!");
                                // SetText(string.Format("开始验证签名文件!", Color.Red);
                                using (FileStream SignFs = new FileStream(sSignFileName, FileMode.Open))
                                {
                                    StreamReader signsr = new StreamReader(SignFs, System.Text.Encoding.UTF8);
                                    string xmlsign = signsr.ReadToEnd();
                                    signsr.Close();
                                    responseXML signrp = new responseXML();//签名回复
                                    XmlDocument xmlSignDoc = new XmlDocument();
                                    try
                                    {
                                        int nDeviceHandle = (int)mainFrm.phDeviceHandle;
                                        xmlsign = XmlSerialize.ReplaceLowOrderASCIICharacters(xmlsign);
                                        xmlsign = XmlSerialize.GetLowOrderASCIICharacters(xmlsign);
                                        Signature sign = XmlSerialize.DeserializeXML<Signature>(xmlsign);
                                        //xmlSignDoc = signrp.SignResponse(sign.RefEBDID, "OK");
                                        xmlsign = XmlSerialize.ReplaceLowOrderASCIICharacters(xmlsign);
                                        xmlsign = XmlSerialize.GetLowOrderASCIICharacters(xmlsign);
                                        string PucStr = sign.SignatureValue;
                                        byte[] pucsingVi = Encoding.UTF8.GetBytes(sign.SignatureValue);

                                        int PlatformVerifySignatureresule = mainFrm.usb.PlatformVerifySignature(nDeviceHandle, 1, xmlArray, xmlArray.Length, pucsingVi);
                                        if (PlatformVerifySignatureresule != 0)
                                        {
                                            lDealTarFiles.RemoveAt(0);//无论是否成功，都移除
                                            SetText(string.Format("签名不通过：{0} 验名值: {1}", DateTime.Now, PlatformVerifySignatureresule), Color.Red);
                                            continue;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Log.Instance.LogWrite("签名文件错误：" + ex.Message);
                                        // xmlSignDoc = signrp.SignResponse("", "Error");
                                    }
                                    //xmlSignDoc.Save(sSourcePath + "\\EBDSign.xml");
                                }
                                SetText("结束验证签名文件！：" + DateTime.Now.ToString(), Color.Red);
                                Console.WriteLine("结束验证签名文件！");
                                #endregion End
                            }

                            if (sAnalysisFileName != "")
                            {
                                using (FileStream fs = new FileStream(sAnalysisFileName, FileMode.Open))
                                {
                                    StreamReader sr = new StreamReader(fs, System.Text.Encoding.UTF8);
                                    String xmlInfo = sr.ReadToEnd();
                                    xmlInfo = xmlInfo.Replace("xmlns:xs", "xmlns");
                                    sr.Close();
                                    xmlInfo = XmlSerialize.ReplaceLowOrderASCIICharacters(xmlInfo);
                                    xmlInfo = XmlSerialize.GetLowOrderASCIICharacters(xmlInfo);
                                    ebd = XmlSerialize.DeserializeXML<EBD>(xmlInfo);
                                    sUrlAddress = sZJPostUrlAddress;  //异步反馈的地址
                                    #region 根据EBD类型处理XML文件
                                    switch (ebd.EBDType)
                                    {
                                        case "EBM":
                                            #region 业务处理
                                            string sqlstr = "";
                                            string strMsgType = ebd.EBM.MsgBasicInfo.MsgType; //播发类型
                                            string strAuxiliaryType = "";
                                            if (ebd.EBM.MsgContent != null)
                                            {
                                                if (ebd.EBM.MsgContent.Auxiliary != null)
                                                {
                                                    strAuxiliaryType = ebd.EBM.MsgContent.Auxiliary.AuxiliaryType; //实时流播发
                                                    if (strAuxiliaryType == "61")
                                                    {
                                                        PlayType = "1";
                                                    }
                                                    else { PlayType = "2"; }
                                                }
                                                else
                                                {
                                                    ebd.EBM.MsgContent.Auxiliary = new Auxiliary();
                                                    ebd.EBM.MsgContent.Auxiliary.AuxiliaryType = "3";
                                                    strAuxiliaryType = "3";
                                                    ebd.EBM.MsgContent.Auxiliary.AuxiliaryDesc = "文本转语";
                                                    PlayType = "1";
                                                }
                                            }
                                            try
                                            {
                                                lock (oLockFile)
                                                {
                                                    if (ebd.EBM.EBMID != null)
                                                    {
                                                        lFeedBack.Add(ebd.EBM.EBMID);//加入反馈列表
                                                    }
                                                }
                                            }
                                            catch (Exception en)
                                            {
                                                Log.Instance.LogWrite("错误480行：" + en.Message);
                                            }
                                            if ((EBMVerifyState || RealAudioFlag) && strMsgType == "2")//实时流在播发时的停止
                                            {
                                                //推流播放，取消播放
                                                if (strMsgType == "2" && PlayType == "1")
                                                {
                                                    SetText("停止播发：" + DateTime.Now.ToString(), Color.Red);
                                                    string strSql = string.Format("update PLAYRECORD set PR_REC_STATUS = '{0}' where PR_SourceID='{1}'", "删除", TsCmdStoreID);
                                                    strSql += " update EBMInfo set EBMState=1 where SEBDID='" + SEBDIDStatusFlag + "' ";
                                                    strSql += "delete from InfoVlaue";
                                                    //string strSql = "update PLAYRECORD set PR_REC_STATUS = '删除'";
                                                    mainForm.dba.UpdateDbBySQL(strSql);
                                                    Tccplayer.Enabled = false;
                                                    ccplay.StopCPPPlayer2();
                                                    RealAudioFlag = false;//标记为已经执行
                                                    lDealTarFiles.RemoveAt(0);//无论是否成功，都移除
                                                    continue;
                                                }
                                                //本地播放，取消播放
                                                if (strMsgType == "2" && PlayType == "2")
                                                {
                                                    SetText("停止播发：" + DateTime.Now.ToString(), Color.Red);
                                                    string strSql = string.Format("update PLAYRECORD set PR_REC_STATUS = '{0}' where PR_SourceID='{1}'", "删除", TsCmdStoreID);
                                                    strSql += " update EBMInfo set EBMState=1 where SEBDID='" + SEBDIDStatusFlag + "' ";
                                                    strSql += "delete from InfoVlaue";
                                                    mainForm.dba.UpdateDbBySQL(strSql);
                                                    Tccplayer.Enabled = false;
                                                    RealAudioFlag = false;//标记为已经执行
                                                    lDealTarFiles.RemoveAt(0);//无论是否成功，都移除
                                                    continue;
                                                }
                                            }
                                            else if (!EBMVerifyState && !RealAudioFlag && strMsgType == "2")
                                            {
                                                ListViewItem listItem = new ListViewItem();
                                                listItem.Text = (list_PendingTask.Items.Count + 1).ToString();
                                                listItem.SubItems.Add(lDealTarFiles[0]);
                                                this.Invoke(new Action(() => { list_PendingTask.Items.Add(listItem); }));
                                                lDealTarFiles.RemoveAt(0);//无论是否成功，都移除
                                                continue;
                                            }
                                            #region AreaCode
                                            listAreaCode.Clear();
                                            if (!string.IsNullOrEmpty(ebd.EBM.MsgContent.AreaCode))
                                            {
                                                string[] AreaCode = ebd.EBM.MsgContent.AreaCode.Split(new char[] { ',' });
                                                for (int a = 0; a < AreaCode.Length; a++)
                                                {
                                                    string strTmpAddr = AreaCode[a];
                                                    int isheng = -1;  //省级
                                                    int ishi = -1;    //市级
                                                    int iIndex = -1;  //县及以下
                                                    string subStr = "";
                                                    subStr = strHBAREACODE.Substring(0, 2);  //省级编码
                                                    isheng = strTmpAddr.IndexOf(subStr);
                                                    subStr = strHBAREACODE.Substring(0, 4);  //市级编码
                                                    ishi = strTmpAddr.IndexOf(subStr);
                                                    iIndex = strTmpAddr.IndexOf(strHBAREACODE);  //是否是本区域
                                                    if ((isheng == 0) || (ishi == 0) || (iIndex == 0) || (strTmpAddr.Substring(2) == "0000000000") || (strTmpAddr.Substring(4) == "00000000"))//(strTmpAddr.Length != 14)
                                                    {
                                                        string strTmpAddrA = ReplaceToAA(strTmpAddr) + "AAAAAAAAAAAAAAAAAA";
                                                        // strTmpAddrA.PadRight(18, 'A');
                                                        strTmpAddrA = strTmpAddrA.Substring(4, 10);
                                                        strTmpAddrA = L_H(strTmpAddrA);
                                                        listAreaCode.Add(strTmpAddrA);
                                                    }
                                                    else
                                                    {
                                                        Log.Instance.LogWrite("非本区域地域码");
                                                    }
                                                }

                                                #endregion End

                                                #region 处理消息
                                                if (!string.IsNullOrEmpty(ebd.EBM.MsgContent.MsgDesc.Trim()))
                                                {
                                                    #region 处理应急内容
                                                    AudioFileListTmp.Clear();
                                                    AudioFileList.Clear();
                                                    string[] mp3files = Directory.GetFiles(sUnTarPath, "*.mp3");
                                                    AudioFileListTmp.AddRange(mp3files);
                                                    if (AudioFileListTmp.Count > 0)
                                                    {
                                                        #region 根据策略判断SingletonInfo.GetInstance().CheckEBMStatusFlag的值  "0"表示需要融合平台审核 "1"表示不需要融合平台审核
                                                        if (!EBMVerifyState)
                                                        {
                                                            SingletonInfo.GetInstance().CheckEBMStatusFlag = StrategyChenck(ebd);
                                                        }
                                                        else
                                                        {
                                                            SingletonInfo.GetInstance().CheckEBMStatusFlag = "1";
                                                        }
                                                        #endregion

                                                        #region  发送给融合平台关于审核信息
                                                        //待审核数据插入数据库
                                                        string lab_EBMType = "音频文件播发";

                                                        sqlstr = "insert into CheckEBMData(EBDID, EBDDID, CodeA, NameA, EBMID, SentTime,EBMStartTime,EBMEndTime, EBMTitle,EBMType,EBMDesc,EBMCode,EBMUrl,CheckStatus)" +
                    "values('" + ebd.SRC.EBRID + "','" + ebd.EBDID + "', '" + ebd.EBM.MsgBasicInfo.SenderCode + "', '" + ebd.EBM.MsgBasicInfo.SenderName + "','" + ebd.EBM.EBMID + "', '" + ebd.EBM.MsgBasicInfo.SentTime + "','" + ebd.EBM.MsgBasicInfo.StartTime + "','" + ebd.EBM.MsgBasicInfo.EndTime + "','" + ebd.EBM.MsgContent.MsgTitle + "','" + lab_EBMType + "','" + ebd.EBM.MsgContent.MsgDesc + "','" + ebd.EBM.MsgContent.AreaCode + "','" + ebd.EBM.MsgContent.Auxiliary.AuxiliaryDesc + "','" + SingletonInfo.GetInstance().CheckEBMStatusFlag + "') SELECT CAST(scope_identity() AS int)";
                                                        ebd.CheckEBMDataID = mainForm.dba.InsertDbBySQLRetID(sqlstr).ToString();

                                                        //MQ发送消息审核  
                                                        CheckEBMDataMQSend(ebd);
                                                        #endregion
                                                        
                                                        if (AudioFlag == "1" && PlayType == "1")//AudioFlag音频文件是否立即播放标志：1：立即播放 2：根据下发时间播放
                                                        {
                                                            string sDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); //ebd.EBM.MsgBasicInfo.StartTime;
                                                            string sEndDateTime = DateTime.Now.AddMinutes(5).ToString("yyyy-MM-dd HH:mm:ss"); //ebd.EBM.MsgBasicInfo.EndTime;
                                                                                                                                              //sDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//ebd.EBM.MsgBasicInfo.StartTime;
                                                                                                                                              //sEndDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); //ebd.EBM.MsgBasicInfo.EndTime;

                                                            string strPID = m_nAudioPIDID + "~0";
                                                            //string sORG_ID = m_AreaCode;//((int)mainForm.dba.getQueryResultBySQL(sqlstr)).ToString();
                                                            string sORG_ID = GetORG_ID(ebd.EBM.MsgContent.AreaCode);



                                                            sqlstr = "insert into TsCmdStore(TsCmd_Type, TsCmd_Mode, TsCmd_UserID, TsCmd_ValueID, TsCmd_Params, TsCmd_Date, TsCmd_Status,TsCmd_EndTime,TsCmd_Note)" +
                                                                    "values('音源播放', '区域', 1, " + sORG_ID + ", '" + strPID + "', '" + sDateTime + "', 0,'" + sEndDateTime + "'," + "'-1'" + ")";

                                                            int iback = mainForm.dba.getResultIDBySQL(sqlstr, "TsCmdStore");

                                                            // for (int i = 0; i < listAreaCode.Count; i++)
                                                            {
                                                                //string cmdOpen = "4C " + listAreaCode[i] + " B0 02 01 04";
                                                                string cmdOpen = "4C " + "AA AA AA AA 00" + " B0 02 01 04";
                                                                Log.Instance.LogWrite("立即播放音频应急开机：" + cmdOpen);
                                                                SetText("立即播放音频应急开机：" + cmdOpen + DateTime.Now.ToString(), Color.Blue);
                                                                string strsum = DataSum(cmdOpen);
                                                                cmdOpen = "FE FE FE " + cmdOpen + " " + strsum + " 16";
                                                                string strsql = "";
                                                                strsql = "insert into CommandPool(CMD_TIME,CMD_BODY,CMD_FLAG)" +
                                                                " VALUES('" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "','" + cmdOpen + "','" + '0' + "')";
                                                                mainForm.dba.UpdateOrInsertBySQL(strsql);
                                                            }
                                                            Thread.Sleep(6000);

                                                            for (int iLoopMedia = 0; iLoopMedia < AudioFileListTmp.Count; iLoopMedia++)
                                                            {
                                                                /*本地播放
                                                                //发送控制信号
                                                                /*推流播放*/
                                                                SetText("音频播放文件：" + AudioFileListTmp[iLoopMedia], Color.Red);
                                                                bCharToAudio = "2";

                                                                try
                                                                {
                                                                    ccplay.TsCmdStoreID = TsCmdStoreID;//PlayRecord停止的标示
                                                                    m_ccplayURL = "file:///" + AudioFileListTmp[iLoopMedia];
                                                                    if (ccplay.m_bPlayFlag == false)
                                                                    {
                                                                        ccplay.m_bPlayFlag = true;
                                                                    }
                                                                    else
                                                                    {
                                                                        ccplay.StopCPPPlayer2();
                                                                        Thread.Sleep(1000);
                                                                        ccplayerthread.Abort();
                                                                        Thread.Sleep(1000);
                                                                        ccplayerthread = new Thread(CPPPlayerThread);
                                                                        ccplayerthread.Start();
                                                                    }
                                                                }
                                                                catch (Exception es)
                                                                {
                                                                    Log.Instance.LogWrite(es.Message);
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else //EBM实时流播放
                                                    {

                                                        #region 根据策略判断SingletonInfo.GetInstance().CheckEBMStatusFlag的值  "0"表示需要融合平台审核 "1"表示不需要融合平台审核
                                                        if (!EBMVerifyState)
                                                        {
                                                            SingletonInfo.GetInstance().CheckEBMStatusFlag = StrategyChenck(ebd);
                                                        }
                                                        else
                                                        {
                                                            SingletonInfo.GetInstance().CheckEBMStatusFlag = "1";
                                                        }
                                                        #endregion

                                                        #region 发送给融合平台关于审核的信息
                                                        //待审核数据插入数据库
                                                        string lab_EBMType = "文本转语音播发";

                                                        sqlstr = "insert into CheckEBMData(EBDID, EBDDID, CodeA, NameA, EBMID, SentTime,EBMStartTime,EBMEndTime, EBMTitle,EBMType,EBMDesc,EBMCode,EBMUrl,CheckStatus)" +
                    "values('" + ebd.SRC.EBRID + "','" + ebd.EBDID + "', '" + ebd.EBM.MsgBasicInfo.SenderCode + "', '" + ebd.EBM.MsgBasicInfo.SenderName + "','" + ebd.EBM.EBMID + "', '" + ebd.EBM.MsgBasicInfo.SentTime + "','" + ebd.EBM.MsgBasicInfo.StartTime + "','" + ebd.EBM.MsgBasicInfo.EndTime + "','" + ebd.EBM.MsgContent.MsgTitle + "','" + lab_EBMType + "','" + ebd.EBM.MsgContent.MsgDesc + "','" + ebd.EBM.MsgContent.AreaCode + "','" + ebd.EBM.MsgContent.Auxiliary.AuxiliaryDesc + "','" + SingletonInfo.GetInstance().CheckEBMStatusFlag + "') SELECT CAST(scope_identity() AS int)";
                                                        ebd.CheckEBMDataID = mainForm.dba.InsertDbBySQLRetID(sqlstr).ToString();

                                                        //MQ发送消息审核  
                                                        CheckEBMDataMQSend(ebd);
                                                        #endregion

                                                        if (strAuxiliaryType != "61" && !EBMVerifyState && SingletonInfo.GetInstance().CheckEBMStatusFlag=="0")//20181102修改
                                                        {
                                                            ListViewItem listItem = new ListViewItem();
                                                            listItem.Text = (list_PendingTask.Items.Count + 1).ToString();
                                                            listItem.SubItems.Add(lDealTarFiles[0]);
                                                            this.Invoke(new Action(() => { list_PendingTask.Items.Add(listItem); }));
                                                            lDealTarFiles.RemoveAt(0);//无论是否成功，都移除
                                                            continue;
                                                        }
                                                        RealAudioFlag = true;
                                                        try
                                                        {

                                                            SetText("EBM开始时间: " + ebd.EBM.MsgBasicInfo.StartTime + "===>EBM结束时间: " + ebd.EBM.MsgBasicInfo.EndTime, Color.Orange);
                                                            DateTime EBStartTime = DateTime.Parse(ebd.EBM.MsgBasicInfo.StartTime).AddSeconds(2);
                                                            if (EBStartTime < DateTime.Now)
                                                            {
                                                                EBStartTime = DateTime.Now.AddSeconds(2);
                                                            }
                                                            string sStartTime = EBStartTime.ToString("yyyy-MM-dd HH:mm:ss"); //ebd.EBM.MsgBasicInfo.StartTime;
                                                            string sDateTime = EBStartTime.ToString("yyyy-MM-dd HH:mm:ss");//ebd.EBM.MsgBasicInfo.StartTime;
                                                            string sEndDateTime = ebd.EBM.MsgBasicInfo.EndTime;
                                                            ccplayerStopTime = DateTime.Parse(sEndDateTime);
                                                            if (TEST == "YES")
                                                            {
                                                                sStartTime = DateTime.Now.AddSeconds(2).ToString("yyyy-MM-dd HH:mm:ss");
                                                                sDateTime = DateTime.Now.AddSeconds(2).ToString("yyyy-MM-dd HH:mm:ss");
                                                                sEndDateTime = DateTime.Now.AddMinutes(5).ToString("yyyy-MM-dd HH:mm:ss");//ebd.EBM.MsgBasicInfo.EndTime;
                                                                ccplayerStopTime = DateTime.Now.AddMinutes(2);

                                                            }
                                                            SetText("实时流开始时间>>>>" + sStartTime + "----结束时间>>>" + ccplayerStopTime.ToString("yyyy-MM-dd HH:mm:ss") + "是否是TEST:" + TEST, Color.Orange);
                                                            string strPID = m_nAudioPIDID + "~1";
                                                            //  string sORG_ID = m_AreaCode;//((int)mainForm.dba.getQueryResultBySQL(sqlstr)).ToString();

                                                            string sORG_ID = GetORG_ID(ebd.EBM.MsgContent.AreaCode);
                                                            sqlstr = "insert into TsCmdStore(TsCmd_Type, TsCmd_Mode, TsCmd_UserID, TsCmd_ValueID, TsCmd_Params, TsCmd_Date, TsCmd_Status,TsCmd_EndTime,TsCmd_Note)" +
                                                                    "values('音源播放', '区域', 1, " + sORG_ID + ", '" + strPID + "', '" + sDateTime + "', 0,'" + sEndDateTime + "'," + "'-1'" + ")";

                                                            TsCmdStoreID = mainForm.dba.UpdateDbBySQLRetID(sqlstr).ToString();
                                                            SendMQOrder(2, strPID, TsCmdStoreID);//MQ发送
                                                            Thread.Sleep(500);
                                                            SetText("立即播放音频延时开始：" + DateTime.Now.ToString(), Color.Blue);
                                                            Thread.Sleep(iMediaDelayTime);//延迟10秒
                                                            Application.DoEvents();
                                                            SetText("立即播放音频开始：" + DateTime.Now.ToString(), Color.Blue);

                                                            string FileNameNum = "";
                                                            //文转语
                                                            if (strAuxiliaryType == "3")
                                                            {
                                                                FileNameNum = rdMQFileName.Next(00, 99).ToString();
                                                                string Message = ebd.EBM.MsgContent.MsgDesc;
                                                                SetText(Message, Color.Olive);
                                                                if (MQStartFlag)
                                                                    MQDLL.SendMessageMQ("PACKETTYPE~TTS|CONTENT~" + Message + "|FILE~" + FileNameNum + ".wav");
                                                            }
                                                            lock (oLockPlay)
                                                            {
                                                                if (strAuxiliaryType == "61") //实时流播发
                                                                {

                                                                    ccplay.TsCmdStoreID = TsCmdStoreID;//PlayRecord停止的标示
                                                                    m_ccplayURL = "udp://@" + m_StreamPortURL;
                                                                    if (ccplay.m_bPlayFlag == false)
                                                                    {
                                                                        ccplay.m_bPlayFlag = true;
                                                                        //--Tccplayer.Enabled = true;
                                                                    }
                                                                    else
                                                                    {
                                                                        ccplay.StopCPPPlayer2();
                                                                        Thread.Sleep(1000);
                                                                        ccplayerthread.Abort();
                                                                        Thread.Sleep(1000);
                                                                        ccplayerthread = new Thread(CPPPlayerThread);
                                                                        ccplayerthread.Start();
                                                                        //--Tccplayer.Enabled = true;
                                                                    }
                                                                }
                                                                else if (strAuxiliaryType == "3")
                                                                {
                                                                    Thread.Sleep(5000);
                                                                    ccplay.TsCmdStoreID = TsCmdStoreID;//PlayRecord停止的标示
                                                                    m_ccplayURL = AudioCloudIP + FileNameNum + ".wav";     //"udp://@" + m_StreamPortURL;
                                                                    if (ccplay.m_bPlayFlag == false)
                                                                    {
                                                                        ccplay.m_bPlayFlag = true;
                                                                    }
                                                                    else
                                                                    {
                                                                        ccplay.StopCPPPlayer2();
                                                                        Thread.Sleep(1000);
                                                                        ccplayerthread.Abort();
                                                                        Thread.Sleep(1000);
                                                                        ccplayerthread = new Thread(CPPPlayerThread);
                                                                        ccplayerthread.Start();
                                                                    }
                                                                }
                                                            }

                                                        }
                                                        catch (Exception es)
                                                        {
                                                            Log.Instance.LogWrite(es.Message);
                                                        }
                                                    }
                                                    #endregion
                                                }
                                                #endregion

                                                #region 移动音频文件到文件库上
                                                try
                                                {
                                                    AudioFileListTmp.Clear();
                                                    AudioFileList.Clear();
                                                    string[] mp3files = Directory.GetFiles(sUnTarPath, "*.mp3");
                                                    AudioFileListTmp.AddRange(mp3files);
                                                    string[] wavfiles = Directory.GetFiles(sUnTarPath, "*.wav");
                                                    AudioFileListTmp.AddRange(wavfiles);

                                                    #region  把音频文件上传到ftp服务器
                                                    if (AudioFileListTmp.Count > 0)
                                                    {
                                                        string ftppath = ebd.EBM.MsgContent.Auxiliary.AuxiliaryDesc;
                                                        string path = AudioFileListTmp[0];
                                                        ftphelper.UploadFile(path, ftppath);
                                                    }
                                                    #endregion

                                                    if (!EBMVerifyState && AudioFileListTmp.Count > 0 && SingletonInfo.GetInstance().CheckEBMStatusFlag=="0")//
                                                    {
                                                        ListViewItem listItem = new ListViewItem();
                                                        listItem.Text = (list_PendingTask.Items.Count + 1).ToString();
                                                        listItem.SubItems.Add(lDealTarFiles[0]);
                                                        this.Invoke(new Action(() => { list_PendingTask.Items.Add(listItem); }));
                                                        lDealTarFiles.RemoveAt(0);//无论是否成功，都移除
                                                        continue;
                                                    }
                                                    string sTmpDealFile = string.Empty;
                                                    string targetPath = string.Empty;

                                                    string strurl = "";
                                                    string sDateTime = "";
                                                    string sStartTime = ebd.EBM.MsgBasicInfo.StartTime;
                                                    string sEndDateTime = ebd.EBM.MsgBasicInfo.EndTime;
                                                    // string sGBCode = "";
                                                    string sORG_ID = "";
                                                    string sAread = "";
                                                    string xmlFilePath = "";
                                                    //if ((AudioFlag == "2")&&(TextFirst=="2")) //拷贝xml文件
                                                    {
                                                        string xmlFile = Path.GetFileName(sAnalysisFileName);
                                                        xmlFilePath = sAudioFilesFolder + "\\" + xmlFile;
                                                        System.IO.File.Copy(sAnalysisFileName, xmlFilePath, true);
                                                    }
                                                    for (int ai = 0; ai < AudioFileListTmp.Count; ai++)
                                                    {
                                                        sTmpDealFile = Path.GetFileName(AudioFileListTmp[ai]);
                                                        targetPath = sAudioFilesFolder + "\\" + sTmpDealFile;
                                                        System.IO.File.Copy(AudioFileListTmp[ai], targetPath, true);
                                                        AudioFileList.Add(targetPath);

                                                        //if ((PlayType == "2"))//(AudioFlag == "2")
                                                        {
                                                            //sDateTime = ebd.EBM.MsgBasicInfo.StartTime;
                                                            //sStartTime = ebd.EBM.MsgBasicInfo.StartTime;
                                                            //sEndDateTime = ebd.EBM.MsgBasicInfo.EndTime;

                                                            //if (TEST == "YES")
                                                            //{
                                                            //    sDateTime = DateTime.Now.AddSeconds(10).ToString("yyyy-MM-dd HH:mm:ss");
                                                            //    sStartTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                            //    sEndDateTime = DateTime.Now.AddMinutes(5).ToString("yyyy-MM-dd HH:mm:ss");//ebd.EBM.MsgBasicInfo.EndTime;

                                                            //}
                                                            SetText("EBM开始时间: " + ebd.EBM.MsgBasicInfo.StartTime + "===>EBM结束时间: " + ebd.EBM.MsgBasicInfo.EndTime, Color.Blue);
                                                            DateTime EbStartTime = DateTime.Parse(ebd.EBM.MsgBasicInfo.StartTime).AddSeconds(2);
                                                            if (EbStartTime < DateTime.Now)
                                                            {
                                                                EbStartTime = DateTime.Now.AddSeconds(2);
                                                            }

                                                            sDateTime = EbStartTime.ToString("yyyy-MM-dd HH:mm:ss");  //ebd.EBM.MsgBasicInfo.StartTime;
                                                            sStartTime = EbStartTime.ToString("yyyy-MM-dd HH:mm:ss"); //ebd.EBM.MsgBasicInfo.StartTime;
                                                            sEndDateTime = ebd.EBM.MsgBasicInfo.EndTime;
                                                            if (TEST == "YES")
                                                            {
                                                                sDateTime = DateTime.Now.AddSeconds(2).ToString("yyyy-MM-dd HH:mm:ss");
                                                                sStartTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                                sEndDateTime = DateTime.Now.AddMinutes(5).ToString("yyyy-MM-dd HH:mm:ss");//ebd.EBM.MsgBasicInfo.EndTime;
                                                            }

                                                            SetText("开始时间: " + sDateTime + "===>结束时间: " + sEndDateTime + "是否是TEST:" + TEST, Color.Blue);
                                                            sAread = ebd.EBM.MsgContent.AreaCode; //区域
                                                            sORG_ID = ebd.EBM.EBMID;
                                                            strurl = targetPath;  //音频文件地址
                                                                                  //sqlstr = "insert into TsCmdStoreMedia(TsCmd_Type, TsCmd_Mode, TsCmd_UserID, TsCmd_ValueID, TsCmd_Params, TsCmd_Date, TsCmd_Status,TsCmd_StartTime,TsCmd_EndTime,TsCmd_XmlFile)" +
                                                                                  //        "values('播放音频', '" + sAread + "', 1, '" + sORG_ID + "', '" + strurl + "', '" + sDateTime + "', 0,'" + sStartTime + "','" + sEndDateTime + "','" + xmlFilePath + "')";
                                                                                  //int identityID = mainForm.dba.UpdateDbBySQLRetID(sqlstr);
                                                                                  // Console.WriteLine(identityID);
                                                            string sORG_ID2 = GetORG_ID(ebd.EBM.MsgContent.AreaCode);
                                                            string paramValue = "1~" + strurl + "~0~1000~128~0~1~1";
                                                            if ((PlayType == "2"))
                                                            {
                                                                SetText("音频文件存库，将在指定时间内播放", Color.Blue);
                                                                sqlstr = "insert into TsCmdStore(TsCmd_Type, TsCmd_Mode, TsCmd_UserID, TsCmd_ValueID, TsCmd_Params, TsCmd_Date,TsCmd_ExcuteTime,TsCmd_SaveTime, TsCmd_Status,TsCmd_EndTime,TsCmd_Note)" +
                                                                   "values('播放视频', '区域', 1, " + sORG_ID2 + ", '1~" + strurl + "~0~1200~192~0~1~1', " + "'" + sDateTime + "'" + ",'" + sDateTime + "','" + sDateTime + "', 0,'" + sEndDateTime + "'," + "'-1'" + ")";
                                                                Thread.Sleep(500);
                                                                //int iback = mainForm.dba.getResultIDBySQL(sqlstr, "TsCmdStore");
                                                              
                                                                TsCmdStoreID = mainForm.dba.UpdateDbBySQLRetID(sqlstr).ToString();
                                                             //   MessageBox.Show(TsCmdStoreID.ToString());
                                                                RealAudioFlag = true;
                                                                Console.WriteLine(TsCmdStoreID);
                                                                SendMQOrder(1, paramValue, TsCmdStoreID);
                                                             
                                                               // MessageBox.Show("paramValue:"+ paramValue.ToString()+"--"+ "TsCmdStoreID:"+ TsCmdStoreID.ToString());
                                                            }
                                                        }
                                                    }
                                                }
                                                catch (Exception fex)
                                                {
                                                    Log.Instance.LogWrite("行号339：" + fex.Message);
                                                }
                                                #endregion End
                                                AudioFileList.Clear();
                                                #region SaveEBDInfo
                                                if (SaveEBD(ebd) == -1)
                                                    Console.WriteLine("Error: 保存EBMInfo出错");
                                                #endregion
                                            }
                                            #endregion End
                                            break;
                                        case "EBMStreamPortRequest":
                                            #region EBM实时流
                                            try
                                            {
                                                XmlDocument xmlDoc = new XmlDocument();
                                                responseXML rp = new responseXML();
                                                rp.SourceAreaCode = ServerForm.strSourceAreaCode;
                                                rp.SourceType = ServerForm.strSourceType;
                                                rp.SourceName = ServerForm.strSourceName;
                                                rp.SourceID = ServerForm.strSourceID;
                                                rp.sHBRONO = ServerForm.strHBRONO;

                                                Random rd = new Random();
                                                string fName = "10" + rp.sHBRONO + "00000000000" + rd.Next(100, 999).ToString();
                                                xmlDoc = rp.EBMStreamResponse(fName, ServerForm.m_StreamPortURL);
                                                UnifyCreateTar(xmlDoc, fName);
                                                string sHeartBeatTarName = sSendTarPath + "\\" + "EBDT_" + fName + ".tar";
                                                string xmlSignFileName = "\\EBDB_" + fName + ".xml";
                                                CreateXML(xmlDoc, ServerForm.strBeSendFileMakeFolder + xmlSignFileName);
                                                send.address = sZJPostUrlAddress;
                                                send.fileNamePath = sHeartBeatTarName;
                                                postfile.UploadFilesByPostThread(send);

                                                ////进行签名
                                                //ServerForm.mainFrm.GenerateSignatureFile(ServerForm.strBeSendFileMakeFolder, fName);
                                                //ServerForm.tar.CreatTar(ServerForm.strBeSendFileMakeFolder, ServerForm.sSendTarPath, fName);//使用新TAR
                                                //string sSendTarName = ServerForm.sSendTarPath + "\\EBDT_" + fName + ".tar";
                                            }
                                            catch (Exception esb)
                                            {
                                                Console.WriteLine("401:" + esb.Message);
                                            }
                                            #endregion End

                                            ListViewItem OMDRequestItemPort = new ListViewItem();
                                            OMDRequestItemPort.Text = "实时流端口请求";
                                            this.Invoke(new Action(() => { list_OMDRequest.Items.Add(OMDRequestItemPort); }));
                                            break;
                                        case "EBMStateRequest":
                                            lock (OMDRequestLock)
                                            {
                                                EBMStateRequest();
                                                Console.WriteLine(">>>>>>>>>>>>>>>>>>>EBMStateRequest");
                                            }
                                            ListViewItem OMDRequestItemEBMStateRequest = new ListViewItem();
                                            OMDRequestItemEBMStateRequest.Text = "播发状态请求";
                                            this.Invoke(new Action(() => { list_OMDRequest.Items.Add(OMDRequestItemEBMStateRequest); }));
                                            break;
                                        case "ConnectionCheck":
                                            //心跳没有签名文件  走不到这一步

                                            //ListViewItem OMDRequestItemHeart = new ListViewItem();
                                            //OMDRequestItemHeart.Text = "心跳请求";
                                            //this.Invoke(new Action(() => { list_OMDRequest.Items.Add(OMDRequestItemHeart); }));

                                            //this.Invoke((EventHandler)delegate
                                            //{
                                            //    this.Text = "在线";
                                            //    dtLinkTime = DateTime.Now;//刷新时间
                                            //});

                                            break;
                                        case "OMDRequest":
                                            #region 运维请求反馈
                                            string strOMDType = ebd.OMDRequest.OMDType;
                                            try
                                            {
                                                XmlDocument xmlStateDoc = new XmlDocument();
                                                responseXML rState = new responseXML();
                                                rState.SourceAreaCode = ServerForm.strSourceAreaCode;
                                                rState.SourceType = ServerForm.strSourceType;
                                                rState.SourceName = ServerForm.strSourceName;
                                                rState.SourceID = ServerForm.strSourceID;
                                                rState.sHBRONO = ServerForm.strHBRONO;
                                                Random rdState = new Random();
                                                string frdStateName = "10" + rState.sHBRONO + "0000000000000" + rdState.Next(100, 999).ToString();
                                                string xmlEBMStateFileName = "\\EBDB_" + frdStateName + ".xml";
                                                List<Device> lDev = new List<Device>();
                                                lock (OMDRequestLock)
                                                {
                                                    TarOMRequest(xmlStateDoc, rState, strOMDType, frdStateName, xmlEBMStateFileName, lDev);
                                                }
                                            }
                                            catch (Exception h)
                                            {
                                                Log.Instance.LogWrite("错误510行:" + h.Message);
                                            }
                                            #endregion End
                                            break;
                                        default:
                                            this.Invoke((EventHandler)delegate
                                            {
                                                this.Text = "在线";
                                                dtLinkTime = DateTime.Now;//刷新时间
                                            });
                                            break;
                                    }
                                    #endregion 根据EBD类型处理XML文件
                                }
                            }
                            lDealTarFiles.RemoveAt(0);//无论是否成功，都移除
                        }
                        catch (Exception dxml)
                        {
                            Log.Instance.LogWrite("处理XML:" + dxml.Message);
                        }
                    }//for循环处理接收到的Tar包
                }
                catch (Exception em)
                {
                    Log.Instance.LogWrite(em.Message);
                }
                #endregion 处理Tar包

            }//while循环处理解压缩文件
        }

        /// <summary>
        /// 根据策略决定当前消息是否需要审核  返回"0"表示需要审核 返回"1"表示不需要审核
        /// </summary>
        /// <param name="EbdInfo"></param>
        /// <returns></returns>
        public string StrategyChenck(EBD EbdInfo)
        {
            string EBMStatusFlag = "1";
            string severity = EbdInfo.EBM.MsgBasicInfo.Severity;
            if (SingletonInfo.GetInstance().audit.TimeList.Count > 0)
            {
                //有一个条件（消息时间，消息等级）不满足就不审核
                foreach (timestrategies item in SingletonInfo.GetInstance().audit.TimeList)
                {
                    string  dt = DateTime.Now.ToLongTimeString();
                    if (DateTime.Parse(item.EndTime) > DateTime.Parse(dt) && DateTime.Parse(dt) > DateTime.Parse(item.StartTime))
                    {
                       // MessageBox.Show("在时间段内");
                        //在时间段内
                        switch (item.EvenType)
                        {
                            case "0":
                                if (severity == "0")
                                {
                                    EBMStatusFlag = "0";
                                }
                                break;
                            case "1":
                                if (severity == "1")
                                {
                                    EBMStatusFlag = "0";
                                }
                                break;
                            case "2":
                                if (severity == "2")
                                {
                                    EBMStatusFlag = "0";
                                }
                                break;
                            case "3":
                                if (severity == "3")
                                {
                                    EBMStatusFlag = "0";
                                }
                                break;
                            case "4":
                                if (severity == "4")
                                {
                                    EBMStatusFlag = "0";
                                }
                                break;
                            case "100":
                                EBMStatusFlag = "0";
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        // MessageBox.Show("不在时间段内");
                        EBMStatusFlag = "1";
                    }
                    if (EBMStatusFlag=="1")
                    {
                        break;
                    }
                }
            }
            else
            {
                //没有添加审核策略 同时人工审核开关已经打开 则必须要审核
                EBMStatusFlag = "0";
            }

            return EBMStatusFlag;
        }

        private void CheckEBMDataMQSend(EBD EbdInfo)
        {
            List<Property> m_lstProperty = new List<Property>();
            Property property;
            property.name = "ID";  
            property.value = EbdInfo.CheckEBMDataID;
            m_lstProperty.Add(property);

            property.name = "EBDID";
            property.value = EbdInfo.SRC.EBRID;
            m_lstProperty.Add(property);

            property.name = "EBDDID";
            property.value = EbdInfo.EBDID;
            m_lstProperty.Add(property);

            property.name = "CodeA";
            property.value = EbdInfo.EBM.MsgBasicInfo.SenderCode;
            m_lstProperty.Add(property);

            property.name = "NameA";
            property.value = EbdInfo.EBM.MsgBasicInfo.SenderName;
            m_lstProperty.Add(property);

            property.name = "EBMID";
            property.value = EbdInfo.EBM.EBMID;
            m_lstProperty.Add(property);

            property.name = "SentTime";
            property.value = EbdInfo.EBM.MsgBasicInfo.SentTime;
            m_lstProperty.Add(property);

            property.name = "EBMStartTime";
            property.value = EbdInfo.EBM.MsgBasicInfo.StartTime;
            m_lstProperty.Add(property);

            property.name = "EBMEndTime";
            property.value = EbdInfo.EBM.MsgBasicInfo.EndTime;
            m_lstProperty.Add(property);

            property.name = "EBMTitle";
            property.value = EbdInfo.EBM.MsgContent.MsgTitle;
            m_lstProperty.Add(property);


            string lab_EBMType = "";
            if (EbdInfo.EBM.MsgContent.Auxiliary != null)
            {
                lab_EBMType = "音频文件播发";
            }
            else
            {
                lab_EBMType = "文本转语音播发";
            }

            property.name = "EBMType";
            property.value = lab_EBMType;
            m_lstProperty.Add(property);

            property.name = "EBMDesc";
            property.value = EbdInfo.EBM.MsgContent.MsgDesc;
            m_lstProperty.Add(property);

            property.name = "EBMCode";
            property.value = EbdInfo.EBM.MsgContent.AreaCode;
            m_lstProperty.Add(property);

            property.name = "EBMUrl";
            property.value = EbdInfo.EBM.MsgContent.Auxiliary.AuxiliaryDesc;
            m_lstProperty.Add(property);

            property.name = "CheckStatus";
            property.value = SingletonInfo.GetInstance().CheckEBMStatusFlag;
            m_lstProperty.Add(property);

            m_mq_checkEBM.SendMQMessage(true, "", m_lstProperty);
        }

        public string GetORG_ID(string code)
        {
            string org = "";
            string sqlstr = "select ORG_ID from Organization where GB_CODE ='" + code + "'";
            DataTable dtMedia = mainForm.dba.getQueryInfoBySQL(sqlstr);
            if (dtMedia != null && dtMedia.Rows.Count >0)
            {

                if (dtMedia.Rows.Count == 1)
                {
                    org = dtMedia.Rows[0][0].ToString();
                }
            }
            //MessageBox.Show(org.ToString());
            return org;
         
        }

        private void TarOMRequest(XmlDocument xmlStateDoc, responseXML rState, string strOMDType, string frdStateName, string xmlEBMStateFileName, List<Device> lDev)
        {
            string sHeartBeatTarName = "";
            DataTable dtMedia = null;
            switch (strOMDType)
            {
                case "EBRDTInfo":
                    SetText("EBRDTInfo    NO:6", Color.Orange);
                    DateTime dtdd = DateTime.Now;
                    Console.WriteLine(dtdd.ToString("yyyy-MM-dd HH:mm:ss"));
                    //SrvInfromUP();
                    string MediaSql = "";
                    string strSRV_ID = "";
                    string strSRV_CODE = "";
                    MediaSql = "select top(99) SRV_ID,SRV_CODE,SRV_GOOGLE from SRV";
                    dtMedia = mainForm.dba.getQueryInfoBySQL(MediaSql);
                    if (dtMedia != null && dtMedia.Rows.Count > 0)
                    {
                        for (int idtM = 0; idtM < dtMedia.Rows.Count; idtM++)
                        {
                            string TrLL = dtMedia.Rows[idtM][2].ToString();
                            Device DV = new Device();
                            strSRV_ID = dtMedia.Rows[idtM][0].ToString();
                            strSRV_CODE = dtMedia.Rows[idtM][1].ToString();
                            if (idtM < 10)
                            {
                                DV.DeviceID = "0" + idtM;
                            }
                            else { DV.DeviceID = idtM.ToString(); }
                            DV.DeviceName = strSRV_ID; if (TrLL != "")
                            {
                                string[] str = TrLL.Split(',');
                                if (str.Length >= 2)
                                {
                                    DV.Longitude = str[1];
                                    DV.Latitude = str[0];
                                }
                                else
                                {
                                    DV.Longitude = SingletonInfo.GetInstance().Longitude;
                                    DV.Latitude = SingletonInfo.GetInstance().Latitude;
                                }
                            }
                            else
                            {
                                DV.Longitude = SingletonInfo.GetInstance().Longitude;
                                DV.Latitude = SingletonInfo.GetInstance().Latitude;
                            }
                            lDev.Add(DV);
                        }
                        xmlStateDoc = rState.DeviceInfoResponse(ebd, lDev, frdStateName);
                        UnifyCreateTar(xmlStateDoc, frdStateName);
                        sHeartBeatTarName = sSendTarPath + "\\" + "EBDT_" + frdStateName + ".tar";
                        send.address = sZJPostUrlAddress;
                        send.fileNamePath = sHeartBeatTarName;
                        postfile.UploadFilesByPostThread(send);
                    }
                    Console.WriteLine(DateTime.Now - dtdd);
                    ListViewItem OMDRequestEBRDTInfo = new ListViewItem();
                    OMDRequestEBRDTInfo.Text = "设备信息请求";
                    this.Invoke(new Action(() => { list_OMDRequest.Items.Add(OMDRequestEBRDTInfo); }));
                    break;
                case "EBRDTState":
                    SetText("EBRDTState     NO:9", Color.Orange);
                    DateTime dt = DateTime.Now;
                    Console.WriteLine(dt.ToString("yyyy-MM-dd HH:mm:ss"));
                    string MediaSqlS = "";
                    string strSRV_CODES = "";
                    MediaSqlS = "select top(99) SRV_ID,SRV_CODE from SRV";
                    dtMedia = mainForm.dba.getQueryInfoBySQL(MediaSqlS);
                    if (dtMedia != null && dtMedia.Rows.Count > 0)
                    {
                        for (int idtM = 0; idtM < dtMedia.Rows.Count; idtM++)
                        {
                            Device DV = new Device();
                            strSRV_CODES = dtMedia.Rows[idtM][1].ToString();
                            if (idtM < 10)
                            {
                                DV.DeviceID = "0" + idtM;
                            }
                            else { DV.DeviceID = idtM.ToString(); }
                            DV.DeviceName = strSRV_CODES;

                            lDev.Add(DV);
                        }
                    }
                    xmlStateDoc = rState.DeviceStateResponse(ebd, lDev, frdStateName);
                    UnifyCreateTar(xmlStateDoc, frdStateName);
                    sHeartBeatTarName = sSendTarPath + "\\" + "EBDT_" + frdStateName + ".tar";
                    send.address = sZJPostUrlAddress;
                    send.fileNamePath = sHeartBeatTarName;
                    postfile.UploadFilesByPostThread(send);

                    Console.WriteLine(DateTime.Now - dt);
                    ListViewItem OMDRequestEBRDTState = new ListViewItem();
                    OMDRequestEBRDTState.Text = "设备状态请求";
                    this.Invoke(new Action(() => { list_OMDRequest.Items.Add(OMDRequestEBRDTState); }));
                    break;
                //EBRSTInfo
                //EBRPSInfo
                //EBRPSInfo---
                case "EBRPSInfo":
                    SetText("EBRPSInfo     NO:2", Color.Orange);
                    try
                    {
                        xmlStateDoc = rState.platformInfoResponse(ebd, lDev, frdStateName);
                        UnifyCreateTar(xmlStateDoc, frdStateName);
                        sHeartBeatTarName = sSendTarPath + "\\" + "EBDT_" + frdStateName + ".tar";
                        send.address = sZJPostUrlAddress;
                        send.fileNamePath = sHeartBeatTarName;
                        postfile.UploadFilesByPostThread(send);
                        ListViewItem OMDRequestEBRPSInfo = new ListViewItem();
                        OMDRequestEBRPSInfo.Text = "台站信息请求";
                        this.Invoke(new Action(() => { list_OMDRequest.Items.Add(OMDRequestEBRPSInfo); }));
                    }
                    catch
                    {
                    }
                    break;
                //EBRSState
                //EBRPSState--
                case "EBRPSState":
                    SetText("EBRPSState    NO:7", Color.Orange);
                    try
                    {
                        xmlStateDoc = rState.platformstateInfoResponse(ebd, lDev, frdStateName);
                        UnifyCreateTar(xmlStateDoc, frdStateName);
                        sHeartBeatTarName = sSendTarPath + "\\" + "EBDT_" + frdStateName + ".tar";
                        send.address = sZJPostUrlAddress;
                        send.fileNamePath = sHeartBeatTarName;
                        postfile.UploadFilesByPostThread(send);
                        ListViewItem OMDRequestEBRPSState = new ListViewItem();
                        OMDRequestEBRPSState.Text = "台站状态请求";
                        this.Invoke(new Action(() => { list_OMDRequest.Items.Add(OMDRequestEBRPSState); }));
                    }
                    catch
                    {
                    }
                    break;
            }
            Console.WriteLine(">>>>>>>>>>>>>>>>>>>" + strOMDType);
        }

        private void UnifyCreateTar(XmlDocument xmlStateDoc, string frdStateName)
        {

            string XMLSavePath = CreateCMLSavePath(frdStateName);
            string xmlSignFileName = "\\EBDB_" + frdStateName + ".xml";
            CreateXML(xmlStateDoc, XMLSavePath + xmlSignFileName);
            ServerForm.mainFrm.GenerateSignatureFile(XMLSavePath, frdStateName);
            ServerForm.tar.CreatTar(XMLSavePath, ServerForm.sSendTarPath, frdStateName);//使用新TAR
        }

        private string CreateCMLSavePath(string FileName)
        {
            string SaveXMLofName = strBeSendFileMakeFolder + "\\" + FileName;// "D:\\work\\93\\BeXmlFiles\\" + FileName;
            if (!Directory.Exists(SaveXMLofName))
            {
                Directory.CreateDirectory(SaveXMLofName);
            }
            else
            {
                ServerForm.DeleteFolder(SaveXMLofName);
            }
            return SaveXMLofName;
        }

        //做成委托
        private int SaveEBD(EBD ebm)
        {
            string EBDVersion = "";//,  --协议版本号
            string SEBDID = "";////--应急广播数据包ID
            string SEDBType = "";//,--事件类型( EBM EBMStateResponse EBMStateRequest OMDRequest EBRSTInfo EBRASInfo EBRBSInfo EBRDTInfo EBMBrdLog EBRASState EBRBSState EBRDTState ConnectionCheck EBDResponse -)
            string SEBRID = "";// ,--数据包来源对象ID
            string EBRID = "";//,--数据包目标对象ID
            string SEBBuidTime = "";//,---数据包生成时间
            string EBMID = "";//,--应急广播消息ID
            string MsaType = "";// ,---- 消息类型 1：请求播发 2：取消播发
            string SenderName = "";//,--发布机构名称
            string SenderCode = "";// ,--发布机构编码
            string SendTime = "";// ,--发布时间
            string EventType = "";//,--事件类型编码
            string Severity = "";// ,--事件级别
            string StartTime = "";// ,--播发起始时间
            string EndTime = "";// ,--播发结束时间
            string LanguageCode = "";// ,--语种代码(中文为:zho)
            string MsgTitle = "";// ,--消息标题文本-
            string MsgDesc = "";//,--消息内容文本
            string AreaCode = "";// ,--覆盖区域编码 eg:110000000000,120000000000,130000000000
            string AuxiliaryType = "";//,--辅助数据类型 61：实时流 2文件
            string AuxiliaryDesc = "";// , --文件名称
            string EBMState = "";// --执行状态

            //EBM处理
            if (ebm != null)
            {
                EBDVersion = ebm.EBDVersion;
                SEBDID = ebd.EBDID;
                SEDBType = ebd.EBDType;
                SEBRID = ebm.SRC.EBRID;
                EBRID = ebm.DEST.EBRID;
                SEBBuidTime = ebm.EBDTime;
                SEBDIDStatusFlag = SEBDID;
                if (ebd.EBDType == "EBM")
                {
                    EBMID = ebm.EBM.EBMID;
                    MsaType = ebm.EBM.MsgBasicInfo.MsgType;
                    SenderName = ebm.EBM.MsgBasicInfo.SenderName;
                    SenderCode = ebm.EBM.MsgBasicInfo.SenderCode;
                    SendTime = ebm.EBM.MsgBasicInfo.SentTime;
                    EventType = ebm.EBM.MsgBasicInfo.EventType;
                    Severity = ebm.EBM.MsgBasicInfo.Severity;
                    StartTime = ebm.EBM.MsgBasicInfo.StartTime;
                    EndTime = ebm.EBM.MsgBasicInfo.EndTime;
                    LanguageCode = ebm.EBM.MsgContent.LanguageCode;
                    MsgTitle = ebm.EBM.MsgContent.MsgTitle;
                    MsgDesc = ebm.EBM.MsgContent.MsgDesc;
                    AreaCode = ebm.EBM.MsgContent.AreaCode;
                    if (ebm.EBM.MsgContent.Auxiliary != null)
                    {
                        AuxiliaryType = ebm.EBM.MsgContent.Auxiliary.AuxiliaryType;
                        AuxiliaryDesc = ebm.EBM.MsgContent.Auxiliary.AuxiliaryDesc;
                    }
                }
                else
                {
                    EBMState = "1";
                }
            }

            StringBuilder sbSql = new StringBuilder(100);
            sbSql.Append("insert into EBMInfo Values(");
            sbSql.Append("'" + EBDVersion + "',");
            sbSql.Append("'" + SEBDID + "',");
            sbSql.Append("'" + SEDBType + "',");
            sbSql.Append("'" + SEBRID + "',");
            sbSql.Append("'" + EBRID + "',");
            sbSql.Append("'" + SEBBuidTime + "',");              //收到时间
            sbSql.Append("'" + EBMID + "',");              //就绪状态
            sbSql.Append("'" + MsaType + "',");         //开始时间
            sbSql.Append("'" + SenderName + "',");         //执行时间
            sbSql.Append("'" + SenderCode + "',");           //结束时间
            sbSql.Append("'" + SendTime + "',");
            sbSql.Append("'" + EventType + "',");
            sbSql.Append("'" + Severity + "',");
            sbSql.Append("'" + StartTime + "',");
            sbSql.Append("'" + EndTime + "',");
            sbSql.Append("'" + LanguageCode + "',");
            sbSql.Append("'" + MsgTitle + "',");
            sbSql.Append("'" + MsgDesc + "',");
            sbSql.Append("'" + AreaCode + "',");
            sbSql.Append("'" + AuxiliaryType + "',");
            sbSql.Append("'" + AuxiliaryDesc + "',");
            sbSql.Append("'" + EBMState + "',");
            sbSql.Append("'" + TsCmdStoreID + "'");
            sbSql.Append(")");
            //mainForm.dba.UpdateOrInsertBySQL(sbSql.ToString());
            return 1;//mainForm.dba.UpdateDbBySQL(sbSql.ToString());

        }

        #region 数据计算校验和
        private string DataSum(string sCmdStr)
        {
            //, char cSplit, ref List<byte> list

            try
            {
                int iSum = 0;
                List<byte> listCmd = new List<byte>();
                string sSum = "";
                if (sCmdStr.Trim() == "")
                    return "";
                string[] sTmp = sCmdStr.Split(' ');
                byte[] cmdByte = new byte[sTmp.Length];
                for (int i = 0; i < sTmp.Length; i++)
                {
                    cmdByte[i] = byte.Parse(sTmp[i], System.Globalization.NumberStyles.HexNumber);
                    listCmd.Add(cmdByte[i]);
                    iSum = iSum + int.Parse(sTmp[i], System.Globalization.NumberStyles.HexNumber);
                }
                sSum = Convert.ToString(iSum, 16).ToUpper().PadLeft(4, '0');
                sSum = sSum.Substring(sSum.Length - 2, 2);
                return sSum;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误");
                return "";
            }
        }
        #endregion 数据计算校验和

        //回复处理线程
        private void FeedBackDeal()
        {
            List<string> lFeedTmp = new List<string>();
            string sqlsearch = string.Empty;
            string sAddress = string.Empty;
            while (true)
            {
                try
                {
                    if (lFeedBack.Count == 0)
                    {
                        Thread.Sleep(5000);
                        //continue;
                    }
                    else
                    {
                        lock (oLockFeedBack)
                        {
                            if (lFeedBack.Count > 0)
                            {
                                lFeedTmp.AddRange(lFeedBack.ToArray());
                                lFeedBack.Clear();
                            }
                        }
                    }
                }
                catch (Exception fbEx)
                {
                    Log.Instance.LogWrite("反馈处理错误673行：" + fbEx.Message);
                }
                for (int iFB = 0; iFB < lFeedTmp.Count; iFB++)
                {
                    try
                    {
                        {
                            #region 处理查询
                            {
                                try
                                {
                                    sAddress = sZJPostUrlAddress;
                                    List<EBMState> lEBMState = new List<EBMState>();
                                    XmlDocument xmlStateDoc = new XmlDocument();
                                    responseXML rState = new responseXML();
                                    rState.SourceAreaCode = strSourceAreaCode;
                                    rState.SourceType = strSourceType;
                                    rState.SourceName = strSourceName;
                                    rState.SourceID = strSourceID;
                                    rState.sHBRONO = strHBRONO;

                                    /*string ebdid = PlatFormDt.Rows[itb][3].ToString();
                                    if (ebdid == "" || ebdid == null)*/
                                    {
                                        string ebdid = ebd.EBDID;
                                    }
                                    try
                                    {
                                        Random rd = new Random();
                                        string fName = "10" + rState.sHBRONO + "0000000000000" + rd.Next(100, 999).ToString();
                                        string xmlStateFileName = "\\EBDB_" + fName + ".xml";
                                        xmlStateDoc = rState.EBMStateRequestResponse(ebd, fName);
                                        CreateXML(xmlStateDoc, sSourcePath + xmlStateFileName);

                                        //进行签名
                                        mainFrm.GenerateSignatureFile(sSourcePath, fName);

                                        tar.CreatTar(sSourcePath, sSendTarPath, fName);//使用新TAR 
                                        string sStateSendTarName = sSendTarPath + "\\" + "EBDT_" + fName + ".tar";//lFeedTmp[iFB]
                                        HttpSendFile.UploadFilesByPost(sAddress, sStateSendTarName);
                                    }
                                    catch (Exception ec)
                                    {
                                        Log.Instance.LogWrite("错误566：" + ec.Message);
                                    }

                                    //移除
                                    lFeedTmp.Remove(lFeedTmp[iFB]);
                                    if (iFB == 0)
                                    {
                                        iFB = 0;
                                    }
                                    else
                                    {
                                        iFB--;
                                    }
                                }
                                catch (Exception dealEx)
                                {
                                    Log.Instance.LogWrite("错误746行：" + dealEx.Message);
                                    continue;
                                }
                            }
                            #endregion End
                        }
                    }
                    catch (Exception forEx)
                    {
                        Log.Instance.LogWrite("For外围错误：" + forEx.Message);
                    }
                }
            }
        }

        #region ToList
        private void PlatToList(DataTable dt, ref List<PlatformBRD> lPlat)
        {//PlatformID,BRDSourceType,BRDSourceID,BRDMsgID,BRDSender,BRDStartTime,BRDEndTime,MediaFileURL
            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    PlatformBRD pm = new PlatformBRD();
                    pm.PlatformBRDID = dt.Rows[i][0].ToString();
                    pm.SourceType = dt.Rows[i][1].ToString();
                    pm.SourceID = dt.Rows[i][2].ToString();
                    pm.MsgID = dt.Rows[i][3].ToString();
                    pm.Sender = dt.Rows[i][4].ToString();
                    pm.BRDStartTime = dt.Rows[i][5].ToString();
                    Console.WriteLine(pm.BRDStartTime);
                    pm.BRDEndTime = dt.Rows[i][6].ToString();
                    pm.AudioFileURL = dt.Rows[i][7].ToString();
                    pm.UnitId = "3424";//播发部门ID
                    pm.UnitName = "公安局";//播发部门名称
                    pm.PersonID = "74";
                    pm.PersonName = "吴局";

                    lPlat.Add(pm);
                }
            }
        }

        private void TermToList(DataTable dt, ref List<TermBRD> lTerm)
        {
            //PlatformID,BRDSourceType,BRDSourceID,BRDMsgID,BRDTime
            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    TermBRD tm = new TermBRD();
                    tm.BRDTime = dt.Rows[i][4].ToString();
                    tm.DeviceID = dt.Rows[i][0].ToString();
                    tm.SourceType = dt.Rows[i][1].ToString();
                    tm.SourceID = dt.Rows[i][2].ToString();
                    tm.MsgID = dt.Rows[i][3].ToString();
                    tm.TermBRDID = dt.Rows[i][0].ToString();
                    tm.ResultCode = "1";
                    tm.ResultDesc = "正常";

                    lTerm.Add(tm);
                }
            }
        }

        private void DeviceDataToList(DataTable dt, ref List<Device> listD)
        {
            string sBaidu = string.Empty;
            int iPos = -1;
            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Device dv = new Device();
                    dv.DeviceID = dt.Rows[i][0].ToString();
                    dv.DeviceCategory = "Term";
                    dv.DeviceType = "TN5415E";
                    dv.DeviceState = "正常";
                    dv.AreaCode = dt.Rows[i][1].ToString();
                    dv.DeviceName = "音柱";
                    dv.AdminLevel = "村级";
                    sBaidu = dt.Rows[i][2].ToString();
                    if (sBaidu.Length > 0)
                    {
                        iPos = sBaidu.IndexOf(",");
                        if (iPos > 0)
                        {
                            dv.Longitude = sBaidu.Substring(0, iPos);
                            dv.Latitude = sBaidu.Substring(iPos + 1);
                        }

                    }
                    listD.Add(dv);
                }
            }
        }

        private void DeviceStateToList(DataTable dt, ref List<Device> listD)
        {
            string devState = string.Empty;
            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Device dv = new Device();
                    dv.DeviceID = dt.Rows[i][0].ToString();
                    dv.DeviceCategory = "Term";
                    dv.DeviceType = "TN5415E";
                    devState = dt.Rows[i][1].ToString();
                    switch (devState)
                    {
                        case "正常":
                        default:
                            dv.DeviceState = "1";
                            break;
                        case "故障":
                            dv.DeviceState = "2";
                            break;
                        case "故障恢复":
                            dv.DeviceState = "3";
                            break;
                    }

                    dv.AreaCode = dt.Rows[i][2].ToString();
                    dv.DeviceName = "音柱";
                    dv.AdminLevel = "村级";
                    listD.Add(dv);
                }
            }
        }
        #endregion End

        private int DateDiff(DateTime DateTime1, DateTime DateTime2)
        {
            int dateDiff = 0;

            TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
            TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
            TimeSpan ts = ts1.Subtract(ts2).Duration();
            dateDiff = (int)(ts.TotalSeconds);
            //Console.WriteLine(DateTime1.ToString() + "-" + DateTime2.ToString() + "=" +dateDiff.ToString());
            return dateDiff;
        }

        /// <summary>
        /// 清空指定的文件夹，但不删除文件夹
        /// </summary>
        /// <param name="folderpath">文件夹路径</param>
        public static void DeleteFolder(string folderpath)
        {
            try
            {

                foreach (string delFile in Directory.GetFileSystemEntries(folderpath))
                {
                    if (File.Exists(delFile))
                    {
                        FileInfo fi = new FileInfo(delFile);
                        if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                            fi.Attributes = FileAttributes.Normal;
                        File.Delete(delFile);//直接删除其中的文件
                        // SetText("删除文件：" + delFile);
                    }
                    else
                    {
                        DirectoryInfo dInfo = new DirectoryInfo(delFile);
                        if (dInfo.GetFiles().Length != 0)
                        {
                            DeleteFolder(dInfo.FullName);//递归删除子文件夹
                        }
                        Directory.Delete(delFile);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("G1475：" + ex.Message);
                Log.Instance.LogWrite("G1475：" + ex.Message);
            }
        }

        public string Str2Hex(string strMsg)
        {
            string result = string.Empty;

            byte[] arrByte = System.Text.Encoding.GetEncoding("GB2312").GetBytes(strMsg);
            for (int i = 0; i < arrByte.Length; i++)
            {
                result += System.Convert.ToString(arrByte[i], 16) + " "; //Convert.ToString(byte, 16)把byte转化成十六进制string 
            }
            result = result.Trim();
            return result;
        }

        public List<string> Str2HexList(string strMsg)
        {
            string result = string.Empty;
            List<string> retStrList = new List<string>();

            byte[] arrByte = System.Text.Encoding.GetEncoding("GB2312").GetBytes(strMsg);
            for (int i = 0; i < arrByte.Length; i++)
            {
                //result += System.Convert.ToString(arrByte[i], 16) + " "; //Convert.ToString(byte, 16)把byte转化成十六进制string 
                retStrList.Add(System.Convert.ToString(arrByte[i], 16));
            }
            //result = result.Trim();
            return retStrList;
        }

        /// <summary>
        /// 字节数组转字符
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public string DecodingSMS(string s)
        {
            string result = string.Empty;
            byte[] arrByte = new byte[s.Length / 2];
            int index = 0;
            for (int i = 0; i < s.Length; i += 2)
            {
                arrByte[index++] = Convert.ToByte(s.Substring(i, 2), 16); //Convert.ToByte(string,16)把十六进制string转化成byte 
            }
            result = Encoding.Default.GetString(arrByte);

            return result;
        }

        private void SaveEBD(EBD ebdstruct, int tscmd_id)
        {
            if (tscmd_id == -1)
            {
                return;
            }
            string sqlstr = "";
            string strEBDType = ebdstruct.EBDType.ToLower();
            string strEBDID = ebdstruct.EBDID;

            switch (strEBDType)
            {
                case "ebm":
                    #region EBM
                    string strEBMID = ebdstruct.EBM.EBMID;//消息ID
                    if (strEBMID == "")
                    {
                        Console.WriteLine("应急广播消息ID无效！");
                    }
                    else
                    {
                        sqlstr = "select count(*) from PlatformBRD where BRDEBMID ='" + strEBMID + "'";
                    }
                    int row_num = (int)mainForm.dba.getQueryResultBySQL(sqlstr);//查询数据库中是否有该记录
                    if (row_num == 0)
                    {
                        try
                        {
                            //BRDAuxiliaryInfo用于保存Auxiliary信息
                            string strBRDTime = ebdstruct.EBDTime;//数据包生成时间
                            //string strBRDSRCAreaCode = ebdstruct.SRC.AreaCode;  //2016-04-01
                            //string strBRDSourceType = ebdstruct.SRC.EBEType;
                            //string strBRDSourceName = ebdstruct.SRC.EBEName;
                            string strBRDSRCAreaCode = "";  //2016-04-01
                            string strBRDSourceType = "";
                            string strBRDSourceName = "";
                            string strBRDSourceID = ebdstruct.SRC.EBRID;

                            string strBRDStartTime = ebdstruct.EBM.StartTime;
                            string strBRDEndTime = ebdstruct.EBM.EndTime;
                            string strBRDSendTime = ebdstruct.EBM.MsgBasicInfo.SentTime;
                            //string strBRDMsgType = ebdstruct.EBM.MsgBasicInfo.MsgType;
                            string strBRDMsgType = ebdstruct.EBM.TestType;
                            //string strBRDExerciseType = ebdstruct.EBM.ExerciseType;  //播放演练类型
                            string strBRDSender = ebdstruct.EBM.MsgBasicInfo.SenderName;  //发布机构名称
                            string strBRDDescription = ebdstruct.EBM.MsgContent.MsgDesc.Trim();  //消息内容
                            string strBRDEventType = ebdstruct.EBM.MsgBasicInfo.EventType;  //事件类型编码
                            string strBRDSeverity = ebdstruct.EBM.MsgBasicInfo.Severity;  //事件类型
                            // string strBRDLanguageOrCharSet = ebdstruct.EBM.MsgContent.LanguageCode + "|" + ebdstruct.EBM.MsgContent.CharSet;
                            string strBRDAuxiliaryInfo = string.Empty;
                            /*  if (ebdstruct.EBM.MsgContent.Auxiliary.Count>0)
                              {
                                  strBRDAuxiliaryInfo = ebdstruct.EBM.MsgContent.Auxiliary[0].AuxiliaryType + "|" + ebdstruct.EBM.MsgContent.Auxiliary[0].AuxiliaryDesc;
                              }*/
                            string strCoverageAreaCode = string.Empty;
                            if (ebdstruct.EBM.MsgContent.AreaCode != null)  //原以多条数据，现以“，"分割
                            {
                                strCoverageAreaCode = ebdstruct.EBM.MsgContent.AreaCode;  //2016-04-03 需分析多条数据
                                //for (int aCount = 0; aCount < ebdstruct.EBM.Coverage.Area.Count; aCount++)
                                //{
                                //    if (aCount == 0)
                                //    {
                                //        strCoverageAreaCode = ebdstruct.EBM.Coverage.Area[aCount].AreaCode + ";" + ebdstruct.EBM.Coverage.Area[aCount].AreaName;
                                //    }
                                //    else
                                //    {
                                //        strCoverageAreaCode = strCoverageAreaCode + "|" + ebdstruct.EBM.Coverage.Area[aCount].AreaCode + ";" + ebdstruct.EBM.Coverage.Area[aCount].AreaName;
                                //    }
                                //}
                            }

                            sqlstr = "insert into PlatformBRD(BRDEBDTime,EBDID,TsCmdID,BRDSRCAreaCode,BRDSRCEBEType,BRDSRCEBEName,BRDSRCEBEID,BRDStartTime,BRDEndTime," +
                                     "BRDSendTime,BRDEBMID,BRDMsgType,BRDExerciseType,BRDSender,BRDMsgDesc,BRDEventType,BRDSeverity,BRDLanguageOrCharSet,BRDAuxiliaryInfo" +
                                     ",BRDCoverageArea) values( '" + strBRDTime + "','" + strEBDID + "'," + tscmd_id + ",'" + strBRDSRCAreaCode + "','" + strBRDSourceType +
                                     "','" + strBRDSourceName + "','" + strBRDSourceID + "','" + strBRDStartTime + "','" + strBRDEndTime +
                                     "','" + strBRDSendTime + "','" + strEBMID + "','" + strBRDMsgType + "','" + "" +//strBRDExerciseType +
                                     "','" + strBRDSender + "','" + strBRDDescription + "','" + strBRDEventType + "','" + strBRDSeverity + "','" + "" +
                                     "','" + strBRDAuxiliaryInfo + "','" + strCoverageAreaCode + "')";
                            mainForm.dba.UpdateDbBySQL(sqlstr);
                        }
                        catch (Exception es)
                        {
                            Log.Instance.LogWrite("插入数据错误：" + es.Message);
                        }
                    }
                    else
                    {
                        try
                        {
                            sqlstr = "update TsCmdStore set TsCmd_Excute='停止',TsCmd_Note='-1' where TsCmd_ID = (select TsCmdID from PlatformBRD where BRDEBMID ='" + strEBMID + "')";
                            mainForm.dba.UpdateDbBySQL(sqlstr);

                            sqlstr = "update PlatformBRD set TsCmdID=" + tscmd_id + " where BRDEBMID ='" + strEBMID + "'";
                            mainForm.dba.UpdateDbBySQL(sqlstr);
                        }
                        catch (Exception es)
                        {
                            Log.Instance.LogWrite("更新数据错误：" + es.Message);
                        }
                    }
                    #endregion End
                    break;
                case "heartbeat":
                    //不保存心跳包
                    break;
                default:
                    break;
            }
        }

        #region 替换后面的“00”为“AA”
        private string ReplaceToAA(string dataStr)
        {
            string lh_Str = "";
            string AA_Str = "";
            if (dataStr != "" && dataStr != " ")
            {
                for (int i = 0; i < dataStr.Length; i = i + 2)
                {
                    AA_Str = dataStr.Substring(i, 2);
                    if (AA_Str == "00")
                    {
                        AA_Str = "AA";
                    }
                    lh_Str = lh_Str + AA_Str;
                }
                lh_Str = lh_Str.TrimEnd(' ');
            }
            else
            {
                lh_Str = "";
            }
            return lh_Str;
        }
        #endregion

        private string L_H(string dataStr)
        {
            string lh_Str = "";
            if (dataStr != "" && dataStr != " ")
            {
                for (int i = 0; i < dataStr.Length; i = i + 2)
                {
                    lh_Str = dataStr.Substring(i, 2) + " " + lh_Str;
                }
                lh_Str = lh_Str.TrimEnd(' ');
            }
            else
            {
                lh_Str = "";
            }
            return lh_Str;
        }

        public string CRCBack(string sOrigin)
        {
            string[] sTmp = sOrigin.Trim().Split(' ');
            byte[] list = new byte[sTmp.Length];
            for (int i = 0; i < sTmp.Length; i++)
            {
                list[i] = (byte.Parse(sTmp[i], System.Globalization.NumberStyles.HexNumber));
            }
            string lists = CalmCRC.GetCRC(list)[0].ToString("X2") + " " + CalmCRC.GetCRC(list)[1].ToString("X2");
            return lists;
        }

        public void ShowMsg(string msgstr)
        {
            txtMsgShow.Text = msgstr;
        }

        public void EBMStateToList(DataTable dtState, ref List<EBMState> lPlat)
        {
            if (dtState != null)
            {
                for (int i = 0; i < dtState.Rows.Count; i++)
                {
                    EBMState ebmState = new EBMState();
                    ebmState.BRDCoverageArea = dtState.Rows[i][0].ToString();
                    ebmState.BRDState = dtState.Rows[i][0].ToString();
                    lPlat.Add(ebmState);
                }
            }
        }

        //应急消息播发状态请求反馈
        private void sendEBMStateRequestResponse()
        {
            string MediaSql = "";
            string TsCmd_XmlFile = "";
            int TsCmd_ID = 0;
            EBD ebdStateRequest;
            try
            {
                MediaSql = "select top(1)TsCmd_ID,TsCmd_XmlFile from TsCmdStoreMedia where TsCmd_ValueID = '" + ebd.EBMStateRequest.EBM.EBMID + "' order by TsCmd_Date desc";
                DataTable dtMedia = mainForm.dba.getQueryInfoBySQL(MediaSql);
                if (dtMedia != null && dtMedia.Rows.Count > 0)
                {
                    for (int idtM = 0; idtM < dtMedia.Rows.Count; idtM++)
                    {
                        TsCmd_ID = (int)dtMedia.Rows[idtM][0];
                        TsCmd_XmlFile = dtMedia.Rows[idtM][1].ToString();
                        using (FileStream fs = new FileStream(TsCmd_XmlFile, FileMode.Open))
                        {
                            StreamReader sr = new StreamReader(fs, System.Text.Encoding.UTF8);
                            String xmlInfo = sr.ReadToEnd();
                            xmlInfo = xmlInfo.Replace("xmlns:xs", "xmlns");
                            sr.Close();
                            xmlInfo = XmlSerialize.ReplaceLowOrderASCIICharacters(xmlInfo);
                            xmlInfo = XmlSerialize.GetLowOrderASCIICharacters(xmlInfo);
                            ebdStateRequest = XmlSerialize.DeserializeXML<EBD>(xmlInfo);
                        }
                        sendEBMStateResponse(ebdStateRequest);
                        SetText(DateTime.Now.ToString() + "应急消息播发状态请求反馈：" + ebd.EBMStateRequest.EBM.EBMID, Color.Orange);
                    }
                }
            }
            catch
            {
            }

        }

        //应急消息播发状态反馈
        private void sendEBMStateResponse(EBD ebdsr)
        {
            #region 先删除解压缩包中的文件
            foreach (string xmlfiledel in Directory.GetFileSystemEntries(sEBMStateResponsePath))
            {
                if (File.Exists(xmlfiledel))
                {
                    FileInfo fi = new FileInfo(xmlfiledel);
                    if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                        fi.Attributes = FileAttributes.Normal;
                    File.Delete(xmlfiledel);//直接删除其中的文件  
                }
            }
            #endregion End
            XmlDocument xmlHeartDoc = new XmlDocument();
            responseXML rHeart = new responseXML();
            rHeart.SourceAreaCode = strSourceAreaCode;
            rHeart.SourceType = strSourceType;
            rHeart.SourceName = strSourceName;
            rHeart.SourceID = strSourceID;
            rHeart.sHBRONO = strHBRONO;
            //try
            //{
            //.HeartBeatResponse();  // rState.EBMStateResponse(ebd);
            Random rd = new Random();
            string fName = ebd.EBDID.ToString() + rd.Next(100, 999).ToString();
            string xmlSignFileName = "\\EBDI_" + ebd.EBDID.ToString() + ".xml";
            xmlHeartDoc = rHeart.EBMStateResponse(ebd, "EBMStateResponse", fName);
            //string xmlStateFileName = "\\EBDB_000000000001.xml";
            CreateXML(xmlHeartDoc, sEBMStateResponsePath + xmlSignFileName);
            tar.CreatTar(sEBMStateResponsePath, sSendTarPath, ebd.EBDID.ToString());// "HB000000000001");//使用新TAR
            //}
            //catch (Exception ec)
            //{
            //    Log.Instance.LogWrite("应急消息播发状态反馈组包错误：" + ec.Message);
            //}
            //string sHeartBeatTarName = sSendTarPath + "\\" + "HB000000000001" + ".tar";
            string sHeartBeatTarName = sSendTarPath + "\\EBDT_" + ebd.EBDID.ToString() + ".tar";
            try
            {
                HttpSendFile.UploadFilesByPost(sZJPostUrlAddress, sHeartBeatTarName);
            }
            catch (Exception w)
            {
                Log.Instance.LogWrite("应急消息播发状态反馈发送平台错误：" + w.Message);
            }
        }

        private void timHold_Tick(object sender, EventArgs e)
        {
            switch (bCharToAudio)
            {
                case "1":
                    {
                        //文转
                        #region 文转语
                        if (mainForm.bMsgStatusFree)
                        {
                            //if (mainForm.bMsgStatusFree)
                            //{
                            //    iHoldTimesCnt = iHoldTimes;
                            //}
                            //string cmdSStr = "54 01 03 01 00";
                            //cmdSStr = cmdSStr + " " + CRCBack(cmdSStr);
                            //SendCRCCmd(mainForm.sndComm, cmdSStr, 1);//

                            //if (iHoldTimesCnt < iHoldTimes)
                            //{
                            //    for (int i = 0; i < listAreaCode.Count; i++)
                            //    {
                            //        string cmdOpen = "4C " + listAreaCode[i] + " C0 02 01 04";
                            //        SendCmd(mainForm.comm, cmdOpen, 1);
                            //    }
                            //    iHoldTimesCnt++;//累加
                            //}
                            //else
                            {
                                timHold.Stop();
                                //string cmdStr = "4C " + EMBCloseAreaCode + " C0 02 00 01";//停止时发关机指令
                                //SendCmd(mainForm.comm, cmdStr, 8);//发送指令
                                Thread.Sleep(2000);
                                for (int i = 0; i < listAreaCode.Count; i++)
                                {
                                    string cmdOpen = "4C " + listAreaCode[i] + " C0 02 00 04";
                                    //  string cmdOpen = "4C AA AA AA AA AA C0 02 01 04";
                                    //  string cmdOpen = "FE FE FE 4C AA AA AA AA AA C0 02 01 04 65 16";
                                    //  string cmdOpen = "FE FE FE 4C AA AA AA AA AA C0 02 00 04 64 16";
                                    //SendCmd(mainForm.comm, cmdOpen, 6);
                                    Log.Instance.LogWrite("文转语结束应急关机：" + cmdOpen);
                                    //2016-04-01  改写数据池
                                    string strsum = DataSum(cmdOpen);
                                    cmdOpen = "FE FE FE " + cmdOpen + " " + strsum + " 16";
                                    //   cmdOpen = "FE FE FE 4C AA AA AA 01 05 B0 02 01 00 03 16";
                                    string strsql = "";
                                    strsql = "insert into CommandPool(CMD_TIME,CMD_BODY,CMD_FLAG)" +
                                    " VALUES('" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "','" + cmdOpen + "','" + '0' + "')";
                                    mainForm.dba.UpdateOrInsertBySQL(strsql);
                                    mainForm.dba.UpdateOrInsertBySQL(strsql);
                                }

                                Log.Instance.LogWrite("文转语播放结束：" + DateTime.Now.ToString());//+ cmdStr);
                                SetText("文转语播放结束" + DateTime.Now.ToString(), Color.Blue);
                                Thread.Sleep(1000);
                                listAreaCode.Clear();//清除应急区域列表
                                //     this.txtMsgShow.Text = "";
                                bCharToAudio = "";
                                //  sendEBMStateResponse(ebd);
                            }
                        }
                        #endregion End
                    }
                    break;
                case "2":
                    {
                        //if (MediaPlayer.playState == WMPLib.WMPPlayState.wmppsStopped || MediaPlayer.playState == WMPLib.WMPPlayState.wmppsMediaEnded)
                        //{
                        //}
                        /*
                        #region 音频播放
                        if (MediaPlayer.playState != WMPLib.WMPPlayState.wmppsPlaying && MediaPlayer.playState != WMPLib.WMPPlayState.wmppsBuffering && MediaPlayer.playState != WMPLib.WMPPlayState.wmppsTransitioning)
                        {
                            iHoldTimesCnt = iHoldTimes;
                            Log.Instance.LogWrite("播放器状态："+MediaPlayer.playState.ToString());
                        }
                        if (iHoldTimesCnt < iHoldTimes)
                        {
                            for (int i = 0; i < listAreaCode.Count; i++)
                            {
                                string cmdOpen = "4C " + listAreaCode[i] + " C0 02 01 04";
                                SendCmd(mainForm.comm, cmdOpen, 1);
                            }
                        }
                        else
                        {
                            timHold.Stop();
                            //string cmdStr = "4C " + EMBCloseAreaCode + " C0 02 00 01";//停止时发关机指令
                            //SendCmd(mainForm.comm, cmdStr, 8);//发送指令 发送8次
                            for (int i = 0; i < listAreaCode.Count; i++)
                            {
                                string cmdOpen = "4C " + listAreaCode[i] + " C0 02 00 01";
                                //  string cmdOpen = "4C AA AA AA AA AA C0 02 01 04";
                                //  string cmdOpen = "FE FE FE 4C AA AA AA AA AA C0 02 01 04 65 16";
                                //  string cmdOpen = "FE FE FE 4C AA AA AA AA AA C0 02 00 04 64 16";
                                //SendCmd(mainForm.comm, cmdOpen, 6);
                                Log.Instance.LogWrite("应急关机：" + cmdOpen);
                                //2016-04-01  改写数据池
                                string strsum = DataSum(cmdOpen);
                                cmdOpen = "FE FE FE " + cmdOpen + " " + strsum + " 16";
                                //   cmdOpen = "FE FE FE 4C AA AA AA 01 05 B0 02 01 00 03 16";
                                string strsql = "";
                                strsql = "insert into CommandPool(CMD_TIME,CMD_BODY,CMD_FLAG)" +
                                " VALUES('" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "','" + cmdOpen + "','" + '0' + "')";
                                mainForm.dba.UpdateOrInsertBySQL(strsql);
                                mainForm.dba.UpdateOrInsertBySQL(strsql);
                            }
                            Log.Instance.LogWrite("语音播放结束：" + DateTime.Now.ToString());// + cmdStr);
                            Thread.Sleep(1000);
                            listAreaCode.Clear();//清除应急区域列表
                            MediaPlayer.Ctlcontrols.stop();
                            MediaPlayer.close();
                            iHoldTimesCnt = 0;
                            //   this.txtMsgShow.Text = "";
                            SetText("播放音频文件结束" + DateTime.Now.ToString());
                            sendEBMStateResponse(ebd);
                            bCharToAudio = "";
                        }
                        #endregion End
                         */
                    }
                    break;
                default:
                    bCharToAudio = "";
                    break;
            }
        }

        private void timHeart_Tick(object sender, EventArgs e)
        {

            XmlDocument xmlHeartDoc = new XmlDocument();
            responseXML rHeart = new responseXML();
            rHeart.SourceAreaCode = strSourceAreaCode;
            rHeart.SourceType = strSourceType;
            rHeart.SourceName = strSourceName;
            rHeart.SourceID = strSourceID;
            rHeart.sHBRONO = strHBRONO;
            try
            {
                xmlHeartDoc = rHeart.HeartBeatResponse();  // rState.EBMStateResponse(ebd);
                string xmlStateFileName = "\\EBDB_000000000009.xml";
                CreateXML(xmlHeartDoc, sHeartSourceFilePath + xmlStateFileName);
                tar.CreatTar(sHeartSourceFilePath, sSendTarPath, "000000000009");//使用新TAR
            }
            catch (Exception ec)
            {
                Log.Instance.LogWrite("心跳处错误：" + ec.Message);
            }
            string sHeartBeatTarName = sSendTarPath + "\\" + "EBDT_000000000009" + ".tar";
            HttpSendFile.UploadFilesByPost(sZJPostUrlAddress, sHeartBeatTarName);

            #region 心跳判断
            if (dtLinkTime != null && dtLinkTime.ToString() != "")
            {
                int timetick = DateDiff(DateTime.Now, dtLinkTime);
                //大于600秒（10分钟）
                if (timetick > OnOffLineInterval)
                {
                    this.Text = "离线";
                }
                else
                {
                    this.Text = "在线";
                }
                if (timetick > OnOffLineInterval * 3)
                {
                    dtLinkTime = DateTime.Now.AddSeconds(-2 * OnOffLineInterval);
                }
            }
            else
            {
                dtLinkTime = DateTime.Now;
            }
            #endregion End
        }

        private void btnHeart_Click(object sender, EventArgs e)
        {

            XmlDocument xmlHeartDoc = new XmlDocument();
            responseXML rHeart = new responseXML();
            rHeart.SourceAreaCode = strSourceAreaCode;
            rHeart.SourceType = strSourceType;
            rHeart.SourceName = strSourceName;
            rHeart.SourceID = strSourceID;
            rHeart.sHBRONO = strHBRONO;
            ServerForm.DeleteFolder(sHeartSourceFilePath);//删除原有XML发送文件的文件夹下的XML
            try
            {
                xmlHeartDoc = rHeart.HeartBeatResponse();
                string HeartName = "01" + rHeart.sHBRONO + "0000000000000000";
                string xmlStateFileName = "EBDB_" + "01" + rHeart.sHBRONO + "0000000000000000.xml";
                CreateXML(xmlHeartDoc, sHeartSourceFilePath + "\\" + xmlStateFileName);

                ServerForm.mainFrm.GenerateSignatureFile(sHeartSourceFilePath, HeartName);
                tar.CreatTar(sHeartSourceFilePath, sSendTarPath, "01" + rHeart.sHBRONO + "0000000000000000");
            }
            catch (Exception ec)
            {
                Log.Instance.LogWrite("心跳处错误：" + ec.Message);
            }
            string sHeartBeatTarName = sSendTarPath + "\\" + "EBDT_" + "01" + rHeart.sHBRONO + "0000000000000000" + ".tar";
            HttpSendFile.UploadFilesByPost(sZJPostUrlAddress, sHeartBeatTarName);

            //XmlDocument xmlHeartDoc = new XmlDocument();
            //responseXML rHeart = new responseXML();
            //rHeart.SourceAreaCode = strSourceAreaCode;
            //rHeart.SourceType = strSourceType;
            //rHeart.SourceName = strSourceName;
            //rHeart.SourceID = strSourceID;
            //rHeart.sHBRONO = strHBRONO;
            //ServerForm.DeleteFolder(sHeartSourceFilePath);//删除原有XML发送文件的文件夹下的XML
            //try
            //{
            //    xmlHeartDoc = rHeart.HeartBeatResponse();
            //    string xmlStateFileName = "EBDB_" + "01" + rHeart.sHBRONO + "0000000000000000.xml";
            //    CreateXML(xmlHeartDoc, sHeartSourceFilePath + "\\" + xmlStateFileName);
            //    tar.CreatTar(sHeartSourceFilePath, sSendTarPath, "01" + rHeart.sHBRONO + "0000000000000000");
            //}
            //catch (Exception ec)
            //{
            //    Log.Instance.LogWrite("心跳处错误：" + ec.Message);
            //}
            //string sHeartBeatTarName = sSendTarPath + "\\" + "EBDT_" + "01" + rHeart.sHBRONO + "0000000000000000" + ".tar";
            //HttpSendFile.UploadFilesByPost(sZJPostUrlAddress, sHeartBeatTarName);

            #region 心跳判断
            if (dtLinkTime != null && dtLinkTime.ToString() != "")
            {
                int timetick = DateDiff(DateTime.Now, dtLinkTime);
                //大于600秒（10分钟）
                if (timetick > OnOffLineInterval)
                {
                    this.Text = "离线";
                }
                else
                {
                    this.Text = "在线";
                }
                if (timetick > OnOffLineInterval * 3)
                {
                    dtLinkTime = DateTime.Now.AddSeconds(-2 * OnOffLineInterval);
                }
            }
            else
            {
                dtLinkTime = DateTime.Now;
            }
            #endregion End
        }

        //线程间同步
        public void SetText(string text, Color colo)
        {
            if (this.txtMsgShow.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text, colo });
            }
            else
            {
                string strs = this.txtMsgShow.Text;
                string[] strR = strs.Split("\r\n".ToCharArray());     //\r\n   为回车符号   
                int i = strR.Length - 1;     //得到   strR数组   的长度   
                if (i > 200)
                {
                    this.txtMsgShow.Clear();
                    this.txtMsgShow.Refresh();
                }
                this.txtMsgShow.ForeColor = colo;
                this.txtMsgShow.AppendText(text);
                this.txtMsgShow.AppendText(Environment.NewLine);
            }
        }

        private void tim_MediaPlay_Tick(object sender, EventArgs e)
        {

        }

        //定时释放内存
        private void tim_ClearMemory_Tick(object sender, EventArgs e)
        {
            ClearMemory();
        }

        #region 内存回收 //2016-04-25 add
        [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize")]
        public static extern int SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);
        /// <summary>
        /// 释放内存
        /// </summary>
        public static void ClearMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
            }
        }
        #endregion

        private void ServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (tim_MediaPlay.Enabled)    //定时查询媒体播放定时器
                {
                    tim_MediaPlay.Enabled = false;
                }
                if (tim_ClearMemory.Enabled)  //清除内存垃圾定时器
                {
                    tim_ClearMemory.Enabled = false;
                }

                if (thTar != null)
                {
                    thTar.Abort();
                    //thTar = null;
                }
                if (thFeedBack != null)
                {
                    thFeedBack.Abort();
                }
                if (httpthread != null)
                {
                    httpthread.Abort();
                    httpthread = null;
                }
                httpServer.StopListen();
                MQDLL.StopActiveMQ();
            }
            catch (Exception em)
            {
                Log.Instance.LogWrite("ServerFormCloseing停止线程错误：" + em.Message);
            }
        }

        //平台信息上报
        private void button1_Click(object sender, EventArgs e)
        {
            XmlDocument xmlHeartDoc = new XmlDocument();
            responseXML rHeart = new responseXML();
            rHeart.SourceAreaCode = strSourceAreaCode;
            rHeart.SourceType = strSourceType;
            rHeart.SourceName = strSourceName;
            rHeart.SourceID = strSourceID;
            rHeart.sHBRONO = strHBRONO;

            ServerForm.DeleteFolder(sHeartSourceFilePath);//删除原有XML发送文件的文件夹下的XML
            string frdStateName = "";
            try
            {
                Random rdState = new Random();
                frdStateName = "10" + rHeart.sHBRONO + "0000000000000" + rdState.Next(100, 999).ToString();
                string xmlEBMStateFileName = "\\EBDB_" + frdStateName + ".xml";

                xmlHeartDoc = rHeart.platformInfoResponse(frdStateName);
                CreateXML(xmlHeartDoc, sHeartSourceFilePath + xmlEBMStateFileName);
                ServerForm.mainFrm.GenerateSignatureFile(sHeartSourceFilePath, frdStateName);
                ServerForm.tar.CreatTar(sHeartSourceFilePath, ServerForm.sSendTarPath, frdStateName);//使用新TAR
                string sHeartBeatTarName = sSendTarPath + "\\" + "EBDT_" + frdStateName + ".tar";
                HttpSendFile.UploadFilesByPost(sZJPostUrlAddress, sHeartBeatTarName);
            }
            catch
            {
            }

        }

        //终端状态上报
        private void button2_Click(object sender, EventArgs e)
        {
            XmlDocument xmlHeartDoc = new XmlDocument();
            responseXML rHeart = new responseXML();
            rHeart.SourceAreaCode = strSourceAreaCode;
            rHeart.SourceType = strSourceType;
            rHeart.SourceName = strSourceName;
            rHeart.SourceID = strSourceID;
            rHeart.sHBRONO = strHBRONO;
            string MediaSql = "";
            string strSRV_ID = "";
            string strSRV_CODE = "";
            ServerForm.DeleteFolder(sHeartSourceFilePath);//删除原有XML发送文件的文件夹下的XML
            string frdStateName = "";
            List<Device> lDev = new List<Device>();
            try
            {
                MediaSql = "select SRV_ID,SRV_CODE,SRV_GOOGLE from SRV";
                DataTable dtMedia = mainForm.dba.getQueryInfoBySQL(MediaSql);
                if (dtMedia != null && dtMedia.Rows.Count > 0)
                {
                    if (dtMedia.Rows.Count > 100)
                    {
                        int mod = dtMedia.Rows.Count / 100 + 1;
                        for (int i = 0; i < mod; i++)
                        {
                            for (int idtM = 0; idtM < dtMedia.Rows.Count; idtM++)
                            {
                                string TrLL = dtMedia.Rows[idtM][2].ToString();
                                Device DV = new Device();
                                strSRV_ID = dtMedia.Rows[idtM][0].ToString().Trim();
                                strSRV_CODE = dtMedia.Rows[idtM][1].ToString();
                                DV.DeviceID = strSRV_ID;
                                DV.DeviceName = strSRV_ID;
                                if (TrLL != "")
                                {
                                    string[] str = TrLL.Split(',');
                                    if (str.Length >= 2)
                                    {
                                        DV.Longitude = str[1];
                                        DV.Latitude = str[0];
                                    }
                                    else
                                    {
                                        DV.Longitude = SingletonInfo.GetInstance().Longitude;
                                        DV.Latitude = SingletonInfo.GetInstance().Latitude;
                                    }
                                }
                                else
                                {
                                    DV.Longitude = SingletonInfo.GetInstance().Longitude;
                                    DV.Latitude = SingletonInfo.GetInstance().Latitude;
                                }
                                lDev.Add(DV);
                            }
                            Random rdState = new Random();
                            frdStateName = "10" + rHeart.sHBRONO + "0000000000000" + rdState.Next(100, 999).ToString();
                            string xmlEBMStateFileName = "\\EBDB_" + frdStateName + ".xml";

                            xmlHeartDoc = rHeart.DeviceStateResponse(lDev, frdStateName);
                            CreateXML(xmlHeartDoc, sHeartSourceFilePath + xmlEBMStateFileName);
                            ServerForm.mainFrm.GenerateSignatureFile(sHeartSourceFilePath, frdStateName);
                            ServerForm.tar.CreatTar(sHeartSourceFilePath, ServerForm.sSendTarPath, frdStateName);//使用新TAR
                            string sHeartBeatTarName = sSendTarPath + "\\" + "EBDT_" + frdStateName + ".tar";
                            HttpSendFile.UploadFilesByPost(sZJPostUrlAddress, sHeartBeatTarName);
                        }
                    }
                    else
                    {
                        for (int idtM = 0; idtM < dtMedia.Rows.Count; idtM++)
                        {
                            Device DV = new Device();
                            strSRV_ID = dtMedia.Rows[idtM][0].ToString();
                            strSRV_CODE = dtMedia.Rows[idtM][1].ToString();
                            DV.DeviceID = strSRV_ID;
                            DV.DeviceName = strSRV_ID;
                            lDev.Add(DV);
                        }
                        Random rdState = new Random();
                        frdStateName = "10" + rHeart.sHBRONO + "0000000000000" + rdState.Next(100, 999).ToString();
                        string xmlEBMStateFileName = "\\EBDB_" + frdStateName + ".xml";

                        xmlHeartDoc = rHeart.DeviceStateResponse(lDev, frdStateName);
                        CreateXML(xmlHeartDoc, sHeartSourceFilePath + xmlEBMStateFileName);
                        ServerForm.mainFrm.GenerateSignatureFile(sHeartSourceFilePath, frdStateName);
                        ServerForm.tar.CreatTar(sHeartSourceFilePath, ServerForm.sSendTarPath, frdStateName);//使用新TAR
                        string sHeartBeatTarName = sSendTarPath + "\\" + "EBDT_" + frdStateName + ".tar";
                        HttpSendFile.UploadFilesByPost(sZJPostUrlAddress, sHeartBeatTarName);
                    }
                }
                else
                {
                    Random rdState = new Random();
                    frdStateName = "10" + rHeart.sHBRONO + "0000000000000" + rdState.Next(100, 999).ToString();
                    string xmlEBMStateFileName = "\\EBDB_" + frdStateName + ".xml";

                    xmlHeartDoc = rHeart.DeviceStateResponse(lDev, frdStateName);
                    CreateXML(xmlHeartDoc, sHeartSourceFilePath + xmlEBMStateFileName);
                    ServerForm.mainFrm.GenerateSignatureFile(sHeartSourceFilePath, frdStateName);
                    ServerForm.tar.CreatTar(sHeartSourceFilePath, ServerForm.sSendTarPath, frdStateName);//使用新TAR
                    string sHeartBeatTarName = sSendTarPath + "\\" + "EBDT_" + frdStateName + ".tar";
                    HttpSendFile.UploadFilesByPost(sZJPostUrlAddress, sHeartBeatTarName);
                }
            }
            catch
            {
            }
        }

        //平台状态上报
        private void button3_Click(object sender, EventArgs e)
        {
            XmlDocument xmlHeartDoc = new XmlDocument();
            responseXML rHeart = new responseXML();
            rHeart.SourceAreaCode = strSourceAreaCode;
            rHeart.SourceType = strSourceType;
            rHeart.SourceName = strSourceName;
            rHeart.SourceID = strSourceID;
            rHeart.sHBRONO = strHBRONO;

            ServerForm.DeleteFolder(sHeartSourceFilePath);//删除原有XML发送文件的文件夹下的XML
            string frdStateName = "";
            try
            {
                Random rdState = new Random();
                frdStateName = "10" + rHeart.sHBRONO + "0000000000000" + rdState.Next(100, 999).ToString();
                string xmlEBMStateFileName = "\\EBDB_" + frdStateName + ".xml";

                xmlHeartDoc = rHeart.platformstateInfoResponse(frdStateName);
                CreateXML(xmlHeartDoc, sHeartSourceFilePath + xmlEBMStateFileName);

                ServerForm.mainFrm.GenerateSignatureFile(sHeartSourceFilePath, frdStateName);

                ServerForm.tar.CreatTar(sHeartSourceFilePath, ServerForm.sSendTarPath, frdStateName);//使用新TAR
                string sHeartBeatTarName = sSendTarPath + "\\" + "EBDT_" + frdStateName + ".tar";
                HttpSendFile.UploadFilesByPost(sZJPostUrlAddress, sHeartBeatTarName);
            }
            catch
            {
            }

        }

        //终端信息上报
        private void button4_Click(object sender, EventArgs e)
        {
            XmlDocument xmlHeartDoc = new XmlDocument();
            responseXML rHeart = new responseXML();
            rHeart.SourceAreaCode = strSourceAreaCode;
            rHeart.SourceType = strSourceType;
            rHeart.SourceName = strSourceName;
            rHeart.SourceID = strSourceID;
            rHeart.sHBRONO = strHBRONO;
            string MediaSql = "";
            string strSRV_ID = "";
            string strSRV_CODE = "";
            ServerForm.DeleteFolder(sHeartSourceFilePath);//删除原有XML发送文件的文件夹下的XML
            string frdStateName = "";
            List<Device> lDev = new List<Device>();
            try
            {
                MediaSql = "select top(20) SRV_ID,SRV_CODE from SRV";
                DataTable dtMedia = mainForm.dba.getQueryInfoBySQL(MediaSql);
                if (dtMedia != null && dtMedia.Rows.Count > 0)
                {
                    if (dtMedia.Rows.Count > 100)
                    {
                        int mod = dtMedia.Rows.Count / 100 + 1;
                        for (int i = 0; i < mod; i++)
                        {
                            for (int idtM = 0; idtM < dtMedia.Rows.Count; idtM++)
                            {
                                Device DV = new Device();
                                strSRV_ID = dtMedia.Rows[idtM][0].ToString();
                                strSRV_CODE = dtMedia.Rows[idtM][1].ToString();
                                DV.DeviceID = strSRV_ID;
                                DV.DeviceName = strSRV_ID;
                                lDev.Add(DV);
                            }
                            Random rdState = new Random();
                            frdStateName = "10" + rHeart.sHBRONO + "0000000000000" + rdState.Next(100, 999).ToString();
                            string xmlEBMStateFileName = "\\EBDB_" + frdStateName + ".xml";

                            xmlHeartDoc = rHeart.DeviceInfoResponse(lDev, frdStateName);
                            CreateXML(xmlHeartDoc, sHeartSourceFilePath + xmlEBMStateFileName);
                            ServerForm.mainFrm.GenerateSignatureFile(sHeartSourceFilePath, frdStateName);
                            ServerForm.tar.CreatTar(sHeartSourceFilePath, ServerForm.sSendTarPath, frdStateName);//使用新TAR
                            string sHeartBeatTarName = sSendTarPath + "\\" + "EBDT_" + frdStateName + ".tar";
                            HttpSendFile.UploadFilesByPost(sZJPostUrlAddress, sHeartBeatTarName);
                        }
                    }
                    else
                    {
                        for (int idtM = 0; idtM < dtMedia.Rows.Count; idtM++)
                        {
                            Device DV = new Device();
                            strSRV_ID = dtMedia.Rows[idtM][0].ToString();
                            strSRV_CODE = dtMedia.Rows[idtM][1].ToString();
                            DV.DeviceID = strSRV_ID;
                            DV.DeviceName = strSRV_ID;
                            lDev.Add(DV);
                        }
                        Random rdState = new Random();
                        frdStateName = "10" + rHeart.sHBRONO + "0000000000000" + rdState.Next(100, 999).ToString();
                        string xmlEBMStateFileName = "\\EBDB_" + frdStateName + ".xml";

                        xmlHeartDoc = rHeart.DeviceInfoResponse(lDev, frdStateName);
                        CreateXML(xmlHeartDoc, sHeartSourceFilePath + xmlEBMStateFileName);

                        ServerForm.mainFrm.GenerateSignatureFile(sHeartSourceFilePath, frdStateName);

                        ServerForm.tar.CreatTar(sHeartSourceFilePath, ServerForm.sSendTarPath, frdStateName);//使用新TAR
                        string sHeartBeatTarName = sSendTarPath + "\\" + "EBDT_" + frdStateName + ".tar";
                        HttpSendFile.UploadFilesByPost(sZJPostUrlAddress, sHeartBeatTarName);
                    }

                }
                else
                {
                    Random rdState = new Random();
                    frdStateName = "10" + rHeart.sHBRONO + "0000000000000" + rdState.Next(100, 999).ToString();
                    string xmlEBMStateFileName = "\\EBDB_" + frdStateName + ".xml";

                    xmlHeartDoc = rHeart.DeviceInfoResponse(lDev, frdStateName);
                    CreateXML(xmlHeartDoc, sHeartSourceFilePath + xmlEBMStateFileName);

                    ServerForm.mainFrm.GenerateSignatureFile(sHeartSourceFilePath, frdStateName);

                    ServerForm.tar.CreatTar(sHeartSourceFilePath, ServerForm.sSendTarPath, frdStateName);//使用新TAR
                    string sHeartBeatTarName = sSendTarPath + "\\" + "EBDT_" + frdStateName + ".tar";
                    HttpSendFile.UploadFilesByPost(sZJPostUrlAddress, sHeartBeatTarName);
                }
            }
            catch
            {
            }
        }

        //XML解析测试
        private void button5_Click(object sender, EventArgs e)
        {
            #region Test1

            //ccplay.StopCPPPlayer2();
            //string strSql = "delete  from PLAYRECORD";
            //mainForm.dba.UpdateDbBySQL(strSql);

            //    string sSignFileName = "E://EBDS_EBDB_100102320000000000010000000000005304.xml";
            //    using (FileStream SignFs = new FileStream(sSignFileName, FileMode.Open))
            //    {
            //        StreamReader signsr = new StreamReader(SignFs, System.Text.Encoding.UTF8);
            //        string xmlsign = signsr.ReadToEnd();
            //        //xmlsign = xmlsign.Replace("xmlns:xs", "xmlns");
            //        signsr.Close();
            //        responseXML signrp = new responseXML();//签名回复
            //        XmlDocument xmlSignDoc = new XmlDocument();
            //        try
            //        {
            //            xmlsign = XmlSerialize.ReplaceLowOrderASCIICharacters(xmlsign);
            //            xmlsign = XmlSerialize.GetLowOrderASCIICharacters(xmlsign);
            //            Signature sign = XmlSerialize.DeserializeXML<Signature>(xmlsign);

            //            int nDeviceHandle = (int)mainFrm.phDeviceHandle;

            //            byte[] strpcData;
            //            byte[] pucCounter = new byte[4];

            //            pucCounter[0] = 0X00;
            //            pucCounter[1] = 0X00;
            //            pucCounter[2] = 0X00;
            //            pucCounter[3] = 0X35;

            //            byte[] pucSignCerSn = new byte[6];

            //            pucSignCerSn[0] = 0X00;
            //            pucSignCerSn[1] = 0X00;
            //            pucSignCerSn[2] = 0X00;
            //            pucSignCerSn[3] = 0X00;
            //            pucSignCerSn[4] = 0X00;
            //            pucSignCerSn[5] = 0X84;

            //            Encoding myEncoding = Encoding.GetEncoding("utf-8");
            //            strpcData = Convert.FromBase64String(sign.SignatureValue);

            //            mainFrm.usb.VerifySignatureWithTrustedCert(ref nDeviceHandle, strpcData, strpcData.Length, pucCounter, pucSignCerSn, strpcData);
            //        }
            //        catch (Exception ex)
            //        {
            //            Log.Instance.LogWrite("签名文件错误：" + ex.Message);
            //           // xmlSignDoc = signrp.SignResponse("", "Error");
            //        }
            //        xmlSignDoc.Save(sSourcePath + "\\EBDSign.xml");
            //    } 
            #endregion
            try
            {

                int nDeviceHandle = (int)mainFrm.phDeviceHandle;
                //mainFrm.usb.PlatformVerifySignature();
                DeleteFolder(sUnTarPath);//删除原有tar解压文件 
                tar.UnpackTarFiles("D://Abutment\\93\\SendTarFilePath\\EBDT_100102321323000000010000000000000974.tar", sUnTarPath);//解压Tar包
                string[] xmlfilenames = Directory.GetFiles(sUnTarPath, "*.xml");//从解压XML文件夹下获取解压的XML文件名
                string sTmpFile = string.Empty;
                string sAnalysisFileName = string.Empty;
                string sSignFileName = string.Empty;

                for (int i = 0; i < xmlfilenames.Length; i++)
                {
                    sTmpFile = Path.GetFileName(xmlfilenames[i]);
                    if (sTmpFile.ToUpper().IndexOf("EBDB") > -1 && sTmpFile.ToUpper().IndexOf("EBDS_EBDB") < 0)
                    {
                        sAnalysisFileName = xmlfilenames[i];
                    }
                    else if (sTmpFile.ToUpper().IndexOf("EBDS_EBDB") > -1)//签名文件
                    {
                        sSignFileName = xmlfilenames[i];//签名文件
                    }
                }
                DeleteFolder(sSourcePath);//删除原有XML发送文件的文件夹下的XML

                if (sSignFileName == "")
                {
                    //continue;
                }
                else
                {
                    Console.WriteLine("开始验证签名文件!");
                    byte[] pucSignature = null;
                    byte[] pucsingVi = null;
                    string PucStr = string.Empty;
                    using (FileStream SignFs = new FileStream(sAnalysisFileName, FileMode.Open))
                    {
                        StreamReader signsr = new StreamReader(SignFs, System.Text.Encoding.UTF8);
                        string xmlsign = signsr.ReadToEnd();
                        signsr.Close();
                        responseXML signrp = new responseXML();//签名回复
                        XmlDocument xmlSignDoc = new XmlDocument();
                        pucSignature = Encoding.UTF8.GetBytes(xmlsign);
                        Console.WriteLine(xmlsign);
                        Console.WriteLine(pucSignature.Length);
                    }
                    using (FileStream SignFs = new FileStream(sSignFileName, FileMode.Open))
                    {
                        StreamReader signsr = new StreamReader(SignFs, System.Text.Encoding.UTF8);
                        string xmlsign = signsr.ReadToEnd();
                        signsr.Close();
                        responseXML signrp = new responseXML();//签名回复
                        XmlDocument xmlSignDoc = new XmlDocument();
                        try
                        {
                            xmlsign = XmlSerialize.ReplaceLowOrderASCIICharacters(xmlsign);
                            xmlsign = XmlSerialize.GetLowOrderASCIICharacters(xmlsign);
                            Signature sign = XmlSerialize.DeserializeXML<Signature>(xmlsign);
                            PucStr = sign.SignatureValue;
                            pucsingVi = Encoding.UTF8.GetBytes(sign.SignatureValue);
                            Console.WriteLine(PucStr);
                            Console.WriteLine(pucsingVi.Length);
                        }
                        catch (Exception ex)
                        {
                            Log.Instance.LogWrite("签名文件错误：" + ex.Message);
                        }
                    }
                    byte[] ddd = new byte[1010];
                    byte[] ddee = new byte[1124];

                    mainFrm.usb.PlatformVerifySignature(nDeviceHandle, 1, pucSignature, pucSignature.Length, pucsingVi);
                    Console.WriteLine("结束验证签名文件！");
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("验签有异常: " + ex.Message);
            }
        }


        /// <summary>
        /// 定时的心跳反馈包
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void HeartUP(object source, System.Timers.ElapsedEventArgs e)
        {
            XmlDocument xmlHeartDoc = new XmlDocument();
            responseXML rHeart = new responseXML();
            rHeart.SourceAreaCode = strSourceAreaCode;
            rHeart.SourceType = strSourceType;
            rHeart.SourceName = strSourceName;
            rHeart.SourceID = strSourceID;
            rHeart.sHBRONO = strHBRONO;
            DeleteFolder(TimesHeartSourceFilePath);//删除原有XML发送文件的文件夹下的XML
            try
            {
                xmlHeartDoc = rHeart.HeartBeatResponse();
                string HreartName = "01" + rHeart.sHBRONO + "0000000000000000";
                string xmlStateFileName = "EBDB_" + "01" + rHeart.sHBRONO + "0000000000000000.xml";
                CreateXML(xmlHeartDoc, TimesHeartSourceFilePath + "\\" + xmlStateFileName);
               // ServerForm.mainFrm.GenerateSignatureFile(TimesHeartSourceFilePath, "01" + rHeart.sHBRONO + "0000000000000000");
                tar.CreatTar(TimesHeartSourceFilePath, sSendTarPath, "01" + rHeart.sHBRONO + "0000000000000000");
            }
            catch (Exception ec)
            {
                Log.Instance.LogWrite("心跳处错误：" + ec.Message);
            }
            string sHeartBeatTarName = sSendTarPath + "\\" + "EBDT_" + "01" + rHeart.sHBRONO + "0000000000000000" + ".tar";
            string pp= HttpSendFile.UploadFilesByPost(sZJPostUrlAddress, sHeartBeatTarName);



            #region 心跳判断
            if (pp=="1")
            {
                //发送成功
                if (dtLinkTime != null && dtLinkTime.ToString() != "")
                {
                    int timetick = DateDiff(DateTime.Now, dtLinkTime);
                    //大于600秒（10分钟）
                    if (timetick > OnOffLineInterval)
                    {
                        this.Text = "离线";
                    }
                    else
                    {
                        this.Text = "在线";
                    }
                    if (timetick > OnOffLineInterval * 3)
                    {
                        dtLinkTime = DateTime.Now.AddSeconds(-2 * OnOffLineInterval);
                    }
                }
                else
                {
                    dtLinkTime = DateTime.Now;
                }
            }
            else
            {
                this.Text = "离线";
            }
            #endregion End
            Thread.Sleep(1000);
        }

        private void StateOrInfoUp(string strOMDType)
        {
            try
            {
                XmlDocument xmlStateDoc = new XmlDocument();
                responseXML rState = new responseXML();
                rState.SourceAreaCode = strSourceAreaCode;
                rState.SourceType = strSourceType;
                rState.SourceName = strSourceName;
                rState.SourceID = strSourceID;
                rState.sHBRONO = strHBRONO;
                Random rdState = new Random();
                string frdStateName = "10" + rState.sHBRONO + "0000000000000" + rdState.Next(100, 999).ToString();
                string xmlEBMStateFileName = "\\EBDB_" + frdStateName + ".xml";
                List<Device> lDev = new List<Device>();
                lock (OMDRequestLock)
                {
                    TarOMRequest(xmlStateDoc, rState, strOMDType, frdStateName, xmlEBMStateFileName, lDev);
                } 
            }
            catch (Exception h)
            {
                Log.Instance.LogWrite("错误510行:" + h.Message);
            }
        }

        /// <summary>
        /// 终端状态上报
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void SrvStateUP(object source, System.Timers.ElapsedEventArgs e)
        {
            string strOMDType = ReturnInfrom(4);
            if (!string.IsNullOrWhiteSpace(strOMDType))
            {
                StateOrInfoUp(strOMDType);
            }

          
        }

        /// <summary>
        /// 终端信息上报
        /// </summary>
        private void SrvInfromUP(object source, System.Timers.ElapsedEventArgs e)
        {
            string strOMDType = ReturnInfrom(2);
            if (!string.IsNullOrWhiteSpace(strOMDType))
            {
                StateOrInfoUp(strOMDType);
            }
        }

        /// <summary>
        /// 平台状态上报
        /// </summary>
        private void TerraceStateUP(object source, System.Timers.ElapsedEventArgs e)
        {
            string strOMDType = ReturnInfrom(3);
            if (!string.IsNullOrWhiteSpace(strOMDType))
            {
                StateOrInfoUp(strOMDType);
            }
        }

        /// <summary>
        /// 平台信息上报
        /// </summary>
        private void TerraceInfrom(object source, System.Timers.ElapsedEventArgs e)
        {
            string strOMDType = ReturnInfrom(1);
            if (!string.IsNullOrWhiteSpace(strOMDType))
            {
                StateOrInfoUp(strOMDType);
            }
        }

        private void CreateXML(XmlDocument XD, string Path)
        {
            CommonFunc ComX = new CommonFunc();
            ComX.SaveXmlWithUTF8NotBOM(XD, Path);
            if (ComX != null)
            {
                ComX = null;
            }
        }

        /// <summary>
        /// ccplayer推流播放停止计时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerCcplayer(object source, System.Timers.ElapsedEventArgs e)
        {
            if (ccplayerStopTime < DateTime.Now)
            {
                try
                {
                    SetText("停止播发：" + DateTime.Now.ToString() + "EBM文件日期: " + ccplayerStopTime, Color.Red);
                    ccplay.StopCPPPlayer2();
                    string strSql = string.Format("update PLAYRECORD set PR_REC_STATUS = '{0}'", "删除");
                    strSql += " update EBMInfo set EBMState=1 where SEBDID='" + SEBDIDStatusFlag + "' ";
                    mainForm.dba.UpdateDbBySQL(strSql);
                    Tccplayer.Enabled = false;
                }
                catch (Exception ex)
                {
                    Log.Instance.LogWrite("直播停止ccplayer推流：" + ex.Message);
                }
            }
            Thread.Sleep(20);
        }

        private void btn_InfroState_Click(object sender, EventArgs e)
        {
            string StateFaleText = btn_InfroState.Text;
            if (StateFaleText == "开启信息状态上报")
            {
                tSrvState.Enabled = true;
                tSrvInfo.Enabled = true;
                tTerraceInfrom.Enabled = true;
                tTerraceState.Enabled = true;
                //InfromActiveTime.Enabled = true;
                btn_InfroState.Text = "关闭信息状态上报";
            }
            else
            {
                tSrvState.Enabled = false;
                tSrvInfo.Enabled = false;
                tTerraceInfrom.Enabled = false;
                tTerraceState.Enabled = false;
                //InfromActiveTime.Enabled = false;
                btn_InfroState.Text = "开启信息状态上报";
            }
        }

        //心跳定时上报
        private void btn_HreartState_Click(object sender, EventArgs e)
        {
            string StateFaleText = btn_HreartState.Text;
            if (StateFaleText == "开启心跳状态上报")
            {
                t.Enabled = true;
                btn_HreartState.Text = "关闭心跳状态上报";
            }
            else
            {
                t.Enabled = false;
                btn_HreartState.Text = "开启心跳状态上报";
            }
        }

        private void btn_Verify_Click(object sender, EventArgs e)
        {
            //EBMVerifyState
            string StateFaleText = btn_Verify.Text;
            if (StateFaleText == "人工审核状态")
            {
                serverini.WriteValue("EBD", "EBMState", "True");
                EBMVerifyState = true;//人工审核状态  true  表示已开启
                btn_Verify.Text = "自动审核状态";
            //    SingletonInfo.GetInstance().CheckEBMStatusFlag = "1";
            }
            else
            {
                serverini.WriteValue("EBD", "EBMState", "False");
                EBMVerifyState = false;
                btn_Verify.Text = "人工审核状态";
               // SingletonInfo.GetInstance().CheckEBMStatusFlag = "0";
            }
        }

        //手动审核任务列表中审核事件
        private void list_PendingTask_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //if (this.list_PendingTask.SelectedItems.Count > 0)
            //{
            //    string EBMPath = this.list_PendingTask.FocusedItem.SubItems[1].Text; 
            //    AnalysisEBM(EBMPath);
            //}
        }

        /// <summary>
        /// 手动审核下发应急包
        /// </summary>
        /// <param name="EBMPath">EBM路径</param>
        private void AnalysisEBM(string EBMPath)
        {
            List<string> lDealTarFiles = new List<string>();
            List<string> AudioFileListTmp = new List<string>();//收集的音频文件列表
            List<string> AudioFileList = new List<string>();//收集的音频文件列表

            SetText("解压文件：" + EBMPath.ToString(), Color.Green);
            try
            {
                #region 解压
                if (File.Exists(EBMPath))
                {
                    try
                    {
                        DeleteFolder(sUnTarPath);
                        tar.UnpackTarFiles(EBMPath, sUnTarPath);
                        //把压缩包解压到专门存放接收到的XML文件的文件夹下
                        SetText("解压文件：" + EBMPath + "成功", Color.Green);
                    }
                    catch (Exception exa)
                    {
                        SetText("删除解压文件夹：" + sUnTarPath + "文件失败!错误信息：" + exa.Message, Color.Red);
                    }
                }
                #endregion 解压
            }
            catch (Exception ex)
            {
                Log.Instance.LogWrite("解压出错：" + ex.Message);
            }
            try
            {
                string[] xmlfilenames = Directory.GetFiles(sUnTarPath, "*.xml");//从解压XML文件夹下获取解压的XML文件名
                string sTmpFile = string.Empty;
                string sAnalysisFileName = "";
                string sSignFileName = "";

                for (int i = 0; i < xmlfilenames.Length; i++)
                {
                    sTmpFile = Path.GetFileName(xmlfilenames[i]);
                    if (sTmpFile.ToUpper().IndexOf("EBDB") > -1 && sTmpFile.ToUpper().IndexOf("EBDS_EBDB") < 0)
                    {
                        sAnalysisFileName = xmlfilenames[i];
                    }
                    else if (sTmpFile.ToUpper().IndexOf("EBDS_EBDB") > -1)//签名文件
                    {
                        sSignFileName = xmlfilenames[i];//签名文件
                    }
                }
                DeleteFolder(sSourcePath);//删除原有XML发送文件的文件夹下的XML

                if (sSignFileName == "")
                {
                    //验证签名功能
                }
                else
                {
                    #region 签名处理
                    Console.WriteLine("开始验证签名文件!");
                    using (FileStream SignFs = new FileStream(sSignFileName, FileMode.Open))
                    {
                        StreamReader signsr = new StreamReader(SignFs, System.Text.Encoding.UTF8);
                        string xmlsign = signsr.ReadToEnd();
                        signsr.Close();
                        responseXML signrp = new responseXML();//签名回复
                        XmlDocument xmlSignDoc = new XmlDocument();
                        try
                        {
                            xmlsign = XmlSerialize.ReplaceLowOrderASCIICharacters(xmlsign);
                            xmlsign = XmlSerialize.GetLowOrderASCIICharacters(xmlsign);
                            Signature sign = XmlSerialize.DeserializeXML<Signature>(xmlsign);
                        }
                        catch (Exception ex)
                        {
                            Log.Instance.LogWrite("签名文件错误：" + ex.Message);
                        }
                    }
                    Console.WriteLine("结束验证签名文件！");
                    #endregion End
                }

                if (sAnalysisFileName != "")
                {
                    using (FileStream fs = new FileStream(sAnalysisFileName, FileMode.Open))
                    {
                        StreamReader sr = new StreamReader(fs, Encoding.UTF8);
                        String xmlInfo = sr.ReadToEnd();
                        xmlInfo = xmlInfo.Replace("xmlns:xs", "xmlns");
                        sr.Close();
                        xmlInfo = XmlSerialize.ReplaceLowOrderASCIICharacters(xmlInfo);
                        xmlInfo = XmlSerialize.GetLowOrderASCIICharacters(xmlInfo);
                        ebd = XmlSerialize.DeserializeXML<EBD>(xmlInfo);
                        if (ebd.EBM.MsgBasicInfo.MsgType == "2")
                        {
                            if (MessageBox.Show("请确定是否要下发关机指令", "应急关机包", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                                == DialogResult.Yes)
                            {
                                SetText("停止播发：" + DateTime.Now.ToString(), Color.Red);
                                string strSql = string.Format("update PLAYRECORD set PR_REC_STATUS = '{0}' where PR_SourceID='{1}'", "删除", TsCmdStoreID);
                                strSql += " update EBMInfo set EBMState=1 where SEBDID='" + SEBDIDStatusFlag + "' ";
                                strSql += "delete from InfoVlaue";
                                //string strSql = "update PLAYRECORD set PR_REC_STATUS = '删除'";
                                mainForm.dba.UpdateDbBySQL(strSql);
                                Tccplayer.Enabled = false;
                                ccplay.StopCPPPlayer2();
                                RealAudioFlag = false;//标记为已经执行
                                list_PendingTask.Items.Remove(list_PendingTask.FocusedItem);
                                return;
                            }
                            else
                            {
                                return;
                            }
                        }
                        AudioFileListTmp.Clear();
                        AudioFileList.Clear();
                        string[] mp3files = Directory.GetFiles(sUnTarPath, "*.mp3");
                        AudioFileListTmp.AddRange(mp3files);
                        string[] wavfiles = Directory.GetFiles(sUnTarPath, "*.wav");
                        AudioFileListTmp.AddRange(wavfiles);
                        EBMInfo EBMInfo = new EBMInfo();
                        EBMInfo.ebd = ebd;
                        if (AudioFileListTmp.Count > 0)
                        {
                            EBMInfo.AudioUrl = AudioFileListTmp[0];
                        }
                        EBMInfo.ShowDialog();
                        if (EBMInfo.DialogResult == DialogResult.OK)
                        {
                            list_PendingTask.Items.Remove(list_PendingTask.FocusedItem);
                            string sqlstr = "";
                            if (AudioFileListTmp.Count > 0)
                            {
                                string sTmpDealFile = string.Empty;
                                string targetPath = string.Empty;
                                string strurl = "";
                                string sDateTime = "";
                                string sStartTime = ebd.EBM.MsgBasicInfo.StartTime;
                                string sEndDateTime = ebd.EBM.MsgBasicInfo.EndTime;
                                // string sGBCode = "";
                                string sORG_ID = "";
                                string sAread = "";
                                string xmlFilePath = "";
                                //if ((AudioFlag == "2")&&(TextFirst=="2")) //拷贝xml文件
                                {
                                    string xmlFile = Path.GetFileName(sAnalysisFileName);
                                    xmlFilePath = sAudioFilesFolder + "\\" + xmlFile;
                                    File.Copy(sAnalysisFileName, xmlFilePath, true);
                                }
                                for (int ai = 0; ai < AudioFileListTmp.Count; ai++)
                                {
                                    sTmpDealFile = Path.GetFileName(AudioFileListTmp[ai]);
                                    targetPath = sAudioFilesFolder + "\\" + sTmpDealFile;
                                    File.Copy(AudioFileListTmp[ai], targetPath, true);
                                    AudioFileList.Add(targetPath);


                                    SetText("EBM开始时间: " + ebd.EBM.MsgBasicInfo.StartTime + "===>EBM结束时间: " + ebd.EBM.MsgBasicInfo.EndTime, Color.Blue);
                                    DateTime EbStartTime = DateTime.Parse(ebd.EBM.MsgBasicInfo.StartTime).AddSeconds(2);
                                    if (EbStartTime < DateTime.Now)
                                    {
                                        EbStartTime = DateTime.Now.AddSeconds(2);
                                    }

                                    sDateTime = EbStartTime.ToString("yyyy-MM-dd HH:mm:ss");  //ebd.EBM.MsgBasicInfo.StartTime;
                                    sStartTime = EbStartTime.ToString("yyyy-MM-dd HH:mm:ss"); //ebd.EBM.MsgBasicInfo.StartTime;
                                    sEndDateTime = ebd.EBM.MsgBasicInfo.EndTime;
                                    if (TEST == "YES")
                                    {
                                        sDateTime = DateTime.Now.AddSeconds(2).ToString("yyyy-MM-dd HH:mm:ss");
                                        sStartTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        sEndDateTime = DateTime.Now.AddMinutes(5).ToString("yyyy-MM-dd HH:mm:ss");//ebd.EBM.MsgBasicInfo.EndTime;
                                    }

                                    SetText("开始时间: " + sDateTime + "===>结束时间: " + sEndDateTime + "是否是TEST:" + TEST, Color.Blue);
                                    sAread = ebd.EBM.MsgContent.AreaCode; //区域
                                    sORG_ID = ebd.EBM.EBMID;
                                    strurl = targetPath;  //音频文件地址
                                    // sqlstr = "insert into TsCmdStoreMedia(TsCmd_Type, TsCmd_Mode, TsCmd_UserID, TsCmd_ValueID, TsCmd_Params, TsCmd_Date, TsCmd_Status,TsCmd_StartTime,TsCmd_EndTime,TsCmd_XmlFile)" +
                                    //        "values('播放音频', '" + sAread + "', 1, '" + sORG_ID + "', '" + strurl + "', '" + sDateTime + "', 0,'" + sStartTime + "','" + sEndDateTime + "','" + xmlFilePath + "')";
                                    //int identityID = mainForm.dba.UpdateDbBySQLRetID(sqlstr);
                                    //Console.WriteLine(identityID);
                                    string sORG_ID2 = GetORG_ID(ebd.EBM.MsgContent.AreaCode) ;
                                    string paramValue = "1~" + strurl + "~0~1000~128~0~1~1";
                                    if ((PlayType == "2"))
                                    {
                                        SetText("音频文件存库，将在指定时间内播放", Color.Blue);
                                        sqlstr = "insert into TsCmdStore(TsCmd_Type, TsCmd_Mode, TsCmd_UserID, TsCmd_ValueID, TsCmd_Params, TsCmd_Date,TsCmd_ExcuteTime,TsCmd_SaveTime, TsCmd_Status,TsCmd_EndTime,TsCmd_Note)" +
                                           "values('播放视频', '区域', 1, " + sORG_ID2 + ", '" + paramValue + "', " + "'" + sDateTime + "'" + ",'" + sDateTime + "','" + sDateTime + "', 0,'" + sEndDateTime + "'," + "'-1'" + ")";
                                        //sqlstr = "insert into TsCmdStore(TsCmd_Type, TsCmd_Mode, TsCmd_UserID, TsCmd_ValueID, TsCmd_Params, TsCmd_Date,TsCmd_ExcuteTime,TsCmd_SaveTime, TsCmd_Status,TsCmd_EndTime,TsCmd_Note)" +
                                        //   "values('播放视频', '区域', 1, " + sORG_ID2 + ", '1~" + strurl + "~0~1200~192~0~1~1', " + "'" + sDateTime + "'" + ",'" + sDateTime + "','" + sDateTime + "', 0,'" + sEndDateTime + "'," + "'-1'" + ")";
                                        //int iback = mainForm.dba.getResultIDBySQL(sqlstr, "TsCmdStore");
                                        TsCmdStoreID = mainForm.dba.UpdateDbBySQLRetID(sqlstr).ToString();
                                        //paramValue = "1~D:\\rhtest_6_1\\apache-tomcat-7.0.69\\webapps\\ch-eoc\\upload/6666.mp3~0~1200~192~0~1~1";//1~D:\\rhtest_6_1\\apache-tomcat-7.0.69\\webapps\\ch-eoc\\upload/6666.mp3~0~1000~128~0~1~1
                                        SendMQOrder(1, paramValue, TsCmdStoreID);//MQ发送
                                        Thread.Sleep(500);
                                        Console.WriteLine(TsCmdStoreID);
                                    }
                                }
                            }
                            else//文本转语音
                            {
                                SetText("EBM开始时间: " + ebd.EBM.MsgBasicInfo.StartTime + "===>EBM结束时间: " + ebd.EBM.MsgBasicInfo.EndTime, Color.Blue);
                                DateTime EBStartTime = DateTime.Parse(ebd.EBM.MsgBasicInfo.StartTime).AddSeconds(2);
                                if (EBStartTime < DateTime.Now)
                                {
                                    EBStartTime = DateTime.Now.AddSeconds(2);
                                }
                                string sStartTime = EBStartTime.ToString("yyyy-MM-dd HH:mm:ss"); //ebd.EBM.MsgBasicInfo.StartTime;
                                string sDateTime = EBStartTime.ToString("yyyy-MM-dd HH:mm:ss");//ebd.EBM.MsgBasicInfo.StartTime;
                                string sEndDateTime = ebd.EBM.MsgBasicInfo.EndTime;
                                ccplayerStopTime = DateTime.Parse(sEndDateTime);
                                if (TEST == "YES")
                                {
                                    sStartTime = DateTime.Now.AddSeconds(2).ToString("yyyy-MM-dd HH:mm:ss");
                                    sDateTime = DateTime.Now.AddSeconds(2).ToString("yyyy-MM-dd HH:mm:ss");
                                    sEndDateTime = DateTime.Now.AddMinutes(5).ToString("yyyy-MM-dd HH:mm:ss");//ebd.EBM.MsgBasicInfo.EndTime;
                                    ccplayerStopTime = DateTime.Now.AddMinutes(2);

                                }
                                SetText("实时流开始时间>>>>" + sStartTime + "----结束时间>>>" + ccplayerStopTime.ToString("yyyy-MM-dd HH:mm:ss") + "是否是TEST:" + TEST, Color.Blue);
                                string strPID = m_nAudioPIDID + "~1";
                                string sORG_ID = GetORG_ID(ebd.EBM.MsgContent.AreaCode);//((int)mainForm.dba.getQueryResultBySQL(sqlstr)).ToString();
                                sqlstr = "insert into TsCmdStore(TsCmd_Type, TsCmd_Mode, TsCmd_UserID, TsCmd_ValueID, TsCmd_Params, TsCmd_Date, TsCmd_Status,TsCmd_EndTime,TsCmd_Note)" +
                                        "values('音源播放', '区域', 1, " + sORG_ID + ", '" + strPID + "', '" + sDateTime + "', 0,'" + sEndDateTime + "'," + "'-1'" + ")";

                                TsCmdStoreID = mainForm.dba.UpdateDbBySQLRetID(sqlstr).ToString();
                                SendMQOrder(2, strPID, TsCmdStoreID);//MQ发送
                                Thread.Sleep(500);
                                SetText("立即播放音频延时开始：" + DateTime.Now.ToString(), Color.Blue);
                                Thread.Sleep(iMediaDelayTime);//延迟10秒
                                Application.DoEvents();
                                SetText("立即播放音频开始：" + DateTime.Now.ToString(), Color.Blue);
                                string FileNameNum = "";
                                FileNameNum = rdMQFileName.Next(00, 99).ToString();
                                string Message = ebd.EBM.MsgContent.MsgDesc;
                                SetText(Message, Color.Olive);
                                if (MQStartFlag)
                                    MQDLL.SendMessageMQ("PACKETTYPE~TTS|CONTENT~" + Message + "|FILE~" + FileNameNum + ".wav");
                                Thread.Sleep(5000);
                                ccplay.TsCmdStoreID = TsCmdStoreID;//PlayRecord停止的标示
                                m_ccplayURL = AudioCloudIP + FileNameNum + ".wav";     //"udp://@" + m_StreamPortURL;
                                if (ccplay.m_bPlayFlag == false)
                                {
                                    ccplay.m_bPlayFlag = true;
                                }
                                else
                                {
                                    ccplay.StopCPPPlayer2();
                                    Thread.Sleep(1000);
                                    ccplayerthread.Abort();
                                    Thread.Sleep(1000);
                                    ccplayerthread = new Thread(CPPPlayerThread);
                                    ccplayerthread.Start();
                                }
                            }
                            #region SaveEBDInfo
                            if (SaveEBD(ebd) == -1)
                                Console.WriteLine("Error: 保存EBMInfo出错");
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 审核完成后下发指令
        /// </summary>
        /// <param name="EBMPath"></param>
        private void AnalysisEBMCheckOver(string EBMPath)
        {
            List<string> lDealTarFiles = new List<string>();
            List<string> AudioFileListTmp = new List<string>();//收集的音频文件列表
            List<string> AudioFileList = new List<string>();//收集的音频文件列表

            SetText("解压文件：" + EBMPath.ToString(), Color.Green);
            try
            {
                #region 解压
                if (File.Exists(EBMPath))
                {
                    try
                    {
                        DeleteFolder(sUnTarPath);
                        tar.UnpackTarFiles(EBMPath, sUnTarPath);
                        //把压缩包解压到专门存放接收到的XML文件的文件夹下
                        SetText("解压文件：" + EBMPath + "成功", Color.Green);
                    }
                    catch (Exception exa)
                    {
                        SetText("删除解压文件夹：" + sUnTarPath + "文件失败!错误信息：" + exa.Message, Color.Red);
                    }
                }
                #endregion 解压
            }
            catch (Exception ex)
            {
                Log.Instance.LogWrite("解压出错：" + ex.Message);
            }
            try
            {
                string[] xmlfilenames = Directory.GetFiles(sUnTarPath, "*.xml");//从解压XML文件夹下获取解压的XML文件名
                string sTmpFile = string.Empty;
                string sAnalysisFileName = "";
                string sSignFileName = "";

                for (int i = 0; i < xmlfilenames.Length; i++)
                {
                    sTmpFile = Path.GetFileName(xmlfilenames[i]);
                    if (sTmpFile.ToUpper().IndexOf("EBDB") > -1 && sTmpFile.ToUpper().IndexOf("EBDS_EBDB") < 0)
                    {
                        sAnalysisFileName = xmlfilenames[i];
                    }
                    else if (sTmpFile.ToUpper().IndexOf("EBDS_EBDB") > -1)//签名文件
                    {
                        sSignFileName = xmlfilenames[i];//签名文件
                    }
                }
                DeleteFolder(sSourcePath);//删除原有XML发送文件的文件夹下的XML

                if (sSignFileName == "")
                {
                    //验证签名功能
                }
                else
                {
                    #region 签名处理
                    Console.WriteLine("开始验证签名文件!");
                    using (FileStream SignFs = new FileStream(sSignFileName, FileMode.Open))
                    {
                        StreamReader signsr = new StreamReader(SignFs, System.Text.Encoding.UTF8);
                        string xmlsign = signsr.ReadToEnd();
                        signsr.Close();
                        responseXML signrp = new responseXML();//签名回复
                        XmlDocument xmlSignDoc = new XmlDocument();
                        try
                        {
                            xmlsign = XmlSerialize.ReplaceLowOrderASCIICharacters(xmlsign);
                            xmlsign = XmlSerialize.GetLowOrderASCIICharacters(xmlsign);
                            Signature sign = XmlSerialize.DeserializeXML<Signature>(xmlsign);
                        }
                        catch (Exception ex)
                        {
                            Log.Instance.LogWrite("签名文件错误：" + ex.Message);
                        }
                    }
                    Console.WriteLine("结束验证签名文件！");
                    #endregion End
                }

                if (sAnalysisFileName != "")
                {
                    using (FileStream fs = new FileStream(sAnalysisFileName, FileMode.Open))
                    {
                        StreamReader sr = new StreamReader(fs, Encoding.UTF8);
                        String xmlInfo = sr.ReadToEnd();
                        xmlInfo = xmlInfo.Replace("xmlns:xs", "xmlns");
                        sr.Close();
                        xmlInfo = XmlSerialize.ReplaceLowOrderASCIICharacters(xmlInfo);
                        xmlInfo = XmlSerialize.GetLowOrderASCIICharacters(xmlInfo);
                        ebd = XmlSerialize.DeserializeXML<EBD>(xmlInfo);
                        if (ebd.EBM.MsgBasicInfo.MsgType == "2")
                        {
                            if (MessageBox.Show("请确定是否要下发关机指令", "应急关机包", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                                == DialogResult.Yes)
                            {
                                SetText("停止播发：" + DateTime.Now.ToString(), Color.Red);
                                string strSql = string.Format("update PLAYRECORD set PR_REC_STATUS = '{0}' where PR_SourceID='{1}'", "删除", TsCmdStoreID);
                                strSql += " update EBMInfo set EBMState=1 where SEBDID='" + SEBDIDStatusFlag + "' ";
                                strSql += "delete from InfoVlaue";
                                //string strSql = "update PLAYRECORD set PR_REC_STATUS = '删除'";
                                mainForm.dba.UpdateDbBySQL(strSql);
                                Tccplayer.Enabled = false;
                                ccplay.StopCPPPlayer2();
                                RealAudioFlag = false;//标记为已经执行
                              //  list_PendingTask.Items.Remove(list_PendingTask.FocusedItem);
                                return;
                            }
                            else
                            {
                                return;
                            }
                        }
                        AudioFileListTmp.Clear();
                        AudioFileList.Clear();
                        string[] mp3files = Directory.GetFiles(sUnTarPath, "*.mp3");
                        AudioFileListTmp.AddRange(mp3files);
                        string[] wavfiles = Directory.GetFiles(sUnTarPath, "*.wav");
                        AudioFileListTmp.AddRange(wavfiles);
                        EBMInfo EBMInfo = new EBMInfo();
                        EBMInfo.ebd = ebd;
                        if (AudioFileListTmp.Count > 0)
                        {
                            EBMInfo.AudioUrl = AudioFileListTmp[0];
                        }
                          //  list_PendingTask.Items.Remove(list_PendingTask.FocusedItem);
                            string sqlstr = "";
                            if (AudioFileListTmp.Count > 0)
                            {
                                string sTmpDealFile = string.Empty;
                                string targetPath = string.Empty;
                                string strurl = "";
                                string sDateTime = "";
                                string sStartTime = ebd.EBM.MsgBasicInfo.StartTime;
                                string sEndDateTime = ebd.EBM.MsgBasicInfo.EndTime;
                                // string sGBCode = "";
                                string sORG_ID = "";
                                string sAread = "";
                                string xmlFilePath = "";
                                //if ((AudioFlag == "2")&&(TextFirst=="2")) //拷贝xml文件
                                {
                                    string xmlFile = Path.GetFileName(sAnalysisFileName);
                                    xmlFilePath = sAudioFilesFolder + "\\" + xmlFile;
                                    File.Copy(sAnalysisFileName, xmlFilePath, true);
                                }
                                for (int ai = 0; ai < AudioFileListTmp.Count; ai++)
                                {
                                    sTmpDealFile = Path.GetFileName(AudioFileListTmp[ai]);
                                    targetPath = sAudioFilesFolder + "\\" + sTmpDealFile;
                                    File.Copy(AudioFileListTmp[ai], targetPath, true);
                                    AudioFileList.Add(targetPath);


                                    SetText("EBM开始时间: " + ebd.EBM.MsgBasicInfo.StartTime + "===>EBM结束时间: " + ebd.EBM.MsgBasicInfo.EndTime, Color.Blue);
                                    DateTime EbStartTime = DateTime.Parse(ebd.EBM.MsgBasicInfo.StartTime).AddSeconds(2);
                                    if (EbStartTime < DateTime.Now)
                                    {
                                        EbStartTime = DateTime.Now.AddSeconds(2);
                                    }

                                    sDateTime = EbStartTime.ToString("yyyy-MM-dd HH:mm:ss");  //ebd.EBM.MsgBasicInfo.StartTime;
                                    sStartTime = EbStartTime.ToString("yyyy-MM-dd HH:mm:ss"); //ebd.EBM.MsgBasicInfo.StartTime;
                                    sEndDateTime = ebd.EBM.MsgBasicInfo.EndTime;
                                    if (TEST == "YES")
                                    {
                                        sDateTime = DateTime.Now.AddSeconds(2).ToString("yyyy-MM-dd HH:mm:ss");
                                        sStartTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        sEndDateTime = DateTime.Now.AddMinutes(5).ToString("yyyy-MM-dd HH:mm:ss");//ebd.EBM.MsgBasicInfo.EndTime;
                                    }

                                    SetText("开始时间: " + sDateTime + "===>结束时间: " + sEndDateTime + "是否是TEST:" + TEST, Color.Blue);
                                    sAread = ebd.EBM.MsgContent.AreaCode; //区域
                                    sORG_ID = ebd.EBM.EBMID;
                                    strurl = targetPath;  //音频文件地址
                                    // sqlstr = "insert into TsCmdStoreMedia(TsCmd_Type, TsCmd_Mode, TsCmd_UserID, TsCmd_ValueID, TsCmd_Params, TsCmd_Date, TsCmd_Status,TsCmd_StartTime,TsCmd_EndTime,TsCmd_XmlFile)" +
                                    //        "values('播放音频', '" + sAread + "', 1, '" + sORG_ID + "', '" + strurl + "', '" + sDateTime + "', 0,'" + sStartTime + "','" + sEndDateTime + "','" + xmlFilePath + "')";
                                    //int identityID = mainForm.dba.UpdateDbBySQLRetID(sqlstr);
                                    //Console.WriteLine(identityID);
                                    string sORG_ID2 = GetORG_ID(ebd.EBM.MsgContent.AreaCode);
                                    string paramValue = "1~" + strurl + "~0~1000~128~0~1~1";
                                    if ((PlayType == "2"))
                                    {
                                        SetText("音频文件存库，将在指定时间内播放", Color.Blue);
                                        sqlstr = "insert into TsCmdStore(TsCmd_Type, TsCmd_Mode, TsCmd_UserID, TsCmd_ValueID, TsCmd_Params, TsCmd_Date,TsCmd_ExcuteTime,TsCmd_SaveTime, TsCmd_Status,TsCmd_EndTime,TsCmd_Note)" +
                                           "values('播放视频', '区域', 1, " + sORG_ID2 + ", '" + paramValue + "', " + "'" + sDateTime + "'" + ",'" + sDateTime + "','" + sDateTime + "', 0,'" + sEndDateTime + "'," + "'-1'" + ")";
                                        //sqlstr = "insert into TsCmdStore(TsCmd_Type, TsCmd_Mode, TsCmd_UserID, TsCmd_ValueID, TsCmd_Params, TsCmd_Date,TsCmd_ExcuteTime,TsCmd_SaveTime, TsCmd_Status,TsCmd_EndTime,TsCmd_Note)" +
                                        //   "values('播放视频', '区域', 1, " + sORG_ID2 + ", '1~" + strurl + "~0~1200~192~0~1~1', " + "'" + sDateTime + "'" + ",'" + sDateTime + "','" + sDateTime + "', 0,'" + sEndDateTime + "'," + "'-1'" + ")";
                                        //int iback = mainForm.dba.getResultIDBySQL(sqlstr, "TsCmdStore");
                                        TsCmdStoreID = mainForm.dba.UpdateDbBySQLRetID(sqlstr).ToString();
                                        //paramValue = "1~D:\\rhtest_6_1\\apache-tomcat-7.0.69\\webapps\\ch-eoc\\upload/6666.mp3~0~1200~192~0~1~1";//1~D:\\rhtest_6_1\\apache-tomcat-7.0.69\\webapps\\ch-eoc\\upload/6666.mp3~0~1000~128~0~1~1
                                        SendMQOrder(1, paramValue, TsCmdStoreID);//MQ发送
                                        Thread.Sleep(500);
                                        Console.WriteLine(TsCmdStoreID);
                                    }
                                }
                            }
                            else//文本转语音
                            {
                                SetText("EBM开始时间: " + ebd.EBM.MsgBasicInfo.StartTime + "===>EBM结束时间: " + ebd.EBM.MsgBasicInfo.EndTime, Color.Blue);
                                DateTime EBStartTime = DateTime.Parse(ebd.EBM.MsgBasicInfo.StartTime).AddSeconds(2);
                                if (EBStartTime < DateTime.Now)
                                {
                                    EBStartTime = DateTime.Now.AddSeconds(2);
                                }
                                string sStartTime = EBStartTime.ToString("yyyy-MM-dd HH:mm:ss"); //ebd.EBM.MsgBasicInfo.StartTime;
                                string sDateTime = EBStartTime.ToString("yyyy-MM-dd HH:mm:ss");//ebd.EBM.MsgBasicInfo.StartTime;
                                string sEndDateTime = ebd.EBM.MsgBasicInfo.EndTime;
                                ccplayerStopTime = DateTime.Parse(sEndDateTime);
                                if (TEST == "YES")
                                {
                                    sStartTime = DateTime.Now.AddSeconds(2).ToString("yyyy-MM-dd HH:mm:ss");
                                    sDateTime = DateTime.Now.AddSeconds(2).ToString("yyyy-MM-dd HH:mm:ss");
                                    sEndDateTime = DateTime.Now.AddMinutes(5).ToString("yyyy-MM-dd HH:mm:ss");//ebd.EBM.MsgBasicInfo.EndTime;
                                    ccplayerStopTime = DateTime.Now.AddMinutes(2);

                                }
                                SetText("实时流开始时间>>>>" + sStartTime + "----结束时间>>>" + ccplayerStopTime.ToString("yyyy-MM-dd HH:mm:ss") + "是否是TEST:" + TEST, Color.Blue);
                                string strPID = m_nAudioPIDID + "~1";
                                string sORG_ID = GetORG_ID(ebd.EBM.MsgContent.AreaCode);//((int)mainForm.dba.getQueryResultBySQL(sqlstr)).ToString();
                                sqlstr = "insert into TsCmdStore(TsCmd_Type, TsCmd_Mode, TsCmd_UserID, TsCmd_ValueID, TsCmd_Params, TsCmd_Date, TsCmd_Status,TsCmd_EndTime,TsCmd_Note)" +
                                        "values('音源播放', '区域', 1, " + sORG_ID + ", '" + strPID + "', '" + sDateTime + "', 0,'" + sEndDateTime + "'," + "'-1'" + ")";

                                TsCmdStoreID = mainForm.dba.UpdateDbBySQLRetID(sqlstr).ToString();
                                SendMQOrder(2, strPID, TsCmdStoreID);//MQ发送
                                Thread.Sleep(500);
                                SetText("立即播放音频延时开始：" + DateTime.Now.ToString(), Color.Blue);
                                Thread.Sleep(iMediaDelayTime);//延迟10秒
                                Application.DoEvents();
                                SetText("立即播放音频开始：" + DateTime.Now.ToString(), Color.Blue);
                                string FileNameNum = "";
                                FileNameNum = rdMQFileName.Next(00, 99).ToString();
                                string Message = ebd.EBM.MsgContent.MsgDesc;
                                SetText(Message, Color.Olive);
                                if (MQStartFlag)
                                    MQDLL.SendMessageMQ("PACKETTYPE~TTS|CONTENT~" + Message + "|FILE~" + FileNameNum + ".wav");
                                Thread.Sleep(5000);
                                ccplay.TsCmdStoreID = TsCmdStoreID;//PlayRecord停止的标示
                                m_ccplayURL = AudioCloudIP + FileNameNum + ".wav";     //"udp://@" + m_StreamPortURL;
                                if (ccplay.m_bPlayFlag == false)
                                {
                                    ccplay.m_bPlayFlag = true;
                                }
                                else
                                {
                                    ccplay.StopCPPPlayer2();
                                    Thread.Sleep(1000);
                                    ccplayerthread.Abort();
                                    Thread.Sleep(1000);
                                    ccplayerthread = new Thread(CPPPlayerThread);
                                    ccplayerthread.Start();
                                }
                            }
                            #region SaveEBDInfo
                            if (SaveEBD(ebd) == -1)
                                Console.WriteLine("Error: 保存EBMInfo出错");
                            #endregion
                        
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void EBMStateRequest()
        {
            SetText("EBMStateRequest    NO:1", Color.Orange);
            try
            {
                XmlDocument xmlStateDoc = new XmlDocument();
                responseXML rState = new responseXML();
                rState.SourceAreaCode = ServerForm.strSourceAreaCode;
                rState.SourceType = ServerForm.strSourceType;
                rState.SourceName = ServerForm.strSourceName;
                rState.SourceID = ServerForm.strSourceID;
                rState.sHBRONO = ServerForm.strHBRONO;

                Random rdState = new Random();

                string frdStateName = "10" + rState.sHBRONO + "0000000000000" + rdState.Next(100, 999).ToString();
                string xmlEBMStateFileName = "\\EBDB_" + frdStateName + ".xml";
                if (ebd == null)
                    return;
                string EBMID = ebd.EBMStateRequest.EBM.EBMID;
                try
                {
                    xmlStateDoc = rState.ResponeEBMStateRequrest(EBMID, frdStateName);
                    UnifyCreateTar(xmlStateDoc, frdStateName);
                    string sHeartBeatTarName = sSendTarPath + "\\" + "EBDT_" + frdStateName + ".tar";
                    send.address = sZJPostUrlAddress;
                    send.fileNamePath = sHeartBeatTarName;
                    postfile.UploadFilesByPostThread(send);
                }
                catch
                {
                }
            }
            catch (Exception h)
            {
                Log.Instance.LogWrite("错误510行:" + h.Message);
            }
        }

        //指令MQ初始化
        private void MQActivStart()
        {
            m_mq = new MQ();
            m_mq.uri = serverini.ReadValue("MQActiveOrder", "ServerUrl"); 
            m_mq.username = serverini.ReadValue("MQActiveOrder", "User"); 
            m_mq.password = serverini.ReadValue("MQActiveOrder", "Password");
            m_mq.Start();
            Thread.Sleep(500);
            m_mq.CreateProducer(true, "fee.bar");
        }

        private void ConnectMQServer()
        {
            try
            {
                m_mq_checkEBM = new MQ();
                m_mq_checkEBM.uri = serverini.ReadValue("MQCheckEbmInfo", "ServerUrl");
                m_mq_checkEBM.username = serverini.ReadValue("MQCheckEbmInfo", "User");
                m_mq_checkEBM.password = serverini.ReadValue("MQCheckEbmInfo", "Password");
                m_mq_checkEBM.Start();
                isConn = true;
                m_consumer = m_mq_checkEBM.CreateConsumer(true, serverini.ReadValue("MQCheckEbmInfo", "ReceiveTopicName"));
                m_consumer.Listener += new MessageListener(consumer_listener_ChenckData);
                m_mq_checkEBM.CreateProducer(true, serverini.ReadValue("MQCheckEbmInfo", "SendTopicName"));//创建消息生产者   //Queue
            }
            catch (Exception ex)
            {
                Log.Instance.LogWrite(ex.Message);
            }

        }

        private void InitFTPServer()
        {
            string ftpserver= serverini.ReadValue("FTPServer", "ftpserver");
            string ftpusername= serverini.ReadValue("FTPServer", "ftpusername");
            string ftppwd= serverini.ReadValue("FTPServer", "ftppwd");
            ftphelper = new FTPHelper(ftpserver, ftpusername, ftppwd);
        }


        /// <summary>
        /// MQ消息接收  字符串
        /// </summary>
        /// <param name="message"></param>
        private void consumer_listener(IMessage message)
        {
            try
            {
                string strMsg;
                ITextMessage msg = (ITextMessage)message;
                strMsg = msg.Text;
                Log.Instance.LogWrite("MQ接收信息打印：" + strMsg);
            }
            catch (Exception ex)
            {
                Log.Instance.LogWrite(ex.Message);
            }
        }

        /// <summary>
        ///  MQ消息接收   键值对
        /// </summary>
        /// <param name="message"></param>
        private void consumer_listener_ChenckData(IMessage message)
        {
            try
            {
               Serialize(message.Properties);
            }
            catch (Exception ex)
            {
                this.m_consumer.Close();
            }
        }

        public void Serialize(IPrimitiveMap MsgMap)
        {

            string ID = MsgMap["ID"].ToString();
            string EBDDID = MsgMap["EBDDID"].ToString();
            string EBMPath = sRevTarPath + "//" + EBDDID + ".tar";
            AnalysisEBMCheckOver(EBMPath);

        }



        private bool SendMQOrder(int Type, string ParamValue, string TsCmd_ID)
        {
            try
            {
                if (ebd != null)
                {
                    string InfoValueStr = "insert into InfoVlaue values('" + ebd.EBDID + "')";
                    mainForm.dba.UpdateDbBySQL(InfoValueStr);
                }
           
                if (!MQStartFlag)
                {
                    Console.WriteLine("MQ标识未启用,取消发送!");
                    return false;
                }
                //"1~D:\\rhtest_6_1\\apache-tomcat-7.0.69\\webapps\\ch-eoc\\upload/1109.mp3~0~1000~128~0~0~0"
                if (ParamValue.Length > 0)
                {
                    if (m_mq == null)
                    {
                        MQActivStart();
                    }
                }
                m_lstProperty = Install(Type, ParamValue, TsCmd_ID);//~0~1200~192~0~1~1应急
                return m_mq.SendMQMessage(true, "Send", m_lstProperty);
            }
            catch (Exception)
            {
                return false;
            }
         
        }

        /// <summary>
        /// 组装MQ指令
        /// </summary>
        /// <param name="Type">指令Type 1(音频文件播发) 2(网络URL播发)</param>
        /// <param name="value"></param>
        private List<Property> Install(int Type, string value, string TsCmd_ID)
        {
            //TsCmd_Mode  区域
            //TsCmd_Date  2017-07-11 19:16:38
            //TsCmd_Status  0
            //USER_PRIORITY  0
            //TsCmd_UserID  14
            //USER_ORG_CODE  P37Q06C02
            //TsCmd_ValueID  22
            //TsCmd_Type  播放视频
            //VOICE                 2
            //TsCmd_Params          1~D:\rhtest_6_1\apache-tomcat-7.0.69\webapps\ch-eoc\upload/1109.mp3~0~1000~128~0~0~0
            //TsCmd_PlayCount       1
            List<Property> InstallList = new List<Property>();
            Property item = new Property();
            item.name = "TsCmd_Mode";
            item.value = "区域";
            InstallList.Add(item);

            Property itemTime = new Property(); ;
            itemTime.name = "TsCmd_Date";
            itemTime.value = DateTime.Now.AddSeconds(2).ToString("yyyy-MM-dd HH:mm:ss");
            InstallList.Add(itemTime);

            Property itemStatus = new Property();
            itemStatus.name = "TsCmd_Status";
            itemStatus.value = "0";
            InstallList.Add(itemStatus);

            Property itemVoice = new Property();
            itemVoice.name = "VOICE";
            itemVoice.value = "3";
            InstallList.Add(itemVoice);

            Property itemTsCmd_ID = new Property();
            itemTsCmd_ID.name = "TsCmd_ID";
            itemTsCmd_ID.value = TsCmd_ID;
            InstallList.Add(itemTsCmd_ID);

            Type t = MQUserInfo.GetType();
            PropertyInfo[] PropertyList = t.GetProperties();
            foreach (var PropertyInfo in PropertyList)
            {
                Property userinfo = new Property();
                userinfo.name = PropertyInfo.Name;
                object valueobj = PropertyInfo.GetValue(MQUserInfo, null);
                userinfo.value = valueobj == null ? "" : valueobj.ToString();
                InstallList.Add(userinfo);

            }
            string strOrder = "";


            if (Type == 1)//音频文件播发
            {
                Property itemType = new Property();
                itemType.name = "TsCmd_Type";
                itemType.value = "播放视频";
                InstallList.Add(itemType);
            }
            else
            {
                Property itemType = new Property();
                itemType.name = "TsCmd_Type";
                itemType.value = "音源播放";
                InstallList.Add(itemType);
            }

            Property itemTsCmd_Params = new Property();
            itemTsCmd_Params.name = "TsCmd_Params";
            itemTsCmd_Params.value = value;
            InstallList.Add(itemTsCmd_Params);

            //打印MQ指令
            foreach (var Property in InstallList)
            {
                strOrder += Property.name + "  " + Property.value + Environment.NewLine;

            }
            Console.WriteLine(strOrder);
            return InstallList;
        }


        private void FindUserInfo(string Name)
        {
            string sql = "select * from Users U inner join Organization O on U.USER_ORG_CODE=O.ORG_CODEA where U.USER_DETAIL='" + Name + "'";
            DataTable dtUser = mainForm.dba.getQueryInfoBySQL(sql);
            if (dtUser.Rows.Count > 0)
            {
                MQUserInfo.USER_PRIORITY = dtUser.Rows[0]["USER_PRIORITY"].ToString();
                MQUserInfo.TsCmd_UserID = dtUser.Rows[0]["USER_ID"].ToString();
                MQUserInfo.USER_ORG_CODE = dtUser.Rows[0]["USER_ORG_CODE"].ToString();
                MQUserInfo.TsCmd_ValueID = dtUser.Rows[0]["ORG_ID"].ToString();
            }
        }

        private class UserInfo
        {
            public string USER_PRIORITY { get; set; }
            public string TsCmd_UserID { get; set; }
            public string USER_ORG_CODE { get; set; }
            public string TsCmd_ValueID { get; set; }
        }


        /// <summary>
        /// 复制大文件
        /// </summary>
        /// <param name="fromPath">源文件的路径</param>
        /// <param name="toPath">文件保存的路径</param>
        /// <param name="eachReadLength">每次读取的长度</param>
        /// <returns>是否复制成功</returns>
        public bool CopyFile(string fromPath, string toPath, int eachReadLength)
        {
            //将源文件 读取成文件流
            FileStream fromFile = new FileStream(fromPath, FileMode.Open, FileAccess.Read);
            //已追加的方式 写入文件流
            FileStream toFile = new FileStream(toPath, FileMode.Append, FileAccess.Write);
            //实际读取的文件长度
            int toCopyLength = 0;
            //如果每次读取的长度小于 源文件的长度 分段读取
            if (eachReadLength < fromFile.Length)
            {
                byte[] buffer = new byte[eachReadLength];
                long copied = 0;
                while (copied <= fromFile.Length - eachReadLength)
                {
                    toCopyLength = fromFile.Read(buffer, 0, eachReadLength);
                    fromFile.Flush();
                    toFile.Write(buffer, 0, eachReadLength);
                    toFile.Flush();
                    //流的当前位置
                    toFile.Position = fromFile.Position;
                    copied += toCopyLength;
                }
                int left = (int)(fromFile.Length - copied);
                toCopyLength = fromFile.Read(buffer, 0, left);
                fromFile.Flush();
                toFile.Write(buffer, 0, left);
                toFile.Flush();
            }
            else
            {
                //如果每次拷贝的文件长度大于源文件的长度 则将实际文件长度直接拷贝
                byte[] buffer = new byte[fromFile.Length];
                fromFile.Read(buffer, 0, buffer.Length);
                fromFile.Flush();
                toFile.Write(buffer, 0, buffer.Length);
                toFile.Flush();
            }
            fromFile.Close();
            toFile.Close();
            return true;
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            string ID = "7";
            string EBDDID = "100102320000000000010000000001010861";
            string EBMPath = sRevTarPath+"//"+ EBDDID + ".tar";
            AnalysisEBMCheckOver(EBMPath);
        }
    }
}
