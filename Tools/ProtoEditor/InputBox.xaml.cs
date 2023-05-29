using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProtoEditor
{
    /// <summary>
    /// InputBox.xaml 的交互逻辑
    /// </summary>
    public partial class InputBox : Window
    {
        public string Input { get; private set; }

        public InputBox(string prompt)
        {
            InitializeComponent();
            promptTextBlock.Text = prompt;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Input = inputTextBox.Text;
            DialogResult = true;
        }
    }
}
