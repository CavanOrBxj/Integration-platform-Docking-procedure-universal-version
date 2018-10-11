using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Xml;
using System.Data;

namespace GRPlatForm
{
    /// <summary>
    /// Http GET，POST处理过程类
    /// </summary>
    public class HttpProcessor
    {
        public TcpClient socket;
        public HttpServerBase srv;
        private Stream inputStream;
        //public StreamWriter outputStream;
        public Stream outputStream;
        public String http_method;
        public String http_url;
        public String http_protocol_versionstring;
        public Hashtable httpHeaders = new Hashtable();
        private string sSeparateString = string.Empty;
        private string sEndLine = "\r\n";

        private static int MAX_POST_SIZE = 100 * 1024 * 1024; // 100MB
        //private const int BUF_SIZE = 1 * 1024 * 1024;

        public HttpProcessor(TcpClient s, HttpServerBase srv)
        {
            this.socket = s;
            this.srv = srv;
        }

        private string streamReadLine(Stream inputStream)
        {
            int next_char;
            string data = "";
            while (true)
            {
                next_char = inputStream.ReadByte();
                if (next_char == '\n') { break; }
                if (next_char == '\r') { continue; }
                if (next_char == -1) { Thread.Sleep(1); continue; };
                data += Convert.ToChar(next_char);
            }
            return data;
        }

        private string streamDataReadLine(Stream inputStream,ref List<byte> lLData)
        {
            int next_char;
            List<byte> lListValue = new List<byte>();
            string data = "";
            while (true)
            {
                next_char = inputStream.ReadByte();
                lListValue.Add((byte)next_char);//
                if (next_char == '\n') 
                {
                    break; 
                }
                if (next_char == '\r')
                {
                    continue; 
                }
                if (next_char == -1) { Thread.Sleep(1); continue; };
                data += Convert.ToChar(next_char);
                
            }
            if (lLData.Count > 0)
                lLData.Clear();
            lLData.AddRange(lListValue);
            return data;
        }

        public void process()
        {
            // we can't use a StreamReader for input, because it buffers up extra data on us inside it's
            // "processed" view of the world, and we want the data raw after the headers
            inputStream = new BufferedStream(socket.GetStream());
            // we probably shouldn't be using a streamwriter for all output from handlers either
            //outputStream = new StreamWriter(new BufferedStream(socket.GetStream()));
            outputStream = new BufferedStream(socket.GetStream());
            try
            {
                if (parseRequest() == false)
                {
                    writeFailure();//返回失败标志
                    outputStream.Flush();
                    inputStream = null; outputStream = null;
                    socket.Close();
                    return;
                }
                readHeaders();
                if (http_method.Equals("GET"))
                {
                   // handleGETRequest();
                }
                else if (http_method.Equals("POST"))
                {
                    handlePOSTRequest();
                }
                else if (http_method.Equals("PUT"))//E:\工作\93\RevTarTmp
                {
                    ServerForm.lRevFiles.Add("F:\\work\\93\\RevTarTmp\\EBDT_100102320000000000010000000000005306.tar");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("处理请求错误: " + ex.Message);
                writeFailure();
            }
            outputStream.Flush();
            inputStream = null; outputStream = null;
            socket.Close();
        }
        /// <summary>
        /// 验证处理请求
        /// </summary>
        /// <returns>处理成功标志</returns>
        public bool parseRequest()
        {
            String request = streamReadLine(inputStream);
            string[] tokens = request.Split(' ');
            if (tokens.Length != 3)
            {
                //throw new Exception("invalid http request line");
                Console.WriteLine("头部验证错误，无法解析，丢弃处理！");
                return false;
            }
            http_method = tokens[0].ToUpper();
            http_url = tokens[1];
            http_protocol_versionstring = tokens[2];
            Console.WriteLine("头部验证字符串：" + request);
            return true;
        }

        public void readHeaders()
        {
            Console.WriteLine("readHeaders()");
            String line;
            sSeparateString = string.Empty;//初始化接收
            while ((line = streamReadLine(inputStream)) != null)
            {
                if (line.Equals(""))
                {
                    Console.WriteLine("got headers");
                    return;
                }

                int separator = line.IndexOf(':');
                if (separator == -1)
                {
                    if (line != "platformtype" && (sSeparateString != "" && !line.Contains(sSeparateString)))
                    {
                        if (line == "" || line == string.Empty)
                        {
                            return;//结束头部
                        }
                        else
                        {
                            Console.WriteLine("头部验证出错!");
                            return;
                        }
                    }
                    else
                    {
                        //Console.WriteLine(line);
                        continue;
                    }
                }
                String name = line.Substring(0, separator);

                int pos = separator + 1;
                while ((pos < line.Length) && (line[pos] == ' '))
                {
                    pos++; // strip any spaces
                }
                string value = line.Substring(pos, line.Length - pos);
                if (name == "Content-Type" && sSeparateString == "")
                {
                    string[] sSeparateVaule = value.Split('=');
                    if (sSeparateVaule.Length > 1)
                    {
                        sSeparateString = sSeparateVaule[1];
                    }
                }
                Console.WriteLine("头部: {0}:{1}", name, value);
                httpHeaders[name] = value;
            }
        }

        public void handleGETRequest()
        {
            srv.handleGETRequest(this);
        }

        private const int BUF_SIZE = 10 * 1024 * 1024;

        public void handlePOSTRequest()
        {
            Console.WriteLine("获取POST数据开始:get post data start");
            string sFileForldPath = ServerForm.sRevTarPath ;// ServerForm.sTarPathName.Substring(0, ServerForm.sTarPathName.LastIndexOf("\\"));//接收文件夹路径
            Console.WriteLine("204:" + sFileForldPath);
            string sFilePath = string.Empty;
            string revfilename = string.Empty;//接收到的文件名
            int content_len = 0;
            //MemoryStream ms = new MemoryStream();
            int iFileLen = -1;
            if (this.httpHeaders.ContainsKey("Content-Length"))
            {
                content_len = Convert.ToInt32(this.httpHeaders["Content-Length"]);
                if (content_len > MAX_POST_SIZE)
                {
                    Console.WriteLine(String.Format("POST Content-Length({0}) too big for this simple server",content_len));
                }
                byte[] buf = new byte[BUF_SIZE];
                List<byte> lListData = new List<byte>();
                string dealFilePath = "";
                int to_read = content_len;
                if (to_read > 0)
                {
                    string DataLine;
                    FileStream fs = null;
                    try
                    {
                        while ((DataLine = streamDataReadLine(inputStream,ref lListData)) != null)
                        {
                            if (DataLine.Equals("--" + sSeparateString) && sSeparateString != "") 
                            {
                                continue;
                            }
                            else if (DataLine.Equals("--" + sSeparateString + "--") && sSeparateString != "") 
                            {
                                if (fs != null)
                                    fs.Close();                                
                                DealTarBack(dealFilePath);//处理接收文件      2016-04-11 与下一句调换顺序
                                ServerForm.lRevFiles.Add(sFilePath);//完成接收文件后把文件增加到处理列表上去
                            }
                            else if (DataLine.Contains("Content-Disposition"))
                            {
                                string[] sSeparateVaule = DataLine.Split('=');
                                if (sSeparateVaule.Length > 1)
                                {
                                    revfilename = sSeparateVaule[sSeparateVaule.Length - 1];//文件名
                                    if (revfilename != "")
                                    {
                                        revfilename = revfilename.Replace("\"", "");
                                        sFilePath = sFileForldPath + "\\" + revfilename;
                                    }
                                    else
                                    {
                                        sFilePath = sFileForldPath + "\\" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".tar";
                                    }
                                    if (fs != null)
                                    {
                                        fs.Close();
                                    }
                                    else
                                    {
                                        fs = new FileStream(sFilePath, FileMode.Create, FileAccess.Write); //打开一个写入流
                                        iFileLen = 0;
                                    }
                                    dealFilePath = sFilePath;
                                    revfilename = string.Empty;
                                }
                                continue;
                            }
                            else if (DataLine.Contains("Content-Type"))
                            {
                                continue;
                            }
                            else //if (DataLine.Length > 0 || lListData.Count> 0)
                            {
                                //为数据内容
                                if (DataLine.Length == 0 && iFileLen == 0)
                                    continue;
                                iFileLen += lListData.Count;
                                fs.Write(lListData.ToArray(), 0, lListData.Count);
                                fs.Flush();
                            }
                        }//while
                        //DealTarBack(dealFilePath);//处理接收文件
                    }
                    catch (Exception em)
                    {
                        Console.WriteLine("295行：" + em.Message);
                    }
                    finally
                    {
                        if (fs != null)
                        {
                            fs.Close();
                        }
                    }
                }
                Console.WriteLine("接收Tar文件成功！");
                writeSuccess();

            }
            Console.WriteLine("get post data end");
           // srv.handlePOSTRequest(this, new StreamReader(inputStream));

        }

        public void DealTarBack(string filepath)
        {
            //Console.WriteLine("317:" + filepath);
            if (File.Exists(filepath))
            {
                try
                {
                    #region 先删除预处理解压缩包中的文件
                    foreach (string xmlfiledel in Directory.GetFileSystemEntries(ServerForm.strBeUnTarFolder))
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

                    ServerForm.tar.UnpackTarFiles(filepath, ServerForm.strBeUnTarFolder);//把压缩包解压到专门存放接收到的XML文件的文件夹下

                    string[] xmlfilenames = Directory.GetFiles(ServerForm.strBeUnTarFolder, "*.xml");//从解压XML文件夹下获取解压的XML文件名
                    string sTmpFile = string.Empty;
                    string sAnalysisFileName = "";

                    for (int i = 0; i < xmlfilenames.Length; i++)
                    {
                        sTmpFile = Path.GetFileName(xmlfilenames[i]);
                        if (sTmpFile.ToUpper().IndexOf("EBDB") > -1)
                        {
                            sAnalysisFileName = xmlfilenames[i];
                            break;
                        }
                    }
                    ServerForm.DeleteFolder(ServerForm.strBeSendFileMakeFolder);//删除原有XML发送文件的文件夹下的XML
                    Console.WriteLine("要解析文件：" + sAnalysisFileName);
                    if (sAnalysisFileName != "")
                    {
                        using (FileStream fsr = new FileStream(sAnalysisFileName, FileMode.Open))
                        {
                            StreamReader sr = new StreamReader(fsr, System.Text.Encoding.UTF8);
                            String xmlInfo = sr.ReadToEnd();
                            xmlInfo = xmlInfo.Replace("xmlns:xs", "xmlns");
                            sr.Close();
                            xmlInfo = XmlSerialize.ReplaceLowOrderASCIICharacters(xmlInfo);
                            xmlInfo = XmlSerialize.GetLowOrderASCIICharacters(xmlInfo);
                            EBD ebdb = XmlSerialize.DeserializeXML<EBD>(xmlInfo);

                            switch (ebdb.EBDType)
                            {
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
                                        string fName ="10" + rp.sHBRONO + "00000000000" + rd.Next(100, 999).ToString();
                                        xmlDoc = rp.EBMStreamResponse(fName, ServerForm.m_StreamPortURL);
                                        string xmlSignFileName = "\\EBDB_" + fName + ".xml";
                                        CommonFunc.SaveXmlWithUTF8NotBOM(xmlDoc, ServerForm.strBeSendFileMakeFolder + xmlSignFileName);

                                        //进行签名
                                        ServerForm.mainFrm.GenerateSignatureFile(ServerForm.strBeSendFileMakeFolder, fName);

                                        ServerForm.tar.CreatTar(ServerForm.strBeSendFileMakeFolder, ServerForm.sSendTarPath, fName);//使用新TAR
                                        string sSendTarName = ServerForm.sSendTarPath + "\\EBDT_" + fName + ".tar";
                                        FileStream fsSnd = new FileStream(sSendTarName, FileMode.Open, FileAccess.Read);
                                        BinaryReader br = new BinaryReader(fsSnd);     //时间戳 
                                        int datalen = (int)fsSnd.Length +2;
                                        int bufferLength = 4096;
                                        long offset = 0; //开始上传时间 
                                        writeHeader(datalen.ToString(), "EBDT_" + fName + ".tar");
                                                                               
                                        byte[] buffer = new byte[4096]; //已上传的字节数 
                                        int size = br.Read(buffer, 0, bufferLength);
                                        while (size > 0)
                                        {
                                            outputStream.Write(buffer, 0, size);
                                            offset += size;
                                            size = br.Read(buffer, 0, bufferLength);
                                        }
                                        outputStream.Write(Encoding.UTF8.GetBytes(sEndLine), 0, 2);
                                        outputStream.Flush();//提交写入的数据                                        
                                        fsSnd.Close();
                                      
                                    }
                                    catch(Exception esb)
                                    {
                                        Console.WriteLine("401:" + esb.Message);
                                    }
                                    #endregion End
                                    break;
                                case "EBM":
                                    #region 业务文件处理反馈
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
                                        string fName ="10" + rp.sHBRONO + "00000000000" + rd.Next(100, 999).ToString();
                                        xmlDoc = rp.EBDResponse(ebdb, "EBDResponse", fName);
                                        string xmlSignFileName = "\\EBDB_" + fName + ".xml";

                                        CommonFunc.SaveXmlWithUTF8NotBOM(xmlDoc, ServerForm.strBeSendFileMakeFolder + xmlSignFileName);
                                        ServerForm.tar.CreatTar(ServerForm.strBeSendFileMakeFolder, ServerForm.sSendTarPath, fName);//使用新TAR
                                        string sSendTarName = ServerForm.sSendTarPath + "\\EBDT_" + fName + ".tar";
                                        FileStream fsSnd = new FileStream(sSendTarName, FileMode.Open, FileAccess.Read);
                                        BinaryReader br = new BinaryReader(fsSnd);     //时间戳 
                                        int datalen = (int)fsSnd.Length +2;
                                        int bufferLength = 4096;
                                        long offset = 0; //开始上传时间 
                                        writeHeader(datalen.ToString(), "EBDT_" + fName + ".tar");
                                        
                                        byte[] buffer = new byte[4096]; //已上传的字节数 
                                        int size = br.Read(buffer, 0, bufferLength);
                                        while (size > 0)
                                        {
                                            outputStream.Write(buffer, 0, size);
                                            offset += size;
                                            size = br.Read(buffer, 0, bufferLength);
                                        }
                                        outputStream.Write(Encoding.UTF8.GetBytes(sEndLine), 0, 2);
                                        outputStream.Flush();//提交写入的数据                                        
                                        fsSnd.Close();
                                       
                                    }
                                    catch(Exception esb)
                                    {
                                        Console.WriteLine("401:" + esb.Message);
                                    }
                                    #endregion End
                                    break;
                                case "ConnectionCheck":
                                    #region 心跳检测反馈
                                    try
                                    {
                                        XmlDocument xmlHeartDoc = new XmlDocument();
                                        responseXML rHeart = new responseXML();
                                        rHeart.SourceAreaCode = ServerForm.strSourceAreaCode;
                                        rHeart.SourceType = ServerForm.strSourceType;
                                        rHeart.SourceName = ServerForm.strSourceName;
                                        rHeart.SourceID = ServerForm.strSourceID;
                                        rHeart.sHBRONO = ServerForm.strHBRONO;
                                        
                                        string fName = "01" + rHeart.sHBRONO + "0000000000000000";
                                        xmlHeartDoc = rHeart.EBDResponse(ebdb, "EBDResponse", fName);
                                        string xmlSignFileName = "\\EBDB_" + fName + ".xml";
                                        CommonFunc.SaveXmlWithUTF8NotBOM(xmlHeartDoc, ServerForm.strBeSendFileMakeFolder + xmlSignFileName);
                                        ServerForm.tar.CreatTar(ServerForm.strBeSendFileMakeFolder, ServerForm.sSendTarPath, fName);//使用新TAR
                                        string sHeartBeatTarName = ServerForm.sSendTarPath + "\\EBDT_" + fName + ".tar";
                                        FileStream fsHeartSnd = new FileStream(sHeartBeatTarName, FileMode.Open, FileAccess.Read);
                                        BinaryReader brHeart = new BinaryReader(fsHeartSnd);
                                        int Heartdatalen = (int)fsHeartSnd.Length + 2;
                                        int bufferHeartLength = 4096;
                                        long HeartOffset = 0; //
                                        writeHeader(Heartdatalen.ToString(),  "\\EBDT_" + fName + ".tar");//,ref fsSave 
                                        byte[] Heartbuffer = new byte[4096]; //已上传的字节数 
                                        int Heartsize = brHeart.Read(Heartbuffer, 0, bufferHeartLength);
                                        while (Heartsize > 0)
                                        {
                                            outputStream.Write(Heartbuffer, 0, Heartsize);
                                            HeartOffset += Heartsize;
                                            Heartsize = brHeart.Read(Heartbuffer, 0, bufferHeartLength);
                                        }
                                        outputStream.Write(Encoding.UTF8.GetBytes(sEndLine), 0, 2);
                                        outputStream.Flush();//提交写入的数据                                        
                                        fsHeartSnd.Close();                                        
                                    }
                                    catch (Exception ep)
                                    {
                                        Log.Instance.LogWrite("错误462行：" + ep.Message);
                                    }
                                    #endregion End
                                    break;
                                case "EBMStateRequest":
                                    #region 状态查询请求反馈
                                    
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
                                        
                                        string frdStateName = "10" + rState.sHBRONO + "0000000000000" +rdState.Next(100, 999).ToString();
                                        string xmlEBMStateFileName = "\\EBDB_" + frdStateName + ".xml";
                                        string MediaSql = "";
                                        string TsCmd_XmlFile = "";
                                        int TsCmd_ID = 0;
                                        EBD ebdStateRequest;
                                        string EBMID = ebdb.EBMStateRequest.EBM.EBMID;
                                      //  string OMDTYPE = ebdbO.omdRequest.OMDType;
                                        try
                                        {
                                            MediaSql = "select top(1)TsCmd_ID,TsCmd_XmlFile from TsCmdStoreMedia where TsCmd_ValueID = '" + EBMID + "' order by TsCmd_Date desc";
                                            DataTable dtMedia = mainForm.dba.getQueryInfoBySQL(MediaSql);
                                            if (dtMedia != null && dtMedia.Rows.Count > 0)
                                            {
                                                for (int idtM = 0; idtM < dtMedia.Rows.Count; idtM++)
                                                {
                                                    TsCmd_ID = (int)dtMedia.Rows[idtM][0];
                                                    TsCmd_XmlFile = dtMedia.Rows[idtM][1].ToString();
                                                    using (FileStream fs = new FileStream(TsCmd_XmlFile, FileMode.Open))
                                                    {
                                                        StreamReader sr2 = new StreamReader(fs, System.Text.Encoding.UTF8);
                                                        xmlInfo = sr2.ReadToEnd();
                                                        xmlInfo = xmlInfo.Replace("xmlns:xs", "xmlns");
                                                        sr.Close();
                                                        xmlInfo = XmlSerialize.ReplaceLowOrderASCIICharacters(xmlInfo);
                                                        xmlInfo = XmlSerialize.GetLowOrderASCIICharacters(xmlInfo);
                                                        ebdStateRequest = XmlSerialize.DeserializeXML<EBD>(xmlInfo);
                                                    }
                                                    xmlStateDoc = rState.EBMStateRequestResponse(ebdStateRequest, frdStateName);
                                                    CommonFunc.SaveXmlWithUTF8NotBOM(xmlStateDoc, ServerForm.strBeSendFileMakeFolder + xmlEBMStateFileName);

                                                    ServerForm.mainFrm.GenerateSignatureFile(ServerForm.strBeSendFileMakeFolder, frdStateName);

                                                    ServerForm.tar.CreatTar(ServerForm.strBeSendFileMakeFolder, ServerForm.sSendTarPath, frdStateName);//使用新TAR

                                                }
                                            }
                                            else
                                            {
                                                //xmlStateDoc = rState.EBMStateRequestResponse(ebdb, frdStateName);
                                                xmlStateDoc = rState.EBDResponseerror(ebdb, "EBDResponse", frdStateName);
                                                CommonFunc.SaveXmlWithUTF8NotBOM(xmlStateDoc, ServerForm.strBeSendFileMakeFolder + xmlEBMStateFileName);

                                                ServerForm.mainFrm.GenerateSignatureFile(ServerForm.strBeSendFileMakeFolder, frdStateName);
                                                ServerForm.tar.CreatTar(ServerForm.strBeSendFileMakeFolder, ServerForm.sSendTarPath, frdStateName);//使用新TAR
                                            }
                                        }
                                        catch (Exception err)
                                        {
                                            Log.Instance.LogWrite("应急消息播发状态请求反馈:" + DateTime.Now.ToString() + err.Message);
                                        }

                                        string sStateBeatTarName = ServerForm.sSendTarPath + "\\EBDT_" + frdStateName + ".tar";
                                        FileStream fsStateSnd = new FileStream(sStateBeatTarName, FileMode.Open, FileAccess.Read);
                                        BinaryReader brState = new BinaryReader(fsStateSnd);//
                                        int Statedatalen = (int)fsStateSnd.Length + 2;
                                        int bufferStateLength = 4096;
                                        long StateOffset = 0; //
                                        writeHeader(Statedatalen.ToString(), "\\EBDT_" + frdStateName + ".tar");//,ref fsSave 
                                        byte[] Statebuffer = new byte[4096]; //已上传的字节数 
                                        int Satesize = brState.Read(Statebuffer, 0, bufferStateLength);
                                        while (Satesize > 0)
                                        {
                                            outputStream.Write(Statebuffer, 0, Satesize);
                                            StateOffset += Satesize;
                                            Satesize = brState.Read(Statebuffer, 0, bufferStateLength);
                                        }
                                        outputStream.Write(Encoding.UTF8.GetBytes(sEndLine), 0, 2);
                                        outputStream.Flush();//提交写入的数据                                        
                                        fsStateSnd.Close();
                                    }
                                    catch (Exception h)
                                    {
                                        Log.Instance.LogWrite("错误510行:" + h.Message);
                                    }
                                    #endregion End
                                    break;
                                case "OMDRequest":
                                    #region 运维请求反馈

                                    string strOMDType = ebdb.OMDRequest.OMDType;
                                    string strOMDSubType = ebdb.OMDRequest.Params.RptType;
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
                                        string MediaSql = "";
                                        string strSRV_ID = "";
                                        string strSRV_CODE = "";
                                        EBD ebdStateRequest;
                                        List<Device> lDev = new List<Device>();
                                        if(strOMDType == "EBRDTInfo")
                                        {
                                            try
                                            {
                                                if (strOMDSubType == "Incremental")
                                                {
                                                    xmlStateDoc = rState.DeviceInfoResponse(ebdb, lDev, frdStateName);
                                                    CommonFunc.SaveXmlWithUTF8NotBOM(xmlStateDoc, ServerForm.strBeSendFileMakeFolder + xmlEBMStateFileName);
                                                    ServerForm.mainFrm.GenerateSignatureFile(ServerForm.strBeSendFileMakeFolder, frdStateName);
                                                    ServerForm.tar.CreatTar(ServerForm.strBeSendFileMakeFolder, ServerForm.sSendTarPath, frdStateName);//使用新TAR
                                                    sendResponse(frdStateName);
                                                }
                                                else 
                                                {
                                                    MediaSql = "select SRV_ID,SRV_CODE,SRV_PHYSICAL_CODE from SRV";
                                                    DataTable dtMedia = mainForm.dba.getQueryInfoBySQL(MediaSql);
                                                    if (dtMedia != null && dtMedia.Rows.Count > 0)
                                                    {
                                                        if (dtMedia.Rows.Count > 150)
                                                        {
                                                            int mod = dtMedia.Rows.Count / 100 + 1;
                                                            for (int i = 0; i < mod; i++)
                                                            {
                                                                frdStateName = "10" + rState.sHBRONO + "0000000000000" + rdState.Next(100, 999).ToString();
                                                                xmlEBMStateFileName = "\\EBDB_" + frdStateName + ".xml";
                                                                for (int idtM = 0; idtM < dtMedia.Rows.Count; idtM++)
                                                                {
                                                                    Device DV = new Device();
                                                                    strSRV_ID = dtMedia.Rows[idtM + i*100][0].ToString();
                                                                    strSRV_CODE = dtMedia.Rows[idtM + i * 100][1].ToString();
                                                                    DV.DeviceID = strSRV_ID;
                                                                    DV.DeviceName = strSRV_ID;
                                                                    DV.DeviceType = dtMedia.Rows[idtM + i * 100][2].ToString();
                                                                    lDev.Add(DV);

                                                                }
                                                                xmlStateDoc = rState.DeviceInfoResponse(ebdb, lDev, frdStateName);
                                                                CommonFunc.SaveXmlWithUTF8NotBOM(xmlStateDoc, ServerForm.strBeSendFileMakeFolder + xmlEBMStateFileName);
                                                                ServerForm.mainFrm.GenerateSignatureFile(ServerForm.strBeSendFileMakeFolder, frdStateName);
                                                                ServerForm.tar.CreatTar(ServerForm.strBeSendFileMakeFolder, ServerForm.sSendTarPath, frdStateName);//使用新TAR
                                                                sendResponse(frdStateName);
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
                                                                DV.DeviceType = dtMedia.Rows[idtM][2].ToString();
                                                                lDev.Add(DV);

                                                            }
                                                            xmlStateDoc = rState.DeviceInfoResponse(ebdb, lDev, frdStateName);
                                                            CommonFunc.SaveXmlWithUTF8NotBOM(xmlStateDoc, ServerForm.strBeSendFileMakeFolder + xmlEBMStateFileName);
                                                            ServerForm.mainFrm.GenerateSignatureFile(ServerForm.strBeSendFileMakeFolder, frdStateName);
                                                            ServerForm.tar.CreatTar(ServerForm.strBeSendFileMakeFolder, ServerForm.sSendTarPath, frdStateName);//使用新TAR
                                                            sendResponse(frdStateName);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        xmlStateDoc = rState.DeviceInfoResponse(ebdb, lDev, frdStateName);
                                                        CommonFunc.SaveXmlWithUTF8NotBOM(xmlStateDoc, ServerForm.strBeSendFileMakeFolder + xmlEBMStateFileName);
                                                        ServerForm.mainFrm.GenerateSignatureFile(ServerForm.strBeSendFileMakeFolder, frdStateName);
                                                        ServerForm.tar.CreatTar(ServerForm.strBeSendFileMakeFolder, ServerForm.sSendTarPath, frdStateName);//使用新TAR
                                                        sendResponse(frdStateName);    
                                                    }
                                                }

                                               
                                            }
                                            catch (Exception err)
                                            {
                                               // Log.Instance.LogWrite("应急消息播发状态请求反馈:" + DateTime.Now.ToString() + err.Message);
                                            } 
                                        }
                                        else if (strOMDType == "EBRDTState")
                                        {
                                            try
                                            {
                                                if (strOMDSubType == "Incremental")
                                                {
                                                    frdStateName = "10" + rState.sHBRONO + "0000000000000" + rdState.Next(100, 999).ToString();
                                                    xmlEBMStateFileName = "\\EBDB_" + frdStateName + ".xml";
                                                    xmlStateDoc = rState.DeviceStateResponse(ebdb, lDev, frdStateName);
                                                    CommonFunc.SaveXmlWithUTF8NotBOM(xmlStateDoc, ServerForm.strBeSendFileMakeFolder + xmlEBMStateFileName);
                                                    ServerForm.mainFrm.GenerateSignatureFile(ServerForm.strBeSendFileMakeFolder, frdStateName);
                                                    ServerForm.tar.CreatTar(ServerForm.strBeSendFileMakeFolder, ServerForm.sSendTarPath, frdStateName);//使用新TAR
                                                    sendResponse(frdStateName);
                                                }
                                                else
                                                {
                                                    MediaSql = "select SRV_ID,SRV_CODE,SRV_PHYSICAL_CODE from SRV";
                                                    DataTable dtMedia = mainForm.dba.getQueryInfoBySQL(MediaSql);
                                                    if (dtMedia != null && dtMedia.Rows.Count > 0)
                                                    {
                                                        if (dtMedia.Rows.Count > 100)
                                                        {
                                                            int mod = dtMedia.Rows.Count / 100 + 1;
                                                            for (int i = 0; i < mod; i++)
                                                            {
                                                                frdStateName = "10" + rState.sHBRONO + "0000000000000" + rdState.Next(100, 999).ToString();
                                                                xmlEBMStateFileName = "\\EBDB_" + frdStateName + ".xml";
                                                                for (int idtM = 0; idtM < dtMedia.Rows.Count; idtM++)
                                                                {
                                                                    Device DV = new Device();
                                                                    strSRV_ID = dtMedia.Rows[idtM + i * 100][0].ToString();
                                                                    strSRV_CODE = dtMedia.Rows[idtM + i * 100][1].ToString();
                                                                    DV.DeviceID = strSRV_ID;
                                                                    DV.DeviceName = strSRV_ID;
                                                                    DV.DeviceType = dtMedia.Rows[idtM + i * 100][2].ToString();
                                                                    lDev.Add(DV);

                                                                }
                                                                xmlStateDoc = rState.DeviceStateResponse(ebdb, lDev, frdStateName);
                                                                CommonFunc.SaveXmlWithUTF8NotBOM(xmlStateDoc, ServerForm.strBeSendFileMakeFolder + xmlEBMStateFileName);
                                                                ServerForm.mainFrm.GenerateSignatureFile(ServerForm.strBeSendFileMakeFolder, frdStateName);
                                                                ServerForm.tar.CreatTar(ServerForm.strBeSendFileMakeFolder, ServerForm.sSendTarPath, frdStateName);//使用新TAR
                                                                sendResponse(frdStateName);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            frdStateName = "10" + rState.sHBRONO + "0000000000000" + rdState.Next(100, 999).ToString();
                                                            xmlEBMStateFileName = "\\EBDB_" + frdStateName + ".xml";
                                                            for (int idtM = 0; idtM < dtMedia.Rows.Count; idtM++)
                                                            {
                                                                Device DV = new Device();
                                                                strSRV_ID = dtMedia.Rows[idtM][0].ToString();
                                                                strSRV_CODE = dtMedia.Rows[idtM][1].ToString();
                                                                DV.DeviceID = strSRV_ID;
                                                                DV.DeviceName = strSRV_ID;
                                                                DV.DeviceType = dtMedia.Rows[idtM][2].ToString();
                                                                lDev.Add(DV);

                                                            }
                                                            xmlStateDoc = rState.DeviceStateResponse(ebdb, lDev, frdStateName);
                                                            CommonFunc.SaveXmlWithUTF8NotBOM(xmlStateDoc, ServerForm.strBeSendFileMakeFolder + xmlEBMStateFileName);
                                                            ServerForm.mainFrm.GenerateSignatureFile(ServerForm.strBeSendFileMakeFolder, frdStateName);
                                                            ServerForm.tar.CreatTar(ServerForm.strBeSendFileMakeFolder, ServerForm.sSendTarPath, frdStateName);//使用新TAR
                                                            sendResponse(frdStateName);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        frdStateName = "10" + rState.sHBRONO + "0000000000000" + rdState.Next(100, 999).ToString();
                                                        xmlEBMStateFileName = "\\EBDB_" + frdStateName + ".xml";
                                                        xmlStateDoc = rState.DeviceStateResponse(ebdb, lDev, frdStateName);
                                                        CommonFunc.SaveXmlWithUTF8NotBOM(xmlStateDoc, ServerForm.strBeSendFileMakeFolder + xmlEBMStateFileName);
                                                        ServerForm.mainFrm.GenerateSignatureFile(ServerForm.strBeSendFileMakeFolder, frdStateName);
                                                        ServerForm.tar.CreatTar(ServerForm.strBeSendFileMakeFolder, ServerForm.sSendTarPath, frdStateName);//使用新TAR
                                                        sendResponse(frdStateName);
                                                    }
                                                }
                                            }
                                            catch (Exception err)
                                            {
                                                
                                            } 
                                        }
                                        else if (strOMDType == "EBRPSInfo")
                                        {
                                            try
                                            {
                                                xmlStateDoc = rState.platformInfoResponse(ebdb, lDev, frdStateName);
                                                CommonFunc.SaveXmlWithUTF8NotBOM(xmlStateDoc, ServerForm.strBeSendFileMakeFolder + xmlEBMStateFileName);
                                                ServerForm.tar.CreatTar(ServerForm.strBeSendFileMakeFolder, ServerForm.sSendTarPath, frdStateName);//使用新TAR
                                                sendResponse(frdStateName);
                                            }
                                            catch (Exception err)
                                            {
                                                // Log.Instance.LogWrite("应急消息播发状态请求反馈:" + DateTime.Now.ToString() + err.Message);
                                            }
                                        }
                                        else if (strOMDType == "EBRPSState")
                                        {
                                            try
                                            {
                                                xmlStateDoc = rState.platformstateInfoResponse(ebdb, lDev, frdStateName);
                                                CommonFunc.SaveXmlWithUTF8NotBOM(xmlStateDoc, ServerForm.strBeSendFileMakeFolder + xmlEBMStateFileName);
                                                ServerForm.mainFrm.GenerateSignatureFile(ServerForm.strBeSendFileMakeFolder, frdStateName);
                                                ServerForm.tar.CreatTar(ServerForm.strBeSendFileMakeFolder, ServerForm.sSendTarPath, frdStateName);//使用新TAR
                                                sendResponse(frdStateName);
                                            }
                                            catch (Exception err)
                                            {
                                                // Log.Instance.LogWrite("应急消息播发状态请求反馈:" + DateTime.Now.ToString() + err.Message);
                                            }
                                        }
                                        else
                                        {
                                            XmlDocument xmlDoc = new XmlDocument();
                                            responseXML rp = new responseXML();
                                            rp.SourceAreaCode = ServerForm.strSourceAreaCode;
                                            rp.SourceType = ServerForm.strSourceType;
                                            rp.SourceName = ServerForm.strSourceName;
                                            rp.SourceID = ServerForm.strSourceID;
                                            rp.sHBRONO = ServerForm.strHBRONO;
                                            
                                            Random rd = new Random();
                                            frdStateName = "10" + rp.sHBRONO + "00000000000" + rd.Next(100, 999).ToString();
                                            xmlEBMStateFileName = "\\EBDB_" + frdStateName + ".xml";

                                            xmlDoc = rp.EBDResponseyunweierror(ebdb, "EBDResponse", frdStateName);
                                            CommonFunc.SaveXmlWithUTF8NotBOM(xmlDoc, ServerForm.strBeSendFileMakeFolder + xmlEBMStateFileName);
                                            ServerForm.mainFrm.GenerateSignatureFile(ServerForm.strBeSendFileMakeFolder, frdStateName);
                                            ServerForm.tar.CreatTar(ServerForm.strBeSendFileMakeFolder, ServerForm.sSendTarPath, frdStateName);//使用新TAR
                                            sendResponse(frdStateName);
                                        }
                                    }
                                    catch (Exception h)
                                    {
                                        Log.Instance.LogWrite("错误510行:" + h.Message);
                                    }
                                    #endregion End
                                    break;
                                
                                default:
                                    break;
                            }

                        }
                    }

                }
                catch(Exception ep)
                {
                    Log.Instance.LogWrite(ep.Message);
                }
            }
        }

        public void sendResponse(string frdStateName)
        {
            string sStateBeatTarName = ServerForm.sSendTarPath + "\\EBDT_" + frdStateName + ".tar";
            FileStream fsStateSnd = new FileStream(sStateBeatTarName, FileMode.Open, FileAccess.Read);
            BinaryReader brState = new BinaryReader(fsStateSnd);
            int Statedatalen = (int)fsStateSnd.Length + 2;
            int bufferStateLength = 4096;
            long StateOffset = 0; //
            writeHeader(Statedatalen.ToString(), "\\EBDT_" + frdStateName + ".tar");
            byte[] Statebuffer = new byte[4096]; //已上传的字节数 
            int Satesize = brState.Read(Statebuffer, 0, bufferStateLength);
            while (Satesize > 0)
            {
                outputStream.Write(Statebuffer, 0, Satesize);
                StateOffset += Satesize;
                Satesize = brState.Read(Statebuffer, 0, bufferStateLength);
            }
            outputStream.Write(Encoding.UTF8.GetBytes(sEndLine), 0, 2);
            outputStream.Flush();//提交写入的数据                                        
            fsStateSnd.Close();
        
        }

        public void writeHeader(string strDataLen,string strTarName)//,ref FileStream fsave
        {
            StringBuilder sbHeader = new StringBuilder(200);

            sbHeader.Append("HTTP/1.1 200 OK" + sEndLine);//HTTP/1.1 200 OK
            sbHeader.Append("Content-Disposition:attachment;filename=" + "\"" + strTarName + "\"" + sEndLine);
            sbHeader.Append("Content-Type:application/x-tar" + sEndLine);
            sbHeader.Append("Server:WinHttpClient" + sEndLine);
            sbHeader.Append("Content-Length:" + strDataLen + sEndLine);
            sbHeader.Append("Date:" + DateTime.Now.ToString("r") + sEndLine);
            sbHeader.Append(sEndLine);
            byte[] bTmp = Encoding.UTF8.GetBytes(sbHeader.ToString());
            outputStream.Write(bTmp, 0, bTmp.Length);

        }

        public void writeSuccess(string content_type = "text/html")
        {
            //outputStream.WriteLine("HTTP/1.0 200 OK");
            //outputStream.WriteLine("Content-Type: " + content_type);
            //outputStream.WriteLine("Connection: close");
            //outputStream.WriteLine("");
        }

        public void writeFailure()
        {
            StringBuilder sbHeader = new StringBuilder(200);
            sbHeader.Append("HTTP/1.0 404 File not found" + sEndLine);
            sbHeader.Append("Connection: close" + sEndLine);
            sbHeader.Append(sEndLine);
            byte[] bTmp = Encoding.UTF8.GetBytes(sbHeader.ToString());
            outputStream.Write(bTmp, 0, bTmp.Length);
        }
    }
    /// <summary>
    /// Http服务基类
    /// </summary>
    public abstract class HttpServerBase
    {
        protected int port;
        protected IPAddress ipServer;
        TcpListener listener;
        bool is_active = true;

        public HttpServerBase(IPAddress ipserver, int port)
        {
            this.port = port;
            this.ipServer = ipserver;
        }

        public HttpServerBase(int port)
        {
            this.port = port;
        }

        public void listen()
        {
            if (ipServer == null)
            {
                listener = new TcpListener(port);//没有具体绑定IP
            }
            else
            {
                listener = new TcpListener(ipServer, port);//绑定具体IP
            }
            listener.Start();
            while (is_active)
            {
                if (listener.Pending())
                {
                    TcpClient s = listener.AcceptTcpClient();
                    HttpProcessor processor = new HttpProcessor(s, this);
                    Thread thread = new Thread(new ThreadStart(processor.process));
                    thread.Name = "监听线程:" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    thread.IsBackground = true;
                    thread.Start();
                    Thread.Sleep(1);
                }
                Thread.Sleep(500);
            }
        }

        public bool StopListen()
        {
            try
            {
                listener.Stop();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public abstract void handleGETRequest(HttpProcessor p);

        public abstract void handlePOSTRequest(HttpProcessor p, StreamReader inputData);
    }
    /// <summary>
    /// Http服务的接口实现
    /// </summary>
    public class HttpServer : HttpServerBase
    {
        public HttpServer(int port)
            : base(port)
        {
        }

        public HttpServer(IPAddress ipaddr, int port)
            : base(ipaddr, port)
        {
        }
        public override void handleGETRequest(HttpProcessor p)
        {
            Console.WriteLine("request: {0}", p.http_url);
            p.writeSuccess();
        }

        public override void handlePOSTRequest(HttpProcessor p, StreamReader inputData)
        {
            Console.WriteLine("POST request: {0}", p.http_url);
            p.writeSuccess();
        }
    }


}
