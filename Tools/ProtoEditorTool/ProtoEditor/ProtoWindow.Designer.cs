namespace ProtoEditor
{
    partial class ProtoWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            SerchText = new TextBox();
            TitleLabel = new Label();
            groupBox1 = new GroupBox();
            GenMsgIDForServer = new Button();
            GenMsgIDForClient = new Button();
            ExportProtoToServer = new Button();
            ExportProtoToClient = new Button();
            SaveButton = new Button();
            treeView1 = new CustomTreeView();
            autoCompleteToolTip = new ToolTip(components);
            ProtoInfoText = new ScintillaNET.Scintilla();
            RefreshBtn = new Button();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // SerchText
            // 
            SerchText.Anchor = AnchorStyles.None;
            SerchText.Font = new Font("Microsoft YaHei UI", 15F, FontStyle.Regular, GraphicsUnit.Point);
            SerchText.Location = new Point(111, 22);
            SerchText.Name = "SerchText";
            SerchText.Size = new Size(354, 46);
            SerchText.TabIndex = 1;
            SerchText.TextChanged += SerchText_TextChanged;
            // 
            // TitleLabel
            // 
            TitleLabel.Anchor = AnchorStyles.None;
            TitleLabel.Font = new Font("Microsoft YaHei UI", 15F, FontStyle.Regular, GraphicsUnit.Point);
            TitleLabel.Location = new Point(12, 9);
            TitleLabel.Name = "TitleLabel";
            TitleLabel.Size = new Size(100, 70);
            TitleLabel.TabIndex = 0;
            TitleLabel.Text = "查找";
            TitleLabel.TextAlign = ContentAlignment.MiddleCenter;
            TitleLabel.Click += TitleLabel_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(RefreshBtn);
            groupBox1.Controls.Add(GenMsgIDForServer);
            groupBox1.Controls.Add(GenMsgIDForClient);
            groupBox1.Controls.Add(ExportProtoToServer);
            groupBox1.Controls.Add(ExportProtoToClient);
            groupBox1.Controls.Add(SaveButton);
            groupBox1.Location = new Point(1322, 115);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(422, 863);
            groupBox1.TabIndex = 3;
            groupBox1.TabStop = false;
            // 
            // GenMsgIDForServer
            // 
            GenMsgIDForServer.Location = new Point(233, 328);
            GenMsgIDForServer.Name = "GenMsgIDForServer";
            GenMsgIDForServer.Size = new Size(149, 69);
            GenMsgIDForServer.TabIndex = 5;
            GenMsgIDForServer.Text = "GenMsgIDForServer";
            GenMsgIDForServer.UseVisualStyleBackColor = true;
            // 
            // GenMsgIDForClient
            // 
            GenMsgIDForClient.Location = new Point(27, 328);
            GenMsgIDForClient.Name = "GenMsgIDForClient";
            GenMsgIDForClient.Size = new Size(149, 69);
            GenMsgIDForClient.TabIndex = 4;
            GenMsgIDForClient.Text = "GenMsgIDForClient";
            GenMsgIDForClient.UseVisualStyleBackColor = true;
            // 
            // ExportProtoToServer
            // 
            ExportProtoToServer.Location = new Point(233, 171);
            ExportProtoToServer.Name = "ExportProtoToServer";
            ExportProtoToServer.Size = new Size(149, 69);
            ExportProtoToServer.TabIndex = 3;
            ExportProtoToServer.Text = "导出协议到服务器";
            ExportProtoToServer.UseVisualStyleBackColor = true;
            // 
            // ExportProtoToClient
            // 
            ExportProtoToClient.Location = new Point(27, 171);
            ExportProtoToClient.Name = "ExportProtoToClient";
            ExportProtoToClient.Size = new Size(149, 69);
            ExportProtoToClient.TabIndex = 2;
            ExportProtoToClient.Text = "导出协议到客户端";
            ExportProtoToClient.UseVisualStyleBackColor = true;
            // 
            // SaveButton
            // 
            SaveButton.Location = new Point(27, 29);
            SaveButton.Name = "SaveButton";
            SaveButton.Size = new Size(149, 69);
            SaveButton.TabIndex = 0;
            SaveButton.Text = "保存";
            SaveButton.UseVisualStyleBackColor = true;
            // 
            // treeView1
            // 
            treeView1.HideSelection = false;
            treeView1.Location = new Point(12, 85);
            treeView1.Name = "treeView1";
            treeView1.Size = new Size(453, 893);
            treeView1.TabIndex = 5;
            // 
            // ProtoInfoText
            // 
            ProtoInfoText.AutoCMaxHeight = 9;
            ProtoInfoText.BiDirectionality = ScintillaNET.BiDirectionalDisplayType.Disabled;
            ProtoInfoText.CaretLineBackColor = Color.White;
            ProtoInfoText.CaretLineVisible = true;
            ProtoInfoText.Lexer = ScintillaNET.Lexer.Cpp;
            ProtoInfoText.LexerName = "cpp";
            ProtoInfoText.Location = new Point(471, 85);
            ProtoInfoText.Name = "ProtoInfoText";
            ProtoInfoText.ScrollWidth = 74;
            ProtoInfoText.Size = new Size(845, 893);
            ProtoInfoText.TabIndents = true;
            ProtoInfoText.TabIndex = 6;
            ProtoInfoText.UseRightToLeftReadingLayout = false;
            ProtoInfoText.WrapMode = ScintillaNET.WrapMode.None;
            ProtoInfoText.Click += ProtoInfoText_Click;
            // 
            // RefreshBtn
            // 
            RefreshBtn.Location = new Point(233, 29);
            RefreshBtn.Name = "RefreshBtn";
            RefreshBtn.Size = new Size(149, 69);
            RefreshBtn.TabIndex = 6;
            RefreshBtn.Text = "刷新";
            RefreshBtn.UseVisualStyleBackColor = true;
            RefreshBtn.Click += RefreshBtn_Click;
            // 
            // ProtoWindow
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1756, 1030);
            Controls.Add(ProtoInfoText);
            Controls.Add(SerchText);
            Controls.Add(treeView1);
            Controls.Add(TitleLabel);
            Controls.Add(groupBox1);
            Name = "ProtoWindow";
            Text = "协议编辑器";
            Load += ProtoWindow_Load;
            groupBox1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TextBox SerchText;
        private Label TitleLabel;
        private GroupBox groupBox1;
        private Button GenMsgIDForServer;
        private Button GenMsgIDForClient;
        private Button ExportProtoToServer;
        private Button ExportProtoToClient;
        private Button SaveButton;
        private CustomTreeView treeView1;
        private ToolTip autoCompleteToolTip;
        private ScintillaNET.Scintilla ProtoInfoText;
        private Button RefreshBtn;
    }
}