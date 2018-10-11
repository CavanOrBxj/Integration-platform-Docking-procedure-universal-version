namespace GRPlatForm
{
    partial class ComSetForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cob_Parity = new System.Windows.Forms.ComboBox();
            this.lbl_Tips = new System.Windows.Forms.Label();
            this.buttonOpenClose = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txt_DataBits = new System.Windows.Forms.TextBox();
            this.cob_StopBits = new System.Windows.Forms.ComboBox();
            this.comboBaudrate = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.comboPortName = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cob_SndParity = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.btnSndOpenClose = new System.Windows.Forms.Button();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.txt_SndDataBits = new System.Windows.Forms.TextBox();
            this.cob_SndStopBits = new System.Windows.Forms.ComboBox();
            this.comSndBaudrate = new System.Windows.Forms.ComboBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.comSndPortName = new System.Windows.Forms.ComboBox();
            this.btnSet = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cob_Parity);
            this.groupBox1.Controls.Add(this.lbl_Tips);
            this.groupBox1.Controls.Add(this.buttonOpenClose);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.txt_DataBits);
            this.groupBox1.Controls.Add(this.cob_StopBits);
            this.groupBox1.Controls.Add(this.comboBaudrate);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.comboPortName);
            this.groupBox1.Location = new System.Drawing.Point(29, 32);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(397, 98);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "通用指令串口设置";
            // 
            // cob_Parity
            // 
            this.cob_Parity.FormattingEnabled = true;
            this.cob_Parity.Items.AddRange(new object[] {
            "Even",
            "Mark",
            "None",
            "Odd",
            "Space"});
            this.cob_Parity.Location = new System.Drawing.Point(65, 56);
            this.cob_Parity.Name = "cob_Parity";
            this.cob_Parity.Size = new System.Drawing.Size(67, 20);
            this.cob_Parity.TabIndex = 50;
            this.cob_Parity.Text = "Even";
            // 
            // lbl_Tips
            // 
            this.lbl_Tips.AutoSize = true;
            this.lbl_Tips.Location = new System.Drawing.Point(155, 95);
            this.lbl_Tips.Name = "lbl_Tips";
            this.lbl_Tips.Size = new System.Drawing.Size(0, 12);
            this.lbl_Tips.TabIndex = 59;
            // 
            // buttonOpenClose
            // 
            this.buttonOpenClose.Location = new System.Drawing.Point(300, 53);
            this.buttonOpenClose.Name = "buttonOpenClose";
            this.buttonOpenClose.Size = new System.Drawing.Size(85, 27);
            this.buttonOpenClose.TabIndex = 56;
            this.buttonOpenClose.Text = "Open";
            this.buttonOpenClose.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(143, 60);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 53;
            this.label7.Text = "停止位：";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(11, 60);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 12);
            this.label8.TabIndex = 54;
            this.label8.Text = "校验位：";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(281, 28);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 12);
            this.label9.TabIndex = 55;
            this.label9.Text = "数据位：";
            // 
            // txt_DataBits
            // 
            this.txt_DataBits.Location = new System.Drawing.Point(345, 24);
            this.txt_DataBits.Name = "txt_DataBits";
            this.txt_DataBits.Size = new System.Drawing.Size(40, 21);
            this.txt_DataBits.TabIndex = 52;
            this.txt_DataBits.Text = "8";
            // 
            // cob_StopBits
            // 
            this.cob_StopBits.FormattingEnabled = true;
            this.cob_StopBits.Items.AddRange(new object[] {
            "None",
            "One",
            "OnePointFive",
            "Two"});
            this.cob_StopBits.Location = new System.Drawing.Point(201, 56);
            this.cob_StopBits.Name = "cob_StopBits";
            this.cob_StopBits.Size = new System.Drawing.Size(70, 20);
            this.cob_StopBits.TabIndex = 51;
            this.cob_StopBits.Text = "One";
            // 
            // comboBaudrate
            // 
            this.comboBaudrate.BackColor = System.Drawing.Color.White;
            this.comboBaudrate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBaudrate.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comboBaudrate.FormattingEnabled = true;
            this.comboBaudrate.Items.AddRange(new object[] {
            "1200",
            "2400",
            "4800",
            "9600",
            "19200",
            "38400",
            "57600",
            "115200"});
            this.comboBaudrate.Location = new System.Drawing.Point(202, 23);
            this.comboBaudrate.Name = "comboBaudrate";
            this.comboBaudrate.Size = new System.Drawing.Size(69, 20);
            this.comboBaudrate.TabIndex = 49;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(143, 27);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(53, 12);
            this.label10.TabIndex = 47;
            this.label10.Text = "波特率：";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(11, 27);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(53, 12);
            this.label11.TabIndex = 46;
            this.label11.Text = "串口号：";
            // 
            // comboPortName
            // 
            this.comboPortName.BackColor = System.Drawing.Color.White;
            this.comboPortName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboPortName.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comboPortName.FormattingEnabled = true;
            this.comboPortName.Location = new System.Drawing.Point(65, 23);
            this.comboPortName.Name = "comboPortName";
            this.comboPortName.Size = new System.Drawing.Size(67, 20);
            this.comboPortName.TabIndex = 48;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cob_SndParity);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.btnSndOpenClose);
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Controls.Add(this.txt_SndDataBits);
            this.groupBox2.Controls.Add(this.cob_SndStopBits);
            this.groupBox2.Controls.Add(this.comSndBaudrate);
            this.groupBox2.Controls.Add(this.label19);
            this.groupBox2.Controls.Add(this.label20);
            this.groupBox2.Controls.Add(this.comSndPortName);
            this.groupBox2.Location = new System.Drawing.Point(29, 167);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(397, 98);
            this.groupBox2.TabIndex = 20;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "短信通用串口设置";
            // 
            // cob_SndParity
            // 
            this.cob_SndParity.FormattingEnabled = true;
            this.cob_SndParity.Items.AddRange(new object[] {
            "Even",
            "Mark",
            "None",
            "Odd",
            "Space"});
            this.cob_SndParity.Location = new System.Drawing.Point(65, 56);
            this.cob_SndParity.Name = "cob_SndParity";
            this.cob_SndParity.Size = new System.Drawing.Size(67, 20);
            this.cob_SndParity.TabIndex = 50;
            this.cob_SndParity.Text = "Even";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(155, 95);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(0, 12);
            this.label15.TabIndex = 59;
            // 
            // btnSndOpenClose
            // 
            this.btnSndOpenClose.Location = new System.Drawing.Point(300, 53);
            this.btnSndOpenClose.Name = "btnSndOpenClose";
            this.btnSndOpenClose.Size = new System.Drawing.Size(85, 27);
            this.btnSndOpenClose.TabIndex = 56;
            this.btnSndOpenClose.Text = "Open";
            this.btnSndOpenClose.UseVisualStyleBackColor = true;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(143, 60);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(53, 12);
            this.label16.TabIndex = 53;
            this.label16.Text = "停止位：";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(11, 60);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(53, 12);
            this.label17.TabIndex = 54;
            this.label17.Text = "校验位：";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(281, 28);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(53, 12);
            this.label18.TabIndex = 55;
            this.label18.Text = "数据位：";
            // 
            // txt_SndDataBits
            // 
            this.txt_SndDataBits.Location = new System.Drawing.Point(345, 24);
            this.txt_SndDataBits.Name = "txt_SndDataBits";
            this.txt_SndDataBits.Size = new System.Drawing.Size(40, 21);
            this.txt_SndDataBits.TabIndex = 52;
            this.txt_SndDataBits.Text = "8";
            // 
            // cob_SndStopBits
            // 
            this.cob_SndStopBits.FormattingEnabled = true;
            this.cob_SndStopBits.Items.AddRange(new object[] {
            "None",
            "One",
            "OnePointFive",
            "Two"});
            this.cob_SndStopBits.Location = new System.Drawing.Point(201, 56);
            this.cob_SndStopBits.Name = "cob_SndStopBits";
            this.cob_SndStopBits.Size = new System.Drawing.Size(70, 20);
            this.cob_SndStopBits.TabIndex = 51;
            this.cob_SndStopBits.Text = "One";
            // 
            // comSndBaudrate
            // 
            this.comSndBaudrate.BackColor = System.Drawing.Color.White;
            this.comSndBaudrate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comSndBaudrate.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comSndBaudrate.FormattingEnabled = true;
            this.comSndBaudrate.Items.AddRange(new object[] {
            "1200",
            "2400",
            "4800",
            "9600",
            "19200",
            "38400",
            "57600",
            "115200"});
            this.comSndBaudrate.Location = new System.Drawing.Point(202, 23);
            this.comSndBaudrate.Name = "comSndBaudrate";
            this.comSndBaudrate.Size = new System.Drawing.Size(69, 20);
            this.comSndBaudrate.TabIndex = 49;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(143, 27);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(53, 12);
            this.label19.TabIndex = 47;
            this.label19.Text = "波特率：";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(11, 27);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(53, 12);
            this.label20.TabIndex = 46;
            this.label20.Text = "串口号：";
            // 
            // comSndPortName
            // 
            this.comSndPortName.BackColor = System.Drawing.Color.White;
            this.comSndPortName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comSndPortName.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comSndPortName.FormattingEnabled = true;
            this.comSndPortName.Location = new System.Drawing.Point(65, 23);
            this.comSndPortName.Name = "comSndPortName";
            this.comSndPortName.Size = new System.Drawing.Size(67, 20);
            this.comSndPortName.TabIndex = 48;
            // 
            // btnSet
            // 
            this.btnSet.Location = new System.Drawing.Point(335, 285);
            this.btnSet.Name = "btnSet";
            this.btnSet.Size = new System.Drawing.Size(91, 29);
            this.btnSet.TabIndex = 21;
            this.btnSet.Text = "设置";
            this.btnSet.UseVisualStyleBackColor = true;
            this.btnSet.Click += new System.EventHandler(this.btnSet_Click);
            // 
            // ComSetForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 332);
            this.Controls.Add(this.btnSet);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "ComSetForm";
            this.Text = "串口设置";
            this.Load += new System.EventHandler(this.ComSetForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cob_Parity;
        private System.Windows.Forms.Label lbl_Tips;
        private System.Windows.Forms.Button buttonOpenClose;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txt_DataBits;
        private System.Windows.Forms.ComboBox cob_StopBits;
        private System.Windows.Forms.ComboBox comboBaudrate;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox comboPortName;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cob_SndParity;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Button btnSndOpenClose;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox txt_SndDataBits;
        private System.Windows.Forms.ComboBox cob_SndStopBits;
        private System.Windows.Forms.ComboBox comSndBaudrate;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.ComboBox comSndPortName;
        private System.Windows.Forms.Button btnSet;
    }
}