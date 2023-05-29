using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApp
{
    public partial class CustomWindow : Window
    {
        public CustomWindow()
        {
            InitializeComponent();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            // 在此处理确定按钮点击事件
            MessageBox.Show("确定按钮被点击");
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // 在此处理取消按钮点击事件
            MessageBox.Show("取消按钮被点击");
        }
    }
}