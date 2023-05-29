

using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ProtoEditor;


public class FirstLevelNode:TreeViewItem
{
    public NodeType NodeType = NodeType.First;
    public string NodeName { get; set; }
    public int MinValue { get; set; }
    public int MaxValue { get; set; }
    public List<ProtoInfo> ProtoInfos = new List<ProtoInfo>();

    public FirstLevelNode()
    {
        this.MouseRightButtonDown += FirstLevelNode_MouseRightButtonDown;
        this.MouseLeftButtonDown += FirstLevelNode_MouseLeftButtonDown;
        this.ContextMenu = CreateContextMenu();
    }
    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);

        ProtoManager.Instance.curseletProtoPath = ProtoInfos[0].FilePath;
        EventManager.Instance.TriggerEvent(EventDefine.TreeViewFirstNodeLeftClickEvent,ProtoInfos[0].FilePath);
    }
    private ContextMenu CreateContextMenu()
    {
        ContextMenu contextMenu = new ContextMenu();

        MenuItem AddProto = new MenuItem { Header = "AddProto" };
        AddProto.Click += AddProto_Click;
        contextMenu.Items.Add(AddProto);

        MenuItem EditProto = new MenuItem { Header = "EditProto" };
        EditProto.Click += EditProto_Click;
        contextMenu.Items.Add(EditProto);
        
        MenuItem DeleteProto = new MenuItem { Header = "DeleteProto" };
        DeleteProto.Click += DeleteProto_Click;
        contextMenu.Items.Add(DeleteProto);
        
        MenuItem ReNameProto = new MenuItem { Header = "ReNameProto" };
        ReNameProto.Click += ReNameProto_Click;
        contextMenu.Items.Add(ReNameProto);
        
        MenuItem ShowProtoInExplorer = new MenuItem { Header = "ShowProtoInExplorer" };
        ShowProtoInExplorer.Click += ShowProtoInExplorer_Click;
        contextMenu.Items.Add(ShowProtoInExplorer);

        return contextMenu;
    }

    private void ReNameProto_Click(object sender, RoutedEventArgs e)
    {
        EventManager.Instance.TriggerEvent(EventDefine.ProtoReNameEvent, ProtoInfos[0].FilePath);
    }

    private void ShowProtoInExplorer_Click(object sender, RoutedEventArgs e)
    {
        EventManager.Instance.TriggerEvent(EventDefine.ShowProtoInExplorerEvent, ProtoInfos[0].FilePath);
    }

    private void DeleteProto_Click(object sender, RoutedEventArgs e)
    {
        EventManager.Instance.TriggerEvent(EventDefine.ProtoDeleteEvent, ProtoInfos[0].FilePath);
    }

    private void EditProto_Click(object sender, RoutedEventArgs e)
    {
        EventManager.Instance.TriggerEvent(EventDefine.ProtoEditClickEvent, ProtoInfos[0].FilePath);
    }

    private void AddProto_Click(object sender, RoutedEventArgs e)
    {
        
        EventManager.Instance.TriggerEvent(EventDefine.ProtoAddEvent, ProtoInfos[0].FilePath);
    }

    //鼠标左键点击事件
    private void FirstLevelNode_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        EventManager.Instance.TriggerEvent(EventDefine.TreeViewFirstNodeLeftClickEvent,ProtoInfos[0].FilePath);
    }
    //鼠标右键点击
    private void FirstLevelNode_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (this.ContextMenu != null)
        {
            ProtoManager.Instance.curseletProtoPath = ProtoInfos[0].FilePath;
            this.ContextMenu.PlacementTarget = this;
            this.ContextMenu.IsOpen = true;

            e.Handled = true;
        }
        
    }
}

