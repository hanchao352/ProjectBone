using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProtoEditor
{
    public enum OpenEnum
    {
        Open,
        Edit,
    }
    public partial class AddNodeForm : Form
    {
        public OpenEnum OpenType = OpenEnum.Open;
        private RootNodeInfo RootNodeInfo = null;
        public List<TreeNode> ExistingNodes { get; set; }
        private int minMsgID;
        public int MinMsgID
        {
            get { return minMsgID; }
            set 
            { 
                minMsgID = value;
                numMinMsgID.Value = minMsgID;
            }
        }
        private int maxMsgID;
        public int MaxMsgID
        {
            get { return maxMsgID; }
            set 
            { 
                maxMsgID = value;
                numMaxMsgID.Value = maxMsgID;
            }
        }
        private string des;
        public string Des
        {
            get { return des; }
            set 
            {
                des = value;
                txtDescription.Text = des;
            }
        }
        public AddNodeForm()
        {
            InitializeComponent();
            numMinMsgID.Maximum = 1 << 30;
            numMaxMsgID.Maximum = 1 << 30;
            this.FormClosed += OnWindowClosed;
            OpenType = OpenEnum.Open;
        }
        public AddNodeForm(OpenEnum openEnum,RootNodeInfo rootNodeInfo)
        {
            InitializeComponent();
            numMinMsgID.Maximum = 1 << 30;
            numMaxMsgID.Maximum = 1 << 30;
            this.FormClosed += OnWindowClosed;
            OpenType = OpenEnum.Edit;
            RootNodeInfo = rootNodeInfo;
        }
        private void OnWindowClosed(object? sender, FormClosedEventArgs e)
        {
            InitializeComponent();
            numMinMsgID.Maximum = 1 << 30;
            numMaxMsgID.Maximum = 1 << 30;
            this.FormClosed += OnWindowClosed;
            OpenType = OpenEnum.Edit;
        }

        //注册事件


        private void OnOKButtonClick(object[] args)
        {
            throw new NotImplementedException();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (numMinMsgID.Value < numMaxMsgID.Value)
            {
                if (OpenType == OpenEnum.Open)
                {
                    MinMsgID = (int)numMinMsgID.Value;
                    MaxMsgID = (int)numMaxMsgID.Value;
                    Des = txtDescription.Text;
                    DialogResult = DialogResult.OK;
                    RootNodeInfo rootNodeInfo = new RootNodeInfo();
                    rootNodeInfo.Des = Des;
                    rootNodeInfo.MinMsgID = MinMsgID;
                    rootNodeInfo.MaxMsgID = MaxMsgID;
                    rootNodeInfo.RootID = Guid.NewGuid();
                    TreeDateManager.Instance.AddRootNodeInfo(rootNodeInfo);
                    EventManager.Instance.TriggerEvent(EventDefine.MainWindow_UpdateTreeView);
                }
                else if (OpenType == OpenEnum.Edit) 
                {

                   
                   
                    MinMsgID = (int)numMinMsgID.Value;
                    MaxMsgID = (int)numMaxMsgID.Value;
                    Des = txtDescription.Text;
                   
                    DialogResult = DialogResult.OK;
                    RootNodeInfo.Des = Des;
                    RootNodeInfo.MinMsgID = MinMsgID;
                    RootNodeInfo.MaxMsgID = MaxMsgID;
                  
                    TreeDateManager.Instance.UpdateRootNodeInfo(RootNodeInfo);
                    EventManager.Instance.TriggerEvent(EventDefine.MainWindow_UpdateTreeView);
                }
                
            }
            else
            {
                MessageBox.Show("MinMsgID 必须小于 MaxMsgID，请检查输入。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }


    }
}
