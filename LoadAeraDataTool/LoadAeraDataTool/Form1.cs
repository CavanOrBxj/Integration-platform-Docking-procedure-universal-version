using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LoadAeraDataTool
{
    public partial class Form1 : Form
    {

        string filepath = "";

        public static dbAccess dba;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtfilename.Text = openFileDialog1.FileName;
            }
           
        }


        //获取数据库连接字符串
        private String GetConnectString()
        {
            string sReturn;
            string sPass;
            sPass = txtDbPass.Text.Trim();
            if (sPass == "")
                sPass = "tuners2012";
            sReturn = "server = " + txtServer.Text.Trim() +
                   ";UID = " + txtDbuser.Text.Trim() +
                    ";Password = " + sPass +
                     ";DataBase = " + txtDb.Text.Trim() + ";"
                     + "MultipleActiveResultSets=True";

            return sReturn;
        }

        

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtServer.Text!="" || txtDb.Text!="" || txtDbuser.Text!="" || txtDbPass.Text!="")
                {


                    dba = new dbAccess();
                    dba.conn.ConnectionString = GetConnectString();

                    if (!dba.OpenConn())
                    {
                        //先判断有误表 AeraCodeNation  没有的话先创建
                        String tableNameStr = "select count(1) from sysobjects where name = 'AeraCodeNation'";

                        SqlCommand cmd = new SqlCommand(tableNameStr, dba.conn);
                        int result = Convert.ToInt32(cmd.ExecuteScalar());
                        if (result == 0)
                        {
                            //未创建  开始创建表 AeraCodeNation
                            if (dba.conn.State==ConnectionState.Open)
                            {
                                dba.conn.Close();
                            }
                            string ConnectionString = "Integrated Security=SSPI;";

                        }


                        MessageBox.Show("数据库连接失败，请检查连接设置！", "提示");
                        return;
                    }

                }
                else
                {
                    MessageBox.Show("数据库信息不能为空！");
                    return;
                }


                if (txtfilename.Text!="")
                {
                    string[] lines = System.IO.File.ReadAllLines(txtfilename.Text);

                    if (lines.Length>0)
                    {
                        foreach (var item in lines)
                        {
                            string[] tmp = item.Split('#');
                            string code = tmp[1];
                            string name = tmp[0];
                            string InfoValueStr = "insert into AeraCodeNation values('" + code + "','" + name + "')";
                            dba.UpdateDbBySQL(InfoValueStr);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("文件名不能为空！");
                }

                MessageBox.Show("数据导入完成");
              
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
