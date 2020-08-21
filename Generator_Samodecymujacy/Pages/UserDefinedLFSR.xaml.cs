using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace SummationGenerator
{
    /// <summary>
    /// Logika interakcji dla klasy UserDefinedLFSR.xaml
    /// </summary>
    public partial class UserDefinedLFSR : Page
    {
        private BitViewModel bitViewModel;
        private MainWindow mainWindow;
        private volatile bool startCalculatingLock;
        private volatile bool pauseCalculatingLock;
        private volatile bool stopCalculatingLock;
        private int calculatingStoppedAt;
        int cyclesUntilSavingCheck;
        public UserDefinedLFSR(MainWindow mainWindow)
        {
            bitViewModel = new BitViewModel();
            this.mainWindow = mainWindow;
            startCalculatingLock = false;
            pauseCalculatingLock = false;
            stopCalculatingLock = false;
            calculatingStoppedAt = 0;
            cyclesUntilSavingCheck = 0;
            InitializeComponent();
            this.DataContext = bitViewModel;
        }
        private void SetUpBackgroundWorker()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync(argument: startButton.Content);
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }
        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            userDefinedProgressBar.Value++;
        }
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            startCalculatingLock = true;
            int iterationsLeft = mainWindow.userDefinedLFSRIterations;
            if (calculatingStoppedAt > 0) iterationsLeft = calculatingStoppedAt;
            if (pauseCalculatingLock || stopCalculatingLock)
            {
                pauseCalculatingLock = false;
                stopCalculatingLock = false;
            }
            string startButtonContent = (string)e.Argument;
            int bitListCount = bitViewModel.Bits.Count;
            int sum;
            bool saveGeneratedValue = false;
            int counter = 0;
            StreamWriter tw = new StreamWriter(mainWindow.FileManagement.Files[0]);
            tw.Dispose();
            BinaryWriter Bw = new BinaryWriter(File.Open(mainWindow.FileManagement.Files[0], FileMode.OpenOrCreate));
            Bw.Dispose();
            if (startButtonContent.Equals("Resume"))
            {
                if (mainWindow.FileManagement.Extension.Equals(".txt"))
                    tw = File.AppendText(mainWindow.FileManagement.Files[0]);
                else
                    Bw = new BinaryWriter(File.Open(mainWindow.FileManagement.Files[0], FileMode.Append));
            }
            else
            {
                iterationsLeft = mainWindow.userDefinedLFSRIterations;
                if (mainWindow.FileManagement.Extension.Equals(".txt"))
                    tw = new StreamWriter(mainWindow.FileManagement.Files[0]);
                else
                    Bw = new BinaryWriter(File.Open(mainWindow.FileManagement.Files[0], FileMode.OpenOrCreate));
                this.Dispatcher.Invoke(() => { userDefinedProgressBar.Maximum = iterationsLeft; });
            }
            int progress = iterationsLeft;
            StringBuilder byteToSave = new StringBuilder();
            while (!pauseCalculatingLock && !stopCalculatingLock && iterationsLeft > 0)
            {
                if (saveGeneratedValue == true && cyclesUntilSavingCheck.Equals(0))
                {
                    if (mainWindow.FileManagement.Extension.Equals(".bin"))
                    {
                        if (byteToSave.Length == 0)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                Bw.Write(Convert.ToByte(byteToSave.ToString(), 2));
                            });
                            byteToSave.Clear();
                        }
                        else
                        {
                            byteToSave.Append(bitViewModel.Bits[bitListCount - 1].BitValue);
                        }
                    }
                    else
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            tw.Write(bitViewModel.Bits[bitListCount - 1].BitValue);
                        });
                    }
                    (sender as BackgroundWorker).ReportProgress(0);
                    iterationsLeft--;
                    saveGeneratedValue = false;
                }
                sum = 0;
                for (int i = 0; i < bitListCount; i++)
                {
                    if (bitViewModel.Bits[i].BitState.Equals("On") && bitViewModel.Bits[i].BitValue.Equals(1)) sum++;
                }
                sum = sum % 2;
                for (int i = bitListCount - 1; i > 0; i--)
                {
                    bitViewModel.Bits[i].BitValue = bitViewModel.Bits[i - 1].BitValue;
                }
                bitViewModel.Bits[0].BitValue = sum;
                if (cyclesUntilSavingCheck.Equals(0) && bitViewModel.Bits[bitListCount - 1].BitValue.Equals(0))
                {
                    cyclesUntilSavingCheck = mainWindow.newCyclesAtZero;
                }
                if (cyclesUntilSavingCheck.Equals(0) && bitViewModel.Bits[bitListCount - 1].BitValue.Equals(1))
                {
                    cyclesUntilSavingCheck = mainWindow.newCyclesAtOne;
                    counter++;
                    saveGeneratedValue = true;
                }
                cyclesUntilSavingCheck--;
            }
            if (iterationsLeft == 0)
            {
                this.Dispatcher.Invoke(() =>
                {
                    startButton.Content = "Start";
                    startButton.IsEnabled = true;
                    userDefinedProgressBar.Value = 0;
                });
            }
            else
            {
                calculatingStoppedAt = iterationsLeft;
            }
            tw.Close();
            Bw.Close();
            startCalculatingLock = false;
        }
        private void LoadLFSRConfiguration()
        {
            String states = stateStringTextBox.Text;
            String values = valueStringTextBox.Text;
            int n = values.Length;
            bitViewModel.Bits.Clear();
            String state;
            int value;
            for (int i = 0; i < n; i++)
            {
                if (states[i].Equals('1')) state = "On";
                else state = "Off";
                if (values[i].Equals('1')) value = 1;
                else value = 0;
                bitViewModel.Bits.Add(new Bit() { BitName = "Bit " + (i + 1), BitState = state, BitValue = value });
            }
        }
        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            int stateStringLength = stateStringTextBox.Text.Length;
            int valueStringLength = valueStringTextBox.Text.Length;
            if (stateStringLength != valueStringLength || stateStringLength == 0 || valueStringLength == 0)
            {
                MessageBox.Show("Value and state strings must be of equal length!" + (char)10
                    + "State string length: " + stateStringLength.ToString() + (char)10 + "Value string length: " + valueStringLength.ToString());
            }
            else if (!startCalculatingLock)
            {
                SetUpBackgroundWorker();
                if (startButton.Content.Equals("Start")) startButton.Content = "Resume";
                startButton.IsEnabled = false;
            }
        }
        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            startButton.Content = "Start";
            stopCalculatingLock = true;
            if (!stateStringTextBox.Text.Length.Equals(valueStringTextBox.Text.Length))
            {
                MessageBox.Show("Value and state strings must be of equal length!");
            }
            else LoadLFSRConfiguration();
            userDefinedProgressBar.Value = 0;
            startButton.IsEnabled = true;
        }

        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (!pauseCalculatingLock) pauseCalculatingLock = true;
            startButton.IsEnabled = true;
        }

        private void helpButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(mainWindow.FileManagement.LoadHelpFile("Generator.txt"));
        }
        private void stateAndValueStringTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((stateStringTextBox.Text.Length == 0 || valueStringTextBox.Text.Length == 0) && !stopCalculatingLock && !pauseCalculatingLock) startButton.IsEnabled = false;
            else
            {
                if (!startCalculatingLock)
                {
                    startButton.IsEnabled = true;
                    if (stateStringTextBox.Text.Length.Equals(valueStringTextBox.Text.Length)) LoadLFSRConfiguration();
                }
            }
        }

        private void loadConfigurationButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.FileManagement.LoadUserDefinedLFSRConfiguration(stateStringTextBox, valueStringTextBox);
        }
        private void saveConfigurationButton_Click(object sender, RoutedEventArgs e)
        {

            mainWindow.FileManagement.SaveUserDefinedLFSRConfiguration(stateStringTextBox, valueStringTextBox);
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
