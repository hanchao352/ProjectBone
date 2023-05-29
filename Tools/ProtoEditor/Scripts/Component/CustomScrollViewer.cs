
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

public class CustomScrollViewer:ScrollViewer
{
   
    public CustomScrollViewer()
    {
        this.MouseRightButtonDown += CustomScrollViewer_MouseRightButtonDown;
        this.ContextMenu = CreateContextMenu();

        
    }

    private void CustomScrollViewer_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
       EventManager.Instance.TriggerEvent(EventDefine.CustomScrollViewerRightClickEvent);
    }
    private ContextMenu CreateContextMenu()
    {
        ContextMenu contextMenu = new ContextMenu();

        MenuItem AddProto = new MenuItem { Header = "AddProto" };
        AddProto.Click += AddProto_Click;
        contextMenu.Items.Add(AddProto);

       

        return contextMenu;
    }

    private void AddProto_Click(object sender, RoutedEventArgs e)
    {
        EventManager.Instance.TriggerEvent(EventDefine.ProtoAddEvent);
    }
}