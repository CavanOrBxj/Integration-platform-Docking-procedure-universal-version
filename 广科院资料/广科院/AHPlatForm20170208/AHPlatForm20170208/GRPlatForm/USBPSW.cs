using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace GRPlatForm
{
    public class USBE
    {
        [DllImport("libTassYJGBCmd_SJJ1313.dll", EntryPoint = "OpenDevice", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int OpenDevice(ref System.IntPtr phDeviceHandle);

        [DllImport("libTassYJGBCmd_SJJ1313.dll", EntryPoint = "CloseDevice", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int CloseDevice(ref int phDeviceHandle);

        //导入证书
        [DllImport("libTassYJGBCmd_SJJ1313.dll", EntryPoint = "ImportTrustedCert", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int ImportTrustedCert(ref int phDeviceHandle, StringBuilder strcertPath);

        //使用设备私钥计算数据签名
        [DllImport("libTassYJGBCmd_SJJ1313.dll", EntryPoint = "GenerateSignatureWithDevicePrivateKey", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int GenerateSignatureWithDevicePrivateKey(ref int phDeviceHandle, int nDataType, byte[] inputData, int nDataLength, byte[] pucCounter, byte[] pucSignCerSn, byte[] pucSignature);

        //使用设备私钥计算数据签名(字符串模式)
        [DllImport("libTassYJGBCmd_SJJ1313.dll", EntryPoint = "GenerateSignatureWithDevicePrivateKey_String", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int GenerateSignatureWithDevicePrivateKey_string(ref int phDeviceHandle, int nDataType, string pcData, byte[] pcResult);

        //使用证书验证数据签名
        [DllImport("libTassYJGBCmd_SJJ1313.dll", EntryPoint = "VerifySignatureWithTrustedCert", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int VerifySignatureWithTrustedCert(ref int phDeviceHandle, int nDataType, byte[] pucData, int nDataLength, byte[] pucCounter, byte[] pucSignCerSn, byte[] pucSignature);

        //使用证书验证数据签名（字符串）
        [DllImport("libTassYJGBCmd_SJJ1313.dll", EntryPoint = "VerifySignatureWithTrustedCert_String", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int VerifySignatureWithTrustedCert_String(ref int phDeviceHandle, int nDataType, byte[] pucData);

        //计算数据摘要
        [DllImport("libTassYJGBCmd_SJJ1313.dll", EntryPoint = "CalcHash", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int CalcHash(ref int phDeviceHandle, int nHashAlg, byte[] pucData, int nDataLength, byte[] pucHash, ref int pnHashLength);

        public int USB_OpenDevice(ref System.IntPtr phDeviceHandle)
        {
            int nReturn = OpenDevice(ref phDeviceHandle);
            return nReturn;
        }

        public int ImportTrustedCert(ref int phDeviceHandle)
        {
            StringBuilder strSrc = new StringBuilder("C:\\Windows\\windows_x32\\data");
            int nReturn = ImportTrustedCert(ref phDeviceHandle, strSrc);
            return nReturn;
        }

        public int USB_CloseDevice(ref int phDeviceHandle)
        {
            int nReturn = CloseDevice(ref phDeviceHandle);
            return nReturn;
        }

        public int GenerateSignatureWithDevicePrivateKey(ref int phDeviceHandle, byte[] strpcData, int size, ref string strSignture, ref string strpucCounter, ref string strpucSignCerSn)
        {
            //签名
            byte[] Signture = new byte[64];
            byte[] pucCounter = new byte[4];
            byte[] pucSignCerSn = new byte[6];

            int nResult = GenerateSignatureWithDevicePrivateKey(ref phDeviceHandle, 1, strpcData, size, pucCounter, pucSignCerSn, Signture);

            strSignture = Convert.ToBase64String(Signture);
            strpucCounter = Convert.ToBase64String(pucCounter);
            strpucSignCerSn = Convert.ToBase64String(pucSignCerSn);

            return nResult;
        }

        public int GenerateSignatureWithDevicePrivateKey_String(ref int phDeviceHandle, string strpcData, ref string strSignture)
        {
            //签名
            byte[] Signture = new byte[200];
            int nResult = GenerateSignatureWithDevicePrivateKey_string(ref phDeviceHandle, 1, strpcData, Signture);
            strSignture = Encoding.Default.GetString(Signture);
            return nResult;
        }
        //验签
        public int VerifySignatureWithTrustedCert(ref int phDeviceHandle, byte[] strpcData, int nDataLength, byte[] pucCounter, byte[] pucSignCerSn, byte[] pucSignature)
        {
            int nResult = VerifySignatureWithTrustedCert(ref phDeviceHandle, 1, strpcData, nDataLength, pucCounter, pucSignCerSn, pucSignature);
            return nResult;
        }
    }
}
