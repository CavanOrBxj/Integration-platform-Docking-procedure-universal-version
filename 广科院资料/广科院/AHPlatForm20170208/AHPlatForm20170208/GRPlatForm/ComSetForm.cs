using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GRPlatForm
{
    public partial class ComSetForm : Form
    {
        private IniFiles ini;
        public ComSetForm()
        {
            InitializeComponent();
            ini = new IniFiles(@Application.StartupPath + "\\Config.ini");
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            ini.WriteValue("CMDCOM", "COM", comboPortName.Text);
            ini.WriteValue("CMDCOM", "BaudRate", comboBaudrate.Text);
            ini.WriteValue("CMDCOM", "DataBits", txt_DataBits.Text);
            ini.WriteValue("CMDCOM", "Parity", cob_Parity.Text);
            ini.WriteValue("CMDCOM", "StopBits", cob_StopBits.Text);

            ini.WriteValue("AudioCOM", "COM", comSndPortName.Text);
            ini.WriteValue("AudioCOM", "BaudRate", comSndBaudrate.Text);
            ini.WriteValue("AudioCOM", "DataBits", txt_SndDataBits.Text);
            ini.WriteValue("AudioCOM", "Parity", cob_SndParity.Text);
            ini.WriteValue("AudioCOM", "StopBits", cob_SndStopBits.Text);

        }

        private void ComSetForm_Load(object sender, EventArgs e)
        {
            string[] sPortRange = System.IO.Ports.SerialPort.GetPortNames();
            if (sPortRange.Length > 0)
            {
                comboPortName.Items.AddRange(sPortRange);
                comSndPortName.Items.AddRange(sPortRange);
            }
            comboPortName.Text = ini.ReadValue("CMDCOM", "COM");
            comboBaudrate.Text = ini.ReadValue("CMDCOM", "BaudRate");
            txt_DataBits.Text = ini.ReadValue("CMDCOM", "DataBits");
            cob_Parity.Text = ini.ReadValue("CMDCOM", "Parity");
            cob_StopBits.Text = ini.ReadValue("CMDCOM", "StopBits");

            comSndPortName.Text = ini.ReadValue("AudioCOM", "COM");
            comSndBaudrate.Text = ini.ReadValue("AudioCOM", "BaudRate");
            txt_SndDataBits.Text = ini.ReadValue("AudioCOM", "DataBits");
            cob_SndParity.Text = ini.ReadValue("AudioCOM", "Parity");
            cob_SndStopBits.Text = ini.ReadValue("AudioCOM", "StopBits");
        }
    }
}
