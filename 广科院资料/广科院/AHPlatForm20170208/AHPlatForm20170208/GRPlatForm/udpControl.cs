using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace GRPlatForm
{
    public class udpControl
    {
        private List<string> arrReceive = null;
        private List<byte> listRecByte = null;
        
        private UdpClient udpClient = null;
        public Thread thdRec = null;
        

        public int TimeOut
        {
            set;
            get;
        }
        public bool RecAndSend
        {
            set;
            get;
        }
        public string sFilterIP
        {
            get;
            set;
        }

        public string sLocalIP
        {
            set;
            get;
        }
        public int RecNum
        {
            set;
            get;
        }

        public Object LockRecObject
        {
            set;
            get;
        }
        public Boolean BKeep
        {
            set;
            get;
        }
        public IPAddress UdpIP
        {
            set;
            get;
        }
        public int UdpPort
        {
            set;
            get;
        }
        public int RecMode { set; get; }
        /// <summary>
        /// 非阻塞模式  false 则为阻塞模式 true 为非阻塞模式
        /// </summary>
        public Boolean IsNotBlock
        {
            set;
            get;
        }

        public udpControl()
        {
            //初始化为阻塞模式
            IsNotBlock = false;
        }

        public Boolean InitUdp()
        {
            try
            {
                if (UdpPort == 0)
                {
                    //udpClient = new UdpClient(new IPEndPoint(UdpIP, UdpPort)
                    if (!GetSinglePort())
                        throw new SocketException();
                }
                else
                {
                    if (UdpIP != null)
                    {
                        udpClient = new UdpClient(new IPEndPoint(UdpIP, UdpPort));  //绑定IP和端口
                    }
                    else
                    {
                        udpClient = new UdpClient(UdpPort);  //绑定端口
                    }
                }
                //udpClient.Client.SetSocketOption(SocketOptionLevel.Socket,
                //        SocketOptionName.ReuseAddress, true);
                udpClient.Client.ReceiveBufferSize = 20 * 1024;
                udpClient.Client.SendBufferSize = 20 * 1024;
                //设置为非阻塞模式
                //主要应用于大批量终端信息传输
                if (IsNotBlock)
                    udpClient.Client.Blocking = false;

                if (TimeOut > 0)
                    udpClient.Client.ReceiveTimeout = TimeOut * 1000;
                LockRecObject = new Object();
                arrReceive = new List<string>();
                listRecByte = new List<byte>();
            }
            catch (SocketException exsc)
            {
                System.Windows.Forms.MessageBox.Show("端口" + UdpPort.ToString() + "错误：" + exsc.Message, "错误");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("InitUdp:" + ex.Message);
                return false;
            }
            return true;
        }

        private Boolean GetSinglePort()
        {
            //维持端口发送
            //确保不占用指定的端口
            Boolean blOK = false;
            while (!blOK)
            {
                if (UdpIP != null)
                {
                    udpClient = new UdpClient(new IPEndPoint(UdpIP, 0));  //绑定IP
                }
                else
                {
                    udpClient = new UdpClient();
                }
                SendMsg(new byte[0], 0, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 65535));
                UdpPort = (udpClient.Client.LocalEndPoint as IPEndPoint).Port;


                blOK = true;
            }
            return true;
        }

        //启用线程，开始监听 
        public void StartRec(string sIP)
        {
            RecNum = 0;
            thdRec = new Thread(RecInfoMsg);
            sFilterIP = sIP;
            thdRec.IsBackground = true;
            thdRec.Name = "UDP监听，端口" + this.UdpPort.ToString();
            thdRec.Start();
        }

        //为插播监听服务 
        public void StartRec(List<string> sIP)
        {
            RecNum = 0;
            thdRec = new Thread(Moniter);
            thdRec.IsBackground = true;
            thdRec.Name = "UDP监听，端口" + this.UdpPort.ToString();
            thdRec.Start(sIP);
        }



        //文件播放
        //监听本机自我转发音频并发送到iSendEP
        public void StartRec(string sIP, List<IPEndPoint> iSendEP)
        {
            RecNum = 0;
            sFilterIP = sIP;
            thdRec = new Thread(RecAndSendByIP);
            thdRec.IsBackground = true;
            thdRec.Name = "UDP监听，端口" + this.UdpPort.ToString();
            thdRec.Start(iSendEP);
        }




        public void UdpStop()
        {
            try
            {
                if (udpClient != null)
                {
                    udpClient.Close();
                    udpClient = null;
                }
                if (thdRec != null)
                {
                    try
                    {
                        thdRec.Abort();
                    }
                    catch (Exception)
                    {
                        thdRec = null;
                    }
                }
                if (arrReceive != null)
                    arrReceive = null;
                if (listRecByte != null)
                    listRecByte = null;
            }
            catch(Exception ex)
            {
                Console.WriteLine("UdpStop:" + ex.Message);
            }

        }


        //打开UDP端口并监听 
        //阻塞模式
        public void RecInfoMsg()
        {
            try
            {
                IPEndPoint iep = null;
                while (BKeep)
                {
                    try
                    {

                        byte[] bRec = udpClient.Receive(ref iep);
                        if (iep.Address.ToString() != sFilterIP)
                        {
                            Thread.Sleep(10);
                            continue;
                        }
                        lock (LockRecObject)
                        {
                            if (RecMode == 0)
                            {
                                for (int i = 0; i < bRec.Length; i++)
                                {
                                    arrReceive.Add(Convert.ToString(bRec[i], 16).PadLeft(2, '0').ToUpper());
                                }
                            }
                            else if (RecMode == 1)
                            {
                                listRecByte.AddRange(bRec);
                            }
                        }
                        Thread.Sleep(100);
                    }
                    catch (SocketException ex)
                    {
                        if (ex.SocketErrorCode == SocketError.TimedOut)
                        {
                            Console.WriteLine("超时：" + ex.Message);
                            continue;
 
                        }
                    }
                }
            }
            finally
            {
            }

        }
        //打开UDP端口并监听转发
        //非阻塞模式
        public void RecAndSendByIP(object o)
        {
            List<IPEndPoint> iSendEP = (o as List<IPEndPoint>);
            try
            {
                IPEndPoint iep = null;
                while (BKeep)
                {
                    try
                    {
                        if (udpClient.Available > 0)
                        {
                            byte[] bRec = udpClient.Receive(ref iep);
                            if (iep.Address.ToString() != sFilterIP)  //是否是指定IP传送的数据
                                continue;

                            SendMsg(bRec, bRec.Length, iSendEP);
                        }
                        else
                        {
                            Thread.Sleep(10);
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("RecAndSendByIP:" + ex.Message);
                    }
                }
            }
            finally
            {
            }

        }




        //打开UDP端口,并监听命令传递信息
        //阻塞模式
        public void Moniter(object o)
        {
            List<string> iSendEP = (o as List<string>);

            
            //int iDataLen = 0;  //0x16字符长度定位
            //int iSum;  //累加校验和
            //string sTotal, sSum;  //组合字符串及累加校验和字符
            //string sLogCode = "";
            //int iPos = 0;

            List<string> listtemp = new List<string>();

            try
            {
                IPEndPoint iep = null;
                while (BKeep)
                {
                    try
                    {
                        if (udpClient.Available > 0)
                        {
                            byte[] bRec = udpClient.Receive(ref iep);
                            listtemp.Clear();
                            //if (iSendEP.IndexOf(iep.Address.ToString()) > -1)  //是否是上级IP传送的数据
                            //    continue;
                            for (int i = 0; i < bRec.Length; i++)
                            {
                                listtemp.Add(Convert.ToString(bRec[i], 16).PadLeft(2, '0').ToUpper());
                            }

                            //iPos = listtemp.IndexOf("4C");
                            //while (iPos > -1)
                            //{
                            //    sTotal = "";
                            //    iSum = 0;
                            //    sSum = "";

                            //    if (iPos + 7 < listtemp.Count)  //判断接收内容是否足够解析
                            //    {
                            //        //得到完整字符串的终止位16所在的位置
                            //        iDataLen = iPos + 7 + Convert.ToInt32(listtemp[iPos + 7], 16) + 2;
                            //        //判断接收内容是否完整
                            //        if (iDataLen < listtemp.Count)
                            //        {
                            //            //非指定指令，返回继续接受
                            //            //34、B4话筒 33、B3电话  38、B8停止插播
                            //            if (!(listtemp[iPos + 6] != "34" || listtemp[iPos + 6] != "B4"
                            //                || listtemp[iPos + 6] != "33" || listtemp[iPos + 6] != "B3"
                            //                || listtemp[iPos + 6] != "38" || listtemp[iPos + 6] != "B8"
                            //                || listtemp[iPos + 6] != "30" || listtemp[iPos + 6] != "B0"))
                            //            {
                            //                Thread.Sleep(500);
                            //                listtemp.RemoveRange(0, iPos + 1);
                            //                continue;
                            //            }
                            //            //如果终止位不是16，则
                            //            if (listtemp[iDataLen] != "16")
                            //            {
                            //                listtemp.RemoveRange(0, iDataLen + 1);
                            //                Thread.Sleep(500);
                            //                continue;
                            //            }

                            //            for (int j = iPos; j <= iDataLen; j++)
                            //            {
                            //                sTotal = sTotal + listtemp[j] + " ";
                            //                //判断SUM校验位
                            //                if (j < iDataLen - 1)
                            //                    iSum = iSum + Convert.ToInt32(listtemp[j], 16);
                            //            }
                            //            sSum = iSum.ToString("x").PadLeft(4, '0');
                            //            //开始解析字符串
                            //            if (Convert.ToInt32(sSum.Substring(sSum.Length - 2, 2), 16) != Convert.ToInt32(listtemp[iDataLen - 1], 16))
                            //            {
                            //                Console.WriteLine("校验和不相等，字符串传输存在错误！ " + sTotal);
                            //                Console.WriteLine(sTotal);
                            //                listtemp.RemoveRange(0, iDataLen + 1);
                            //                continue;
                            //            }
                            //            lock (LockRecObject)
                            //            {
                            //                arrReceive.Add(iep.Address.ToString() + "|" + sTotal.Trim());
                            //            }
                            //            listtemp.RemoveRange(0, iDataLen + 1);
                            //            iPos = listtemp.IndexOf("4C");
                            //        }
                            //    }
                            //}
                            //listtemp.Clear();
                            //Thread.Sleep(10);
                        }
                        else
                        {
                            Thread.Sleep(10);
                            continue;
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Moniter:" + ex.Message);
                    }
                }
            }
            finally
            {
                //if (udpClient != null)
                //{
                //    udpClient.Close();
                //    udpClient = null;
                //}
                //try
                //{
                //    thdRec.Abort();
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine("Moniter:" + ex.Message);
                //    thdRec = null;
                //}
            }

        }


        //打开UDP端口并监听 
        //阻塞模式
        public void RecInfoMsgByIP(Object o)
        {
            string sIP = (o as string);
            try
            {
                IPEndPoint iep = null;
                while (BKeep)
                {
                    try
                    {
                        byte[] bRec = udpClient.Receive(ref iep);
                        if (iep.Address.ToString() != sIP)  //是否是上级IP传送的数据
                            continue;
                        RecNum += bRec.Length;
                        lock (LockRecObject)
                        {
                            if (RecMode == 0)
                            {
                                for (int i = 0; i < bRec.Length; i++)
                                {
                                    arrReceive.Add(Convert.ToString(bRec[i], 16).PadLeft(2, '0').ToUpper());
                                }
                            }
                            else if (RecMode == 1)
                            {
                                listRecByte.AddRange(bRec);
                            }
                        }
                        Thread.Sleep(10);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("RecInfoMsgByIP1:" + ex.Message);
                    }
                }
            }
            finally
            {
                //if (udpClient != null)
                //{   
                //    udpClient.Close();
                //    udpClient = null;
                //}
                //try
                //{
                //    thdRec.Abort();
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine("RecInfoMsgByIP2:" + ex.Message);
                //    thdRec = null;
                //}
            }

        }


        //发送消息
        public int SendMsg(String Msg)
        {
            return 1;

        }


        //发送消息
        public int SendMsg(byte[] bSend, int iLen, List<IPEndPoint> SendIP)
        {

            if (SendIP != null && iLen > 0 && SendIP.Count > 0)
            {
                int iBuffSize = iLen * SendIP.Count;
                if (iBuffSize > udpClient.Client.SendBufferSize)
                    udpClient.Client.SendBufferSize = iBuffSize + 1000;
            }
            for (int i = 0; i < SendIP.Count; i++)
            {
                try
                {
                    this.udpClient.Send(bSend, iLen, SendIP[i]);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("SendMsg:" + ex.Message);
                }
            }
            return 1;
        }


        //发送消息
        public int SendMsg(byte[] bSend, int iLen, IPEndPoint SendIP)
        {
            try
            {
                this.udpClient.Send(bSend, iLen, SendIP);
            }
            catch (Exception ex)
            {
                Console.WriteLine("SendMsg:" + ex.Message);
                return -1;
            }

            return 1;

        }

        //返回完整信息列表
        public Boolean GetDataList(ref List<string> list)
        {
            if (arrReceive == null) return false;
            if (list == null)
                list = new List<String>();
            try
            {
                lock (LockRecObject)
                {
                    if (arrReceive.Count == 0)
                        return false;
                    list.AddRange(arrReceive.GetRange(0, arrReceive.Count));
                    arrReceive.Clear();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetDataList " + ex.Message);
                return false;
            }
        }


        //返回完整信息列表
        public Boolean GetDataList(ref List<byte> list)
        {
            if (listRecByte == null) return false;
            if (list == null)
                list = new List<byte>();
            try
            {
                if (listRecByte.Count == 0)
                    return false;
                lock (LockRecObject)
                {
                    list.AddRange(listRecByte.GetRange(0, listRecByte.Count));
                    listRecByte.Clear();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetDataList " + ex.Message);
                return false;
            }
        }
    }
    
}
