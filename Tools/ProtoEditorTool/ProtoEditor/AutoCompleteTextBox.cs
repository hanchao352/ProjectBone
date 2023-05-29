using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

    public partial class AutoCompleteTextBox : UserControl
    {
    private readonly TextBox _textBox;
    private readonly ListBox _suggestionListBox;
    private IEnumerable<string> _autoCompleteItems;

    public AutoCompleteTextBox()
    {
        _textBox = new TextBox
        {
            Dock = DockStyle.Top,
        };
        _suggestionListBox = new ListBox
        {
            Visible = false,
            Dock = DockStyle.Fill,
        };

        _textBox.TextChanged += OnTextChanged;
        _textBox.KeyDown += OnTextBoxKeyDown;
        _suggestionListBox.MouseDoubleClick += OnSuggestionListBoxMouseDoubleClick;

        Controls.Add(_suggestionListBox);
        Controls.Add(_textBox);
    }

    private void OnTextChanged(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(_textBox.Text) && _autoCompleteItems != null)
        {
            var suggestions = _autoCompleteItems.Where(item => item.StartsWith(_textBox.Text, StringComparison.InvariantCultureIgnoreCase)).ToList();

            if (suggestions.Count > 0)
            {
                _suggestionListBox.DataSource = suggestions;
                _suggestionListBox.Visible = true;
                return;
            }
        }

        _suggestionListBox.Visible = false;
    }

    private void OnTextBoxKeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Down && _suggestionListBox.Visible)
        {
            _suggestionListBox.Focus();
        }
    }

    private void OnSuggestionListBoxMouseDoubleClick(object sender, MouseEventArgs e)
    {
        SelectSuggestion();
    }

    private void SelectSuggestion()
    {
        if (_suggestionListBox.SelectedItem != null)
        {
            _textBox.Text = _suggestionListBox.SelectedItem.ToString();
            _textBox.SelectionStart = _textBox.Text.Length;
            _textBox.Focus();
            _suggestionListBox.Visible = false;
        }
    }

    public void SetAutoCompleteItems(IEnumerable<string> items)
    {
        _autoCompleteItems = items;
    }
}

