using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SummationGenerator.Pages
{
    public partial class OptionsPage : Page
    {
        private MainWindow mainWindow;
        public OptionsPage(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            InitializeComponent();
        }

        private void applyChangesButton_Click(object sender, RoutedEventArgs e)
        {
            if (userDefinedLFSRBitStateFileTextBox.Text.Length == 0 
                || userDefinedLFSRBitValueFileTextBox.Text.Length == 0 
                || _12BitLFSRGeneratedCodeFileTextBox.Text.Length == 0 
                || userDefinedLFSRGeneratedCodeFileTextBox.Text.Length == 0 
                || userDefinedGeneratedCodeLengthTextBox.Text.Length == 0 
                || _12BitLFSRGeneratedCodeLengthTextBox.Text.Length == 0 
                || newCyclesAtOneTextBox.Text.Length == 0 
                || newCyclesAtZeroTextBox.Text.Length == 0 
                || plainTextFilePathTextBox.Text.Length == 0 
                || encryptKeyFilePathTextBox.Text.Length == 0 
                || encryptedTextFilePathTextBox.Text.Length == 0 
                || toDecryptTextFilePathTextBox.Text.Length == 0 
                || decryptKeyFilePathTextBox.Text.Length == 0 
                || decryptKeyFilePathTextBox.Text.Length == 0)
            {
                MessageBox.Show("One or more options are empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                ComboBoxItem typeItem = (ComboBoxItem)saveAndLoadFilesExtensionComboBox.SelectedItem;
                mainWindow.FileManagement.Extension = typeItem.Content.ToString();
                mainWindow.FileManagement.Files[0] = mainWindow.FileManagement.AddExtension(_12BitLFSRGeneratedCodeFileTextBox, true);
                mainWindow.FileManagement.Files[1] = mainWindow.FileManagement.AddExtension(userDefinedLFSRGeneratedCodeFileTextBox, true);
                mainWindow.FileManagement.Files[2] = mainWindow.FileManagement.AddExtension(userDefinedLFSRBitStateFileTextBox, false);
                mainWindow.FileManagement.Files[3] = mainWindow.FileManagement.AddExtension(userDefinedLFSRBitValueFileTextBox, false);
                mainWindow.FileManagement.Files[4] = mainWindow.FileManagement.AddExtension(plainTextFilePathTextBox, false);
                mainWindow.FileManagement.Files[5] = mainWindow.FileManagement.AddExtension(encryptKeyFilePathTextBox, true);
                mainWindow.FileManagement.Files[6] = mainWindow.FileManagement.AddExtension(encryptedTextFilePathTextBox, true);
                mainWindow.FileManagement.Files[7] = mainWindow.FileManagement.AddExtension(toDecryptTextFilePathTextBox, true);
                mainWindow.FileManagement.Files[8] = mainWindow.FileManagement.AddExtension(decryptKeyFilePathTextBox, true);
                mainWindow.FileManagement.Files[9] = mainWindow.FileManagement.AddExtension(decryptKeyFilePathTextBox, false);
                mainWindow.NewCyclesAtOne = Int32.Parse(newCyclesAtOneTextBox.Text);
                mainWindow.NewCyclesAtZero = Int32.Parse(newCyclesAtZeroTextBox.Text);
                mainWindow.UserDefinedLFSRIterations = Int32.Parse(userDefinedGeneratedCodeLengthTextBox.Text);
                mainWindow._12bitLFSRIterations = Int32.Parse(_12BitLFSRGeneratedCodeLengthTextBox.Text);
            }
        }

        private void selectUserDefinedLFSRBitStateFileButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.FileManagement.ChooseFile(userDefinedLFSRBitStateFileTextBox);
        }

        private void selectUserDefinedLFSRBitValueFileButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.FileManagement.ChooseFile(userDefinedLFSRBitValueFileTextBox);
        }

        private void selectUserDefinedLFSRGeneratedCodeFileButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.FileManagement.ChooseFile(userDefinedLFSRGeneratedCodeFileTextBox);
        }
        private void nextOptionsPageButton_Click(object sender, RoutedEventArgs e)
        {
            firstPageGrid.Visibility = Visibility.Hidden;
            secondPageGrid.Visibility = Visibility.Visible;
        }
        private void previousOptionsPageButton_Click(object sender, RoutedEventArgs e)
        {
            secondPageGrid.Visibility = Visibility.Hidden;
            firstPageGrid.Visibility = Visibility.Visible;
        }

        private void selectPlainTextFilePathButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.FileManagement.ChooseFile(plainTextFilePathTextBox);
        }

        private void encryptKeyFilePathButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.FileManagement.ChooseFile(encryptKeyFilePathTextBox);
        }

        private void encryptedTextFilePathButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.FileManagement.ChooseFile(encryptedTextFilePathTextBox);
        }

        private void toDecryptTextFilePathButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.FileManagement.ChooseFile(toDecryptTextFilePathTextBox);
        }

        private void decryptKeyFilePathButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.FileManagement.ChooseFile(decryptKeyFilePathTextBox);
        }

        private void decryptedTextFilePathTextButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.FileManagement.ChooseFile(decryptKeyFilePathTextBox);
        }
        private void select12BitLFSRGeneratedCodeFileButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.FileManagement.ChooseFile(_12BitLFSRGeneratedCodeFileTextBox);
        }

        private void AllowOnlyDigits(object sender, TextCompositionEventArgs e)
        {
            int output;
            if (!int.TryParse(e.Text, out output))
            {
                e.Handled = true;
            }
        }
        private void FilterSpaces(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }
        private void FilterForbiddenCharacters(object sender, TextCompositionEventArgs e)
        {
            String[] forbiddenCharacters = { ">", "<", "/", "\"", "\\", "*", "?", ":", "|" };
            for (int i = 0; i < 9; i++)
            {
                if (e.Text.Equals(forbiddenCharacters[i]))
                {
                    e.Handled = true;
                    break;
                }
            }
        }
    }
}
