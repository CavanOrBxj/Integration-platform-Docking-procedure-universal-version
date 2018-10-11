using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Web;
using System.Windows.Forms;

namespace GRPlatForm
{
    public class HttpSendFile
    {

        public static string PostTarFile(string url, string filepath)
        {
            string result = "";
            try
            {
                //HttpPostedFile myFile = file1.PostedFile;
                //将文件转换成字节形式
                Guid guidSplit = Guid.NewGuid();
                string sSplitStr = "--" + guidSplit.ToString() + "--";
                byte[] splitbyte = StrToByte(sSplitStr, "UTF-8");
                FileStream fs = new FileStream(filepath, FileMode.Open);
                //获取文件大小
                long size = fs.Length + splitbyte.Length;//增加分隔符数据
                byte[] fileByte = new byte[size];
                //将文件读到byte数组中
                fs.Read(fileByte, 0, fileByte.Length);
                fs.Close();
                splitbyte.CopyTo(fileByte, size - splitbyte.Length);
                //str = path.Substring(pos + 1);
                string filename = filepath.Substring(filepath.LastIndexOf("\\") + 1);
                //通过WebClient类来提交文件数据
                //Content-Type:multipart/form-data;boundary=THIS_STRING_SEPARATES
                //定义提交URL地址,它会接收文件的字节数据，并保存，再返回相应的结果[此处具体用的时候要修改]
                string postUrl = url;
                System.Net.WebClient webClient = new System.Net.WebClient();
                webClient.Headers.Add("Content-Type", "multipart/form-data;boundary=" + guidSplit.ToString());
                webClient.Headers.Add("Charset", "UTF-8");
                webClient.Headers.Add("--" + guidSplit.ToString());
                webClient.Headers.Add("Content-Disposition", "attachment; filename=\"" + filename + "\"");
                webClient.Headers.Add("Content-Type", "application/x-tar");
                webClient.Headers.Add("--" + guidSplit.ToString());
                byte[] responseArray = webClient.UploadData(postUrl, "POST", fileByte);

                //将返回的字节数据转成字符串（也就是uploadpic.aspx里面的页面输出内容）
                result = System.Text.Encoding.Default.GetString(responseArray, 0, responseArray.Length);

                //返回结果的处理
                switch (result)
                {
                    case "-1":
                        // Console.WriteLine("文件上传时发生异常，未提交成功。");
                        result = "文件上传时发生异常，未提交成功。";
                        break;
                    case "0":
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                result = "上传异常！";
            }
            return result;
        }

        public static byte[] StrToByte(string msgstr, string CodeType)
        {
            byte[] byteArry;
            switch (CodeType)
            {
                case "Unicode":
                    byteArry = System.Text.Encoding.Unicode.GetBytes(msgstr);
                    break;
                case "UTF-8":
                    byteArry = System.Text.Encoding.UTF8.GetBytes(msgstr);
                    break;
                default:
                    byteArry = System.Text.Encoding.Default.GetBytes(msgstr);
                    break;
            }
            return byteArry;
        }

        /// 将本地文件上传到指定的服务器(HttpWebRequest方法)   
        /// </summary>   
        /// <param name="address">文件上传到的服务器</param>   
        /// <param name="fileNamePath">要上传的本地文件（全路径）</param>   
        /// <param name="saveName">文件上传后的名称</param>    
        /// <returns>成功返回1，失败返回0</returns>   
        public static string UploadFilesByPost(string address, string fileNamePath)
        {
            string returnValue = "0";     // 要上传的文件   
            FileStream fs = new FileStream(fileNamePath, FileMode.Open, FileAccess.Read);
            BinaryReader r = new BinaryReader(fs);     //时间戳   

            string sguidSplit = Guid.NewGuid().ToString();
            string filename = fileNamePath.Substring(fileNamePath.LastIndexOf("\\") + 1);

            StringBuilder sb = new StringBuilder(300);

            string strPostHeader = sb.ToString();
            HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(new Uri(address));
            httpReq.Method = "POST";     //对发送的数据不使用缓存   
            httpReq.AllowWriteStreamBuffering = false;     //设置获得响应的超时时间（300秒）   
            httpReq.Timeout = 30000;
            httpReq.ContentType = "multipart/form-data; boundary=" + sguidSplit;
            httpReq.Accept = "text/plain, */*";
            httpReq.UserAgent = "WinHttpClient";
            //httpReq.Connection = "Keep-Alive";

            httpReq.Headers["Accept-Language"] = "zh-cn";

            sb.Append("--" + sguidSplit + "\r\n");
            sb.Append("Content-Disposition: form-data; name=\"file\"; filename=\"" + filename + "\"\r\n");
            sb.Append("Content-Type: application/octet-stream;Charset=UTF-8\r\n");
            sb.Append("\r\n");

            byte[] boundaryBytes = Encoding.ASCII.GetBytes(sb.ToString());     //请求头部信息  
            byte[] bEndBytes = Encoding.ASCII.GetBytes("\r\n--" + sguidSplit + "--\r\n");
            long length = fs.Length + boundaryBytes.Length + bEndBytes.Length;
            long fileLength = fs.Length;
            httpReq.ContentLength = length;
            try
            {
                int bufferLength = 4096;//每次上传4k  
                byte[] buffer = new byte[bufferLength]; //已上传的字节数   
                long offset = 0;         //开始上传时间   
                DateTime startTime = DateTime.Now;

                int size = r.Read(buffer, 0, bufferLength);
                Stream postStream = httpReq.GetRequestStream();         //发送请求头部消息   
                postStream.Write(boundaryBytes, 0, boundaryBytes.Length);
                while (size > 0)
                {
                    postStream.Write(buffer, 0, size);
                    offset += size;
                    TimeSpan span = DateTime.Now - startTime;
                    double second = span.TotalSeconds;
                    Application.DoEvents();
                    size = r.Read(buffer, 0, bufferLength);
                }
                //添加尾部的时间戳 
                postStream.Write(bEndBytes, 0, bEndBytes.Length);
                postStream.Close();
                //获取服务器端的响应   
                WebResponse webRespon = httpReq.GetResponse();
                Stream s = webRespon.GetResponseStream();
                //读取服务器端返回的消息  
                StreamReader sr = new StreamReader(s);
                String sReturnString = sr.ReadLine();
                s.Close();
                sr.Close();
                returnValue = "1";
                // Console.WriteLine(sReturnString);
            }
            catch
            {
                returnValue = "0";
            }
            finally
            {
                fs.Close();
                r.Close();
            }
            return returnValue;
        }

        public static string UploadFilesByPostNoSplit(string address, string fileNamePath)
        {
            string returnValue = "0";     // 要上传的文件   
            FileStream fs = new FileStream(fileNamePath, FileMode.Open, FileAccess.Read);
            BinaryReader r = new BinaryReader(fs);     //时间戳   

            string sguidSplit = Guid.NewGuid().ToString();
            string filename = fileNamePath.Substring(fileNamePath.LastIndexOf("\\") + 1);
            StringBuilder sb = new StringBuilder(300);
            string strPostHeader = sb.ToString();
            HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(new Uri(address));
            httpReq.Method = "POST";     //对发送的数据不使用缓存   
            httpReq.AllowWriteStreamBuffering = false;     //设置获得响应的超时时间（300秒）   
            httpReq.Timeout = 30000;
            httpReq.ContentType = "multipart/form-data; boundary=" + sguidSplit;
            httpReq.Accept = "text/plain, */*";
            httpReq.UserAgent = "WinHttpClient";
            //httpReq.Connection = "Keep-Alive";

            httpReq.Headers["Accept-Language"] = "zh-cn";

            sb.Append("--" + sguidSplit + "\r\n");
            sb.Append("Content-Disposition: form-data;filename=\"" + filename + "\"\r\n");
            sb.Append("Content-Type: application/x-tar\r\n");
            sb.Append("\r\n");

            byte[] boundaryBytes = Encoding.ASCII.GetBytes(sb.ToString());     //请求头部信息  
            byte[] bEndBytes = Encoding.ASCII.GetBytes("\r\n--" + sguidSplit + "--\r\n");
            long length = fs.Length + boundaryBytes.Length + bEndBytes.Length;
            long fileLength = fs.Length;
            httpReq.ContentLength = length;
            try
            {
                int bufferLength = 4096;//每次上传4k  
                byte[] buffer = new byte[bufferLength]; //已上传的字节数   
                long offset = 0;         //开始上传时间   
                DateTime startTime = DateTime.Now;

                int size = r.Read(buffer, 0, bufferLength);
                Stream postStream = httpReq.GetRequestStream();         //发送请求头部消息   
                postStream.Write(boundaryBytes, 0, boundaryBytes.Length);
                while (size > 0)
                {
                    postStream.Write(buffer, 0, size);
                    offset += size;
                    TimeSpan span = DateTime.Now - startTime;
                    double second = span.TotalSeconds;
                    Application.DoEvents();
                    size = r.Read(buffer, 0, bufferLength);
                }
                //添加尾部的时间戳 
                postStream.Write(bEndBytes, 0, bEndBytes.Length);
                postStream.Close();         //获取服务器端的响应   
                WebResponse webRespon = httpReq.GetResponse();
                Stream s = webRespon.GetResponseStream();
                //读取服务器端返回的消息  
                StreamReader sr = new StreamReader(s);
                String sReturnString = sr.ReadLine();
                s.Close();
                sr.Close();
                returnValue = "1";
                Console.WriteLine(sReturnString);

            }
            catch
            {
                returnValue = "0";
            }
            finally
            {
                fs.Close();
                r.Close();
            }
            return returnValue;
        }
    }
}