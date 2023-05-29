using Newtonsoft.Json.Linq;
using ScintillaNET;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ProtoEditor
{
    public partial class ProtoWindow : Form
    {
        private ContextMenuStrip treeViewContextMenu;
        private ContextMenuStrip RootNodeContextMenu;
        private ContextMenuStrip SecondChildNodeContextMenu;


        string[] autoCompleteItems;
        public ProtoWindow()
        {

            RegEvent();


            this.FormClosed += OnWindowClosed;
            InitializeComponent();
            InitializeContextMenus();
            treeView1.MouseDown += TreeView1_MouseDown;
            treeView1.NodeMouseClick += TreeView1_NodeMouseClick;
            treeView1.BeforeSelect += TreeView1_BeforeSelect;
            treeView1.AfterSelect += TreeView1_AfterSelect;
            ProtoInfoText.TextChanged += ProtoInfoText_TextChanged;
            InitTreeView();
            // SetProtoTextReadOnly(true);
            InitButtonClick();
            InitTextBoxTips();
            SerStyle();



        }
        void SetProtoTextReadOnly(bool readOnly)
        {
            ProtoInfoText.ReadOnly = readOnly;
            ProtoInfoText.BackColor = readOnly ? Color.LightGray : SystemColors.Window;
        }
        private void ProtoInfoText_TextChanged(object? sender, EventArgs e)
        {

        }

        private void InitTextBoxTips()
        {

            // ProtoInfoText.LexerName = Lexer.Container;
            // ProtoInfoText.TextChanged += ScintillaInput_TextChanged;
            autoCompleteItems = TextBoxTipsDef.tips.ToArray();

            // 配置 TextBox 的 AutoComplete 功能


            // 添加数据源
            AutoCompleteStringCollection autoCompleteCollection = new AutoCompleteStringCollection();
            autoCompleteCollection.AddRange(autoCompleteItems);

            //   ProtoInfoText.TextChanged += ProtoInfoText_TextChanged;

            ProtoInfoText.CharAdded += ProtoInfoText_CharAdded;
            ProtoInfoText.Delete += ProtoInfoText_Delete;


        }
        public bool IsMatch(string input, string pattern)
        {
            int inputIndex = 0, patternIndex = 0;

            while (inputIndex < input.Length && patternIndex < pattern.Length)
            {
                if (char.ToLower(input[inputIndex]) == char.ToLower(pattern[patternIndex]))
                {
                    patternIndex++;
                }
                inputIndex++;
            }

            return patternIndex == pattern.Length;
        }
        private void ShowAutoComplete()
        {
            int currentPos = ProtoInfoText.CurrentPosition;
            int wordStartPos = ProtoInfoText.WordStartPosition(currentPos, true);

            // 根据光标前面的文本，直到遇到空白符停止，进行自动提示
            string charsTillNow = ProtoInfoText.GetTextRange(wordStartPos, currentPos - wordStartPos);

            if (!string.IsNullOrEmpty(charsTillNow))
            {
                string autoCompleteOptions = string.Join(" ", TextBoxTipsDef.tips.Where(s => IsMatch(s, charsTillNow)));
                if (!string.IsNullOrEmpty(autoCompleteOptions))
                {
                    ProtoInfoText.AutoCShow(charsTillNow.Length, autoCompleteOptions);
                }
            }
        }
        private void ProtoInfoText_Delete(object? sender, ModificationEventArgs e)
        {

            ShowAutoComplete();

        }
        private void ProtoInfoText_CharAdded(object? sender, CharAddedEventArgs e)
        {
            //if (char.IsLetterOrDigit((char)e.Char) || e.Char == '_')
            //{
            //    int currentPos = ProtoInfoText.CurrentPosition;
            //    int wordStartPos = ProtoInfoText.WordStartPosition(currentPos, true);

            //    string currentText = ProtoInfoText.GetTextRange(wordStartPos, currentPos - wordStartPos);

            //    var filteredAutoCompleteData = TextBoxTipsDef.tips.FindAll(item => item.StartsWith(currentText));

            //    if (filteredAutoCompleteData.Count > 0)
            //    {
            //        string joinedAutoCompleteList = string.Join(" ", filteredAutoCompleteData);
            //        ProtoInfoText.AutoCShow(currentText.Length, joinedAutoCompleteList);
            //    }
            //}
            ShowAutoComplete();
        }
        private void SerStyle()
        {
            ProtoInfoText.Lexer = Lexer.Cpp;

            // 设置词法分析器的样式，例如关键字、注释等的颜色和字体
            ProtoInfoText.StyleResetDefault();
            ProtoInfoText.Styles[Style.Default].Font = "Consolas";
            ProtoInfoText.Styles[Style.Default].Size = 10;
            ProtoInfoText.StyleClearAll();

            // 设置关键字列表
            ProtoInfoText.SetKeywords(0, "syntax package import option optional required repeated group extensions message service rpc enum oneof map reserved to max default");
            ProtoInfoText.SetKeywords(1, "double float int32 int64 uint32 uint64 sint32 sint64 fixed32 fixed64 sfixed32 sfixed64 bool string bytes ");
            // 设置注释样式
            ProtoInfoText.Styles[Style.Cpp.Comment].ForeColor = Color.FromArgb(0, 128, 0); // Green
            ProtoInfoText.Styles[Style.Cpp.CommentLine].ForeColor = Color.FromArgb(0, 128, 0); // Green
            ProtoInfoText.Styles[Style.Cpp.CommentDoc].ForeColor = Color.FromArgb(128, 128, 128); // Gray
            ProtoInfoText.Styles[Style.Cpp.Number].ForeColor = Color.FromArgb(163, 21, 21); // Red
            ProtoInfoText.Styles[Style.Cpp.String].ForeColor = Color.FromArgb(163, 21, 21); // Red
            ProtoInfoText.Styles[Style.Cpp.Character].ForeColor = Color.FromArgb(163, 21, 21); // Red
            ProtoInfoText.Styles[Style.Cpp.Preprocessor].ForeColor = Color.FromArgb(128, 128, 128); // Gray
            ProtoInfoText.Styles[Style.Cpp.Operator].ForeColor = Color.FromArgb(0, 0, 0); // Black
            ProtoInfoText.Styles[Style.Cpp.Regex].ForeColor = Color.FromArgb(0, 0, 255); // Blue
            ProtoInfoText.Styles[Style.Cpp.CommentLineDoc].ForeColor = Color.FromArgb(128, 128, 128); // Gray
            ProtoInfoText.Styles[Style.Cpp.Word].ForeColor = Color.FromArgb(0, 0, 255); // Blue
            ProtoInfoText.Styles[Style.Cpp.Word2].ForeColor = Color.FromArgb(43, 145, 175); // Teal
            ProtoInfoText.Styles[Style.Cpp.CommentDocKeyword].ForeColor = Color.FromArgb(128, 128, 128); // Gray
            ProtoInfoText.Styles[Style.Cpp.CommentDocKeywordError].ForeColor = Color.FromArgb(128, 128, 128); // Gray
            ProtoInfoText.Styles[Style.Cpp.GlobalClass].ForeColor = Color.FromArgb(0, 0, 255); // Blue



        }



        private void ScintillaInput_TextChanged(object? sender, EventArgs e)
        {
            var currentLine = ProtoInfoText.Lines[ProtoInfoText.CurrentLine];
            var lineText = currentLine.Text.TrimEnd();


            var filteredAutoCompleteData = TextBoxTipsDef.tips.FindAll(item => item.StartsWith(lineText));

            if (filteredAutoCompleteData.Count > 0)
            {
                string joinedAutoCompleteList = string.Join(" ", filteredAutoCompleteData);
                ProtoInfoText.AutoCShow(lineText.Length, joinedAutoCompleteList);
            }
        }

        private void TreeView1_BeforeSelect(object? sender, TreeViewCancelEventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                if (treeView1.SelectedNode.Index != e.Node.Index)
                {
                    SecondChildNode secondChildNode = treeView1.SelectedNode as SecondChildNode;
                    string oldstr = File.ReadAllText(secondChildNode.ProtoInfo.ProtoPath);
                    string newstr = ProtoInfoText.Text;
                    if (oldstr != newstr)
                    {
                        DialogResult result = MessageBox.Show(StringDefine.ProtoWindow_NotSaveNow, StringDefine.ProtoWindow_OKText, MessageBoxButtons.YesNo);
                        if (result == DialogResult.Yes)
                        {
                            // SetProtoTextReadOnly(true);
                            treeView1.Refresh();
                        }
                        else
                        {
                            e.Cancel = true;
                            treeView1.Refresh();
                        }

                    }
                    else
                    {
                        // SetProtoTextReadOnly(true);



                    }
                }
            }
        }

        private void TreeView1_AfterSelect(object? sender, TreeViewEventArgs e)
        {
            if (e.Node != null)
            {

                // SetProtoTextReadOnly(true);

                OnNodeLeftClick();
                treeView1.Refresh();


            }
        }

        private void OnWindowClosed(object? sender, FormClosedEventArgs e)
        {
            UnRegEvent();
        }

        void RegEvent()
        {
            EventManager.Instance.RegisterEvent(EventDefine.MainWindow_UpdateTreeView, UpdateTree);
            EventManager.Instance.RegisterEvent(EventDefine.MainWindow_SelectTreeView, SeletcTreeView);
        }

        void InitButtonClick()
        {
            SaveButton.Click += OnSaveButtonClick;
            // EditButton.Click += OnEditButtonClick;
            ExportProtoToClient.Click += OnExportProtoToClientClick;
            ExportProtoToServer.Click += OnExportProtoToServerClick;
            GenMsgIDForClient.Click += OnGenMsgIDForClientClick;
            GenMsgIDForServer.Click += OnGenMsgIDForServerClick;
        }

        private void OnSaveButtonClick(object? sender, EventArgs e)
        {
            if (ProtoInfoText.ReadOnly == true)
            {
                MessageBox.Show(StringDefine.ProtoWindow_NoNeedSave, StringDefine.ProtoWindow_OKText);
                return;
            }
            SecondChildNode secondChildNode = treeView1.SelectedNode as SecondChildNode;
            if (secondChildNode != null)
            {
                string strvalue = ProtoInfoText.Text;
                ProtoFileInfo protoFileInfo = ProtoManager.Instance.TryParseProtoFile(secondChildNode.ProtoInfo.ProtoPath, strvalue);
                if (protoFileInfo != null)
                {
                    File.WriteAllText(secondChildNode.ProtoInfo.ProtoPath, strvalue);
                    //  SetProtoTextReadOnly(true);
                    ProtoManager.Instance.UpdateProtoInfos();
                }
                else
                {

                }

            }
            else
            {

            }



        }

        private void OnEditButtonClick(object? sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                // SetProtoTextReadOnly(false);

            }
            else
            {
                MessageBox.Show(StringDefine.ProtoWindow_NoFileCanEditor, StringDefine.ProtoWindow_OKText);
            }
            treeView1.Refresh();
        }

        private void OnExportProtoToClientClick(object? sender, EventArgs e)
        {
            ProtoManager.Instance.ExportToClient();
        }

        private void OnExportProtoToServerClick(object? sender, EventArgs e)
        {
            ProtoManager.Instance.ExportToServer();
        }

        private void OnGenMsgIDForClientClick(object? sender, EventArgs e)
        {
            ProtoManager.Instance.GenerateMsgIDCSharpFileForClient();
        }

        private void OnGenMsgIDForServerClick(object? sender, EventArgs e)
        {
            ProtoManager.Instance.GenerateMsgIDCSharpFileForServer();
        }



        private void SeletcTreeView(object[] args)
        {
            string path = args[0].ToString();
            for (int i = 0; i < treeView1.Nodes.Count; i++)
            {
                SecondChildNode secondChildNode = treeView1.Nodes[i] as SecondChildNode;
                if (secondChildNode != null)
                {
                    if (secondChildNode.ProtoInfo.ProtoPath == path)
                    {
                        treeView1.SelectedNode = secondChildNode;
                        OnNodeLeftClick();
                        treeView1.Refresh();
                        return;
                    }
                }
            }


        }

        void UnRegEvent()
        {
            EventManager.Instance.UnregisterEvent(EventDefine.MainWindow_UpdateTreeView, UpdateTree);
            EventManager.Instance.UnregisterEvent(EventDefine.MainWindow_SelectTreeView, SeletcTreeView);
        }

        void InitTreeView()
        {
            treeView1.Nodes.Clear();
            List<ProtoFileInfo> protolist = ProtoManager.Instance.GetProtoInfos();
            for (int i = 0; i < protolist.Count; i++)
            {
                SecondChildNode newNode = new SecondChildNode();
                newNode.ProtoInfo = protolist[i];
                string nodename = $"{protolist[i].ProtoField.Name}({protolist[i].MinMsgId}-{protolist[i].MaxMsgId})";
                newNode.Text = nodename;
                treeView1.Nodes.Add(newNode);
            }

        }

        public void UpdateTree()
        {
            treeView1.Nodes.Clear();
            List<ProtoFileInfo> protolist = ProtoManager.Instance.GetProtoInfos();
            for (int i = 0; i < protolist.Count; i++)
            {
                SecondChildNode newNode = new SecondChildNode();
                newNode.ProtoInfo = protolist[i];
                string nodename = $"{protolist[i].ProtoField.Name}({protolist[i].MinMsgId}-{protolist[i].MaxMsgId})";
                newNode.Text = nodename;
                treeView1.Nodes.Add(newNode);
            }
        }



        private void InitializeContextMenus()
        {
            // TreeView右键菜单
            treeViewContextMenu = new ContextMenuStrip();
            treeViewContextMenu.Items.Add("添加Proto协议", null, AddRootNode_Click);

            // 一级节点右键菜单
            RootNodeContextMenu = new ContextMenuStrip();
            RootNodeContextMenu.Items.Add("添加Proto协议", null, AddChildNode_Click);
            RootNodeContextMenu.Items.Add("删除proto协议", null, DeleteNode_Click);
            RootNodeContextMenu.Items.Add("重命名文件", null, EditNode_Click);
            RootNodeContextMenu.Items.Add("打开Proto文件夹", null, OnOpenInExplorer);
            //二级节点右键菜单
            SecondChildNodeContextMenu = new ContextMenuStrip();
            SecondChildNodeContextMenu.Items.Add("添加Proto协议", null, OnAddProtoClick);
            SecondChildNodeContextMenu.Items.Add("删除Proto协议", null, OnDeleteProtoClick);
            SecondChildNodeContextMenu.Items.Add("编辑Proto协议", null, OnEditorProtoClick);
            SecondChildNodeContextMenu.Items.Add("打开Proto文件夹", null, OnOpenInExplorer);
        }

        private void RootNodeOnAddProtoClick(object? sender, EventArgs e)
        {
            TreeNode selectedNode = treeView1.SelectedNode;
            RootNode rootNode = selectedNode as RootNode;
            if (rootNode == null)
                return;

            ProtoInfoWindow inputDialog = new ProtoInfoWindow();
            inputDialog.Location = GetCenterLocation(this, inputDialog);
            inputDialog.Show();





        }

        private Point GetCenterLocation(Form parentForm, Form childForm)
        {
            int x = parentForm.Location.X + (parentForm.Width - childForm.Width) / 2;
            int y = parentForm.Location.Y + (parentForm.Height - childForm.Height) / 2;
            return new Point(x, y);
        }
        private void OnOpenInExplorer(object? sender, EventArgs e)
        {
            TreeNode selectedNode = treeView1.SelectedNode;
            SecondChildNode Node = selectedNode as SecondChildNode;
            if (Node == null)
                return;
            string filePath = Node.ProtoInfo.ProtoPath;
            if (File.Exists(filePath))
            {
                // 获取文件所在的目录路径
                string folderPath = Path.GetDirectoryName(filePath);

                // 使用 Windows 资源管理器打开文件夹
                Process.Start("explorer.exe", folderPath);
            }
            else
            {
                Console.WriteLine("文件不存在: " + filePath);
            }
        }

        private void OnEditorProtoClick(object? sender, EventArgs e)
        {

        }

        private void OnDeleteProtoClick(object? sender, EventArgs e)
        {

        }

        private void OnAddProtoClick(object? sender, EventArgs e)
        {

        }

        private void TreeView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                TreeNode node = treeView1.GetNodeAt(e.Location);
                if (node == null)
                {
                    treeView1.ContextMenuStrip = treeViewContextMenu;
                    treeViewContextMenu.Show(treeView1, e.Location);
                }
                else
                {
                    if (node.Level == 0)
                    {
                        treeView1.SelectedNode = node;
                        treeView1.ContextMenuStrip = RootNodeContextMenu;
                        RootNodeContextMenu.Show(treeView1, e.Location);
                    }
                    else if (node.Level == 1)
                    {
                        treeView1.SelectedNode = node;
                        treeView1.ContextMenuStrip = SecondChildNodeContextMenu;
                        SecondChildNodeContextMenu.Show(treeView1, e.Location);
                    }

                }
            }
            treeView1.SelectedNode = treeView1.GetNodeAt(e.Location);

        }
        private void TreeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {

            if (e.Button == MouseButtons.Left && e.Node.Level == 0)
            {
                OnNodeLeftClick();
            }

        }

        void OnNodeLeftClick()
        {
            SecondChildNode secondChildNode = treeView1.SelectedNode as SecondChildNode;
            if (secondChildNode != null)
            {
                if (File.Exists(secondChildNode.ProtoInfo.ProtoPath) == true)
                {
                    string value = File.ReadAllText(secondChildNode.ProtoInfo.ProtoPath);
                    ProtoInfoText.Text = value;
                }

            }
            else
            {

            }
        }

        private void AddRootNode_Click(object sender, EventArgs e)
        {

            ProtoInfoWindow addNodeForm = new ProtoInfoWindow();
            addNodeForm.Location = GetCenterLocation(this, addNodeForm);
            addNodeForm.OpenEnum = OpenEnum.Open;
            addNodeForm.ShowDialog(this);



        }

        private void AddChildNode_Click(object sender, EventArgs e)
        {
            ProtoInfoWindow addNodeForm = new ProtoInfoWindow();
            addNodeForm.Location = GetCenterLocation(this, addNodeForm);
            addNodeForm.OpenEnum = OpenEnum.Open;
            addNodeForm.ShowDialog(this);
        }

        private void DeleteNode_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeView1.SelectedNode;
            if (selectedNode != null)
            {
                SecondChildNode rootNode = selectedNode as SecondChildNode;
                File.Delete(rootNode.ProtoInfo.ProtoPath);
                ProtoInfoText.Text = string.Empty;
                ProtoManager.Instance.UpdateProtoInfos();
                EventManager.Instance.TriggerEvent(EventDefine.MainWindow_UpdateTreeView);

            }
        }

        private void EditNode_Click(object sender, EventArgs e)
        {


            SecondChildNode selectedNode = treeView1.SelectedNode as SecondChildNode;
            if (selectedNode != null)
            {

                ProtoInfoWindow inputDialog = new ProtoInfoWindow(selectedNode.ProtoInfo);
                inputDialog.OpenEnum = OpenEnum.Edit;
                inputDialog.Location = GetCenterLocation(this, inputDialog);
                inputDialog.OpenEnum = OpenEnum.Edit;
                inputDialog.Show();


            }
        }
        private void SerchText_TextChanged(object sender, EventArgs e)
        {

        }

        private void TitleLabel_Click(object sender, EventArgs e)
        {

        }

        private void TreeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Level == 0)
            {
                e.Cancel = true;
            }
        }

        private void TreeView1_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Level == 0)
            {
                e.Cancel = true;
            }
        }

        private void ProtoWindow_Load(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void ProtoInfoText_Click(object sender, EventArgs e)
        {

        }

        private void RefreshBtn_Click(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();
            List<ProtoFileInfo> protolist = ProtoManager.Instance.UpdateProtoInfos();
            for (int i = 0; i < protolist.Count; i++)
            {
                SecondChildNode newNode = new SecondChildNode();
                newNode.ProtoInfo = protolist[i];
                string nodename = $"{protolist[i].ProtoField.Name}({protolist[i].MinMsgId}-{protolist[i].MaxMsgId})";
                newNode.Text = nodename;
                treeView1.Nodes.Add(newNode);
            }
        }
    }


}
