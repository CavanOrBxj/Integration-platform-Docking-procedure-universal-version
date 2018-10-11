using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Net;
using System.Web;
using System.Globalization;


namespace GRPlatForm
{
    public class Serialize
    {
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="path"></param>
        /// <param name="objectType"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static object ConvertFileToObject(string path, Type objectType, Encoding encoding)
        {
            object convertedObject = null;
            if (!string.IsNullOrEmpty(path))
            {
                //string str = File.ReadAllText(path, Encoding.Unicode);
                //str = StripHTML(str);
                //File.WriteAllText(path, str, Encoding.Unicode); 
                 //System.Text.RegularExpressions.Regex.Replace(HttpUtility.HtmlEncode(str), @"[\x00-\x08]|[\x0B-\x0C]|[\x0E-\x1F]", "");
                XmlSerializer ser = new XmlSerializer(objectType);
                using (StreamReader reader = new StreamReader(path, encoding))
                {
                    convertedObject = ser.Deserialize(reader);
                    reader.Close();
                }
            }
            return convertedObject;
        }

        public static string StripHTML(string source)
        {
            try
            {
                string result;
                // Remove HTML Development formatting
                // Replace line breaks with space
                // because browsers inserts space
                result = source.Replace("\r", " ");
                //// Replace line breaks with space
                //// because browsers inserts space
                //result = result.Replace("\n", " ");
                //// Remove step-formatting
                //result = result.Replace("\t", string.Empty);
                //// Remove repeating spaces because browsers ignore them
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //                                                      @"( )+", " ");
                //// Remove the header (prepare first by clearing attributes)
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         @"<( )*head([^>])*>", "<head>",
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         @"(<( )*(/)( )*head( )*>)", "</head>",
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         "(<head>).*(</head>)", string.Empty,
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                //// remove all scripts (prepare first by clearing attributes)
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         @"<( )*script([^>])*>", "<script>",
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         @"(<( )*(/)( )*script( )*>)", "</script>",
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                ////result = System.Text.RegularExpressions.Regex.Replace(result,
                ////         @"(<script>)([^(<script>\.</script>)])*(</script>)",
                ////         string.Empty,
                ////         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         @"(<script>).*(</script>)", string.Empty,
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //// remove all styles (prepare first by clearing attributes)
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         @"<( )*style([^>])*>", "<style>",
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         @"(<( )*(/)( )*style( )*>)", "</style>",
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         "(<style>).*(</style>)", string.Empty,
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //// insert tabs in spaces of <td> tags
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         @"<( )*td([^>])*>", "\t",
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //// insert line breaks in places of <BR> and <LI> tags
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         @"<( )*br( )*>", "\r",
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         @"<( )*li( )*>", "\r",
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //// insert line paragraphs (double line breaks) in place
                //// if <P>, <DIV> and <TR> tags
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         @"<( )*div([^>])*>", "\r\r",
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         @"<( )*tr([^>])*>", "\r\r",
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         @"<( )*p([^>])*>", "\r\r",
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //// Remove remaining tags like <a>, links, images,
                //// comments etc - anything that's enclosed inside < >
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         @"<[^>]*>", string.Empty,
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //// replace special characters:
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         @" ", " ",
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         @"•", " * ",
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         @"‹", "<",
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         @"›", ">",
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         @"™", "(tm)",
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         @"⁄", "/",
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         @"<", "<",
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         @">", ">",
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         @"©", "(c)",
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         @"®", "(r)",
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //// Remove all others. More can be added, see
                ////http://hotwired.lycos.com/webmonkey/reference/special_characters/
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         @"&(.{2,6});", string.Empty,
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                //// for testing
                ////System.Text.RegularExpressions.Regex.Replace(result,
                ////       this.txtRegex.Text,string.Empty,
                ////       System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //// make line breaking consistent
                //result = result.Replace("\n", "\r");
                //// Remove extra line breaks and tabs:
                //// replace over 2 breaks with 2 and over 4 tabs with 4.
                //// Prepare first to remove any whitespaces in between
                //// the escaped characters and remove redundant tabs in between line breaks
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         "(\r)( )+(\r)", "\r\r",
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         "(\t)( )+(\t)", "\t\t",
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         "(\t)( )+(\r)", "\t\r",
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         "(\r)( )+(\t)", "\r\t",
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //// Remove redundant tabs
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         "(\r)(\t)+(\r)", "\r\r",
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //// Remove multiple tabs following a line break with just one tab
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         "(\r)(\t)+", "\r\t",
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //// Initial replacement target string for line breaks
                //string breaks = "\r\r\r";
                //// Initial replacement target string for tabs
                //string tabs = "\t\t\t\t\t";
                //for (int index = 0; index < result.Length; index++)
                //{
                //    result = result.Replace(breaks, "\r\r");
                //    result = result.Replace(tabs, "\t\t\t\t");
                //    breaks = breaks + "\r";
                //    tabs = tabs + "\t";
                //}
                // That's it.
                
                result = System.Text.RegularExpressions.Regex.Replace(HttpUtility.HtmlEncode(result), @"[\x00-\x08]|[\x0B-\x0C]|[\x0E-\x1F]", "");
                return result;
            }
            catch
            {
                return source;
            }
        }

        public static string ObjectToXmlSerializer(Object Obj)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            //去除xml声明
            settings.OmitXmlDeclaration = true;
            settings.Encoding = Encoding.Default;
            settings.Indent = true;//换行缩进
            System.IO.MemoryStream mem = new MemoryStream();
            using (XmlWriter writer = XmlWriter.Create(mem, settings))
            {
                //去除默认命名空间xmlns:xsd和xmlns:xsi
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                XmlSerializer formatter = new XmlSerializer(Obj.GetType());
                formatter.Serialize(writer, Obj, ns);
            }
            return Encoding.Default.GetString(mem.ToArray());
        }

        //反序列化
        public static T ObjectToXmlDESerializer<T>(string str) where T : class
        {
            object obj;
            using (System.IO.MemoryStream mem = new MemoryStream(Encoding.Default.GetBytes(str)))
            {
                using (XmlReader reader = XmlReader.Create(mem))
                {
                    XmlSerializer formatter = new XmlSerializer(typeof(T));
                    obj = formatter.Deserialize(reader);
                }
            }
            return obj as T;
        }

        /// </summary>
        /// <param name="tmp"></param>
        /// <returns></returns>
        public static string ReplaceLowOrderASCIICharacters(string tmp)
        {
            StringBuilder info = new StringBuilder();
            foreach (char cc in tmp)
            {
                int ss = (int)cc;
                if (((ss >= 0) && (ss <= 8)) || ((ss >= 11) && (ss <= 12)) || ((ss >= 14) && (ss <= 32)))
                    info.AppendFormat("&#x{0:X};", ss);
                else info.Append(cc);
            }
            return info.ToString();
        }
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetLowOrderASCIICharacters(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            int pos, startIndex = 0, len = input.Length;
            if (len <= 4) return input;
            StringBuilder result = new StringBuilder();
            while ((pos = input.IndexOf("&#x", startIndex)) >= 0)
            {
                bool needReplace = false;
                string rOldV = string.Empty, rNewV = string.Empty;
                int le = (len - pos < 6) ? len - pos : 6;
                int p = input.IndexOf(";", pos, le);
                if (p >= 0)
                {
                    rOldV = input.Substring(pos, p - pos + 1);
                    // 计算 对应的低位字符
                    short ss;
                    if (short.TryParse(rOldV.Substring(3, p - pos - 3), NumberStyles.AllowHexSpecifier, null, out ss))
                    {
                        if (((ss >= 0) && (ss <= 8)) || ((ss >= 11) && (ss <= 12)) || ((ss >= 14) && (ss <= 32)))
                        {
                            needReplace = true;
                            rNewV = Convert.ToChar(ss).ToString();
                        }
                    }
                    pos = p + 1;
                }
                else pos += le;
                string part = input.Substring(startIndex, pos - startIndex);
                if (needReplace) result.Append(part.Replace(rOldV, rNewV));
                else result.Append(part);
                startIndex = pos;
            }
            result.Append(input.Substring(startIndex));
            return result.ToString();
        }

    }
}

