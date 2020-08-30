using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SummationGenerator.Pages
{
    public partial class EncryptorPage : Page
    {
        private MainWindow mainWindow;
        public EncryptorPage(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            InitializeComponent();
        }
        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            startButton.IsEnabled = false;
            String toEncrypt = toEncryptTextBox.Text;
            StringBuilder toEncryptInBytes = new StringBuilder();
            foreach (char c in toEncrypt)
            {
                toEncryptInBytes.Append(Convert.ToString(c, 2).PadLeft(8, '0'));
            }
            toEncrypt = toEncryptInBytes.ToString();
            String key = keyTextBox.Text;
            if (key.Length < toEncrypt.Length)
            {
                MessageBox.Show("Key is shorter than text to encrypt so it will be copied till desired length.");
                while (key.Length < toEncrypt.Length)
                {
                    key += key;
                }
                key = key.Substring(0, toEncrypt.Length);
                keyTextBox.Text = key;
            }
            toEncryptInBytes.Clear();
            for (int i = 0; i < toEncrypt.Length; i++)
            {
                toEncryptInBytes.Append(toEncrypt[i] ^ key[i]);
            }
            encryptedTextBox.Text = toEncryptInBytes.ToString();
            startButton.IsEnabled = true;
        }
        private void checkForProperStringsLength(object sender, TextChangedEventArgs e)
        {
            if (toEncryptTextBox.Text.Length > 0 && keyTextBox.Text.Length > 0) startButton.IsEnabled = true;
            else startButton.IsEnabled = false;
        }

        private void loadConfigurationButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.FileManagement.LoadEncryptorFiles(4, 5, toEncryptTextBox, keyTextBox, false);
        }
        private void saveConfigurationButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.FileManagement.SaveEncryptorFiles(4, 5, 6, toEncryptTextBox, keyTextBox, encryptedTextBox);
        }

        private void helpButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(mainWindow.FileManagement.LoadHelpFile("NISTEncrypt.txt"));
        }
        private void FilterSpaces(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }
        private void AllowOnlyZeroAndOne(object sender, TextCompositionEventArgs e)
        {
            int output;
            if (int.TryParse(e.Text, out output) == false)
            {
                e.Handled = true;
            }
            else
            {
                if (e.Text != "0" && e.Text != "1")
                {
                    e.Handled = true;
                }
            }
        }
    }
}
