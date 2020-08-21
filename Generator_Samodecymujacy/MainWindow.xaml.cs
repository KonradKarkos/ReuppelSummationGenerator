using SummationGenerator.Pages;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace SummationGenerator
{
    public partial class MainWindow : Window
    {
        public int newCyclesAtZero = 5;
        public int newCyclesAtOne = 10;
        public int _12bitLFSRIterations = 10000;
        public int userDefinedLFSRIterations = 10000;
        public FileManagement FileManagement = new FileManagement();
        private _12bitLFSR _12BitLFSR;
        private DecryptorPage DecryptorPage;
        private EncryptorPage EncryptorPage;
        private OptionsPage OptionsPage;
        private TestsPage TestsPage;
        private UserDefinedLFSR UserDefinedLFSR;
        public MainWindow()
        {
            InitializeComponent();
            _12BitLFSR = new _12bitLFSR(this);
            DecryptorPage = new DecryptorPage(this);
            EncryptorPage = new EncryptorPage(this);
            OptionsPage = new OptionsPage(this);
            TestsPage = new TestsPage(this);
            UserDefinedLFSR = new UserDefinedLFSR(this);
        }
        private void openMenuButton_Click(object sender, RoutedEventArgs e)
        {
            closeMenuButton.Visibility = Visibility.Visible;
            openMenuButton.Visibility = Visibility.Collapsed;
        }

        private void closeMenuButton_Click(object sender, RoutedEventArgs e)
        {
            closeMenuButton.Visibility = Visibility.Collapsed;
            openMenuButton.Visibility = Visibility.Visible;
        }

        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            switch (((ListViewItem)((ListView)sender).SelectedItem).Name)
            {
                case "_12BitLFSRListViewItem":
                    mainFrame.Content = _12BitLFSR;
                    break;
                case "userDefinedLFSRListViewItem":
                    mainFrame.Content = UserDefinedLFSR;
                    break;
                case "NISTEncryptorListViewItem":
                    mainFrame.Content = EncryptorPage;
                    break;
                case "NISTDecryptorListViewItem":
                    mainFrame.Content = DecryptorPage;
                    break;
                case "testsListViewItem":
                    mainFrame.Content = TestsPage;
                    break;
                case "optionsListViewItem":
                    mainFrame.Content = OptionsPage;
                    break;
                default:
                    break;
            }
        }

    }
}
