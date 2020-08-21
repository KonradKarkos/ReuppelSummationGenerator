using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace SummationGenerator
{
    //Głównym celem utworzenia tej klasy było uporządkowanie kodu i uczynienie go bardziej czytelnym
    public class FileManagement
    {
        public String Extension { get; set; } = ".txt";
        public String[] Files { get; set; } = { "12BitLFSRGeneratedCode.txt", "UserGeneratedCode.txt", "OnOffConf.txt", "ValuesConf.txt", "Plain.txt", "EncryptKey.txt", "EncryptedText.txt", "EncryptedText.txt", "DecryptKey.txt", "Decrypted.txt" };
        public FileManagement()
        {
            char c = (char)10;
            Directory.CreateDirectory("Help");
            if (!File.Exists("Help\\Generator.txt"))
            {
                StreamWriter sw = new StreamWriter("Help\\Generator.txt");
                sw.Write("The summation generator relies on the LFSR register, which always outputs the last register bit, simultaneously converting the bit values to the values of the preceding bits. The value of the first bit is changed to modulo 2 from the sum of the values of the activated bits. If value of the last bit was 0, the program waits 5 cycles by default before checking the next value. In case of giving 1 to the output, the program writes the value from the output to the file by default after 10 cycles." + c +
                "On \"12 bit LFSR\" page you can quickly operate on 12 bit LFSR using simple interface." + c +
                "On \"User defined LFSR\" you can create LFSR of unlimited size using equal-length strings of 0 and 1 in both textboxes. All other characters are blocked." + c +
                "'Stop' buttons on \"12 bit LFSR\" page actvates all bits and sets their value on 1. On \"User defined LFSR\" page loads again configuration from textboxes."+c);
                sw.Dispose();
            }
            if (!File.Exists("Help\\NISTEncrypt.txt"))
            {
                StreamWriter sw = new StreamWriter("Help\\NISTEncrypt.txt");
                sw.Write("Page \"NIST encryptor\" encrypts plain text with key string build from 0 and 1 (stream encoder) performing exclusive or on coresponding bits. Resulting stream is also saved as 0 and 1." + c +
                "Page \"NIST decryptor\" supports decryption of 0/1 string using exclusive or from the given string of 0/1 and key. Both the encoder and the decryptor support only ASCII compliant characters");
                sw.Dispose();
            }
            if (!File.Exists("Help\\Tests.txt"))
            {
                StreamWriter sw = new StreamWriter("Help\\Tests.txt");
                sw.Write("On \"Tests\" page after choosing file and pressing \"Start\" the selected file will be divided into sequences of 20,000 characters long (or 2,500 characters long if it is a binary file) on which tests according to the FIPS 140-2 standard will be performed." + c+
                    "Monobit test checks the number of ones and zeros in each string, if any of these components is more than 51.375% of the string length the test will fail."+c+
                    "The poker test is based on checking the number of 4-bit digits in a sequence. It is done by dividing the string into 5,000 4-bit strings, converting them to decimal numbers, and counting the number of occurrences of each one. The poker test factor X is calculated from the formula X = (16/5000) * (SUM i = 0 -> i = 15 [f (i)] ^ 2) - 5000, where f (i) is the number of occurrences of consecutive numbers. If the X factor is not within the range (2.16, 46.17), the test fails." + c+
                    "The runs test counts the number of sequences of zeros and ones with lengths 1,2,3,4,5 and 6+, which must fall within the following ranges: " + c+
                    "1  2315 - 2685"+c+
                    "2  1114 - 1386" + c+
                    "3  527 - 723" + c+
                    "4  240 - 384" + c+
                    "5  103 - 209" + c+
                    "6+ 103 - 209"+c+
                    "In other case test will fail."+c+
                    "The long-run test will fail when there is at least one sequence of 0s and 1s with a length greater than or equal to 23.");
                sw.Dispose();
            }
        }
        public String AddExtension(TextBox fileNameTextBox, bool isResultFile)
        {
            String fileName = fileNameTextBox.Text;
            String fileNameEnding;
            if (fileName.Length > 4)
            {
                fileNameEnding = fileName.Substring(fileName.Length - 4);
            }
            else
            {
                fileNameEnding = fileName;
            }
            if (!fileNameEnding.Equals(".txt") && !isResultFile)
            {
                fileName += ".txt";
            }
            else if (!fileNameEnding.Equals(Extension) && isResultFile)
            {
                if (fileNameEnding.Equals(".txt") || fileNameEnding.Equals(".bin")) fileName = fileName.Substring(0, fileName.Length - 4);
                fileName += Extension;
            }
            fileNameTextBox.Text = fileName;
            return fileName;
        }
        public void ChooseFile(TextBox fileNameTextBox)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text Files (*.txt)|*.txt|Binary Files (*.bin)|*.bin";
            if (openFileDialog.ShowDialog() == true)
            {
                fileNameTextBox.Text = openFileDialog.FileName;
            }
        }
        public void LoadEncryptorFiles(int toProcessFileIndex, int keyFileIndex, TextBox toProcessTextBox, TextBox keyTextBox, bool decryptor)
        {
            if (File.Exists(Files[toProcessFileIndex]) && File.Exists(Files[keyFileIndex]))
            {
                StringBuilder sb = new StringBuilder();
                if (!decryptor || (decryptor && Extension.Equals(".txt")))
                {
                    StreamReader s = new StreamReader(Files[toProcessFileIndex]);
                    String line;
                    while ((line = s.ReadLine()) != null)
                    {
                        sb.Append(line);
                    }
                    toProcessTextBox.Text = sb.ToString();
                    sb.Clear();
                    s.Dispose();
                }
                else
                {
                    BinaryReader br = new BinaryReader(File.Open(Files[toProcessFileIndex], FileMode.Open));
                    int fileLength = (int)br.BaseStream.Length;
                    while (fileLength > 0)
                    {
                        sb.Append(Convert.ToString(br.ReadByte(), 2));
                        fileLength--;
                    }
                    toProcessTextBox.Text = sb.ToString();
                    br.Dispose();
                }
                if (Extension.Equals(".txt"))
                {
                    StreamReader s = new StreamReader(Files[keyFileIndex]);
                    String linia;
                    while ((linia = s.ReadLine()) != null)
                    {
                        foreach (char c in linia)
                        {
                            if (c.Equals('0') || c.Equals('1'))
                            {
                                sb.Append(c);
                            }
                        }
                    }
                    s.Dispose();
                    keyTextBox.Text = sb.ToString();
                }
                else
                {
                    BinaryReader br = new BinaryReader(File.Open(Files[keyFileIndex], FileMode.Open));
                    int dlugosc = (int)br.BaseStream.Length;
                    while (dlugosc > 0)
                    {
                        sb.Append(Convert.ToString(br.ReadByte(), 2));
                        dlugosc--;
                    }
                    keyTextBox.Text = sb.ToString();
                    br.Dispose();
                }
            }
            else
            {
                MessageBox.Show("Configuration loading stopped - at least one of configuration files doesn't exist.");
            }
        }
        public void SaveEncryptorFiles(int toProcessFileIndex, int keyFileIndex, int resultFileIndex, TextBox toProcessTextBox, TextBox keyTextBox, TextBox resultTextBox)
        {
            StreamWriter streamWriter = new StreamWriter(Files[toProcessFileIndex]);
            if (toProcessTextBox.LineCount > 0)
            {
                for (int i = 0; i < toProcessTextBox.LineCount; i++)
                {
                    streamWriter.Write(toProcessTextBox.GetLineText(i));
                }
            }
            streamWriter.Dispose();
            if (Extension.Equals(".txt"))
            {
                streamWriter = new StreamWriter(Files[keyFileIndex]);
                if (keyTextBox.LineCount > 0)
                {
                    for (int i = 0; i < keyTextBox.LineCount; i++)
                    {
                        streamWriter.Write(keyTextBox.GetLineText(i));
                    }
                }
                streamWriter.Dispose();
                streamWriter = new StreamWriter(Files[resultFileIndex]);
                if (resultTextBox.LineCount > 0)
                {
                    for (int i = 0; i < resultTextBox.LineCount; i++)
                    {
                        streamWriter.Write(resultTextBox.GetLineText(i));
                    }
                }
                streamWriter.Dispose();
            }
            else
            {
                int byteCount = keyTextBox.Text.Length / 8;
                Byte[] bytesRead = new Byte[byteCount];
                for (int i = 0; i < byteCount; i++)
                {
                    bytesRead[i] = Convert.ToByte(keyTextBox.Text.Substring(8 * i, 8), 2);
                }
                File.WriteAllBytes(Files[keyFileIndex], bytesRead);
                byteCount = resultTextBox.Text.Length / 8;
                bytesRead = new Byte[byteCount];
                for (int i = 0; i < byteCount; i++)
                {
                    bytesRead[i] = Convert.ToByte(resultTextBox.Text.Substring(8 * i, 8), 2);
                }
                File.WriteAllBytes(Files[resultFileIndex], bytesRead);
            }
        }
        public void LoadUserDefinedLFSRConfiguration(TextBox stateTextBox, TextBox valueTextBox)
        {
            if (File.Exists(Files[2]) && File.Exists(Files[3]))
            {
                StreamReader streamReader = new StreamReader(Files[2]);
                String line;
                StringBuilder stringBuilder = new StringBuilder();
                while ((line = streamReader.ReadLine()) != null)
                {
                    foreach (char c in line)
                    {
                        if (c.Equals('0') || c.Equals('1'))
                        {
                            stringBuilder.Append(c);
                        }
                    }
                }
                stateTextBox.Text = stringBuilder.ToString();
                stringBuilder.Clear();
                streamReader.Dispose();
                streamReader = new StreamReader(Files[3]);
                while ((line = streamReader.ReadLine()) != null)
                {
                    foreach (char c in line)
                    {
                        if (c.Equals('0') || c.Equals('1'))
                        {
                            stringBuilder.Append(c);
                        }
                    }
                }
                streamReader.Dispose();
                valueTextBox.Text = stringBuilder.ToString();
            }
            else
            {
                MessageBox.Show("Configuration loading stopped - at least one of configuration files doesn't exist.");
            }
        }
        public void SaveUserDefinedLFSRConfiguration(TextBox stateTextBox, TextBox valueTextBox)
        {

            StreamWriter streamWriter = new StreamWriter(Files[2]);
            if (stateTextBox.LineCount > 0)
            {
                for (int i = 0; i < stateTextBox.LineCount; i++)
                {
                    streamWriter.Write(stateTextBox.GetLineText(i));
                }
            }
            streamWriter.Dispose();
            streamWriter = new StreamWriter(Files[3]);
            if (valueTextBox.LineCount > 0)
            {
                for (int i = 0; i < valueTextBox.LineCount; i++)
                {
                    streamWriter.Write(valueTextBox.GetLineText(i));
                }
            }
            streamWriter.Dispose();
        }
        public String LoadHelpFile(String fileName)
        {
            char c = (char)10;
            StringBuilder stringBuilder = new StringBuilder();
            StreamReader streamReader = new StreamReader("Help\\"+fileName);
            String line;
            while ((line = streamReader.ReadLine()) != null)
            {
                stringBuilder.Append(line + c);
            }
            return stringBuilder.ToString();
        }
    }
}
