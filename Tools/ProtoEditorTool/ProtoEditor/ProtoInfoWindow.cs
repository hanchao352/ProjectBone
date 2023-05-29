using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace ProtoEditor
{
    public partial class ProtoInfoWindow : Form
    {
        public OpenEnum OpenEnum = OpenEnum.Open;
        public ProtoFileInfo ProtoFileInfo;
        public ProtoInfoWindow()
        {
            InitializeComponent();
           
            this.textBoxFileName.Text = StringDefine.ProtoWindow_InputDefaultText;
           
        }
        public ProtoInfoWindow(ProtoFileInfo protoFileInfo)
        {
            InitializeComponent();
            this.ProtoFileInfo = protoFileInfo;
        }

        protected override void OnShown(EventArgs e)
        {
            if (OpenEnum == OpenEnum.Open)
            {
                this.Text = "请输入Proto文件名";
            }
            else if (OpenEnum == OpenEnum.Edit)
            {
                this.Text = "请输入Proto文件名";
            }
        }
        private void ButtonCancel_Click(object sender, EventArgs e)
        {

            this.Close();
        }

        private void ButtonConfirm_Click(object sender, EventArgs e)
        {
            string text = this.textBoxFileName.Text;
            if (string.IsNullOrWhiteSpace(text)|| string.IsNullOrEmpty(text))
            {
                MessageBox.Show(StringDefine.ProtoWindow_ErrorTipsText,StringDefine.ProtoWindow_OKText);
                return;
            }



            if (OpenEnum == OpenEnum.Open)
            {
                this.Close();
                string[] str = text.Split('.');
                string fileName = str[0] + ".proto";
                string newFilePath = Path.Combine(ProtoManager.Instance.GetProtoDirPath(), fileName);
                if (!File.Exists(newFilePath))
                {
                    File.Create(newFilePath).Dispose();



                    Prototemplate.Instance.MessageName = str[0];
                    string protostr = Prototemplate.Instance.GetTemplate();
                    File.WriteAllText(newFilePath, Prototemplate.Instance.GetTemplate());


                    ProtoManager.Instance.UpdateProtoInfos();
                    EventManager.Instance.TriggerEvent(EventDefine.MainWindow_UpdateTreeView);
                    EventManager.Instance.TriggerEvent(EventDefine.MainWindow_SelectTreeView, newFilePath);

                }
            }
            else if (OpenEnum == OpenEnum.Edit)
            {
                this.Close();
                string[] str = text.Split('.');
                string newname = str[0] + ".proto";        
                ProtoManager.Instance.ReNameProto(this.ProtoFileInfo.ProtoPath, newname);
                ProtoManager.Instance.UpdateProtoInfos();
                EventManager.Instance.TriggerEvent(EventDefine.MainWindow_UpdateTreeView);
               
            }
            
          
        }

    

        private void TextBoxInput_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxFileName.Text))
            {
                textBoxFileName.Text = StringDefine.ProtoWindow_InputDefaultText;
                textBoxFileName.ForeColor = Color.Gray;
            }
        }

        private void TextBoxInput_Enter(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxFileName.Text))
            {
                textBoxFileName.Text = "";
                textBoxFileName.ForeColor = Color.Gray;
            }

        }

    }
}
