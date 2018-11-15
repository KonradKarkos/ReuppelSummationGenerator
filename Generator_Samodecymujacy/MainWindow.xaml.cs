using Microsoft.Win32;
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
        public Bit[] bity = new Bit[12];
        public bool[] locki = { false, false, false, false, false, false };
        Bit[] tabl = new Bit[1];
        public String[] pliki = { "KodAuto.txt", "KodRecz.txt", "OnOffKonf.txt", "WatrtosciKonf.txt" };
        public int opoznienie = 1;
        public int d = 5;
        public int k = 10;
        public int kodauto = 10000;
        public int kodrecz = 10000;
        public int wstrzymane1=0, wstrzymane2=0;
        public MainWindow()
        {
            tabl[0] = new Bit() { Bity = "Pocz", Stan = "Off", Value = 0 };
            for (int i = 0; i < 12; i++)
            {
                bity[i] = new Bit() { Bity = "Bit " + (i + 1), Stan = "On", Value = 1 };
            }
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
            int indeks = (int)((object[])obiekt)[7];
            locki[indeks] = true;
            //dla zera - 5, a dla 1 - 10 cykli
            //pobranie danych do pracy - listy z infromacjami o bitach, indeksów zmiennych służących do synchronizacji i listy graficznej
            Bit[] tab = (Bit[])((object[])obiekt)[0];
            int pauzab = (int)((object[])obiekt)[1];
            int stopb = (int)((object[])obiekt)[2];
            ListView lista = (ListView)((object[])obiekt)[3];
            String kontent = (String)((object[])obiekt)[4];
            String plik = (String)((object[])obiekt)[5];
            int dlugosc = (int)((object[])obiekt)[6];
            if (indeks == 4 && wstrzymane1 > 0) dlugosc = wstrzymane1;
            if (indeks == 5 && wstrzymane2 > 0) dlugosc = wstrzymane2;
            int n = tab.Length;
            int suma;
            //zmienna służąca do synchronizacji zapisywania wygenerowanych bitów
            bool zapisz = false;
            int cykle = 0;
            int licznik = 0;
            StreamWriter tw;
            if (kontent.Equals("Wznów"))
            {
                tw = File.AppendText(plik);
            }
            else
            {
                tw = new StreamWriter(plik);
            }
            //pętla wykonująca działa rejestru dopóki nie zostanie wciśnięty przycisk stop lub pauza
            while (!locki[pauzab] && !locki[stopb] && dlugosc>0)
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
                if (cykle.Equals(0) && tab[n - 1].Value.Equals(0)) cykle = d;
                if (cykle.Equals(0) && tab[n - 1].Value.Equals(1))
                {
                    cykle = k;
                    licznik++;
                    zapisz = true;
                }
                cykle = cykle - 1;
                dlugosc--;
                //Czekanie 2 ms aby obserwacja obliczeń była możliwa dla ludzkiego oka
                Thread.Sleep(2);
            }
            if (dlugosc == 0) kontent = "Start";
            else
            {
                if (indeks == 4) wstrzymane1 = dlugosc;
                if (indeks == 5) wstrzymane2 = dlugosc;
            }
            tw.Close();
            locki[indeks] = false;
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
                //wykorzystanie zmiennej boolowskiej do synchronizacji
                locki[lok] = true;
                if (!textBox.Text.Length.Equals(textBox1.Text.Length))
                {
                    MessageBox.Show("Ilość bitów oznaczonych jako włączone/wyłączone musi się zgadzać z ilością wartości do przypisania!");
                }
                else wczytaj();
            }
            if (lista != null) lista.Items.Refresh();
            Thread.Sleep(50);
            locki[lok] = false;
            poczatek.IsEnabled = true;
        }
        //metoda wstrzymująca działanie rejestru
        private void pauz(int lok, Button przycisk)
        {
            if (!locki[lok]) locki[lok] = true;
            Thread.Sleep(50);
            locki[lok] = false;
            przycisk.IsEnabled = true;
        }
        //rozpoczęcie działania rejestru z trybu "automatycznego"
        private void start_Click(object sender, RoutedEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(Cykl, new object[] { bity, 1, 0, LFSR, start.Content, pliki[0], kodauto, 5});
            if(start.Content.Equals("Start"))start.Content = "Wznów";
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
        //metoda wczytująca ręcznie zapisaną konfigurację bitów
        private void wczytaj()
        {
            String OnOff = textBox.Text;
            String wartosci = textBox1.Text;
            int n = wartosci.Length;
            tabl = new Bit[n];
            String stan = "On";
            int wartosc = 1;
            //Stworzenie listy bitów o wczytanych parametrach
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
            int dlugosc1 = textBox.Text.Length, dlugosc2 = textBox1.Text.Length;
            if (dlugosc1 != dlugosc2 || dlugosc1 == 0 || dlugosc2 == 0)
            {
                MessageBox.Show("Ilość bitów oznaczonych jako włączone/wyłączone musi się zgadzać z ilością wartości do przypisania!"+(char)10
                    +"Długość ciągu On/Off: "+dlugosc1.ToString()+(char)10+"Długość ciągu wartości: "+dlugosc2.ToString());
            }
            else
            {
                if (tabl[0].Bity.Equals("Pocz")) wczytaj();
                ThreadPool.QueueUserWorkItem(Cykl, new object[] { tabl, 3, 2, LFSR_Reczne, Start_Recz.Content, pliki[1], kodrecz, 4 });
                if(Start_Recz.Content.Equals("Start"))Start_Recz.Content = "Wznów";
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
            char c = (char)10;
            MessageBox.Show("Generator samodecymujący polega na samotaktującym się rejestrze LFSR, który podaje na wyjście zawsze ostatni bit rejestru zamieniając jednocześnie wartości bitów na wartości bitów poprzedzających. Wartość pierwszego bitu jest zamieniana na modulo 2 z sumy wartości włączonych bitów. W przypadku podania na wyjście 0 program czeka 5 cykli przed sprawdzeniem kolejnej wartości. W przypadku podania na wyjście 1 program zapisuje do pliku wartość z wyjścia po 10 cyklach."+c+
                "W zakładce 'Automatyczne' można dowolnie ustawić 12 bitów z pomocą prostego interfejsu."+c+
                "W zakładce 'Ręczne' można ustawić dowolnej długości ciąg bitów wpisując ciągi złożone z 0 i 1 w obu wyznaczonych miejscach. Wszelkie inne symbole zostaną pominięte, a oba ciągi 0 i 1 muszą być tej samej długości."+c+
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
        //wczytanie konfiguracji zapisanej w plikach
        private void wczyt_konf_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(pliki[2]) && File.Exists(pliki[3]))
            {
                StreamReader s = new StreamReader(pliki[2]);
                String linia;
                StringBuilder sb = new StringBuilder();
                while((linia=s.ReadLine())!=null)
                {
                    //pętla wczytująca tylko odpowiednie dane na wypadek gdyby ktoś chciał wczytać plik wydedytowany poza programem
                    foreach(char c in linia)
                    {
                        if (c.Equals('0') || c.Equals('1'))
                        {
                            sb.Append(c);
                        }
                    }
                }
                textBox.Text = sb.ToString();
                sb.Clear();
                s.Dispose();
                s = new StreamReader(pliki[3]);
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
                textBox1.Text = sb.ToString();
            }
            else
            {
                MessageBox.Show("Nie istnieje przynajmniej jeden z plików konfiguracyjnych. Wczytywanie wstrzymane.");
            }
        }
        //zapisanie konfiguracji do plików
        private void zapisz_konf_Click(object sender, RoutedEventArgs e)
        {
            StreamWriter s = new StreamWriter(pliki[2]);
            if (textBox.LineCount > 0)
            {
                for (int i = 0; i < textBox.LineCount; i++)
                {
                    s.Write(textBox.GetLineText(i));
                }
            }
            s.Dispose();
            s = new StreamWriter(pliki[3]);
            if (textBox1.LineCount > 0)
            {
                for (int i = 0; i < textBox1.LineCount; i++)
                {
                    s.Write(textBox1.GetLineText(i));
                }
            }
            s.Dispose();
        }
        //funkcja blokująca wprowadzane znaki poza liczbami
        private void Sprawdz(object sender, TextCompositionEventArgs e)
        {
            int output;
            if(int.TryParse(e.Text,out output)==false)
            {
                e.Handled = true;
            }
        }
        //funkcja blokująca klawisz spacji
        private void Sprawdz_Key(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.Space)
            {
                e.Handled = true;
            }
        }
        //funkcja blokująca znaki niedopuszczone w nazwach plików
        private void Sprawdz2(object sender, TextCompositionEventArgs e)
        {
            String[] zakazane = {">","<","/","\"","\\","*","?",":","|" };
            for (int i = 0; i < 9; i++)
            {
                if (e.Text.Equals(zakazane[i]))
                {
                    e.Handled = true;
                    break;
                }
            }
        }
        private void Sprawdz_zero_jeden(object sender, TextCompositionEventArgs e)
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
        private String dodaj_rozszerzenie(TextBox nazwa)
        {
            String linie = nazwa.Text;
            String koniec = linie.Substring(linie.Length - 4);
            nazwa.Text = koniec;
            if (!koniec.Equals(".txt"))
            {
                linie += ".txt";
            }
            return linie;
        }

        private void Zastosuj_Click(object sender, RoutedEventArgs e)
        {
            if (txt_onoff.Text.Length == 0 || txt_wart.Text.Length == 0 || txt_zapis_auto.Text.Length == 0 || txt_zapis_recz.Text.Length == 0 || dlugosc_auto.Text.Length == 0 || dlugosc_auto.Text.Length == 0 || k_box.Text.Length == 0 || d_box.Text.Length == 0) 
            {
                MessageBox.Show("Żadna z opcji nie może pozostać pusta!");
            }
            else
            {
                pliki[0] = dodaj_rozszerzenie(txt_zapis_auto);
                pliki[1] = dodaj_rozszerzenie(txt_zapis_recz);
                pliki[2] = dodaj_rozszerzenie(txt_onoff);
                pliki[3] = dodaj_rozszerzenie(txt_wart);
                k = Int32.Parse(k_box.Text);
                d = Int32.Parse(d_box.Text);
                kodrecz = Int32.Parse(dlugosc_recz.Text);
                kodauto = Int32.Parse(dlugosc_auto.Text);
            }
        }

        private void wybierz_plik(TextBox tab)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                tab.Text = openFileDialog.FileName;
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            wybierz_plik(txt_onoff);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            wybierz_plik(txt_wart);
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            wybierz_plik(txt_zapis_recz);
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            wybierz_plik(txt_zapis_auto);
        }
    }
}
