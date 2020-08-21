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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SummationGenerator.Pages
{
    /// <summary>
    /// Logika interakcji dla klasy DecryptorPage.xaml
    /// </summary>
    public partial class DecryptorPage : Page
    {
        private MainWindow mainWindow;
        public DecryptorPage(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            InitializeComponent();
        }
        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            startButton.IsEnabled = false;
            if (toDecryptTextBox.Text.Length % 8 > 0)
            {
                MessageBox.Show("Length of text to decrypt must be divisible by 8");
            }
            else
            {
                String textToDecrypt = toDecryptTextBox.Text;
                String key = keyTextBox.Text;
                if (key.Length < textToDecrypt.Length)
                {
                    MessageBox.Show("Key is shorter than text to decrypt so it will be copied till desired length.");
                    while (key.Length < textToDecrypt.Length)
                    {
                        key += key;
                    }
                    key = key.Substring(0, textToDecrypt.Length);
                    keyTextBox.Text = key;
                }
                StringBuilder XOR = new StringBuilder();
                int textLength = textToDecrypt.Length / 8;
                Byte[] decryptedCharacters = new Byte[textLength];
                for (int i = 0; i < textLength; i++)
                {
                    for (int j = i * 8; j < (i * 8) + 8; j++)
                    {
                        XOR.Append(textToDecrypt[j] ^ key[j]);
                    }
                    decryptedCharacters[i] = Convert.ToByte(XOR.ToString(), 2);
                    XOR.Clear();
                }
                decryptedTextBox.Text = Encoding.ASCII.GetString(decryptedCharacters);
            }
            startButton.IsEnabled = true;
        }
        private void toDecryptTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (toDecryptTextBox.Text.Length > 0 && keyTextBox.Text.Length > 0) startButton.IsEnabled = true;
            else startButton.IsEnabled = false;
        }
        private void helpButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(mainWindow.FileManagement.LoadHelpFile("NISTEncrypt.txt"));
        }

        private void loadConfigurationButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.FileManagement.LoadEncryptorFiles(7, 8, toDecryptTextBox, keyTextBox, true);
        }

        private void saveConfigurationButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.FileManagement.SaveEncryptorFiles(7, 8, 9, toDecryptTextBox, keyTextBox, decryptedTextBox);
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
