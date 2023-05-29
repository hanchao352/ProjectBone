using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace ProtoEditor
{
    public partial class MainWindow : Window
    {
        private string _selectedFilePath;
        private string[] _filePaths;
        private CompletionWindow completionWindow;
        private string protoFilesPath = @"F:\GameProject\PtotoFiles";
        public MainWindow()
        {
            ProtoManager.Instance.Initialize();
            EventManager.Instance.Initialize();
            RegisterEvent();
           
            InitializeComponent();
            InitTreeView();
            textEditor.TextArea.TextEntered += TextEditor_TextEntered;
            textEditor.TextArea.TextEntering += TextEditor_TextEntering;
            textEditor.Document.Changed += Document_TextChanged;
            textEditor.KeyDown += TextEditor_KeyDown;
     

        }

        public void InitTreeView()
        {
            treeView.Items.Clear();
            List<ProtoInfo> ProtoInfos = ProtoManager.Instance.UpdateProtoInfos();
            for (int i = 0; i < ProtoInfos.Count; i++)
            {
                FirstLevelNode item = new FirstLevelNode();
                int msgid=0;
                if (ProtoInfos[i].Messages.Count>0)
                {
                    msgid = ProtoInfos[i].Messages[0].MsgID;
                }
                item.Tag = ProtoInfos[i].FilePath;
                item.NodeType = NodeType.First;
                item.NodeName = $"{msgid}:{Path.GetFileName(ProtoInfos[i].FilePath)}" ;
                item.Header = item.NodeName;
                item.ProtoInfos.Add(ProtoInfos[i]);
                treeView.Items.Add(item);
            }
        }

        public void UpdateTreeView()
        {
            treeView.Items.Clear();
            treeView.Items.Clear();
            List<ProtoInfo> ProtoInfos = ProtoManager.Instance.UpdateProtoInfos();
            for (int i = 0; i < ProtoInfos.Count; i++)
            {
                FirstLevelNode item = new FirstLevelNode();
                int msgid=0;
                if (ProtoInfos[i].Messages.Count>0)
                {
                    msgid = ProtoInfos[i].Messages[0].MsgID;
                }
                item.Tag = ProtoInfos[i].FilePath;
                item.NodeType = NodeType.First;
                item.NodeName = $"{msgid}:{Path.GetFileName(ProtoInfos[i].FilePath)}" ;
                item.Header = item.NodeName;
                item.ProtoInfos.Add(ProtoInfos[i]);
                treeView.Items.Add(item);
            }
        }

        public void RegisterEvent()
        {
            EventManager.Instance.RegisterEvent(EventDefine.TreeViewFirstNodeLeftClickEvent, TreeViewFirstNodeLeftClickEvent);
            EventManager.Instance.RegisterEvent(EventDefine.TreeViewFirstNodeRightClickEvent, TreeViewFirstNodeRightClickEvent);
            EventManager.Instance.RegisterEvent(EventDefine.ProtoEditClickEvent, ProtoEditClickEvent);
            EventManager.Instance.RegisterEvent(EventDefine.ProtoDeleteEvent, ProtoDeleteEvent);
            EventManager.Instance.RegisterEvent(EventDefine.ProtoAddEvent, ProtoAddEvent);
            EventManager.Instance.RegisterEvent(EventDefine.ShowProtoInExplorerEvent, ShowProtoInExplorerEvent);
            EventManager.Instance.RegisterEvent(EventDefine.ProtoReNameEvent, ProtoReNameEvent);
            
        }

        private void ProtoReNameEvent(object[] args)
        {
            string oldpath = args[0] as string;
            var inputDialog = new InputBox("Enter new proto file name:");
            inputDialog.Owner = this;
            if (inputDialog.ShowDialog() == true)
            {
                string inputstr = inputDialog.Input;
                if (string.IsNullOrWhiteSpace(inputstr))
                {
                    return;
                }
                
                string[] str= inputstr.Split('.');
                string fileName = str[0]+".proto";
                string newFilePath = Path.Combine(ProtoManager.Instance.GetProtoDirPath(), fileName);
               
               
                // 2. Create the new proto file and add it to the current directory
                if (!File.Exists(fileName))
                {
                    try
                    {
                        // 使用 File.Move 方法重命名文件
                        File.Move(oldpath, newFilePath);
                        Prototemplate.Instance.MessageName = str[0];
                        File.WriteAllText( newFilePath,Prototemplate.Instance.GetTemplate());
                        ProtoManager.Instance.UpdateProtoInfos();
                        UpdateTreeView();
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine("文件重命名失败。");
                        Console.WriteLine(ex.Message);
                    }
                   
                }
                else
                {
                    MessageBox.Show($"The file {fileName} already exists. Please choose a different name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ShowProtoInExplorerEvent(object[] args)
        {
            string path = args[0] as string;
            if (!string.IsNullOrEmpty(path))
            {
                System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{path}\"");
            }
        }

        private void ProtoAddEvent(object[] args)
        {
            var inputDialog = new InputBox("Enter new proto file name:");
            inputDialog.Owner = this;
            if (inputDialog.ShowDialog() == true)
            {
                string inputstr = inputDialog.Input;
                if (string.IsNullOrWhiteSpace(inputstr))
                {
                    return;
                }
                string[] str= inputstr.Split('.');
                string fileName = str[0]+".proto";
                string newFilePath = Path.Combine(ProtoManager.Instance.GetProtoDirPath(), fileName);
               
                
               
                if (!File.Exists(fileName))
                {
                    File.Create(newFilePath).Dispose();

                   
                    textEditor.Text = File.ReadAllText(newFilePath);
                    Prototemplate.Instance.MessageName = str[0];
                    File.WriteAllText( newFilePath,Prototemplate.Instance.GetTemplate());
                    
                    saveButton.IsEnabled = true;
                    
                    _filePaths = _filePaths.Append(newFilePath).ToArray();
                    ProtoManager.Instance.UpdateProtoInfos();
                    UpdateTreeView();
                }
                else
                {
                    MessageBox.Show($"The file {fileName} already exists. Please choose a different name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ProtoDeleteEvent(object[] args)
        {
            
                string path = args[0] as string;
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete {Path.GetFileName(path)}?", "Delete Proto File", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    File.Delete(path);
                    ProtoManager.Instance.UpdateProtoInfos();
                    UpdateTreeView();
                }
            
        }

        private void ProtoEditClickEvent(object[] args)
        {
            string path = args[0] as string;
            if (!string.IsNullOrEmpty(path))
            {
                textEditor.IsReadOnly = !textEditor.IsReadOnly;
            }
        }

        private void TreeViewFirstNodeRightClickEvent(object[] args)
        {
           
        }

        private void TreeViewFirstNodeLeftClickEvent(object[] args)
        {
                string path = args[0] as string;
                if (File.Exists(path)==false)
                {
                    return;
                }
                textEditor.Text = File.ReadAllText(path);
                saveButton.IsEnabled = true;
            
        }


        private void TextEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab && completionWindow != null)
            {
                e.Handled = true;
                TextEditor_TextEntered(sender, new TextCompositionEventArgs(InputManager.Current.PrimaryKeyboardDevice, new TextComposition(InputManager.Current, textEditor, "\t")));
            }
        }

        
        private void Document_TextChanged(object sender, EventArgs e)
        {
            try
            {
                
                var document = sender as TextDocument;
                if (document != null)
                {
                    var textArea = textEditor.TextArea;
                    var caretLine = document.GetLineByOffset(textArea.Caret.Offset);

                    if (caretLine.LineNumber > 1 && textArea.Caret.Offset > 0 && document.GetCharAt(textArea.Caret.Offset - 1) == '\n')
                    {
                        var previousLine = document.GetLineByOffset(caretLine.Offset - 1);
                        var previousLineText = document.GetText(previousLine.Offset, previousLine.Length);

                        var indentRegex = new Regex(@"^[\s]*");
                        var match = indentRegex.Match(previousLineText);

                        if (match.Success)
                        {
                            var indentation = match.Value;
                            Console.WriteLine("Match:"+indentation);
                            if (caretLine.Length < indentation.Length)
                            {
                                document.Insert(caretLine.Offset, indentation);
                                textArea.Caret.Offset = caretLine.Offset + indentation.Length;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Error occurred: " + ex.Message);
            }
        }


        private void TextEditor_TextEntered(object sender, TextCompositionEventArgs e)
        {
            var line = textEditor.Document.GetLineByNumber(textEditor.TextArea.Caret.Line);
            string str = textEditor.Document.GetText(line);
            Console.WriteLine(e.Text+"  "+str);
            CodeCompletionProvider provider = new CodeCompletionProvider();

            // Generate the completion data.
            IList<ICompletionData> completionData = provider.GenerateCompletionData(textEditor);
            if (string.IsNullOrWhiteSpace(e.Text) || completionData.Count == 0)
            {
                completionWindow?.Close();
            }
            else if (e.Text == "=" && !string.IsNullOrWhiteSpace(e.Text))
            {
                // Create a new instance of your CodeCompletionProvider.
               

                // Create a new completion window and display the completion data.
                completionWindow = new CompletionWindow(textEditor.TextArea);
                completionWindow.Closed += delegate { completionWindow = null; };

                // Add completion data to the completion list.
                foreach (var data in completionData)
                {
                    completionWindow.CompletionList.CompletionData.Add(data);
                }
               
                int length = str.Length;
                if (length == 1 )
                {
                    if (string.IsNullOrWhiteSpace(str)==false && SugDef.IsMatch(str[0].ToString())  )
                    {
                        completionWindow.Show();
                    }
                }
                else
                {
                    if ( str.Contains("=")==false  )
                    {
                        completionWindow.Show();
                    }
                }
            }
        }
        private void TextEditor_TextEntering(object sender, TextCompositionEventArgs e)
        {
            var line = textEditor.Document.GetLineByNumber(textEditor.TextArea.Caret.Line);
            string str = textEditor.Document.GetText(line);
            CodeCompletionProvider provider = new CodeCompletionProvider();

            // Generate the completion data.
            IList<ICompletionData> completionData = provider.GenerateCompletionData(textEditor);
            if (string.IsNullOrWhiteSpace(e.Text) || completionData.Count == 0 )
            {
                completionWindow?.Close();
            }
            else if (e.Text.Length > 0 && completionWindow != null)
            {
                if (str.Contains("="))
                {
                    completionWindow.Close();
                }
                if (!char.IsLetterOrDigit(e.Text[0]) && str.Contains("=")==false)
                {
                    // Whenever a non-letter character has been typed, close the completion window.
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
            else if (e.Text.Length > 0 && string.IsNullOrWhiteSpace(e.Text)==false)
            {
                // Create a new instance of your CodeCompletionProvider.
              

                // Create a new completion window and display the completion data.
                completionWindow = new CompletionWindow(textEditor.TextArea);
                completionWindow.Closed += delegate { completionWindow = null; };
                
                // Corrected: Add completion data to the completion list.
                foreach (var data in completionData)
                {
                    completionWindow.CompletionList.CompletionData.Add(data);
                }
                
                int length = str.Length;
              
                if (length == 1 )
                {
                    if (string.IsNullOrWhiteSpace(e.Text[0].ToString()) )
                    {
                        completionWindow.Close();
                    }
                    else if (string.IsNullOrWhiteSpace(str)==false && SugDef.IsMatch(str[0].ToString())  )
                    {
                        completionWindow.Show();
                    }
                    
                    
                }
                else
                {
                    if ( str.Contains("=")==false  )
                    {
                        completionWindow.Show();
                    }
                }
               
            }
        }
        

        

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string _selectedFilePath = ProtoManager.Instance.curseletProtoPath;
            if (!string.IsNullOrEmpty(_selectedFilePath))
            {
                File.WriteAllText(_selectedFilePath, textEditor.Text);
                MessageBox.Show("Changes saved successfully!", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchTerm = searchBox.Text.ToLowerInvariant();

            if (string.IsNullOrEmpty(searchTerm))
            {
                // Display all files if search term is empty
                ProtoManager.Instance.UpdateProtoInfos();
                UpdateTreeView();
                return;
            }
            treeView.Items.Clear();
            List<ProtoInfo> ProtoInfos = ProtoManager.Instance.UpdateProtoInfos();
            for (int i = 0; i < ProtoInfos.Count; i++)
            {
                int msgid=0;
                if (ProtoInfos[i].Messages.Count>0)
                {
                    msgid = ProtoInfos[i].Messages[0].MsgID;
                }
                string filename= $"{msgid}:{Path.GetFileName(ProtoInfos[i].FilePath)}"  ;
                if (filename.ToLowerInvariant().Contains(searchTerm))
                {
                    
                    TreeViewItem item = new TreeViewItem();
                    item.Header = $"{msgid}:{Path.GetFileName(ProtoInfos[i].FilePath)}"  ;
                    item.Tag = ProtoInfos[i].FilePath;
                    treeView.Items.Add(item);
                }
               
            }
           
        }
        private void EditMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_selectedFilePath))
            {
                textEditor.IsReadOnly = !textEditor.IsReadOnly;
            }
        }

       
       
        
       

        private int SortByMsgId(ProtoInfo x, ProtoInfo y)
        {
            int id1= x.Messages.Count<=0?0:x.Messages[0].MsgID;
            int id2= y.Messages.Count<=0?0:y.Messages[0].MsgID;
            return id1.CompareTo(id2);
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            ProtoManager.Instance.ExportProtos();
        }

        private void ExportCSForClient_Click(object sender, RoutedEventArgs e)
        {
            ProtoManager.Instance.GenerateMsgIDCSharpFileForClient();
        }

        private void ExportCSForServer_Click(object sender, RoutedEventArgs e)
        {
            ProtoManager.Instance.GenerateMsgIDCSharpFileForServer();
           
        }
    }
}
