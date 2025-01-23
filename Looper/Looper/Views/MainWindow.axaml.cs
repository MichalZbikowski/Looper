using System.Text.RegularExpressions;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Looper.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var textBox1 = this.FindControl<TextBox>("TextBox1");
            textBox1.AddHandler(TextInputEvent, NumericTextBox_TextInput, RoutingStrategies.Tunnel);

            var textBox2 = this.FindControl<TextBox>("TextBox2");
            textBox2.AddHandler(TextInputEvent, NumericNegativeTextBox_TextInput, RoutingStrategies.Tunnel);
        }

        private void NumericTextBox_TextInput(object sender, TextInputEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void NumericNegativeTextBox_TextInput(object sender, TextInputEventArgs e)
        {
            e.Handled = !IsTextAllowedv2(e.Text);
        }

        private static bool IsTextAllowed(string text)
        {
            Regex regex = new("[^0-9]+"); // Regex that matches disallowed text
            return !regex.IsMatch(text);
        }

        private static bool IsTextAllowedv2(string text)
        {
            Regex regex = new(@"[^0-9-]+"); // Regex that matches disallowed text, including minus sign
            return !regex.IsMatch(text);
        }


    }
}