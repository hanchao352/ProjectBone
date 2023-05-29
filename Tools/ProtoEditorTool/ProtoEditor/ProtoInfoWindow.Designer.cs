namespace ProtoEditor
{
    partial class ProtoInfoWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        public TextBox textBoxFileName;
        private Button buttonConfirm;
        private Button buttonCancel;
        private Panel panel;
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
            textBoxFileName = new TextBox();
            buttonConfirm = new Button();
            buttonCancel = new Button();
            panel = new Panel();
            panel.SuspendLayout();
            SuspendLayout();
            // 
            // textBoxFileName
            // 
            textBoxFileName.Font = new Font("Microsoft YaHei UI", 15F, FontStyle.Regular, GraphicsUnit.Point);
            textBoxFileName.ForeColor = Color.Gray;
            textBoxFileName.Location = new Point(101, 98);
            textBoxFileName.Name = "textBoxFileName";
            textBoxFileName.Size = new Size(328, 46);
            textBoxFileName.TabIndex = 0;
            textBoxFileName.GotFocus += TextBoxInput_Enter;
            textBoxFileName.LostFocus += TextBoxInput_Leave;
            // 
            // buttonConfirm
            // 
            buttonConfirm.Location = new Point(101, 185);
            buttonConfirm.Name = "buttonConfirm";
            buttonConfirm.Size = new Size(100, 49);
            buttonConfirm.TabIndex = 1;
            buttonConfirm.Text = "确定";
            buttonConfirm.Click += ButtonConfirm_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new Point(329, 185);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(100, 49);
            buttonCancel.TabIndex = 2;
            buttonCancel.Text = "取消";
            buttonCancel.Click += ButtonCancel_Click;
            // 
            // panel
            // 
            panel.Controls.Add(textBoxFileName);
            panel.Controls.Add(buttonConfirm);
            panel.Controls.Add(buttonCancel);
            panel.Dock = DockStyle.Fill;
            panel.Location = new Point(0, 0);
            panel.Name = "panel";
            panel.Size = new Size(525, 330);
            panel.TabIndex = 0;
            // 
            // ProtoInfoWindow
            // 
            ClientSize = new Size(525, 330);
            Controls.Add(panel);
            Name = "ProtoInfoWindow";
            StartPosition = FormStartPosition.Manual;
            panel.ResumeLayout(false);
            panel.PerformLayout();
            ResumeLayout(false);
        }




        #endregion
    }
}