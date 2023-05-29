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
    [ToolboxItem(true)]
    public partial class CustomTreeView : TreeView
    {
        public CustomTreeView()
        {
            // 设置 HideSelection 为 false 以保持选中项高亮
            this.HideSelection = false;
        }

        protected override void OnLostFocus(EventArgs e)
        {
            // 当失去焦点时，强制重新绘制控件
            base.OnLostFocus(e);
            this.Invalidate();
        }

        protected override void OnGotFocus(EventArgs e)
        {
            // 当获得焦点时，强制重新绘制控件
            base.OnGotFocus(e);
            this.Invalidate();
        }
    }
}
