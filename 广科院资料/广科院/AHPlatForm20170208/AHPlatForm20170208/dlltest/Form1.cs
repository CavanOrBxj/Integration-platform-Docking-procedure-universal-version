using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO.Pipes;
using System.IO;
using System.Runtime.InteropServices;
using HANDLE = System.IntPtr;

namespace dlltest
{
    public partial class Form1 : Form
    {
        [System.Runtime.InteropServices.DllImport("TTSDLL.dll", EntryPoint = "TTSConvertOut", ExactSpelling = true, CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl, CharSet = System.Runtime.InteropServices.CharSet.Ansi, SetLastError = true)]
        public static extern void TTSConvertOut([System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)]string szPath, [System.Runtime.InteropServices.InAttribute()][System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string szContent);
   

        System.Diagnostics.ProcessStartInfo p = null;
        System.Diagnostics.Process Proc;

        public static System.Threading.ManualResetEvent mre = new System.Threading.ManualResetEvent(false);
        AnonymousPipeServerStream pipeStream;
        public Form1()
        {
            InitializeComponent();
            try
            {
                    mre.Reset();
                    Process process = new Process();
                    process.StartInfo.FileName = "cppPlayer.exe";
                    //创建匿名管道流实例，

                    using (pipeStream = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable))
                    {
                        //pipeStream.
                        //将句柄传递给子进程
                        string strlhandle = mre.ToString();
                        string strpipe = pipeStream.GetClientHandleAsString();

                        //string strPam = strlhandle + " " + 
                        process.StartInfo.Arguments = "file:///D:/123.mp3 192.168.4.90 9885 103 " + strlhandle + " " + strpipe + " 1600 128";
                        process.StartInfo.UseShellExecute = true;
                        process.Start();
                    }
                    //process.WaitForExit();
                    //process.Close();
            }
            catch (Exception dxml)
            {
                
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
          //  SetEvent(phandle);
            

        }
    }
}
