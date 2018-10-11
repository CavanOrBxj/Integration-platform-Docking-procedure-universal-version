using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.Collections;
using System.Xml;
using System.Collections.Specialized;
using AxWMPLib;
using System.Runtime.InteropServices;//2016-04-25 Dllimport 引用添加
using System.IO;

using System.Net;
using System.Net.Sockets;

namespace GRPlatForm
{
    public partial class mainForm : Form
    {
        public IniFiles ini;
        //子窗体定义
        private ComSetForm comsetFrm;
        private ServerIPSetForm setipFrm;
        private ServerSetForm setServerFrm;
        private TmpFolderSetForm tmpforldFrm;
        private InfoSetForm infoFrm;
        private ServerForm serverFrm;
        //
        public static dbAccess dba;
        public List<string> lTarPathName = new List<string>();//接收到的Tar包列表
        //public string sTarPathName = "";//
        public static string sSendTarName = "";//发送Tar包名字
        //public static string sRevTarPath = "";//接收Tar包存放路径
        //public static string sSendTarPath = "";//发送Tar包存放路径
        //public static string sSourcePath = "";//f需压缩文件路径
        //public static string sUnTarPath = "";//Tar包解压缩路径
        //public static string sAudioFilesFolder = "";//音频文件存放位置

        private bool Listening = false;     //是否没有执行完invoke相关操作
        private bool ComClosing = false;       //是否正在关闭串口，执行Application.DoEvents，并阻止再次invoke

        public string strSourceType = "";
        public string strSourceName = "";
        public string strSourceID = "";
        public string m_UsbPwsSupport = "";
        public static SerialPort comm = new SerialPort();
        public static SerialPort sndComm = new SerialPort();//临时发送语音用

        public static ComSet commSet = new ComSet();
        public static ComSet sndCommSet = new ComSet();//

        public static bool bWaitOrNo = true;//等待 2016-04-01
        public static bool bMsgStatusFree = false;//

        private List<byte> lCommData = new List<byte>();
        private object oComm = new object();
        private Thread thComm;

        private string CMDSND="";

        public USBE usb = new USBE();
        public System.IntPtr phDeviceHandle = (IntPtr)0;

        public mainForm()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
            ini = new IniFiles(@Application.StartupPath + "\\Config.ini");
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            m_UsbPwsSupport = ini.ReadValue("USBPSW", "USBPSWSUPPART");

            //打开密码器
            if (m_UsbPwsSupport == "1")
            {
                try
                {
                    int nReturn = usb.USB_OpenDevice(ref phDeviceHandle);
                    if (nReturn != 0)
                    {
                        MessageBox.Show("密码器打开失败！");
                    }
                }
                catch (Exception em)
                {
                    MessageBox.Show("密码器打开失败：" + em.Message);
                }
            }

            //初始化写日志线程
            string sLogPath = Application.StartupPath + "\\Log";
            if (!System.IO.Directory.Exists(sLogPath))
                System.IO.Directory.CreateDirectory(sLogPath);
            Log.Instance.LogDirectory = sLogPath + "\\";
            Log.Instance.FileNamePrefix = "EBD_";
            Log.Instance.CurrentMsgType = MsgLevel.Debug;
            Log.Instance.logFileSplit = LogFileSplit.Daily;
            Log.Instance.MaxFileSize = 2;
            Log.Instance.InitParam();
        }

        public void OpenCom(SerialPort scom, ComSet cs)
        {
            if (scom.IsOpen)
            {
                ComClosing = true;
                while (Listening) Application.DoEvents();
                //打开时点击，则关闭串口
                scom.Close();
            }
            else
            {
                //关闭时点击，则设置好端口，波特率后打开
                scom.PortName = cs.PortName;
                scom.BaudRate = cs.BaudRate;
                scom.DataBits = cs.DataBits;//8;
                scom.StopBits = cs.StopBits;   //(StopBits)Enum.Parse(typeof(StopBits), cob_StopBits.Text);
                scom.Parity = cs.Parity;  //(Parity)Enum.Parse(typeof(Parity), cob_Parity.Text);
                //scom.DataReceived += comm_DataReceived;//注册接收事件
                try
                {
                    scom.Open();
                    ComClosing = false;
                }
                catch (Exception ex)
                {
                    //捕获到异常信息，创建一个新的comm对象，之前的不能用了。
                    scom = new SerialPort();
                    //现实异常信息给客户。
                    MessageBox.Show(ex.Message);
                }
            }
        }

        #region  串口接收事件
        void SndComm_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //54 01 03 01 0X CRC 16
            try
            {
                int n = sndComm.BytesToRead;
                byte[] buf = new byte[n];//声明一个临时数组存储当前来的串口数据
                byte[] bufDeal = new byte[n];//声明一个临时数组用于回传处理
                int iRet = sndComm.Read(buf, 0, n);//读取缓冲数据
                buf.CopyTo(bufDeal, 0);
                lock (oComm)
                {
                    lCommData.AddRange(bufDeal);
                }
                //Log.Instance.LogWrite(string.Join(" ", buf.Select(t => t.ToString("X2")).ToArray()));

            }
            catch (Exception ExComm)
            {
                Log.Instance.LogWrite("处理数据错误:" + ExComm.Message);
            }
        }

        void SndCommDataDeal()
        {
            List<byte> lTmp = new List<byte>();
            while (true)
            {
                try
                {
                    lock (oComm)
                    {
                        if (lCommData.Count == 0)
                        {
                            Thread.Sleep(10);
                            continue;
                        }
                        lTmp.InsertRange(lTmp.Count, lCommData);
                        lCommData.Clear();
                    }
                }
                catch (Exception ExCom)
                {
                    Log.Instance.LogWrite("L222串口数据处理：" + ExCom.Message);
                }

                try
                {
                    if (lTmp.Count >= 10)
                    {

                        for (int i = 0; i < lTmp.Count - 6; i++)
                        {
                            if (lTmp[i] == 0x54)
                            {
                                if (i + 6 <= lTmp.Count)
                                {
                                    Log.Instance.LogWrite("L236:" + lTmp[i].ToString("X2") + lTmp[i + 1].ToString("X2") + lTmp[i + 2].ToString("X2") + lTmp[i + 3].ToString("X2") + lTmp[i + 4].ToString("X2")+lTmp[i + 5].ToString("X2"));
                                    if (lTmp[i + 2] == 0x0D)
                                    {
                                        if (lTmp[i + 4] == 0x01 && lTmp[i + 3] == 0x01 && lTmp[i + 5] == 0x64)
                                        {
                                            bMsgStatusFree = true;
                                            Log.Instance.LogWrite("Free");
                                        }
                                        else
                                        {
                                            bMsgStatusFree = false;
                                        }
                                        lock (oComm)
                                        {
                                            lTmp.RemoveRange(0, i + 6);
                                        }
                                    }
                                }
                                else
                                {
                                    lock (oComm)
                                    {
                                        if (i > 0)
                                        {
                                            lTmp.RemoveRange(0, i - 1);
                                        }
                                        else
                                        {
                                            lTmp.RemoveRange(0, i);
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
                catch
                {
                    Log.Instance.LogWrite("AudioCom Err!");
                }

            }
        }

        void comm_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            if (ComClosing) return;//如果正在关闭，忽略操作，直接返回，尽快的完成串口监听线程的一次循环
            try
            {
                Listening = true;//设置标记，说明我已经开始处理数据，一会儿要使用系统UI的。
                //Thread.Sleep(1000);         //延时1秒
                Int32 n = comm.BytesToRead;//先记录下来，避免某种原因，人为的原因，操作几次之间时间长，缓存不一致
                byte[] buf = new byte[n];//声明一个临时数组存储当前来的串口数据
                byte[] bufDeal = new byte[n];//声明一个临时数组用于回传处理
                comm.Read(buf, 0, n);//读取缓冲数据
                buf.CopyTo(bufDeal, 0);

                //因为要访问ui资源，所以需要使用invoke方式同步ui。
                this.Invoke((EventHandler)(delegate
                {
                    string str = string.Join(" ", bufDeal.Select(t => t.ToString("X2")).ToArray());
                    //if (serverFrm != null && !serverFrm.IsDisposed)
                    //{
                    //    serverFrm.ShowMsg(str);
                    //}
                    //builder.Append(string.Join(" ", buf.Select(t => t.ToString("X2")).ToArray()));
                    //直接按ASCII规则转换成字符串
                }));
            }
            catch (Exception ess)
            {
                Log.Instance.LogWrite("外层1:" + ess.Message);
            }
            finally
            {
                Listening = false;//我用完了，ui可以关闭串口了。
            }

        }
        #endregion

        #region 菜单响应

        private void mnuServerAddrSet_Click(object sender, EventArgs e)
        {
            if (setServerFrm == null || setServerFrm.IsDisposed)
            {
                setServerFrm = new ServerSetForm();
                setServerFrm.MdiParent = this;
                setServerFrm.Show();
            }
            else
            {
                if (setServerFrm.WindowState == FormWindowState.Minimized)
                {
                    setServerFrm.WindowState = FormWindowState.Normal;
                }
                else
                    setServerFrm.Activate();
            }
        }

        private void mnuComSet_Click(object sender, EventArgs e)
        {
            if (comsetFrm == null || comsetFrm.IsDisposed == true)
            {
                comsetFrm = new ComSetForm();
                comsetFrm.MdiParent = this;
                comsetFrm.Show();
            }
            else
            {
                if (comsetFrm.WindowState == FormWindowState.Minimized)
                {
                    comsetFrm.WindowState = FormWindowState.Normal;
                }
                else
                    comsetFrm.Activate();
            }
        }

        private void ServerIPSet_Click(object sender, EventArgs e)
        {
            if (setipFrm == null || setipFrm.IsDisposed)
            {
                setipFrm = new ServerIPSetForm();
                setipFrm.MdiParent = this;
                setipFrm.Show();
            }
            else
            {
                if (setipFrm.WindowState == FormWindowState.Minimized)
                {
                    setipFrm.WindowState = FormWindowState.Normal;
                }
                else
                    setipFrm.Activate();
            }
        }

        private void mnuFolderSet_Click(object sender, EventArgs e)
        {
            if (tmpforldFrm == null || tmpforldFrm.IsDisposed)
            {
                tmpforldFrm = new TmpFolderSetForm();
                tmpforldFrm.MdiParent = this;
                tmpforldFrm.Show();
            }
            else
            {
                if (tmpforldFrm.WindowState == FormWindowState.Minimized)
                {
                    tmpforldFrm.WindowState = FormWindowState.Normal;
                }
                else
                    tmpforldFrm.Activate();
            }
        }

        private void mnuSysInfoSet_Click(object sender, EventArgs e)
        {
            if (infoFrm == null || infoFrm.IsDisposed)
            {
                infoFrm = new InfoSetForm();
                infoFrm.MdiParent = this;
                infoFrm.Show();
            }
            else
            {
                if (infoFrm.WindowState == FormWindowState.Minimized)
                {
                    infoFrm.WindowState = FormWindowState.Normal;
                }
                else
                    infoFrm.Activate();
            }
        }

        private void mnuServerStart_Click(object sender, EventArgs e)
        {
            if (serverFrm == null || serverFrm.IsDisposed)
            {
                try
                {
                    serverFrm = new ServerForm();
                    serverFrm.MdiParent = this;
                    serverFrm.Show();
                }
                catch
                {
                }
            }
            else
            {
                if (serverFrm.WindowState == FormWindowState.Minimized)
                {
                    serverFrm.WindowState = FormWindowState.Normal;
                }
                else
                    serverFrm.Activate();
            }
        }

        #endregion End

        private void mnuExit_Click(object sender, EventArgs e)
        {
            if (serverFrm != null)
            {
                serverFrm.Close();
                serverFrm.Dispose();
            }
            if (comm != null)
            {
                comm.Close();
                comm.Dispose();
            }
            if (sndComm != null)
            {
                sndComm.Close();
                sndComm.Dispose();
            }
            this.Dispose(true);
            Application.ExitThread();
        }

        private void mainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.WindowState = FormWindowState.Minimized;
            if (serverFrm != null)
                serverFrm.Hide();
            this.Visible = false;
            this.Hide();
            this.ShowInTaskbar = false;
            this.nIcon.Visible = true;
            //关闭密码器
            if (m_UsbPwsSupport == "1")
            {
                try
                {
                    int nDeviceHandle = (int)phDeviceHandle;
                    int nReturn = usb.USB_CloseDevice(ref nDeviceHandle);

                }
                catch (Exception em)
                {
                    MessageBox.Show("密码器关闭失败：" + em.Message);
                }
            }
            //GC.Collect();
        }

        private void mnuShow_Click(object sender, EventArgs e)
        {
            if (!this.ShowInTaskbar)
            {
                this.Visible = true;
                this.Show();
                this.ShowInTaskbar = true;
                nIcon.Visible = false;
                foreach (Form frm in this.MdiChildren)
                {
                    if (!frm.IsDisposed & frm != null)
                        frm.Show();
                }
            }
        }

        private void mnuQuit_Click(object sender, EventArgs e)
        {
            if (serverFrm != null)
            {
                serverFrm.Close();
                serverFrm.Dispose();
            }
            if (comm != null)
            {
                comm.Close();
                comm.Dispose();
            }
            if (sndComm != null)
            {
                sndComm.Close();
                sndComm.Dispose();
            }
            if (thComm != null)
            {
                thComm.Abort();
                thComm = null;
            }
            this.Dispose(true);
            Application.ExitThread();
        }


        public void GenerateSignatureFile(string strPath,string strEBDID)
        {
            if (m_UsbPwsSupport != "1")
            {
                return;
            }

            string sSignFileName = "\\EBDB_" + strEBDID + ".xml";

            using (FileStream SignFs = new FileStream(strPath + sSignFileName, FileMode.Open))
            {
                StreamReader signsr = new StreamReader(SignFs, System.Text.Encoding.UTF8);
                string xmlsign = signsr.ReadToEnd();
                 signsr.Close();
                responseXML signrp = new responseXML();
                XmlDocument xmlSignDoc = new XmlDocument();
                try
                {
                    //对文件进行签名
                    int nDeviceHandle = (int)phDeviceHandle;
                    byte[] pucSignature = Encoding.UTF8.GetBytes(xmlsign);

                    string strSignture = "";
                    string strpucCounter = "";
                    string strpucSignCerSn = "";

                    int nReturn = usb.GenerateSignatureWithDevicePrivateKey(ref nDeviceHandle, pucSignature, pucSignature.Length, ref  strSignture, ref  strpucCounter, ref  strpucSignCerSn);

                    //生成签名文件
                    string xmlSIGNFileName = "\\EBDS_EBDB_" + strEBDID + ".xml";

                    string strrefdid = "";
                    string strResultValue = "";

                    xmlSignDoc = signrp.SignResponse(strEBDID, strpucCounter, strpucSignCerSn, strSignture);
                    
                    CommonFunc.SaveXmlWithUTF8NotBOM(xmlSignDoc, strPath + xmlSIGNFileName);

                }
                catch (Exception ex)
                {
                    Log.Instance.LogWrite("签名文件错误：" + ex.Message);
                    //xmlSignDoc = signrp.SignResponse("", "Error");
                }
               // xmlSignDoc.Save(sSourcePath + "\\EBDSign.xml");
            }
            
        }
    }
}
