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
using System.Windows.Threading;

namespace SummationGenerator.Pages
{
    public partial class _12bitLFSR : Page
    {
        private BitViewModel bitViewModel;
        private MainWindow mainWindow;
        private volatile bool startCalculatingLock;
        private volatile bool pauseCalculatingLock;
        private volatile bool stopCalculatingLock;
        private int calculatingStoppedAt;
        int cyclesUntilSavingCheck;
        public _12bitLFSR(MainWindow mainWindow)
        {
            bitViewModel = new BitViewModel();
            for (int i = 0; i < 12; i++)
            {
                bitViewModel.Bits.Add(new Bit() { BitName = "Bit " + (i + 1), BitState = "On", BitValue = 1 });
            }
            this.mainWindow = mainWindow;
            startCalculatingLock = false;
            pauseCalculatingLock = false;
            stopCalculatingLock = false;
            calculatingStoppedAt = 0;
            mainWindow._12bitLFSRIterations = 10000;
            mainWindow.newCyclesAtOne = 10;
            mainWindow.newCyclesAtZero = 5;
            cyclesUntilSavingCheck = 0;
            InitializeComponent();
            this.DataContext = bitViewModel;
            
        }
        private void radioButtonOnOff_Checked(object sender, RoutedEventArgs e) 
        {
            RadioButton r = (RadioButton)sender;
            bitViewModel.Bits[Int32.Parse(r.Name.Split('_')[1])].BitState = r.Content.ToString();
        }
        private void radioButtonValue_Checked(object sender, RoutedEventArgs e) 
        {
            RadioButton r = (RadioButton)sender;
            bitViewModel.Bits[Int32.Parse(r.Name.Split('_')[1])].BitValue = Int32.Parse(r.Content.ToString());
        }

        private void start_Click(object sender, RoutedEventArgs e)
        {
            if (!startCalculatingLock)
            {
                SetUpBackgroundWorker();
                if (startButton.Content.Equals("Start")) startButton.Content = "Resume";
                startButton.IsEnabled = false;
            }
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

        private void stop_Click(object sender, RoutedEventArgs e)
        {
            stopCalculatingLock = true;
            startButton.Content = "Start";
            for (int i = 0; i < bitViewModel.Bits.Count; i++)
            {
                bitViewModel.Bits[i].BitState = "On";
                bitViewModel.Bits[i].BitValue = 1;
            }
            startButton.IsEnabled = true;
            autoDefinedProgressBar.Value = 0;
        }

        private void pause_Click(object sender, RoutedEventArgs e)
        {
            if (!pauseCalculatingLock) pauseCalculatingLock = true;
            startButton.IsEnabled = true;
        }

        private void help_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(mainWindow.FileManagement.LoadHelpFile("Generator.txt"));
        }
        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            autoDefinedProgressBar.Value++;
        }
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            startCalculatingLock = true;
            int iterationsLeft = mainWindow._12bitLFSRIterations;
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
                iterationsLeft = mainWindow._12bitLFSRIterations;
                if (mainWindow.FileManagement.Extension.Equals(".txt"))
                    tw = new StreamWriter(mainWindow.FileManagement.Files[0]);
                else
                    Bw = new BinaryWriter(File.Open(mainWindow.FileManagement.Files[0], FileMode.OpenOrCreate));
                this.Dispatcher.Invoke(() => { autoDefinedProgressBar.Maximum = iterationsLeft; });
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
                    autoDefinedProgressBar.Value = 0;
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
    }
}
