using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private List<ProtoInfo> ProtoInfos= new List<ProtoInfo>();
        private string protoFilesPath = @"F:\GameProject\PtotoFiles";
        List<string> excludedFiles = new List<string> { "standmsgid.proto" };
        public MainWindow()
        {
            InitializeComponent();
            StarProtoParser();
            ParseProtoFiles();
           
            textEditor.TextArea.TextEntered += TextEditor_TextEntered;
            textEditor.TextArea.TextEntering += TextEditor_TextEntering;
            textEditor.Document.Changed += Document_TextChanged;
            textEditor.KeyDown += TextEditor_KeyDown;
     

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
        private void ParseProtoFiles()
        {
            // Load your proto files here
           
           
            treeView.Items.Clear();
            for (int i = 0; i < ProtoInfos.Count; i++)
            {
                TreeViewItem item = new TreeViewItem();
                int msgid=0;
                if (ProtoInfos[i].Messages.Count>0)
                {
                    msgid = ProtoInfos[i].Messages[0].MsgID;
                }
                item.Header =$"{msgid}:{Path.GetFileName(ProtoInfos[i].FilePath)}" ;
                item.Tag = ProtoInfos[i].FilePath;
                treeView.Items.Add(item);
            }
           

            
           
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            dynamic selectedItem = e.NewValue;
            if (selectedItem != null && selectedItem.Tag != null)
            {
                _selectedFilePath = selectedItem.Tag;
                textEditor.Text = File.ReadAllText(_selectedFilePath);
                saveButton.IsEnabled = true;
            }
            else
            {
                _selectedFilePath = null;
                textEditor.Text = string.Empty;
                saveButton.IsEnabled = false;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
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
                ParseProtoFiles();
                return;
            }

            // treeView.ItemsSource = _filePaths.Where(path => Path.GetFileName(path).ToLowerInvariant().Contains(searchTerm))
            //     .Select(filePath => new
            //     {
            //        
            //         Header = Path.GetFileName(filePath),
            //         Tag = filePath
            //     }).ToList();
            treeView.Items.Clear();
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
            //treeView.ItemsSource = _filePaths;
            //ProtoInfo protoInfo = ProtoParser.ParseProtoFile(filePath);
        }

        private void AddMenuItem_Click(object sender, RoutedEventArgs e)
        {
              // 1. Create a dialog for getting the new proto file name
            var inputDialog = new InputBox("Enter new proto file name:");
            inputDialog.Owner = this;
            if (inputDialog.ShowDialog() == true)
            {
                string newFileName = inputDialog.Input;
                string newFilePath = Path.Combine(@"F:\GameProject\PtotoFiles", newFileName);

                // 2. Create the new proto file and add it to the current directory
                if (!File.Exists(newFilePath))
                {
                    File.Create(newFilePath).Dispose();

                    // 3. Read the new proto file and display its content in the text editor
                    _selectedFilePath = newFilePath;
                    textEditor.Text = File.ReadAllText(_selectedFilePath);
                    saveButton.IsEnabled = true;

                    // 4. Add the new proto file to the _filePaths list and TreeView
                    _filePaths = _filePaths.Append(newFilePath).ToArray();
                    ParseProtoFiles();
                }
                else
                {
                    MessageBox.Show($"The file {newFileName} already exists. Please choose a different name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            dynamic selectedItem = treeView.SelectedItem;
            if (selectedItem != null && selectedItem.Tag != null)
            {
                string selectedFilePath = selectedItem.Tag;
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete {Path.GetFileName(selectedFilePath)}?", "Delete Proto File", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    File.Delete(selectedFilePath);
                    // Update the _filePaths array and TreeView
                    ParseProtoFiles();
                }
            }
        }

        private void TextBlock_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            if (textBlock != null)
            {
                // Open the context menu for the clicked TextBlock
                textBlock.ContextMenu.PlacementTarget = textBlock;
                textBlock.ContextMenu.IsOpen = true;

                // Set handled to true to prevent bubbling up of the event
                e.Handled = true;
            }
        }

        private void EditMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_selectedFilePath))
            {
                textEditor.IsReadOnly = !textEditor.IsReadOnly;
            }
        }

        private void ShowInFolderMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_selectedFilePath))
            {
                string folderPath = Path.GetDirectoryName(_selectedFilePath);
                string fileToSelect = _selectedFilePath;
                System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{fileToSelect}\"");
            }
        }
       
        
        private void StarProtoParser()
        {
            ProtoInfos.Clear();
            
          
           _filePaths  = Directory.GetFiles(protoFilesPath, "*.proto")
               .Where(filePath => !excludedFiles.Contains(Path.GetFileName(filePath))).ToArray();
           for (int i = 0; i < _filePaths.Length; i++)
           {
               string protopath = _filePaths[i];
               ProtoInfo protoInfo = ProtoParser.ParseProtoFile(protopath);
               ProtoInfos.Add(protoInfo);
           }
           ProtoInfos.Sort(SortByMsgId);
        }

        private int SortByMsgId(ProtoInfo x, ProtoInfo y)
        {
            int id1= x.Messages.Count<=0?0:x.Messages[0].MsgID;
            int id2= y.Messages.Count<=0?0:y.Messages[0].MsgID;
            return id1.CompareTo(id2);
        }
    }
}
