namespace GRPlatForm
{
    partial class ServerForm
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
            this.components = new System.ComponentModel.Container();
            this.timHold = new System.Windows.Forms.Timer(this.components);
            this.timHeart = new System.Windows.Forms.Timer(this.components);
            this.tim_MediaPlay = new System.Windows.Forms.Timer(this.components);
            this.tim_ClearMemory = new System.Windows.Forms.Timer(this.components);
            this.ConnectionStatusLabel = new DevExpress.XtraEditors.LabelControl();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnHeart = new System.Windows.Forms.Button();
            this.txtServerPort = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.txtMsgShow = new System.Windows.Forms.TextBox();
            this.barManagerStockChart = new DevExpress.XtraBars.BarManager(this.components);
            this.barWorckspace = new DevExpress.XtraBars.Bar();
            this.ViewSubitem = new DevExpress.XtraBars.BarSubItem();
            this.barSubItem1 = new DevExpress.XtraBars.BarSubItem();
            this.barCheckItem1 = new DevExpress.XtraBars.BarCheckItem();
            this.barCheckItem3 = new DevExpress.XtraBars.BarCheckItem();
            this.barCheckItem4 = new DevExpress.XtraBars.BarCheckItem();
            this.helpBarItemButton = new DevExpress.XtraBars.BarSubItem();
            this.barButtonItem2 = new DevExpress.XtraBars.BarButtonItem();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.dockManager1 = new DevExpress.XtraBars.Docking.DockManager(this.components);
            this.dockPanel3 = new DevExpress.XtraBars.Docking.DockPanel();
            this.dockPanel3_Container = new DevExpress.XtraBars.Docking.ControlContainer();
            this.dockPanel1 = new DevExpress.XtraBars.Docking.DockPanel();
            this.dockPanel1_Container = new DevExpress.XtraBars.Docking.ControlContainer();
            this.dockPanel2 = new DevExpress.XtraBars.Docking.DockPanel();
            this.dockPanel2_Container = new DevExpress.XtraBars.Docking.ControlContainer();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.barCheckItem2 = new DevExpress.XtraBars.BarCheckItem();
            this.barEditItem1 = new DevExpress.XtraBars.BarEditItem();
            this.repositoryItemTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.repositoryItemTextEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.repositoryItemTextEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.listView1 = new System.Windows.Forms.ListView();
            this.listMessage_Type = new System.Windows.Forms.ListView();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.barManagerStockChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).BeginInit();
            this.dockPanel3.SuspendLayout();
            this.dockPanel3_Container.SuspendLayout();
            this.dockPanel1.SuspendLayout();
            this.dockPanel1_Container.SuspendLayout();
            this.dockPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit3)).BeginInit();
            this.SuspendLayout();
            // 
            // timHold
            // 
            this.timHold.Interval = 1000;
            this.timHold.Tick += new System.EventHandler(this.timHold_Tick);
            // 
            // timHeart
            // 
            this.timHeart.Interval = 5000;
            this.timHeart.Tick += new System.EventHandler(this.timHeart_Tick);
            // 
            // tim_MediaPlay
            // 
            this.tim_MediaPlay.Interval = 1000;
            this.tim_MediaPlay.Tick += new System.EventHandler(this.tim_MediaPlay_Tick);
            // 
            // tim_ClearMemory
            // 
            this.tim_ClearMemory.Interval = 600000;
            this.tim_ClearMemory.Tick += new System.EventHandler(this.tim_ClearMemory_Tick);
            // 
            // ConnectionStatusLabel
            // 
            this.ConnectionStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ConnectionStatusLabel.Location = new System.Drawing.Point(1320, -127);
            this.ConnectionStatusLabel.Name = "ConnectionStatusLabel";
            this.ConnectionStatusLabel.Size = new System.Drawing.Size(48, 14);
            this.ConnectionStatusLabel.TabIndex = 13;
            this.ConnectionStatusLabel.Text = "连接状态";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.panel1);
            this.panel3.Controls.Add(this.panel2);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(200, 24);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1313, 535);
            this.panel3.TabIndex = 16;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button5);
            this.panel1.Controls.Add(this.button4);
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.btnHeart);
            this.panel1.Controls.Add(this.txtServerPort);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.btnStart);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 467);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1313, 68);
            this.panel1.TabIndex = 0;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(27, 10);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(38, 13);
            this.button5.TabIndex = 5;
            this.button5.Text = "button5";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(562, 27);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(115, 36);
            this.button4.TabIndex = 4;
            this.button4.Text = "终端信息上报";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(446, 27);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(115, 36);
            this.button3.TabIndex = 4;
            this.button3.Text = "平台状态信息";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(330, 27);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(115, 36);
            this.button2.TabIndex = 4;
            this.button2.Text = "终端状态上报";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(214, 27);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(115, 36);
            this.button1.TabIndex = 4;
            this.button1.Text = "平台信息上报";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnHeart
            // 
            this.btnHeart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHeart.Location = new System.Drawing.Point(1087, 11);
            this.btnHeart.Name = "btnHeart";
            this.btnHeart.Size = new System.Drawing.Size(112, 42);
            this.btnHeart.TabIndex = 3;
            this.btnHeart.Text = "发送心跳";
            this.btnHeart.UseVisualStyleBackColor = true;
            this.btnHeart.Click += new System.EventHandler(this.btnHeart_Click);
            // 
            // txtServerPort
            // 
            this.txtServerPort.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtServerPort.Location = new System.Drawing.Point(100, 31);
            this.txtServerPort.Name = "txtServerPort";
            this.txtServerPort.Size = new System.Drawing.Size(100, 26);
            this.txtServerPort.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(15, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "服务端口:";
            // 
            // btnStart
            // 
            this.btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStart.Location = new System.Drawing.Point(1199, 11);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(107, 42);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "启动服务";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.txtMsgShow);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1313, 450);
            this.panel2.TabIndex = 1;
            // 
            // txtMsgShow
            // 
            this.txtMsgShow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMsgShow.Location = new System.Drawing.Point(0, 0);
            this.txtMsgShow.Multiline = true;
            this.txtMsgShow.Name = "txtMsgShow";
            this.txtMsgShow.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMsgShow.Size = new System.Drawing.Size(1313, 450);
            this.txtMsgShow.TabIndex = 0;
            // 
            // barManagerStockChart
            // 
            this.barManagerStockChart.AllowCustomization = false;
            this.barManagerStockChart.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.barWorckspace,
            this.bar1});
            this.barManagerStockChart.DockControls.Add(this.barDockControlTop);
            this.barManagerStockChart.DockControls.Add(this.barDockControlBottom);
            this.barManagerStockChart.DockControls.Add(this.barDockControlLeft);
            this.barManagerStockChart.DockControls.Add(this.barDockControlRight);
            this.barManagerStockChart.DockManager = this.dockManager1;
            this.barManagerStockChart.Form = this;
            this.barManagerStockChart.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.barButtonItem1,
            this.helpBarItemButton,
            this.ViewSubitem,
            this.barSubItem1,
            this.barCheckItem1,
            this.barCheckItem2,
            this.barCheckItem3,
            this.barCheckItem4,
            this.barEditItem1,
            this.barButtonItem2});
            this.barManagerStockChart.MainMenu = this.barWorckspace;
            this.barManagerStockChart.MaxItemId = 18;
            this.barManagerStockChart.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemTextEdit1,
            this.repositoryItemTextEdit2,
            this.repositoryItemTextEdit3});
            this.barManagerStockChart.StatusBar = this.bar1;
            // 
            // barWorckspace
            // 
            this.barWorckspace.BarName = "Main menu";
            this.barWorckspace.DockCol = 0;
            this.barWorckspace.DockRow = 0;
            this.barWorckspace.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.barWorckspace.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.ViewSubitem, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.helpBarItemButton)});
            this.barWorckspace.OptionsBar.AllowQuickCustomization = false;
            this.barWorckspace.OptionsBar.DrawDragBorder = false;
            this.barWorckspace.OptionsBar.UseWholeRow = true;
            this.barWorckspace.Text = "Main menu";
            // 
            // ViewSubitem
            // 
            this.ViewSubitem.Caption = "视图选项";
            this.ViewSubitem.Id = 4;
            this.ViewSubitem.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barSubItem1)});
            this.ViewSubitem.Name = "ViewSubitem";
            // 
            // barSubItem1
            // 
            this.barSubItem1.Caption = "Panels";
            this.barSubItem1.Id = 6;
            this.barSubItem1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barCheckItem1),
            new DevExpress.XtraBars.LinkPersistInfo(this.barCheckItem3),
            new DevExpress.XtraBars.LinkPersistInfo(this.barCheckItem4)});
            this.barSubItem1.Name = "barSubItem1";
            this.barSubItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barCheckItem1_CheckedChanged);
            // 
            // barCheckItem1
            // 
            this.barCheckItem1.Caption = "WatchList";
            this.barCheckItem1.Checked = true;
            this.barCheckItem1.Id = 7;
            this.barCheckItem1.Name = "barCheckItem1";
            this.barCheckItem1.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.barCheckItem1_CheckedChanged);
            this.barCheckItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barCheckItem1_CheckedChanged);
            // 
            // barCheckItem3
            // 
            this.barCheckItem3.Caption = "Transaction";
            this.barCheckItem3.Checked = true;
            this.barCheckItem3.Id = 9;
            this.barCheckItem3.Name = "barCheckItem3";
            this.barCheckItem3.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.barCheckItem1_CheckedChanged);
            this.barCheckItem3.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barCheckItem1_CheckedChanged);
            // 
            // barCheckItem4
            // 
            this.barCheckItem4.Caption = "Top Volumes";
            this.barCheckItem4.Checked = true;
            this.barCheckItem4.Id = 10;
            this.barCheckItem4.Name = "barCheckItem4";
            this.barCheckItem4.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.barCheckItem1_CheckedChanged);
            this.barCheckItem4.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barCheckItem1_CheckedChanged);
            // 
            // helpBarItemButton
            // 
            this.helpBarItemButton.Caption = "帮助";
            this.helpBarItemButton.Id = 3;
            this.helpBarItemButton.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem2)});
            this.helpBarItemButton.Name = "helpBarItemButton";
            // 
            // barButtonItem2
            // 
            this.barButtonItem2.Caption = "About";
            this.barButtonItem2.Id = 17;
            this.barButtonItem2.Name = "barButtonItem2";
            // 
            // bar1
            // 
            this.bar1.BarName = "Custom 3";
            this.bar1.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Bottom;
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom;
            this.bar1.OptionsBar.AllowQuickCustomization = false;
            this.bar1.OptionsBar.DrawDragBorder = false;
            this.bar1.OptionsBar.UseWholeRow = true;
            this.bar1.Text = "Custom 3";
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(1513, 24);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 559);
            this.barDockControlBottom.Size = new System.Drawing.Size(1513, 23);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 24);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 535);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1513, 24);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 535);
            // 
            // dockManager1
            // 
            this.dockManager1.Form = this;
            this.dockManager1.HiddenPanels.AddRange(new DevExpress.XtraBars.Docking.DockPanel[] {
            this.dockPanel2,
            this.dockPanel1});
            this.dockManager1.MenuManager = this.barManagerStockChart;
            this.dockManager1.RootPanels.AddRange(new DevExpress.XtraBars.Docking.DockPanel[] {
            this.dockPanel3});
            this.dockManager1.TopZIndexControls.AddRange(new string[] {
            "DevExpress.XtraBars.BarDockControl",
            "DevExpress.XtraBars.StandaloneBarDockControl",
            "System.Windows.Forms.StatusBar",
            "System.Windows.Forms.MenuStrip",
            "System.Windows.Forms.StatusStrip",
            "DevExpress.XtraBars.Ribbon.RibbonStatusBar",
            "DevExpress.XtraBars.Ribbon.RibbonControl"});
            // 
            // dockPanel3
            // 
            this.dockPanel3.Controls.Add(this.dockPanel3_Container);
            this.dockPanel3.Dock = DevExpress.XtraBars.Docking.DockingStyle.Left;
            this.dockPanel3.ID = new System.Guid("8c962095-11fd-4d9a-b599-b94d7ed2112e");
            this.dockPanel3.Location = new System.Drawing.Point(0, 24);
            this.dockPanel3.Name = "dockPanel3";
            this.dockPanel3.OriginalSize = new System.Drawing.Size(200, 200);
            this.dockPanel3.Size = new System.Drawing.Size(200, 535);
            this.dockPanel3.Text = "消息类型";
            // 
            // dockPanel3_Container
            // 
            this.dockPanel3_Container.Controls.Add(this.listMessage_Type);
            this.dockPanel3_Container.Location = new System.Drawing.Point(4, 23);
            this.dockPanel3_Container.Name = "dockPanel3_Container";
            this.dockPanel3_Container.Size = new System.Drawing.Size(192, 508);
            this.dockPanel3_Container.TabIndex = 0;
            // 
            // dockPanel1
            // 
            this.dockPanel1.Controls.Add(this.dockPanel1_Container);
            this.dockPanel1.Dock = DevExpress.XtraBars.Docking.DockingStyle.Top;
            this.dockPanel1.FloatVertical = true;
            this.dockPanel1.ID = new System.Guid("c1b463ea-0058-456d-9905-709af1365f8a");
            this.dockPanel1.Location = new System.Drawing.Point(0, 268);
            this.dockPanel1.Name = "dockPanel1";
            this.dockPanel1.OriginalSize = new System.Drawing.Size(200, 267);
            this.dockPanel1.SavedDock = DevExpress.XtraBars.Docking.DockingStyle.Top;
            this.dockPanel1.SavedIndex = 1;
            this.dockPanel1.SavedParent = this.dockPanel3;
            this.dockPanel1.Size = new System.Drawing.Size(200, 267);
            this.dockPanel1.Text = "dockPanel1";
            this.dockPanel1.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
            // 
            // dockPanel1_Container
            // 
            this.dockPanel1_Container.Controls.Add(this.listView1);
            this.dockPanel1_Container.Location = new System.Drawing.Point(4, 23);
            this.dockPanel1_Container.Name = "dockPanel1_Container";
            this.dockPanel1_Container.Size = new System.Drawing.Size(192, 240);
            this.dockPanel1_Container.TabIndex = 0;
            // 
            // dockPanel2
            // 
            this.dockPanel2.Controls.Add(this.dockPanel2_Container);
            this.dockPanel2.Dock = DevExpress.XtraBars.Docking.DockingStyle.Fill;
            this.dockPanel2.ID = new System.Guid("f2c02802-9276-4587-a434-7cbbfccbad95");
            this.dockPanel2.Location = new System.Drawing.Point(0, 357);
            this.dockPanel2.Name = "dockPanel2";
            this.dockPanel2.OriginalSize = new System.Drawing.Size(192, 212);
            this.dockPanel2.SavedDock = DevExpress.XtraBars.Docking.DockingStyle.Fill;
            this.dockPanel2.SavedIndex = 2;
            this.dockPanel2.SavedParent = this.dockPanel3;
            this.dockPanel2.Size = new System.Drawing.Size(200, 178);
            this.dockPanel2.Text = "dockPanel2";
            this.dockPanel2.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
            // 
            // dockPanel2_Container
            // 
            this.dockPanel2_Container.Location = new System.Drawing.Point(4, 23);
            this.dockPanel2_Container.Name = "dockPanel2_Container";
            this.dockPanel2_Container.Size = new System.Drawing.Size(192, 151);
            this.dockPanel2_Container.TabIndex = 0;
            // 
            // barButtonItem1
            // 
            this.barButtonItem1.Caption = "test";
            this.barButtonItem1.Id = 0;
            this.barButtonItem1.Name = "barButtonItem1";
            // 
            // barCheckItem2
            // 
            this.barCheckItem2.Caption = "StockChart";
            this.barCheckItem2.Checked = true;
            this.barCheckItem2.Id = 8;
            this.barCheckItem2.Name = "barCheckItem2";
            // 
            // barEditItem1
            // 
            this.barEditItem1.Caption = "barEdittem1";
            this.barEditItem1.Edit = this.repositoryItemTextEdit1;
            this.barEditItem1.Id = 11;
            this.barEditItem1.Name = "barEditItem1";
            // 
            // repositoryItemTextEdit1
            // 
            this.repositoryItemTextEdit1.AutoHeight = false;
            this.repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
            // 
            // repositoryItemTextEdit2
            // 
            this.repositoryItemTextEdit2.AutoHeight = false;
            this.repositoryItemTextEdit2.Name = "repositoryItemTextEdit2";
            // 
            // repositoryItemTextEdit3
            // 
            this.repositoryItemTextEdit3.AutoHeight = false;
            this.repositoryItemTextEdit3.Name = "repositoryItemTextEdit3";
            // 
            // listView1
            // 
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(192, 240);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // listMessage_Type
            // 
            this.listMessage_Type.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listMessage_Type.Location = new System.Drawing.Point(0, 0);
            this.listMessage_Type.Name = "listMessage_Type";
            this.listMessage_Type.Size = new System.Drawing.Size(192, 508);
            this.listMessage_Type.TabIndex = 0;
            this.listMessage_Type.UseCompatibleStateImageBehavior = false;
            // 
            // labelControl1
            // 
            this.labelControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelControl1.Location = new System.Drawing.Point(1465, 4);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(48, 14);
            this.labelControl1.TabIndex = 22;
            this.labelControl1.Text = "连接状态";
            // 
            // ServerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1513, 582);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.ConnectionStatusLabel);
            this.Controls.Add(this.dockPanel3);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "ServerForm";
            this.Text = "消息服务";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ServerForm_FormClosing);
            this.Load += new System.EventHandler(this.ServerForm_Load);
            this.panel3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.barManagerStockChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).EndInit();
            this.dockPanel3.ResumeLayout(false);
            this.dockPanel3_Container.ResumeLayout(false);
            this.dockPanel1.ResumeLayout(false);
            this.dockPanel1_Container.ResumeLayout(false);
            this.dockPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timHold;
        private System.Windows.Forms.Timer timHeart;
        private System.Windows.Forms.Timer tim_MediaPlay;
        private System.Windows.Forms.Timer tim_ClearMemory;
        private DevExpress.XtraEditors.LabelControl ConnectionStatusLabel;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnHeart;
        private System.Windows.Forms.TextBox txtServerPort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox txtMsgShow;
        private DevExpress.XtraBars.BarManager barManagerStockChart;
        private DevExpress.XtraBars.Bar barWorckspace;
        private DevExpress.XtraBars.BarSubItem ViewSubitem;
        private DevExpress.XtraBars.BarSubItem barSubItem1;
        private DevExpress.XtraBars.BarCheckItem barCheckItem1;
        private DevExpress.XtraBars.BarCheckItem barCheckItem3;
        private DevExpress.XtraBars.BarCheckItem barCheckItem4;
        private DevExpress.XtraBars.BarSubItem helpBarItemButton;
        private DevExpress.XtraBars.BarButtonItem barButtonItem2;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.Docking.DockManager dockManager1;
        private DevExpress.XtraBars.Docking.DockPanel dockPanel3;
        private DevExpress.XtraBars.Docking.ControlContainer dockPanel3_Container;
        private DevExpress.XtraBars.Docking.DockPanel dockPanel2;
        private DevExpress.XtraBars.Docking.ControlContainer dockPanel2_Container;
        private DevExpress.XtraBars.Docking.DockPanel dockPanel1;
        private DevExpress.XtraBars.Docking.ControlContainer dockPanel1_Container;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraBars.BarCheckItem barCheckItem2;
        private DevExpress.XtraBars.BarEditItem barEditItem1;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit1;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit2;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit3;
        private System.Windows.Forms.ListView listMessage_Type;
        private System.Windows.Forms.ListView listView1;
        private DevExpress.XtraEditors.LabelControl labelControl1;
    }
}