using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using Microsoft.Win32.SafeHandles;
using System.Diagnostics;
using System.Threading;
namespace GRPlatForm
{
    public class ccplayer
    {
        string strNO;
        string m_strCPPPlayPath;
        string m_strMediaPathName;
        string m_strIP;
        string m_Port;
        string m_strPiPeName;
        string m_strEventName;
        string m_nAudioPID;
        string m_nVedioPID;
        string m_nVedioRat;
        string m_nAuioRat;
        Microsoft.Win32.SafeHandles.SafeWaitHandle m_handle;
        NamedPipeServerStream pipeStream = null;
        public bool init(string strCPPPlayPath, string strMediaPathName, string strIP, string Port, string strPiPeName, string strEventName, string nAudioPID, string nVedioPID, string nVedioRat, string nAuioRat)
        {
            m_strCPPPlayPath = strCPPPlayPath;
            m_strMediaPathName = strMediaPathName;
            m_strIP = strIP;
            m_Port = Port;
          //  m_strPiPeName = strPiPeName;
           // m_strEventName = strEventName;
            m_nAudioPID = nAudioPID;
            m_nVedioPID = nVedioPID;
            m_nVedioRat = nVedioRat;
            m_nAuioRat = nAuioRat;

            return true;
        }
        public bool CreatePipeandEvent(string strPiPeName, string strEventName)
        {
            try
            {
                m_strPiPeName = strPiPeName ;
                m_strEventName = strEventName ;
                pipeStream = new NamedPipeServerStream(strPiPeName);
                m_handle = CoreEvent.CreateEvent(default(IntPtr), false, false, strEventName);
                strNO = strNO + "1";
                return true;

            }
            catch (Exception es)
            {
                return false;
            }
        }
        public void StopCPPPlayer()
        {
            try
            {
                pipeStream.WaitForConnection();
                using (StreamReader sr = new StreamReader(pipeStream))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        Thread.Sleep(500);
                        string strValue = line;
                        if (line == "-1000")
                        {
                            break;
                        }
                    }
                }
                CoreEvent.SetEvent(m_handle);
                pipeStream.Close();
                CoreEvent.CloseHandle(m_handle);
            }
            catch (Exception es)
	        {
		        return;
	        }
        }
        public void StopCPPPlayer2()
        {
            try
            {
                CoreEvent.SetEvent(m_handle);
                pipeStream.Close();
                CoreEvent.CloseHandle(m_handle);
            }
            catch (Exception es)
            {
                return;
            }
        }
        public bool CreateCPPPlayer()
        {
            string strPath = m_strCPPPlayPath;
	        try
	        {
                string strCmd = m_strMediaPathName; //  "file:///D:/123.mp3 192.168.4.90 4007 103 hshuh-448-48485-5844 101 pipename 11600 128";
                strCmd += " ";
		        strCmd += m_strIP;
		        strCmd += " ";
		        strCmd += m_Port;
		        strCmd += " ";
		        strCmd += m_nAudioPID;
		        strCmd += " ";
		        strCmd += m_strEventName;
		        strCmd += " ";
		        strCmd += m_nVedioPID;
		        strCmd += " ";
		        strCmd += m_strPiPeName;
		        strCmd += " ";
		        strCmd += m_nVedioRat;
		        strCmd += " ";
		        strCmd += m_nAuioRat;

                Process proc = new Process();
                proc.StartInfo.FileName = "cppPlayer.exe";
                proc.StartInfo.WorkingDirectory = strPath;
                proc.StartInfo.Arguments = strCmd;
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.Start();

               return true;
	        }
            catch (Exception es)
	        {
		        return false;
	        }
        }

    }
}
