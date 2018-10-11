using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Xml;
using System.Collections.Specialized;
using AxWMPLib;
using System.Runtime.InteropServices;//2016-04-25 Dllimport 引用添加
using DevExpress.XtraBars;


namespace GRPlatForm
{
    public partial class ServerForm : Form
    {
        public static string sTarPathName = "";//全局变量
        public static string sTmptarFileName = "";//定义处理Tar包临时文件名

        Thread thTar = null;//解压回复线程
        Thread httpthread = null;//HTTP服务
        Thread thFeedBack = null;//回复状态线程
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
        public static string sSourcePath = "";//f需压缩文件路径
        public static string sUnTarPath = "";//Tar包解压缩路径
        public static string sAudioFilesFolder = "";//音频文件存放位置

        public string sServerIP = "";
        public string sServerPort = "";
        private IPAddress iServerIP;
        private int iServerPort = 0;

        private string sZJPostUrlAddress = "";//总局接收地址
        private string sYXPostUrlAddress = "";//永新接收地址
        private AxWindowsMediaPlayer MediaPlayer;
        public static mainForm mainFrm;
        private int iHoldTimes = 600;///
        private int iHoldTimesCnt = 0;//记数器
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

        public static string sEBMStateResponsePath = string.Empty;
        private DateTime dtLinkTime = new DateTime();//用于判断平台连接状态
        private const int OnOffLineInterval = 300;//离线在线间隔
        /*2016-03-31*/
        private List<string> listAreaCode;  //2016-04-01
       // private string AreaCode;            //2016-04-01
        private string EMBCloseAreaCode = "";//关闭区域逻辑代码
        private string strAreaFlag = "";     //区域标志

        private int iAudioDelayTime = 0;//文转语延迟时间
        private int iMediaDelayTime = 0;//音频延迟时间
        private string bCharToAudio = "";  //1文转语，2 音频播放 
        private EBD ebd;

        delegate void SetTextCallback(string text); //在界面上显示信息委托
        private string AudioFlag = "";//********音频文件是否立即播放标志：1：立即播放 2：根据下发时间播放
        private string TEST = "";//********音频文件是否处于测试状态：test:测试状态，即收到的TAR包内xml的开始、结束时间无论是否过期，开始时间+1，结束时间+30
        private string TextFirst = "";//********文转语是否处于优先级1：文转语优先 2：语音优先
        private string PlayType = "";

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
        public static string m_AreaCode;

        [System.Runtime.InteropServices.DllImport("TTSDLL.dll", EntryPoint = "TTSConvertOut", ExactSpelling = true, CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl, CharSet = System.Runtime.InteropServices.CharSet.Ansi, SetLastError = true)]
        public static extern void TTSConvertOut([System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)]string szPath, [System.Runtime.InteropServices.InAttribute()][System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string szContent);

        public ServerForm()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
            MediaPlayer = new AxWindowsMediaPlayer();
            MediaPlayer.PlayStateChange+=new _WMPOCXEvents_PlayStateChangeEventHandler(MediaPlayer_PlayStateChange);  //注册媒体播放状态改变
            ((System.ComponentModel.ISupportInitialize)(MediaPlayer)).BeginInit();
            this.Controls.Add(MediaPlayer);
            ((System.ComponentModel.ISupportInitialize)(MediaPlayer)).EndInit();

            serverini = new IniFiles(@Application.StartupPath + "\\Config.ini");
            dtLinkTime = DateTime.Now.AddSeconds(-1 - OnOffLineInterval);                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (btnStart.Text == "启动服务")
            {
                btnStart.Text = "停止服务";
                //timHeart.Enabled = true;
                txtServerPort.Enabled = false;
                tim_MediaPlay.Enabled = true;
            }
            else
            {
                #region 停止服务
                try
                {
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
                }
                catch(Exception em)
                {
                    Log.Instance.LogWrite("停止线程错误：" + em.Message);
                }
                btnStart.Text = "启动服务";
                //timHeart.Enabled = false;
                txtServerPort.Enabled = true;
                tim_MediaPlay.Enabled = false;
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
                System.Net.IPAddress[] ipArr;
                ipArr = System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName());
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

        }

        private void ServerForm_Load(object sender, EventArgs e)  //页面参数初始化
        {

            MediaPlayer.uiMode = "none";
            MediaPlayer.Hide();
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

            string clsareacode = "0000000000" + serverini.ReadValue("INFOSET", "EMBAreaCode");

            EMBCloseAreaCode = clsareacode.Substring(clsareacode.Length - 10, 10);
            EMBCloseAreaCode = L_H(EMBCloseAreaCode);

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


            m_AreaCode = serverini.ReadValue("AREA", "AreaCode");

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
            catch(Exception em)
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
            sYXPostUrlAddress = serverini.ReadValue("INFOSET", "YXURL");
            if (sZJPostUrlAddress == "" || sYXPostUrlAddress == "")
            {
                MessageBox.Show("回馈地址不能为空，请重新设置！");
                this.Close();
            }
            timHeart.Enabled = false;
            this.Text = "离线";
            if (tim_ClearMemory.Enabled == false)
            {
                tim_ClearMemory.Enabled = true;
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
                    Thread.Sleep(2000);
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
                    this.Invoke((EventHandler)delegate
                    {
                        this.Text = "在线";
                        dtLinkTime = DateTime.Now;//刷新时间
                    });
                }
                #region 处理Tar包
                if (lDealTarFiles.Count == 0)
                {
                    continue;//没有处理文件包不处理
                }
                try
                {
                    for (int li = 0; li < lDealTarFiles.Count; li++)
                    {
                        SetText("解压文件："+lDealTarFiles[li].ToString());
                        try
                        {
                            #region 解压
                            if(File.Exists(lDealTarFiles[li]))
                            {
                                try
                                {
                                    DeleteFolder(sUnTarPath);
                                    tar.UnpackTarFiles(lDealTarFiles[li], sUnTarPath);
                                    //把压缩包解压到专门存放接收到的XML文件的文件夹下
                                    SetText("解压文件：" + lDealTarFiles[li].ToString() + "成功");
                                }
                                catch (Exception exa)
                                {
                                    SetText("删除解压文件夹：" + sUnTarPath + "文件失败!错误信息："+exa.Message);
                                }
                            }
                            #endregion 解压
                        }
                        catch (Exception ex)
                        {
                            Log.Instance.LogWrite("解压出错：" + ex.Message);
                            //lDealTarFiles.RemoveAt(li);
                            if (li != 0)
                            {
                                li--;
                            }
                            else
                            {
                                //li = 0;
                            }
                        }
                        lDealTarFiles.RemoveAt(li);//无论是否成功，都移除
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
                                //continue;
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
                                        //xmlSignDoc = signrp.SignResponse(sign.RefEBDID, "OK");
                                    }
                                    catch (Exception ex)
                                    {
                                        Log.Instance.LogWrite("签名文件错误：" + ex.Message);
                                       // xmlSignDoc = signrp.SignResponse("", "Error");
                                    }
                                    //xmlSignDoc.Save(sSourcePath + "\\EBDSign.xml");
                                }
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
                                            string strAuxiliaryType = ebd.EBM.MsgContent.Auxiliary.AuxiliaryType; //实时流播发
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
                                            if (strMsgType == "2" && PlayType == "1")
                                            {
                                                ccplay.StopCPPPlayer2();
                                                return;
                                            }
                                            #region AreaCode
                                            listAreaCode.Clear();
                                            strAreaFlag = "";  //本平台是否播放的标志
                                            if (!string.IsNullOrEmpty(ebd.EBM.MsgContent.AreaCode))
                                            {
                                                string[] AreaCode = ebd.EBM.MsgContent.AreaCode.Split(new char[] { ',' });
                                                for (int a = 0; a < AreaCode.Length; a++)
                                                {
                                                    string strTmpAddr = AreaCode[a];
                                                    int isheng = -1;  //省级
                                                    int ishi = -1;    //市级
                                                    int iIndex = -1;  //县及以下
                                                    string subStr="";
                                                    subStr=strHBAREACODE.Substring(0,2);  //省级编码
                                                    isheng = strTmpAddr.IndexOf(subStr);
                                                    subStr = strHBAREACODE.Substring(0, 4);  //市级编码
                                                    ishi = strTmpAddr.IndexOf(subStr);
                                                    iIndex = strTmpAddr.IndexOf(strHBAREACODE);  //是否是本区域
                                                    if ((isheng==0)||(ishi==0)||(iIndex == 0) || (strTmpAddr.Substring(2) == "0000000000") || (strTmpAddr.Substring(4) == "00000000"))//(strTmpAddr.Length != 14)
                                                    {
                                                        strAreaFlag = "1";//1代表命令发送到本区域，2代表上一级，3代表上上一级
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
                                                        if (AudioFlag == "1" && PlayType == "1")//AudioFlag音频文件是否立即播放标志：1：立即播放 2：根据下发时间播放
                                                        {
                                                            string sDateTime = ebd.EBM.MsgBasicInfo.StartTime;
                                                            string sEndDateTime = ebd.EBM.MsgBasicInfo.EndTime;

                                                            string strPID = m_nAudioPIDID + "~0";
                                                            string sORG_ID = m_AreaCode;//((int)mainForm.dba.getQueryResultBySQL(sqlstr)).ToString();

                                                            sqlstr = "insert into TsCmdStore(TsCmd_Type, TsCmd_Mode, TsCmd_UserID, TsCmd_ValueID, TsCmd_Params, TsCmd_Date, TsCmd_Status,TsCmd_EndTime,TsCmd_Note)" +
                                                                    "values('音源播放', '区域', 1, " + m_AreaCode + ", '" + strPID + "', '" + sDateTime + "', 0,'" + sEndDateTime + "'," + "'-1'" + ")";

                                                            int iback = mainForm.dba.getResultIDBySQL(sqlstr, "TsCmdStore");

                                                            // for (int i = 0; i < listAreaCode.Count; i++)
                                                            {
                                                                //string cmdOpen = "4C " + listAreaCode[i] + " B0 02 01 04";
                                                                string cmdOpen = "4C " + "AA AA AA AA 00" + " B0 02 01 04";
                                                                Log.Instance.LogWrite("立即播放音频应急开机：" + cmdOpen);
                                                                SetText("立即播放音频应急开机：" + cmdOpen + DateTime.Now.ToString());
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
                                                                SetText("音频播放文件：" + AudioFileListTmp[iLoopMedia]);
                                                                bCharToAudio = "2";
                                                                try
                                                                {
                                                                    lock (oLockPlay)
                                                                    {
                                                                        SetText("立即播放音频延时开始：" + DateTime.Now.ToString());
                                                                        Thread.Sleep(iMediaDelayTime);//延迟10秒
                                                                        Application.DoEvents();
                                                                        SetText("立即播放音频开始：" + DateTime.Now.ToString());
                                                                        string strURL = "";
                                                                        ccplay.init("", "file:///" + AudioFileListTmp[iLoopMedia], m_strIP, m_Port, "pipe", "EVENT", m_nAudioPID, m_nVedioPID, m_nVedioRat, m_nAuioRat);
                                                                        ccplay.CreatePipeandEvent("pipename", "eventname");
                                                                        ccplay.CreateCPPPlayer();
                                                                        Thread.Sleep(2000);
                                                                        ccplay.StopCPPPlayer();
                                                                    }
                                                                    sDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//ebd.EBM.MsgBasicInfo.StartTime;
                                                                    sEndDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); //ebd.EBM.MsgBasicInfo.EndTime;

                                                                    strPID = "NULL";
                                                                    sORG_ID = m_AreaCode;//((int)mainForm.dba.getQueryResultBySQL(sqlstr)).ToString();

                                                                    sqlstr = "insert into TsCmdStore(TsCmd_Type, TsCmd_Mode, TsCmd_UserID, TsCmd_ValueID, TsCmd_Params, TsCmd_Date, TsCmd_Status,TsCmd_EndTime,TsCmd_Note)" +
                                                                            "values('关', '区域', 1, " + m_AreaCode + ", 'NULL', '" + sDateTime + "', 0,'" + sEndDateTime + "'," + "'-1'" + ")";

                                                                    iback = mainForm.dba.getResultIDBySQL(sqlstr, "TsCmdStore");

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
                                                        try
                                                        {
                                                            string sDateTime = ebd.EBM.MsgBasicInfo.StartTime;
                                                            string sEndDateTime = ebd.EBM.MsgBasicInfo.EndTime;

                                                            string strPID = m_nAudioPIDID + "~0";
                                                            string sORG_ID = m_AreaCode;//((int)mainForm.dba.getQueryResultBySQL(sqlstr)).ToString();

                                                            sqlstr = "insert into TsCmdStore(TsCmd_Type, TsCmd_Mode, TsCmd_UserID, TsCmd_ValueID, TsCmd_Params, TsCmd_Date, TsCmd_Status,TsCmd_EndTime,TsCmd_Note)" +
                                                                    "values('音源播放', '区域', 1, " + m_AreaCode + ", '" + strPID + "', '" + sDateTime + "', 0,'" + sEndDateTime + "'," + "'-1'" + ")";

                                                            int iback = mainForm.dba.getResultIDBySQL(sqlstr, "TsCmdStore");

                                                            SetText("立即播放音频延时开始：" + DateTime.Now.ToString());
                                                            Thread.Sleep(iMediaDelayTime);//延迟10秒
                                                            Application.DoEvents();
                                                            SetText("立即播放音频开始：" + DateTime.Now.ToString());
                                                            string strURL = "";
                                                            lock (oLockPlay)
                                                            {
                                                                if (strAuxiliaryType == "61") //实时流播发
                                                                {
                                                                    strURL = "udp://@" + m_StreamPortURL;
                                                                    ccplay.init("", strURL, m_strIP, m_Port, "pipe", "EVENT", m_nAudioPID, m_nVedioPID, m_nVedioRat, m_nAuioRat);
                                                                    ccplay.CreatePipeandEvent("pipename", "eventname");
                                                                    ccplay.CreateCPPPlayer();
                                                                    Thread.Sleep(2000);
                                                                    ccplay.StopCPPPlayer();
                                                                }
                                                            }
                                                            sDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//ebd.EBM.MsgBasicInfo.StartTime;
                                                            sEndDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); //ebd.EBM.MsgBasicInfo.EndTime;

                                                            strPID = "NULL";
                                                            sORG_ID = m_AreaCode;//((int)mainForm.dba.getQueryResultBySQL(sqlstr)).ToString();

                                                            sqlstr = "insert into TsCmdStore(TsCmd_Type, TsCmd_Mode, TsCmd_UserID, TsCmd_ValueID, TsCmd_Params, TsCmd_Date, TsCmd_Status,TsCmd_EndTime,TsCmd_Note)" +
                                                                    "values('关', '区域', 1, " + m_AreaCode + ", 'NULL', '" + sDateTime + "', 0,'" + sEndDateTime + "'," + "'-1'" + ")";

                                                            iback = mainForm.dba.getResultIDBySQL(sqlstr, "TsCmdStore");

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
                                                            sDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                            sStartTime = ebd.EBM.MsgBasicInfo.StartTime;
                                                            sEndDateTime = DateTime.Now.AddMinutes(5).ToString("yyyy-MM-dd HH:mm:ss");//ebd.EBM.MsgBasicInfo.EndTime;
                                                            SetText("音频文件存库，将在指定时间内播放");
                                                           /* if (TEST == "YES")
                                                            {
                                                                DateTime dt = DateTime.Now;
                                                                DateTime dt1, dt2;
                                                                dt1 = dt.AddMinutes(1);
                                                                dt2 = dt.AddMinutes(30);
                                                                sStartTime = dt1.ToString("yyyy-MM-dd HH:mm:ss");
                                                                sEndDateTime = dt2.ToString("yyyy-MM-dd HH:mm:ss");
                                                            }*/
                                                            sAread = ebd.EBM.MsgContent.AreaCode; //区域
                                                            sORG_ID = ebd.EBM.EBMID;
                                                            strurl = targetPath;  //音频文件地址
                                                            sqlstr = "insert into TsCmdStoreMedia(TsCmd_Type, TsCmd_Mode, TsCmd_UserID, TsCmd_ValueID, TsCmd_Params, TsCmd_Date, TsCmd_Status,TsCmd_StartTime,TsCmd_EndTime,TsCmd_XmlFile)" +
                                                                     "values('播放音频', '" + sAread + "', 1, '" + sORG_ID + "', '" + strurl + "', '" + sDateTime + "', 0,'" + sStartTime + "','" + sEndDateTime + "','" + xmlFilePath + "')";
                                                            mainForm.dba.UpdateOrInsertBySQL(sqlstr);
                                                           
                                                            string sORG_ID2 = m_AreaCode;

                                                            if ((PlayType == "2"))
                                                            {
                                                                sqlstr = "insert into TsCmdStore(TsCmd_Type, TsCmd_Mode, TsCmd_UserID, TsCmd_ValueID, TsCmd_Params, TsCmd_Date, TsCmd_Status,TsCmd_EndTime,TsCmd_Note)" +
                                                                   "values('播放视频', '区域', 1, " + sORG_ID2 + ", '1~" + strurl + "~0~1200~192~0~1~0', '" + sDateTime + "', 0,'" + sEndDateTime + "'," + "'-1'" + ")";
                                                                int iback = mainForm.dba.getResultIDBySQL(sqlstr, "TsCmdStore");

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
                                            }
                                            #endregion End
                                                break;
                                        case "EBMStateRequest":
                                            break;
                                        case "ConnectionCheck":
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
                System.Windows.Forms.MessageBox.Show(ex.Message, "错误");
                return "";
            }
        }
        #endregion 数据计算校验和

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
                catch(Exception fbEx)
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
                                        string fName = "10" + rState.sHBRONO +"0000000000000"+ rd.Next(100, 999).ToString();
                                        string xmlStateFileName = "\\EBDB_" + fName + ".xml";
                                        xmlStateDoc = rState.EBMStateRequestResponse(ebd, fName);
                                        CommonFunc.SaveXmlWithUTF8NotBOM(xmlStateDoc, sSourcePath + xmlStateFileName);

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
        }//回复处理线程

        #region 数据发送到串口
        private void SendCmd(SerialPort comSend, string sCmdStr, int SendTick = 1)
        {
            //, char cSplit, ref List<byte> list

            try
            {
                int iSum = 0;
                List<byte> listCmd = new List<byte>();
                string sSum = "";
                if (sCmdStr.Trim() == "")
                    return;
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

                listCmd.Add(byte.Parse(sSum, System.Globalization.NumberStyles.HexNumber));
                listCmd.Add(byte.Parse("16", System.Globalization.NumberStyles.HexNumber));

                listCmd.Insert(0, byte.Parse("FE", System.Globalization.NumberStyles.HexNumber));
                listCmd.Insert(0, byte.Parse("FE", System.Globalization.NumberStyles.HexNumber));
                listCmd.Insert(0, byte.Parse("FE", System.Globalization.NumberStyles.HexNumber));
                for (int j = 0; j < SendTick; j++)
                {
                    SendMsg(comSend, listCmd.ToArray());//发送数据
                    Thread.Sleep(150);
                }

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "错误");
                return;
            }

        }

        private void SendCRCCmd(SerialPort comSend, string sCmdStr, int SendTick = 1)
        {
            try
            {
               // int iSum = 0;
                List<byte> listCmd = new List<byte>();
               // string sSum = "";
                if (sCmdStr.Trim() == "")
                    return;
                string[] sTmp = sCmdStr.Split(' ');
                byte[] cmdByte = new byte[sTmp.Length];
                for (int i = 0; i < sTmp.Length; i++)
                {
                    cmdByte[i] = byte.Parse(sTmp[i], System.Globalization.NumberStyles.HexNumber);
                    listCmd.Add(cmdByte[i]);
                    //iSum = iSum + int.Parse(sTmp[i], System.Globalization.NumberStyles.HexNumber);
                }
                //sSum = Convert.ToString(iSum, 16).ToUpper().PadLeft(4, '0');
                //sSum = sSum.Substring(sSum.Length - 2, 2);

                //listCmd.Add(byte.Parse(sSum, System.Globalization.NumberStyles.HexNumber));
                listCmd.Add(byte.Parse("16", System.Globalization.NumberStyles.HexNumber));

                listCmd.Insert(0, byte.Parse("FE", System.Globalization.NumberStyles.HexNumber));
                listCmd.Insert(0, byte.Parse("FE", System.Globalization.NumberStyles.HexNumber));
                listCmd.Insert(0, byte.Parse("FE", System.Globalization.NumberStyles.HexNumber));
                for (int j = 0; j < SendTick; j++)
                {
                    SendMsg(comSend, listCmd.ToArray());//发送数据
                    Thread.Sleep(150);
                }

            }
            catch (Exception ex)
            {
                Log.Instance.LogWrite(sCmdStr);
                System.Windows.Forms.MessageBox.Show(ex.Message, "错误");
                return;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comSend"></param>
        /// <param name="msgbyte"></param>
        /// <returns></returns>
        private Boolean SendMsg(SerialPort comSend, byte[] msgbyte)
        {
            if (comSend.IsOpen)
            {
                try
                {
                    //转换列表为数组后发送
                    comSend.Write(msgbyte.ToArray(), 0, msgbyte.Length);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comSend"></param>
        /// <param name="msgstr">16进制字符串</param>
        /// <returns></returns>
        private Boolean SendMsg(SerialPort comSend, string msgstr)
        {
            string[] sTmp = msgstr.Split(' ');
            byte[] msgbyte = new byte[sTmp.Length];
            for (int i = 0; i < sTmp.Length; i++)
            {
                msgbyte[i] = byte.Parse(sTmp[i], System.Globalization.NumberStyles.HexNumber);
            }

            if (comSend.IsOpen)
            {
                try
                {
                    //转换列表为数组后发送
                    comSend.Write(msgbyte.ToArray(), 0, msgbyte.Length);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            //return true;
        }

        #endregion End 数据发送

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

        private void PlayAudio(string url)
        {
            SetText("播放音频文件url：" + url);
            MediaPlayer.Ctlcontrols.stop();
            MediaPlayer.URL = url;
            MediaPlayer.Ctlcontrols.play();
            //switch (mplayer.playState)
            //{
            //    case WMPLib.WMPPlayState.wmppsReady:
            //        mplayer.Ctlcontrols.play();
            //        break;
            //    case WMPLib.WMPPlayState.wmppsStopped:
            //        mplayer.Ctlcontrols.next();
            //        break;
            //}
          //  mainForm.bMsgStatusFree = true;  //2016-04-06

        }

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
            result = System.Text.Encoding.Default.GetString(arrByte);

            return result;
        }

        private void SaveEBD(EBD ebdstruct,int tscmd_id)
        {
            //return;
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
                                     "','" + strBRDSendTime + "','" + strEBMID + "','" + strBRDMsgType + "','" + ""+//strBRDExerciseType +
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
                        catch(Exception es)
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
        private void sendEBMStateRequestResponse()//(EBD ebdsr, string EBDstyle)
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
                        SetText(DateTime.Now.ToString() + "应急消息播发状态请求反馈：" + ebd.EBMStateRequest.EBM.EBMID);                        
                    }
                }
            }
            catch (Exception err)
            {
                Log.Instance.LogWrite("应急消息播发状态请求反馈:" + DateTime.Now.ToString() + err.Message);
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
                CommonFunc.SaveXmlWithUTF8NotBOM(xmlHeartDoc, sEBMStateResponsePath + xmlSignFileName);
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
                                SetText("文转语播放结束" + DateTime.Now.ToString());
                                Thread.Sleep(1000);
                                listAreaCode.Clear();//清除应急区域列表
                                iHoldTimesCnt = 0;
                                //this.txtMsgShow.Text = "";
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
        private void MediaPlayer_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {   /*0:    // Undefined
              1:    // Stopped
              2:    // Paused
              3:    // Playing
              4:    // ScanForward
              5:    // ScanReverse
              6:    // Buffering
              7:    // Waiting
              8:    // MediaEnded
              9:    // Transitioning
              10:   // Ready
              11:   // Reconnecting
              12:   // Last
               */
            if (e.newState == 8)
            {
                try
                {
                    // MessageBox.Show("播放完毕");
                    for (int i = 0; i < listAreaCode.Count; i++)
                    {
                        string cmdOpen = "4C " + listAreaCode[i] + " C0 02 00 01";
                        Log.Instance.LogWrite("状态改变事件-媒体播放完毕应急关机：" + cmdOpen);
                        string strsum = DataSum(cmdOpen);
                        cmdOpen = "FE FE FE " + cmdOpen + " " + strsum + " 16";
                        string strsql = "";
                        strsql = "insert into CommandPool(CMD_TIME,CMD_BODY,CMD_FLAG)" +
                        " VALUES('" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "','" + cmdOpen + "','" + '0' + "')";
                        mainForm.dba.UpdateOrInsertBySQL(strsql);
                        mainForm.dba.UpdateOrInsertBySQL(strsql);
                    }
                    Log.Instance.LogWrite("状态改变事件-语音播放结束：" + DateTime.Now.ToString());// + cmdStr);                    
                    SetText("播放音频文件结束:" + DateTime.Now.ToString());
                    try
                    {
                        SetText("状态改变事件 - 语音播放结束反馈");
                        //sendEBMStateResponse(ebd);
                    }
                    catch (Exception es)
                    {
                        SetText("播放音频文件结束反馈错误:" + es.Message);
                        Log.Instance.LogWrite("状态改变事件-播放音频文件结束反馈错误:" + es.Message);
                    }
                }
                catch (Exception ea)
                {
                    SetText("状态改变事件-播放音频文件结束处理错误:" + ea.Message);
                    Log.Instance.LogWrite("状态改变事件-播放音频文件结束处理错误:" + ea.Message);
                }
                finally
                {
                    listAreaCode.Clear();//清除应急区域列表
                    MediaPlayer.Ctlcontrols.stop();
                    MediaPlayer.URL = "";
                    MediaPlayer.close();
                    iHoldTimesCnt = 0;
                    bCharToAudio = "";
                }
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
                CommonFunc.SaveXmlWithUTF8NotBOM(xmlHeartDoc, sHeartSourceFilePath + xmlStateFileName);
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
                string xmlStateFileName = "EBDB_" + "01" + rHeart.sHBRONO + "0000000000000000.xml";
                CommonFunc.SaveXmlWithUTF8NotBOM(xmlHeartDoc, sHeartSourceFilePath + "\\" + xmlStateFileName);
                tar.CreatTar(sHeartSourceFilePath, sSendTarPath, "01" + rHeart.sHBRONO + "0000000000000000");
            }
            catch (Exception ec)
            { 
                Log.Instance.LogWrite("心跳处错误：" + ec.Message);
            }
            string sHeartBeatTarName = sSendTarPath + "\\" + "EBDT_" + "01" + rHeart.sHBRONO + "0000000000000000" + ".tar";
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

        public void SetText(string text)              //线程间同步
        {
            if (this.txtMsgShow.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                string strs = this.txtMsgShow.Text;
                string[] strR = strs.Split("\r\n".ToCharArray());     //\r\n   为回车符号   
                int i = strR.Length - 1;     //得到   strR数组   的长度   
                if (i > 100)
                {
                    this.txtMsgShow.Clear();
                    this.txtMsgShow.Refresh();
                }
                this.txtMsgShow.Text += text;
                this.txtMsgShow.Text += Environment.NewLine;
            }
        }

        private void CMDtoPool(string AreaCode,string OpenFlag)
        {
            string[] AreaCodeList = AreaCode.Split(new char[] { ',' });
            List<string> SLAreaCode = new List<string>();
            listAreaCode.Clear();
            for (int a = 0; a < AreaCodeList.Length; a++)
            {
                string strTmpAddr = AreaCodeList[a];
                int iIndex = -1;
                iIndex = strTmpAddr.IndexOf(strHBAREACODE);  //是否是本区域
                if ((iIndex == 0) || (strTmpAddr.Substring(2) == "0000000000") || (strTmpAddr.Substring(4) == "00000000"))//(strTmpAddr.Length != 14)
                {
                    strAreaFlag = "1";//1代表命令发送到本区域，2代表上一级，3代表上上一级
                    string strTmpAddrA = ReplaceToAA(strTmpAddr) + "AAAAAAAAAAAAAAAAAA";
                    // strTmpAddrA.PadRight(18, 'A');
                    strTmpAddrA = strTmpAddrA.Substring(4, 10);
                    strTmpAddrA = L_H(strTmpAddrA);
                    SLAreaCode.Add(strTmpAddrA);
                    listAreaCode.Add(strTmpAddrA);
                }
                else
                {
                    strAreaFlag = "";
                }
            }
            for (int i = 0; i < SLAreaCode.Count; i++)
            {
                string cmdOpen="";
                if (OpenFlag == "OPEN")
                {
                    cmdOpen = "4C " + SLAreaCode[i] + " C0 02 01 04";
                    Log.Instance.LogWrite("应急开机：" + cmdOpen);
                }
                else
                {
                    cmdOpen = "4C " + SLAreaCode[i] + " C0 02 00 01";
                    Log.Instance.LogWrite("应急关机：" + cmdOpen);
                }
                string strsum = DataSum(cmdOpen);
                cmdOpen = "FE FE FE " + cmdOpen + " " + strsum + " 16";
                string strsql = "";
                strsql = "insert into CommandPool(CMD_TIME,CMD_BODY,CMD_FLAG)" +
                " VALUES('" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "','" + cmdOpen + "','" + '0' + "')";
                mainForm.dba.UpdateOrInsertBySQL(strsql);
            }
        }

        private void tim_MediaPlay_Tick(object sender, EventArgs e)
        {

        }

        private void tim_ClearMemory_Tick(object sender, EventArgs e)  //定时释放内存
        {
            ClearMemory();
        }
        
        #region 内存回收//2016-04-25 add
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
            string frdStateName="";
            try
            {
                Random rdState = new Random();
                frdStateName = "10" + rHeart.sHBRONO + "0000000000000" + rdState.Next(100, 999).ToString();
                string xmlEBMStateFileName = "\\EBDB_" + frdStateName + ".xml";

                xmlHeartDoc = rHeart.platformInfoResponse(frdStateName);
                CommonFunc.SaveXmlWithUTF8NotBOM(xmlHeartDoc, sHeartSourceFilePath + xmlEBMStateFileName);
                ServerForm.mainFrm.GenerateSignatureFile(sHeartSourceFilePath, frdStateName);
                ServerForm.tar.CreatTar(sHeartSourceFilePath, ServerForm.sSendTarPath, frdStateName);//使用新TAR
                string sHeartBeatTarName = sSendTarPath + "\\" + "EBDT_" + frdStateName + ".tar";
                HttpSendFile.UploadFilesByPost(sZJPostUrlAddress, sHeartBeatTarName);
            }
            catch (Exception err)
            {
                // Log.Instance.LogWrite("应急消息播发状态请求反馈:" + DateTime.Now.ToString() + err.Message);
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
                MediaSql = "select SRV_ID,SRV_CODE from SRV";
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

                            xmlHeartDoc = rHeart.DeviceStateResponse(lDev, frdStateName);
                            CommonFunc.SaveXmlWithUTF8NotBOM(xmlHeartDoc, sHeartSourceFilePath + xmlEBMStateFileName);
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
                        CommonFunc.SaveXmlWithUTF8NotBOM(xmlHeartDoc, sHeartSourceFilePath + xmlEBMStateFileName);
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
                    CommonFunc.SaveXmlWithUTF8NotBOM(xmlHeartDoc, sHeartSourceFilePath + xmlEBMStateFileName);
                    ServerForm.mainFrm.GenerateSignatureFile(sHeartSourceFilePath, frdStateName);
                    ServerForm.tar.CreatTar(sHeartSourceFilePath, ServerForm.sSendTarPath, frdStateName);//使用新TAR
                    string sHeartBeatTarName = sSendTarPath + "\\" + "EBDT_" + frdStateName + ".tar";
                    HttpSendFile.UploadFilesByPost(sZJPostUrlAddress, sHeartBeatTarName);
                }
            }
            catch (Exception err)
            {
                // Log.Instance.LogWrite("应急消息播发状态请求反馈:" + DateTime.Now.ToString() + err.Message);
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
                CommonFunc.SaveXmlWithUTF8NotBOM(xmlHeartDoc, sHeartSourceFilePath + xmlEBMStateFileName);

                ServerForm.mainFrm.GenerateSignatureFile(sHeartSourceFilePath, frdStateName);

                ServerForm.tar.CreatTar(sHeartSourceFilePath, ServerForm.sSendTarPath, frdStateName);//使用新TAR
                string sHeartBeatTarName = sSendTarPath + "\\" + "EBDT_" + frdStateName + ".tar";
                HttpSendFile.UploadFilesByPost(sZJPostUrlAddress, sHeartBeatTarName);
            }
            catch (Exception err)
            {
                // Log.Instance.LogWrite("应急消息播发状态请求反馈:" + DateTime.Now.ToString() + err.Message);
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
                MediaSql = "select SRV_ID,SRV_CODE from SRV";
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
                            CommonFunc.SaveXmlWithUTF8NotBOM(xmlHeartDoc, sHeartSourceFilePath + xmlEBMStateFileName);
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
                        CommonFunc.SaveXmlWithUTF8NotBOM(xmlHeartDoc, sHeartSourceFilePath + xmlEBMStateFileName);

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
                    CommonFunc.SaveXmlWithUTF8NotBOM(xmlHeartDoc, sHeartSourceFilePath + xmlEBMStateFileName);

                    ServerForm.mainFrm.GenerateSignatureFile(sHeartSourceFilePath, frdStateName);

                    ServerForm.tar.CreatTar(sHeartSourceFilePath, ServerForm.sSendTarPath, frdStateName);//使用新TAR
                    string sHeartBeatTarName = sSendTarPath + "\\" + "EBDT_" + frdStateName + ".tar";
                    HttpSendFile.UploadFilesByPost(sZJPostUrlAddress, sHeartBeatTarName);
                }
            }
            catch (Exception err)
            {
                // Log.Instance.LogWrite("应急消息播发状态请求反馈:" + DateTime.Now.ToString() + err.Message);
            }
        }

        //XML解析测试
        private void button5_Click(object sender, EventArgs e)
        {//E:\work\93\RevTarTmp\EBDT_100102320000000000010000000000004579

            ccplay.StopCPPPlayer2();

            string sSignFileName = "F://EBDS_EBDB_100102320000000000010000000000005304.xml";
            using (FileStream SignFs = new FileStream(sSignFileName, FileMode.Open))
            {
                StreamReader signsr = new StreamReader(SignFs, System.Text.Encoding.UTF8);
                string xmlsign = signsr.ReadToEnd();
                //xmlsign = xmlsign.Replace("xmlns:xs", "xmlns");
                signsr.Close();
                responseXML signrp = new responseXML();//签名回复
                XmlDocument xmlSignDoc = new XmlDocument();
                try
                {
                    xmlsign = XmlSerialize.ReplaceLowOrderASCIICharacters(xmlsign);
                    xmlsign = XmlSerialize.GetLowOrderASCIICharacters(xmlsign);
                    Signature sign = XmlSerialize.DeserializeXML<Signature>(xmlsign);

                    int nDeviceHandle = (int)mainFrm.phDeviceHandle;

                    byte[] strpcData;
                    byte[] pucCounter = new byte[4];

                    pucCounter[0] = 0X00;
                    pucCounter[1] = 0X00;
                    pucCounter[2] = 0X00;
                    pucCounter[3] = 0X35;

                    byte[] pucSignCerSn = new byte[6];

                    pucSignCerSn[0] = 0X00;
                    pucSignCerSn[1] = 0X00;
                    pucSignCerSn[2] = 0X00;
                    pucSignCerSn[3] = 0X00;
                    pucSignCerSn[4] = 0X00;
                    pucSignCerSn[5] = 0X84;

                    Encoding myEncoding = Encoding.GetEncoding("utf-8");
                    strpcData = Convert.FromBase64String(sign.SignatureValue);


                    mainFrm.usb.VerifySignatureWithTrustedCert(ref nDeviceHandle, strpcData, strpcData.Length, pucCounter, pucSignCerSn, strpcData);
                }
                catch (Exception ex)
                {
                    Log.Instance.LogWrite("签名文件错误：" + ex.Message);
                   // xmlSignDoc = signrp.SignResponse("", "Error");
                }
                xmlSignDoc.Save(sSourcePath + "\\EBDSign.xml");
            }
        }

        private void barCheckItem1_CheckedChanged(object sender,DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BarCheckItem item = e.Item as BarCheckItem;
            if (item == null) return;
            switch (item.Caption)
            {
                case "dockPanel3":
                    if (!item.Checked)
                        dockPanel3.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
                    else
                        dockPanel3.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
                    break;
                case "dockPanel1":
                    if (!item.Checked)
                        panelContainer1.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
                    else
                        panelContainer1.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
                    break;
            }
        }
    }
}
