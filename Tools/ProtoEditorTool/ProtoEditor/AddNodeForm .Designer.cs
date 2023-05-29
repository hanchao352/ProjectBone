namespace ProtoEditor
{
    partial class AddNodeForm
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
            numMinMsgID = new NumericUpDown();
            numMaxMsgID = new NumericUpDown();
            txtDescription = new TextBox();
            btnOK = new Button();
            btnCancel = new Button();
            lblMinMsgID = new Label();
            lblMaxMsgID = new Label();
            lblDescription = new Label();
            ((System.ComponentModel.ISupportInitialize)numMinMsgID).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numMaxMsgID).BeginInit();
            SuspendLayout();
            // 
            // numMinMsgID
            // 
            numMinMsgID.Location = new Point(172, 22);
            numMinMsgID.Margin = new Padding(6, 6, 6, 6);
            numMinMsgID.Name = "numMinMsgID";
            numMinMsgID.Size = new Size(220, 30);
            numMinMsgID.TabIndex = 0;
            // 
            // numMaxMsgID
            // 
            numMaxMsgID.Location = new Point(172, 70);
            numMaxMsgID.Margin = new Padding(6, 6, 6, 6);
            numMaxMsgID.Name = "numMaxMsgID";
            numMaxMsgID.Size = new Size(220, 30);
            numMaxMsgID.TabIndex = 1;
            // 
            // txtDescription
            // 
            txtDescription.Location = new Point(172, 118);
            txtDescription.Margin = new Padding(6, 6, 6, 6);
            txtDescription.Name = "txtDescription";
            txtDescription.Size = new Size(217, 30);
            txtDescription.TabIndex = 2;
            // 
            // btnOK
            // 
            btnOK.Location = new Point(22, 166);
            btnOK.Margin = new Padding(6, 6, 6, 6);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(138, 42);
            btnOK.TabIndex = 3;
            btnOK.Text = "确定";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(255, 166);
            btnCancel.Margin = new Padding(6, 6, 6, 6);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(138, 42);
            btnCancel.TabIndex = 4;
            btnCancel.Text = "取消";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // lblMinMsgID
            // 
            lblMinMsgID.AutoSize = true;
            lblMinMsgID.Location = new Point(22, 26);
            lblMinMsgID.Margin = new Padding(6, 0, 6, 0);
            lblMinMsgID.Name = "lblMinMsgID";
            lblMinMsgID.Size = new Size(105, 24);
            lblMinMsgID.TabIndex = 5;
            lblMinMsgID.Text = "MinMsgID:";
            // 
            // lblMaxMsgID
            // 
            lblMaxMsgID.AutoSize = true;
            lblMaxMsgID.Location = new Point(22, 74);
            lblMaxMsgID.Margin = new Padding(6, 0, 6, 0);
            lblMaxMsgID.Name = "lblMaxMsgID";
            lblMaxMsgID.Size = new Size(108, 24);
            lblMaxMsgID.TabIndex = 6;
            lblMaxMsgID.Text = "MaxMsgID:";
            // 
            // lblDescription
            // 
            lblDescription.AutoSize = true;
            lblDescription.Location = new Point(22, 124);
            lblDescription.Margin = new Padding(6, 0, 6, 0);
            lblDescription.Name = "lblDescription";
            lblDescription.Size = new Size(113, 24);
            lblDescription.TabIndex = 7;
            lblDescription.Text = "Description:";
            // 
            // AddNodeForm
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(416, 231);
            Controls.Add(lblDescription);
            Controls.Add(lblMaxMsgID);
            Controls.Add(lblMinMsgID);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            Controls.Add(txtDescription);
            Controls.Add(numMaxMsgID);
            Controls.Add(numMinMsgID);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(6, 6, 6, 6);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AddNodeForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "添加节点";
            ((System.ComponentModel.ISupportInitialize)numMinMsgID).EndInit();
            ((System.ComponentModel.ISupportInitialize)numMaxMsgID).EndInit();
           
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.NumericUpDown numMinMsgID;
        private System.Windows.Forms.NumericUpDown numMaxMsgID;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblMinMsgID;
        private System.Windows.Forms.Label lblMaxMsgID;
        private System.Windows.Forms.Label lblDescription;
        #endregion
    }
}