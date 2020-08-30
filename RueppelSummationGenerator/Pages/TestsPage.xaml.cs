using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SummationGenerator.Pages
{
    public partial class TestsPage : Page
    {
        private bool textFile = true;
        private MainWindow mainWindow;
        public TestsPage(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            InitializeComponent();
        }
        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            startButton.IsEnabled = false;
            String fileToTest = testFilePathTextBox.Text;
            bool isTextFile = textFile;
            if (!File.Exists(fileToTest))
            {
                MessageBox.Show("File does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                FileInfo f = new FileInfo(fileToTest);
                if ((f.Length % 20000 != 0 && isTextFile == true) || ((f.Length * 8) % 20000 != 0 && isTextFile == false))
                {
                    MessageBox.Show("File content length must be divisible by 20000!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    StreamWriter streamWriter = new StreamWriter("TestResults.txt");
                    longRunProgressBar.Value = 0;
                    monoBitProgressBar.Value = 0;
                    runsProgressBar.Value = 0;
                    pokerProgressBar.Value = 0;
                    longRunProgressBar.Visibility = Visibility.Visible;
                    monoBitProgressBar.Visibility = Visibility.Visible;
                    runsProgressBar.Visibility = Visibility.Visible;
                    pokerProgressBar.Visibility = Visibility.Visible;
                    StreamReader streamReader = new StreamReader(fileToTest);
                    streamReader.Dispose();
                    BinaryReader binaryReader = new BinaryReader(File.Open(fileToTest, FileMode.Open));
                    binaryReader.Dispose();
                    int stringLength = (int)f.Length;
                    longRunProgressBar.Maximum = stringLength / 20000;
                    monoBitProgressBar.Maximum = stringLength / 20000;
                    runsProgressBar.Maximum = stringLength / 20000;
                    pokerProgressBar.Maximum = stringLength / 20000;
                    char[] currentlyTestedCharacters = new char[20000];
                    StringBuilder stringBuilder = new StringBuilder();
                    int onesCounter;
                    int[] pokerTestCounterArray = new int[16];
                    int runIndex = 0;
                    bool[] testResults = { true, true, true, true };
                    char runsCheckCharacter;
                    int runLength = 0;
                    int indeks;
                    int[] runsOf0 = new int[6];
                    int[] runsOf1 = new int[6];
                    int[] pokerTestedBits = new int[4];
                    for (int i = 0; i < stringLength; i += 20000)
                    {
                        runIndex = 0;
                        runLength = 0;
                        if (!testResults[0] && !testResults[1] && !testResults[2] && !testResults[3]) break;
                        for (int u = 0; u < 16; u++)
                        {
                            pokerTestCounterArray[u] = 0;
                        }
                        for (int y = 0; y < 6; y++)
                        {
                            runsOf0[y] = 0;
                            runsOf1[y] = 0;
                        }
                        onesCounter = 0;
                        if (isTextFile)
                        {
                            streamReader = new StreamReader(fileToTest);
                            streamReader.Read(currentlyTestedCharacters, 0, 20000);
                        }
                        else
                        {
                            binaryReader = new BinaryReader(File.Open(fileToTest, FileMode.Open));
                            for (int z = 0; z < 2500; z++)
                            {
                                stringBuilder.Append(Convert.ToString(binaryReader.ReadByte(), 2).PadLeft(8, '0'));
                            }
                            currentlyTestedCharacters = stringBuilder.ToString().ToCharArray();
                            stringBuilder.Clear();
                        }
                        if (testResults[0] || testResults[1])
                            for (int j = 1; j <= 20000; j++)
                            {
                                if (testResults[0] && currentlyTestedCharacters[j - 1].Equals('1')) onesCounter++;
                                if (testResults[1] && j % 4 == 0 && j != 0)
                                {
                                    pokerTestedBits[3] = currentlyTestedCharacters[j - 4] - '0';
                                    pokerTestedBits[2] = currentlyTestedCharacters[j - 3] - '0';
                                    pokerTestedBits[1] = currentlyTestedCharacters[j - 2] - '0';
                                    pokerTestedBits[0] = currentlyTestedCharacters[j - 1] - '0';
                                    indeks = (int)Math.Pow(2, 3) * pokerTestedBits[3] + (int)Math.Pow(2, 2) * pokerTestedBits[2] + (2 * pokerTestedBits[1]) + pokerTestedBits[0];
                                    pokerTestCounterArray[indeks]++;
                                }
                            }
                        if (testResults[0]) monoBitProgressBar.Value++;
                        if (testResults[1]) pokerProgressBar.Value++;
                        if (testResults[2] || testResults[3])
                            while (runIndex < 20000)
                            {
                                runsCheckCharacter = currentlyTestedCharacters[runIndex];
                                while (runIndex < 20000 && currentlyTestedCharacters[runIndex].Equals(runsCheckCharacter))
                                {
                                    runLength++;
                                    runIndex++;
                                }
                                if (testResults[2])
                                {
                                    if (runLength >= 6 && runsCheckCharacter.Equals('0')) runsOf0[5]++;
                                    else if (runLength >= 6 && runsCheckCharacter.Equals('1')) runsOf1[5]++;
                                    else
                                    {
                                        if (runsCheckCharacter.Equals('0')) runsOf0[runLength - 1]++;
                                        else runsOf1[runLength - 1]++;
                                    }
                                }
                                if (runLength >= 26 && testResults[3])
                                {
                                    streamWriter.WriteLine("Long-run test has been failed. Found run of \"" + runsCheckCharacter + "\" with length " + runLength + ", it ends at " + (i + runIndex - 1) + " index");
                                    testResults[3] = false;
                                    longRunResultTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                                    longRunResultTextBlock.Text = "Failed!";
                                }
                                runLength = 0;
                            }
                        if (testResults[2]) runsProgressBar.Value++;
                        if (testResults[3]) longRunProgressBar.Value++;
                        if (testResults[0])
                        {
                            if (onesCounter > 10275 || onesCounter < 9725)
                            {
                                streamWriter.WriteLine("Monobit test failed. In part beginning at " + i + " index number of ones is " + onesCounter + ", which leaves us with " + (20000 - onesCounter) + " zeros.");
                                testResults[0] = false;
                                monoBitResultTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                                monoBitResultTextBlock.Text = "Failed!";
                            }
                        }
                        if (testResults[1])
                        {
                            ///X=(16/5000)*(SUM i=0 -> i=15 [f(i)]^2)-5000
                            double sum = 0.0;
                            for (int y = 0; y < 16; y++)
                            {
                                sum = sum + pokerTestCounterArray[y] * pokerTestCounterArray[y];
                            }
                            double X = 16.0 / 5000.0 * sum - 5000.0;
                            if (X < 2.16 || X > 46.17)
                            {
                                streamWriter.WriteLine("Poker test failed. The X factor calculated from part beginning at " + i + " index equals " + X);
                                testResults[1] = false;
                                pokerResultTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                                pokerResultTextBlock.Text = "Failed!";
                            }
                        }
                        if (testResults[2])
                        {
                            if ((runsOf0[0] < 2315 || runsOf0[0] > 2685)
                                ||
                                (runsOf0[1] < 1114 || runsOf0[1] > 1386)
                                ||
                                (runsOf0[2] < 527 || runsOf0[2] > 723)
                                ||
                                (runsOf0[3] < 240 || runsOf0[3] > 384)
                                ||
                                (runsOf0[4] < 103 || runsOf0[4] > 209)
                                ||
                                (runsOf0[5] < 103 || runsOf0[5] > 209)
                                ||
                                (runsOf1[0] < 2315 || runsOf1[0] > 2685)
                                ||
                                (runsOf1[1] < 1114 || runsOf1[1] > 1386)
                                ||
                                (runsOf1[2] < 527 || runsOf1[2] > 723)
                                ||
                                (runsOf1[3] < 240 || runsOf1[3] > 384)
                                ||
                                (runsOf1[4] < 103 || runsOf1[4] > 209)
                                ||
                                (runsOf1[5] < 103 || runsOf1[5] > 209))
                            {
                                streamWriter.WriteLine("Runs test has been failed. Runs of ceratin length has been found:");
                                streamWriter.WriteLine("Runs of 0:");
                                streamWriter.WriteLine("1 - " + runsOf0[0]);
                                streamWriter.WriteLine("2 - " + runsOf0[1]);
                                streamWriter.WriteLine("3 - " + runsOf0[2]);
                                streamWriter.WriteLine("4 - " + runsOf0[3]);
                                streamWriter.WriteLine("5 - " + runsOf0[4]);
                                streamWriter.WriteLine("6+ - " + runsOf0[5]);
                                streamWriter.WriteLine("Runs of 1:");
                                streamWriter.WriteLine("1 - " + runsOf1[0]);
                                streamWriter.WriteLine("2 - " + runsOf1[1]);
                                streamWriter.WriteLine("3 - " + runsOf1[2]);
                                streamWriter.WriteLine("4 - " + runsOf1[3]);
                                streamWriter.WriteLine("5 - " + runsOf1[4]);
                                streamWriter.WriteLine("6 - " + runsOf1[5]);
                                testResults[2] = false;
                                runsResultTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                                runsResultTextBlock.Text = "Failed!";
                            }
                        }
                    }
                    if (testResults[0])
                    {
                        streamWriter.WriteLine("MonoBit test passed.");
                        monoBitResultTextBlock.Foreground = new SolidColorBrush(Colors.Green);
                        monoBitResultTextBlock.Text = "Passed!";
                    }
                    if (testResults[1])
                    {
                        streamWriter.WriteLine("Poker test passe.");
                        pokerResultTextBlock.Foreground = new SolidColorBrush(Colors.Green);
                        pokerResultTextBlock.Text = "Passed!";
                    }
                    if (testResults[2])
                    {
                        streamWriter.WriteLine("Runs test passed.");
                        runsResultTextBlock.Foreground = new SolidColorBrush(Colors.Green);
                        runsResultTextBlock.Text = "Passed!";
                    }
                    if (testResults[3])
                    {
                        streamWriter.WriteLine("Long-run test passed.");
                        longRunResultTextBlock.Foreground = new SolidColorBrush(Colors.Green);
                        longRunResultTextBlock.Text = "Passed!";
                    }
                    streamWriter.Dispose();
                    binaryReader.Dispose();
                }
            }
            startButton.IsEnabled = true;

        }
        private void filePathLength_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (testFilePathTextBox.Text.Length > 0) startButton.IsEnabled = true;
            else startButton.IsEnabled = false;
        }
        private void helpButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(mainWindow.FileManagement.LoadHelpFile("Tests.txt"));
        }
        private void ASCIIRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            textFile = true;
        }

        private void bitRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            textFile = false;
        }

        private void selectFileButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.FileManagement.ChooseFile(testFilePathTextBox);
        }
    }
}
