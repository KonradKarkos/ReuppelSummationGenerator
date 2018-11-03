using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Generator_Samodecymujacy
{
    public class Bit
    {
        public String Bity { get; set; }
        public String Stan { get; set; }
        public int Value { get; set; }
    }
    public partial class MainWindow : Window
    {
        Bit[] bity = new Bit[12];
        bool[] locki = new bool[5];
        static String plik = "Kod.txt";
        Bit[] tabl = new Bit[1];
        public MainWindow()
        {
            tabl[0] = new Bit() { Bity = "Pocz", Stan = "Off", Value = 0 };
            if (!File.Exists(plik))
            {
                File.Create(plik);
            }
            for(int i=0;i<4;i++)
            {
                locki[i] = false;
            }
            for (int i = 0; i < 12; i++)
            bity[i] = new Bit() { Bity = "Bit " + (i+1), Stan = "On", Value = 1 };
            InitializeComponent();
            LFSR.ItemsSource = bity;
        }
        //metoda ustawiająca status bitu na On
        private void on(int indeks)
        {
            bity[indeks].Stan = "On";
            if (LFSR != null) LFSR.Items.Refresh();
        }
        //metoda ustawiająca status bitu na Off
        private void off(int indeks)
        {
            bity[indeks].Stan = "Off";
            if (LFSR != null) LFSR.Items.Refresh();
        }
        //metoda ustawiająca watrtość bitu na 0
        private void zero(int indeks)
        {
            bity[indeks].Value = 0;
            if (LFSR != null) LFSR.Items.Refresh();
        }
        //metoda ustawiająca watrtość bitu na 1
        private void jeden(int indeks)
        {
            bity[indeks].Value = 1;
            if (LFSR != null) LFSR.Items.Refresh();
        }
        //metoda odpowiedzialna za pracę rejestru
        private void Cykl(Object obiekt)
        {
            locki[4] = true;
            //dla zera - 5, a dla 1 - 10 cykli
            //pobranie danych do pracy - listy z infromacjami o bitach, indeksów zmiennych służących do synchronizacji i listy graficznej
            Bit[] tab = (Bit[])((object[])obiekt)[0];
            int pauzab = (int)((object[])obiekt)[1];
            int stopb = (int)((object[])obiekt)[2];
            ListView lista = (ListView)((object[])obiekt)[3];
            int n = tab.Length;
            int suma;
            //zmienna służąca do synchronizacji zapisywania wygenerowanych bitów
            bool zapisz = false;
            int cykle = 0;
            int licznik = 0;
            TextWriter tw = new StreamWriter(plik);
            //pętla wykonująca działa rejestru dopóki nie zostanie wciśnięty przycisk stop lub pauza
            while (!locki[pauzab] && !locki[stopb])
            {
                suma = 0;
                //sumowanie włączonych bitów
                for (int i = 0; i < n; i++)
                {
                    if (tab[i].Stan.Equals("On") && tab[i].Value.Equals(1)) suma = suma + 1;
                }
                //modulo dwa sumy
                suma = suma % 2;
                //zapisanie ostatniego bitu do pliku jeśli minęło 10 cykli po "wyrzuceniu" jedynki
                if (zapisz == true && cykle.Equals(0))
                {
                    this.Dispatcher.Invoke(() => { tw.Write(tab[n - 1].Value);});
                    zapisz = false;
                }
                //przesunięcie wartości bitów w rejestrze
                for (int i = n - 1; i > 0; i--)
                {
                    tab[i].Value = tab[i - 1].Value;
                }
                //przypisanie wartości sumy włączonych bitów po modulo 2 do pierwszego bitu
                tab[0].Value = suma;
                this.Dispatcher.Invoke(() => { if (lista != null) lista.Items.Refresh(); });
                //ustawienie ilości cykli jeśli została wykonana odpowiednia ilość
                if (cykle.Equals(0) && tab[n - 1].Value.Equals(0)) cykle = 5;
                if (cykle.Equals(0) && tab[n - 1].Value.Equals(1))
                {
                    cykle = 10;
                    licznik++;
                    zapisz = true;
                }
                cykle = cykle - 1;
                //Czekanie 50 ms aby obserwacja obliczeń była możliwa dla ludzkiego oka
                Thread.Sleep(50);
            }
            tw.Close();
            locki[4] = false;
        }
        //Bit 1
        private void radioButton_Checked(object sender, RoutedEventArgs e){ on(0);}
        private void radioButton1_Checked(object sender, RoutedEventArgs e){ off(0);}
        private void radioButton2_Checked(object sender, RoutedEventArgs e){ jeden(0);}
        private void radioButton3_Checked(object sender, RoutedEventArgs e){ zero(0);}
        //Bit 2
        private void radioButton_Copy_Checked(object sender, RoutedEventArgs e){ on(1);}
        private void radioButton1_Copy_Checked(object sender, RoutedEventArgs e){ off(1);}
        private void radioButton2_Copy_Checked(object sender, RoutedEventArgs e){ jeden(1);}
        private void radioButton3_Copy_Checked(object sender, RoutedEventArgs e){ zero(1);}
        //Bit 3
        private void radioButton_Copy1_Checked(object sender, RoutedEventArgs e){ on(2);}
        private void radioButton1_Copy1_Checked(object sender, RoutedEventArgs e){ off(2);}
        private void radioButton2_Copy1_Checked(object sender, RoutedEventArgs e){ jeden(2);}
        private void radioButton3_Copy1_Checked(object sender, RoutedEventArgs e){ zero(2);}
        //Bit 4
        private void radioButton_Copy2_Checked(object sender, RoutedEventArgs e){ on(3);}
        private void radioButton1_Copy2_Checked(object sender, RoutedEventArgs e){ off(3);}
        private void radioButton2_Copy2_Checked(object sender, RoutedEventArgs e){ jeden(3);}
        private void radioButton3_Copy2_Checked(object sender, RoutedEventArgs e){ zero(3);}
        //Bit 5
        private void radioButton_Copy3_Checked(object sender, RoutedEventArgs e){ on(4);}
        private void radioButton1_Copy3_Checked(object sender, RoutedEventArgs e){ off(4);}
        private void radioButton2_Copy3_Checked(object sender, RoutedEventArgs e){ jeden(4);}
        private void radioButton3_Copy3_Checked(object sender, RoutedEventArgs e){ zero(4);}
        //Bit 6
        private void radioButton_Copy4_Checked(object sender, RoutedEventArgs e){ on(5);}
        private void radioButton1_Copy4_Checked(object sender, RoutedEventArgs e){ off(5);}
        private void radioButton2_Copy4_Checked(object sender, RoutedEventArgs e){ jeden(5);}
        private void radioButton3_Copy4_Checked(object sender, RoutedEventArgs e){ zero(5);}
        //Bit 7
        private void radioButton_Copy5_Checked(object sender, RoutedEventArgs e){ on(6);}
        private void radioButton1_Copy5_Checked(object sender, RoutedEventArgs e){ off(6);}
        private void radioButton2_Copy5_Checked(object sender, RoutedEventArgs e){ jeden(6);}
        private void radioButton3_Copy5_Checked(object sender, RoutedEventArgs e){ zero(6);}
        //Bit 8
        private void radioButton_Copy6_Checked(object sender, RoutedEventArgs e){ on(7);}
        private void radioButton1_Copy6_Checked(object sender, RoutedEventArgs e){ off(7);}
        private void radioButton2_Copy6_Checked(object sender, RoutedEventArgs e){ jeden(7);}
        private void radioButton3_Copy6_Checked(object sender, RoutedEventArgs e){ zero(7);}
        //Bit 9
        private void radioButton_Copy7_Checked(object sender, RoutedEventArgs e){ on(8);}
        private void radioButton1_Copy7_Checked(object sender, RoutedEventArgs e){ off(8);}
        private void radioButton2_Copy7_Checked(object sender, RoutedEventArgs e){ jeden(8);}
        private void radioButton3_Copy7_Checked(object sender, RoutedEventArgs e){ zero(8);}
        //Bit 10
        private void radioButton_Copy8_Checked(object sender, RoutedEventArgs e){ on(9);}
        private void radioButton1_Copy8_Checked(object sender, RoutedEventArgs e){ off(9);}
        private void radioButton2_Copy8_Checked(object sender, RoutedEventArgs e){ jeden(9);}
        private void radioButton3_Copy8_Checked(object sender, RoutedEventArgs e){ zero(9);}
        //Bit 11
        private void radioButton_Copy9_Checked(object sender, RoutedEventArgs e){ on(10);}
        private void radioButton1_Copy9_Checked(object sender, RoutedEventArgs e){ off(10);}
        private void radioButton2_Copy9_Checked(object sender, RoutedEventArgs e){ jeden(10);}
        private void radioButton3_Copy9_Checked(object sender, RoutedEventArgs e){ zero(10);}
        //Bit 12
        private void radioButton_Copy10_Checked(object sender, RoutedEventArgs e){ on(11);}
        private void radioButton1_Copy10_Checked(object sender, RoutedEventArgs e){ off(11);}
        private void radioButton2_Copy10_Checked(object sender, RoutedEventArgs e){ jeden(11);}
        private void radioButton3_Copy10_Checked(object sender, RoutedEventArgs e){ zero(11);}

        //metoda zatrzymująca działanie rejestru i resetująca stan bitów
        private void stop_metoda(int lok, ListView lista, Button poczatek, Bit[] tabela, bool wczytanie)
        {
            if (!wczytanie)
            {
                int dlugosc = tabela.Length;
                locki[lok] = true;
                for (int i = 0; i < dlugosc; i++)
                {
                    tabela[i].Stan = "On";
                    tabela[i].Value = 1;
                }
            }
            else
            {
                locki[lok] = true;
                if (!textBox.Text.Length.Equals(textBox1.Text.Length))
                {
                    MessageBox.Show("Ilość bitów oznaczonych jako włączone/wyłączone musi się zgadzać z ilością wartości do przypisania!");
                }
                else wczytaj();
            }
            if (lista != null) lista.Items.Refresh();
            Thread.Sleep(70);
            locki[lok] = false;
            poczatek.IsEnabled = true;
        }
        //metoda wstrzymująca działanie rejestru
        private void pauz(int lok, Button przycisk)
        {
            if (!locki[lok]) locki[lok] = true;
            Thread.Sleep(70);
            locki[lok] = false;
            przycisk.IsEnabled = true;
        }
        //rozpoczęcie działania rejestru z trybu "automatycznego"
        private void start_Click(object sender, RoutedEventArgs e)
        {
            start.Content = "Wznów";
            ThreadPool.QueueUserWorkItem(Cykl, new object[] { bity, 1,0, LFSR });
            start.IsEnabled = false;
        }

        private void stop_Click(object sender, RoutedEventArgs e)
        {
            start.Content = "Start";
            stop_metoda(0, LFSR, start, bity, false);
        }

        private void pause_Click(object sender, RoutedEventArgs e)
        {
            pauz(1,start);
        }

        private void wczytaj()
        {
            String OnOff = textBox.Text;
            String wartosci = textBox1.Text;
            StringBuilder s = new StringBuilder();
            foreach (char c in OnOff)
            {
                if (c.Equals('1') || c.Equals('0'))
                {
                    s.Append(c);
                }
            }
            OnOff = s.ToString();
            s.Clear();
            foreach (char c in wartosci)
            {
                if (c.Equals('1') || c.Equals('0'))
                {
                    s.Append(c);
                }
            }
            wartosci = s.ToString();
            int n = wartosci.Length;
            tabl = new Bit[n];
            String stan = "On";
            int wartosc = 1;
            for (int i = 0; i < n; i++)
            {
            if (OnOff[i].Equals('1')) stan = "On";
            else if (OnOff[i].Equals('0')) stan = "Off";
            if (wartosci[i].Equals('1')) wartosc = 1;
            else if (wartosci[i].Equals('0')) wartosc = 0;
            tabl[i] = new Bit() { Bity = "Bit " + (i + 1), Stan = stan, Value = wartosc };
            }
            LFSR_Reczne.ItemsSource = tabl;
        }

        //rozpoczęcie działania rejestru z trybu "ręcznego"
        private void Start_Recz_Click(object sender, RoutedEventArgs e)
        {
            if (!textBox.Text.Length.Equals(textBox1.Text.Length))
            {
                MessageBox.Show("Ilość bitów oznaczonych jako włączone/wyłączone musi się zgadzać z ilością wartości do przypisania!");
            }
            else
            {
                if (tabl[0].Bity.Equals("Pocz")) wczytaj();
                ThreadPool.QueueUserWorkItem(Cykl, new object[] { tabl, 3, 2, LFSR_Reczne });
                Start_Recz.Content = "Wznów";
                Start_Recz.IsEnabled = false;
            }
        }

        private void Stop_recz_Click(object sender, RoutedEventArgs e)
        {
            Start_Recz.Content = "Start";
            stop_metoda(2, LFSR_Reczne, Start_Recz, tabl, true);
        }

        private void Puaza_Recz_Click(object sender, RoutedEventArgs e)
        {
            pauz(3, Start_Recz);
        }

        private void HelpPop()
        {
            MessageBox.Show("Generator samodecymujący polega na samotaktującym się rejestrze LFSR, który podaje na wyjście zawsze ostatni bit rejestru zamieniając jednocześnie wartości bitów na wartości bitów poprzedzających. Wartość pierwszego bitu jest zamieniana na modulo 2 z sumy wartości włączonych bitów. W przypadku podania na wyjście 0 program czeka 5 cyklów przed sprawdzeniem kolejnej wartości. W przypadku podania na wyjście 1 program zapisuje do pliku wartość z wyjścia po 10 cyklach."+(char)10+
                "W zakładce 'Automatyczne' można dowolnie ustawić 12 bitów z pomocą prostego interfejsu."+(char)10+
                "W zakładce 'Ręczne' można ustawić dowolnej długości ciąg bitów wpisując ciągi złożone z 0 i 1 w obu wyznaczonych miejscach. Wszelkie inne symbole zostaną pominięte, a oba ciągi 0 i 1 muszą być tej samej długości."+(char)10+
                "Przycisk 'Stop' w zakładce 'Automatyczne' ustawia wszystkie bity na 'On' oraz ich wartość na 1. W zakładce 'Ręczne' wczytuje od nowa ciąg z wypełnionych pól.");
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            HelpPop();
        }

        private void Help2_Click(object sender, RoutedEventArgs e)
        {
            HelpPop();
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((textBox.Text.Length == 0 || textBox1.Text.Length == 0) && !locki[2] && !locki[3]) Start_Recz.IsEnabled = false;
            else
            {
                if (!locki[4])
                {
                    Start_Recz.IsEnabled = true;
                    if (textBox.Text.Length.Equals(textBox1.Text.Length)) wczytaj();
                }
            }
        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((textBox.Text.Length == 0 || textBox1.Text.Length == 0) && !locki[2] && !locki[3]) Start_Recz.IsEnabled = false;
            else
            {
                if (!locki[4])
                {
                    Start_Recz.IsEnabled = true;
                    if (textBox.Text.Length.Equals(textBox1.Text.Length)) wczytaj();
                }
            }
        }
    }
}
