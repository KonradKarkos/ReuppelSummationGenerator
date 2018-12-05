using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
        /*
         zamki odpowiedzialne za synchronizację, odpowiadają kolejno za:
         0 - zamek stop generacji automatycznej
         1- zamek pauzy generacji automatycznej
         2 - zamek stop generacji ręcznej
         3 - zamek pauzy generacji ręcznek
         4 - zamek startu ręcznego
         5 - zamek stratu automatycznego
         */
        public bool TestTekst = true;
        public bool[] locki = { false, false, false, false, false, false };
        Bit[] tabl = new Bit[1];
        public int opoznienie = 1;
        public int d = 5;
        public int k = 10;
        public int kodauto = 10000;
        public int kodrecz = 10000;
        public int wstrzymane1=0, wstrzymane2=0;
        bool przyspiesz = false;
        ObslugaPlikow OB = new ObslugaPlikow();
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
            Button Przycisk = (Button)((object[])obiekt)[8];
            String kontent = (String)((object[])obiekt)[4];
            String plik = (String)((object[])obiekt)[5];
            int dlugosc = (int)((object[])obiekt)[6];
            ProgressBar PB = (ProgressBar)((object[])obiekt)[9];
            //jeśli operacja została zapauzowana to wczytywana jest ilość bitów pozostałych do generacji
            if (indeks == 4 && wstrzymane1 > 0) dlugosc = wstrzymane1;
            if (indeks == 5 && wstrzymane2 > 0) dlugosc = wstrzymane2;
            if (locki[pauzab] || locki[stopb])
            {
                locki[pauzab] = false;
                locki[stopb] = false;
            }
            int n = tab.Length;
            int suma;
            //zmienna służąca do synchronizacji zapisywania wygenerowanych bitów
            bool zapisz = false;
            int cykle = 0;
            int licznik = 0;
            //zainicjowanie obiektów klas wpisujących, tylko jedna z nich będzie wykorzystwana, więc są natychmiast zamykane bo inaczej kompilator się buntuje jeśli nie zostaną zalkowane w pamięci
            StreamWriter tw = new StreamWriter(plik);
            tw.Dispose();
            BinaryWriter Bw = new BinaryWriter(File.Open(plik,FileMode.OpenOrCreate));
            Bw.Dispose();
            if (kontent.Equals("Wznów"))
            {
                if (OB.rozszerzenie.Equals(".txt"))
                    tw = File.AppendText(plik);
                else
                    Bw = new BinaryWriter(File.Open(plik, FileMode.Append));
            }
            else
            {
                dlugosc = (int)((object[])obiekt)[6];
                if (OB.rozszerzenie.Equals(".txt"))
                    tw = new StreamWriter(plik);
                else
                    Bw = new BinaryWriter(File.Open(plik, FileMode.OpenOrCreate));
                this.Dispatcher.Invoke(() => { PB.Maximum = dlugosc; });
            }
            int bajta = 8;
            StringBuilder dobajtu = new StringBuilder();
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
                    //sposób zapisu zależny od rozszerzenia pliku
                    if (OB.rozszerzenie.Equals(".bin"))
                    {
                        bajta--;
                        if (bajta == 0)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                Bw.Write(Convert.ToByte(dobajtu.ToString(), 2));
                            });
                            dobajtu.Clear();
                            bajta = 8;
                        }
                        else
                        {
                            dobajtu.Append(tab[n-1].Value);
                        }
                    }
                    else
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            tw.Write(tab[n - 1].Value);
                        });
                    }
                    this.Dispatcher.Invoke(() => { PB.Value++; });
                    dlugosc--;
                    zapisz = false;
                }
                //przesunięcie wartości bitów w rejestrze
                for (int i = n - 1; i > 0; i--)
                {
                    tab[i].Value = tab[i - 1].Value;
                }
                //przypisanie wartości sumy włączonych bitów po modulo 2 do pierwszego bitu
                tab[0].Value = suma;
                //odświeżenie podglądu rejestru
                if (!przyspiesz)
                {
                    this.Dispatcher.Invoke(() => { if (lista != null) lista.Items.Refresh(); });
                }
                //ustawienie ilości cykli jeśli została wykonana odpowiednia ilość
                if (cykle.Equals(0) && tab[n - 1].Value.Equals(0)) cykle = d;
                if (cykle.Equals(0) && tab[n - 1].Value.Equals(1))
                {
                    cykle = k;
                    licznik++;
                    zapisz = true;
                }
                cykle = cykle - 1;
            }
            if (dlugosc == 0)
            {
                this.Dispatcher.Invoke(() =>
                {
                    Przycisk.Content = "Start";
                    Przycisk.IsEnabled = true;
                    PB.Value = 0;
                });
            }
            else
            {
                if (indeks == 4) wstrzymane1 = dlugosc;
                if (indeks == 5) wstrzymane2 = dlugosc;
            }
            tw.Close();
            Bw.Close();
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
        private void stop_metoda(int lok, ListView lista, Button poczatek, Bit[] tabela, bool wczytanie, ProgressBar PB)
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
            PB.Value = 0;
            if (lista != null) lista.Items.Refresh();
            Thread.Sleep(50);
            poczatek.IsEnabled = true;
        }
        //metoda wstrzymująca działanie rejestru
        private void pauz(int lok, Button przycisk)
        {
            if (!locki[lok]) locki[lok] = true;
            Thread.Sleep(50);
            przycisk.IsEnabled = true;
        }
        //rozpoczęcie działania rejestru z trybu "automatycznego"
        private void start_Click(object sender, RoutedEventArgs e)
        {
            if (!locki[5])
            {
                ThreadPool.QueueUserWorkItem(Cykl, new object[] { bity, 1, 0, LFSR, start.Content, OB.pliki[0], kodauto, 5, start, pb_auto });
                if (start.Content.Equals("Start")) start.Content = "Wznów";
                start.IsEnabled = false;
            }
        }

        private void stop_Click(object sender, RoutedEventArgs e)
        {
            start.Content = "Start";
            stop_metoda(0, LFSR, start, bity, false, pb_auto);
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
            else if(!locki[4])
            {
                if (tabl[0].Bity.Equals("Pocz")) wczytaj();
                ThreadPool.QueueUserWorkItem(Cykl, new object[] { tabl, 3, 2, LFSR_Reczne, Start_Recz.Content, OB.pliki[1], kodrecz, 4, Start_Recz,pb_Recz });
                if(Start_Recz.Content.Equals("Start"))Start_Recz.Content = "Wznów";
                Start_Recz.IsEnabled = false;
            }
        }

        private void Stop_recz_Click(object sender, RoutedEventArgs e)
        {
            Start_Recz.Content = "Start";
            stop_metoda(2, LFSR_Reczne, Start_Recz, tabl, true, pb_Recz);
        }

        private void Puaza_Recz_Click(object sender, RoutedEventArgs e)
        {
            pauz(3, Start_Recz);
        }

        private void HelpPop(String opcja)
        {
            MessageBox.Show(OB.Wczytaj_Pomoc(opcja));
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            HelpPop("Generator.txt");
        }

        private void Help2_Click(object sender, RoutedEventArgs e)
        {
            HelpPop("Generator.txt");
        }
        //automatyczne wprowadzanie danych do listy bitów
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
            OB.Wczytaj_Konf_Recz(textBox, textBox1);
        }
        //zapisanie konfiguracji do plików
        private void zapisz_konf_Click(object sender, RoutedEventArgs e)
        {

            OB.Zapisz_Konf_Recz(textBox, textBox1);
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
        //funkcja dopuszczająca wpisywanie tylko 0 i 1 w obiekcie do któego jest przypisana
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
        
        //zatwierdzenie zmian wprowadzonych w opcjach
        private void Zastosuj_Click(object sender, RoutedEventArgs e)
        {
            //niewykonanie akcji w wypadku jeśli któryś textbox w opcjach jest pusty
            if (txt_onoff.Text.Length == 0 || txt_wart.Text.Length == 0 || txt_zapis_auto.Text.Length == 0 || txt_zapis_recz.Text.Length == 0 || dlugosc_auto.Text.Length == 0 || dlugosc_auto.Text.Length == 0 || k_box.Text.Length == 0 || d_box.Text.Length == 0 || TextBoxJawny.Text.Length==0 || TextBoxKlucz.Text.Length==0 || TextBoxZaszyfrowany.Text.Length==0 || TextBoxDoOdszyfrowania.Text.Length==0 || TextBoxKluczDoOdszyfrowania.Text.Length==0 || TextBoxOdszyfrowany.Text.Length==0) 
            {
                MessageBox.Show("Żadna z opcji nie może pozostać pusta!");
            }
            else
            {
                //dodanie rozszerzeń do plików wpisanych w opcjach
                ComboBoxItem typeItem = (ComboBoxItem)comboBox.SelectedItem;
                OB.rozszerzenie = typeItem.Content.ToString();
                OB.pliki[0] = OB.dodaj_rozszerzenie(txt_zapis_auto, true);
                OB.pliki[1] = OB.dodaj_rozszerzenie(txt_zapis_recz, true);
                OB.pliki[2] = OB.dodaj_rozszerzenie(txt_onoff, false);
                OB.pliki[3] = OB.dodaj_rozszerzenie(txt_wart, false);
                OB.pliki[4] = OB.dodaj_rozszerzenie(TextBoxJawny, false);
                OB.pliki[5] = OB.dodaj_rozszerzenie(TextBoxKlucz, true);
                OB.pliki[6] = OB.dodaj_rozszerzenie(TextBoxZaszyfrowany, true);
                OB.pliki[7] = OB.dodaj_rozszerzenie(TextBoxDoOdszyfrowania, true);
                OB.pliki[8] = OB.dodaj_rozszerzenie(TextBoxKluczDoOdszyfrowania, true);
                OB.pliki[9] = OB.dodaj_rozszerzenie(TextBoxOdszyfrowany, false);
                k = Int32.Parse(k_box.Text);
                d = Int32.Parse(d_box.Text);
                kodrecz = Int32.Parse(dlugosc_recz.Text);
                kodauto = Int32.Parse(dlugosc_auto.Text);
                if (Przyspieszenie.IsChecked == false) przyspiesz = false;
                else przyspiesz = true;
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            OB.wybierz_plik(txt_onoff);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            OB.wybierz_plik(txt_wart);
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            OB.wybierz_plik(txt_zapis_recz);
        }
        //przyciski pokazujące więcej opcji programu do wyboru
        private void Dalej_Click(object sender, RoutedEventArgs e)
        {
            Pierwszy_Opcje.Visibility = Visibility.Hidden;
            Drugi_opcje.Visibility = Visibility.Visible;
        }
        private void Cofnij_Click(object sender, RoutedEventArgs e)
        {
            Drugi_opcje.Visibility = Visibility.Hidden;
            Pierwszy_Opcje.Visibility = Visibility.Visible;
        }

        private void WybierzJawny_Click(object sender, RoutedEventArgs e)
        {
            OB.wybierz_plik(TextBoxJawny);
        }

        private void WybierzKlucz_Click(object sender, RoutedEventArgs e)
        {
            OB.wybierz_plik(TextBoxKlucz);
        }

        private void WybierzZaszyfrowany_Click(object sender, RoutedEventArgs e)
        {
            OB.wybierz_plik(TextBoxZaszyfrowany);
        }
        //szyfrowanie NIST wprowadzonego tekstu jawnego za pomocą klucza
        private void Start_Szyfr_Click_1(object sender, RoutedEventArgs e)
        {
            String Jawny = BoxJawny.Text;
            StringBuilder JawnyWBajtach = new StringBuilder();
            //zamiana tekstu jawnego na reprezentację binarną
            foreach(char c in Jawny)
            {
                JawnyWBajtach.Append(Convert.ToString(c, 2).PadLeft(8, '0'));
            }
            Jawny = JawnyWBajtach.ToString();
            String Klucz = BoxKlucz.Text;
            if (Klucz.Length < Jawny.Length)
            {
                MessageBox.Show("Długość klucza jest mniejsza od długości tekstu do zaszyfrowania w bitach. Klucz zostanie skopiowany i zrównany z długością tekstu jawnego w bitach.");
                //zrównanie długości klucza z długością reprezentacji binarnej tekstu jawnego
                while (Klucz.Length < Jawny.Length)
                {
                    Klucz += Klucz;
                }
                Klucz = Klucz.Substring(0, Jawny.Length);
                BoxKlucz.Text = Klucz;
            }
            JawnyWBajtach.Clear();
            //Exclusive or kolejnych bitów tekstu jawnego i klucza
            for (int i = 0; i < Jawny.Length; i++)
            {
                JawnyWBajtach.Append(Jawny[i] ^ Klucz[i]);
            }
            BoxZaszyfrowany.Text = JawnyWBajtach.ToString();
        }
        //mechanizm zabezpieczający przed uruchomienie szyfratora podczas gdy pola przechowujące tekst jawny i szyfr są puste
        private void BoxJawny_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (BoxJawny.Text.Length > 0 && BoxKlucz.Text.Length > 0) Start_Szyfr.IsEnabled = true;
            else Start_Szyfr.IsEnabled = false;
        }

        private void Wczyt_Szyfr_Click(object sender, RoutedEventArgs e)
        {
            OB.wczytaj_pliki_szyfratora(4, 5, BoxJawny, BoxKlucz,false);
        }
        //Zapisywanie zawartości w pliku binarnym lub tekstowym zależnie od wyboru
        private void Zapisz_Szyfr_Click(object sender, RoutedEventArgs e)
        {
            OB.zapisz_pliki_szyfratora(4, 5, 6, BoxJawny, BoxKlucz, BoxZaszyfrowany);
        }

        private void Help_Szyfr_Click(object sender, RoutedEventArgs e)
        {
            HelpPop("Szyfrator.txt");
        }

        private void Start_Deszyfr_Click(object sender, RoutedEventArgs e)
        {
            if (BoxZaszdoDesz.Text.Length % 8 > 0)
            {
                MessageBox.Show("Długość zakodowanego tekstu w bitach musi być podzielna przez 8!");
            }
            else
            {
                String DoOdszyfrowania = BoxZaszdoDesz.Text;
                String Klucz = BoxKluczDesz.Text;
                if (Klucz.Length < DoOdszyfrowania.Length)
                {
                    MessageBox.Show("Długość klucza jest mniejsza od długości tekstu do odszyfrowania. Klucz zostanie skopiowany i zrównany z długością tekstu do odszyfrowania.");
                    //zrównanie długości klucza z długością reprezentacji binarnej tekstu do odszyfrowania
                    while (Klucz.Length < DoOdszyfrowania.Length)
                    {
                        Klucz += Klucz;
                    }
                    Klucz = Klucz.Substring(0, DoOdszyfrowania.Length);
                    BoxKluczDesz.Text = Klucz;
                }
                StringBuilder XOR = new StringBuilder();
                int dlugosc = DoOdszyfrowania.Length / 8;
                Byte[] znaki = new Byte[dlugosc];
                for (int i = 0; i < dlugosc; i++)
                {
                    //Exclusive or z bitów tworzących każdy znak po kolei
                    for (int j = i * 8; j < (i * 8) + 8; j++)
                    {
                        XOR.Append(DoOdszyfrowania[j] ^ Klucz[j]);
                    }
                    znaki[i] = Convert.ToByte(XOR.ToString(), 2);
                    XOR.Clear();
                }
                BoxZdeszyfrowany.Text = Encoding.ASCII.GetString(znaki);
            }
        }

        private void Wczyt_Deszyfr_Click(object sender, RoutedEventArgs e)
        {
            OB.wczytaj_pliki_szyfratora(7, 8, BoxZaszdoDesz, BoxKluczDesz, true);
        }

        private void Zapisz_Deszyfr_Click(object sender, RoutedEventArgs e)
        {
            OB.zapisz_pliki_szyfratora(7, 8, 9, BoxZaszdoDesz, BoxKluczDesz, BoxZdeszyfrowany);
        }

        private void BoxZaszdoDesz_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (BoxZaszdoDesz.Text.Length > 0 && BoxKluczDesz.Text.Length > 0) Start_Deszyfr.IsEnabled = true;
            else Start_Deszyfr.IsEnabled = false;
        }

        private void WybierzDoOdszyfrowania_Click(object sender, RoutedEventArgs e)
        {
            OB.wybierz_plik(TextBoxDoOdszyfrowania);
        }

        private void WybierzKluczDoOdszyfrowania_Click(object sender, RoutedEventArgs e)
        {
            OB.wybierz_plik(TextBoxKluczDoOdszyfrowania);
        }

        private void WybierzOdszyfrowany_Click(object sender, RoutedEventArgs e)
        {
            OB.wybierz_plik(TextBoxOdszyfrowany);
        }

        private void DlugoscPliku_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (PlikTest.Text.Length > 0) TestujButton.IsEnabled = true;
            else TestujButton.IsEnabled = false;
        }

        private void RadioButton4_Checked(object sender, RoutedEventArgs e)
        {
            TestTekst = true;
        }

        private void RadioButton5_Checked(object sender, RoutedEventArgs e)
        {
            TestTekst = false;
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            OB.wybierz_plik(txt_zapis_auto);
        }

        private void TestujButton_Click(object sender, RoutedEventArgs e)
        {
            String plik = PlikTest.Text;
            bool tekstowy = TestTekst;
            if (!File.Exists(plik))
            {
                MessageBox.Show("Nie istnieje taki plik!");
            }
            else
            {
                FileInfo f = new FileInfo(plik);
                if ((f.Length % 20000 != 0 && tekstowy == true) || ((f.Length * 8) % 20000 != 0 && tekstowy == false))
                {
                    MessageBox.Show("Długość ciągu musi być podzielna przez 20000!");
                }
                else
                {
                    StreamWriter sw = new StreamWriter("TestResults.txt");
                    BarLongRun.Value = 0;
                    BarMonoBit.Value = 0;
                    BarRun.Value = 0;
                    BarPoker.Value = 0;
                    BarLongRun.Visibility = Visibility.Visible;
                    BarMonoBit.Visibility = Visibility.Visible;
                    BarRun.Visibility = Visibility.Visible;
                    BarPoker.Visibility = Visibility.Visible;
                    StreamReader sr = new StreamReader(plik);
                    sr.Dispose();
                    BinaryReader br = new BinaryReader(File.Open(plik, FileMode.Open));
                    br.Dispose();
                    int dlugosc = (int)f.Length;
                    BarLongRun.Maximum = dlugosc / 20000;
                    BarMonoBit.Maximum = dlugosc / 20000;
                    BarRun.Maximum = dlugosc / 20000;
                    BarPoker.Maximum = dlugosc / 20000;
                    char[] ciag = new char[20000];
                    StringBuilder sb = new StringBuilder();
                    int jedynki;
                    int[] pokerowy = new int[16];
                    int IndeksSerii = 0;
                    bool[] testy = { true, true, true, true };
                    char c;
                    int DlugoscSerii = 0;
                    int indeks;
                    int[] Seria0 = new int[6];
                    int[] Seria1 = new int[6];
                    int[] bity = new int[4];
                    for (int i = 0; i < dlugosc; i+=20000)
                    {
                        IndeksSerii = 0;
                        DlugoscSerii = 0;
                        if (!testy[0] && !testy[1] && !testy[2] && !testy[3]) break;
                        //"czyszczenie" tablic i zmiennych wykorzystywanych w programie
                        for(int u=0;u<16;u++)
                        {
                            pokerowy[u] = 0;
                        }
                        for(int y=0;y<6;y++)
                        {
                            Seria0[y] = 0;
                            Seria1[y] = 0;
                        }
                        jedynki = 0;
                        if (tekstowy)
                        {
                            sr = new StreamReader(plik);
                            sr.Read(ciag, 0, 20000);
                        }
                        else
                        {
                            br = new BinaryReader(File.Open(plik, FileMode.Open));
                            for (int z = 0; z < 2500; z++)
                            {
                                sb.Append(Convert.ToString(br.ReadByte(), 2).PadLeft(8,'0'));
                            }
                            ciag = sb.ToString().ToCharArray();
                            sb.Clear();
                        }
                        // zaczęcie od pozycji 1 w celu łatwiejszego dzielenia ciągów do testu pokerowego
                        if(testy[0] || testy[1])
                        for (int j = 1; j <= 20000; j++)
                        {
                            //do testu monobitu
                            if (testy[0] && ciag[j-1].Equals('1')) jedynki++;
                            //do testu pokerowego
                            if (testy[1] && j %4==0 && j!=0)
                            {
                                bity[3] = ciag[j - 4] - '0';
                                bity[2] = ciag[j - 3] - '0';
                                bity[1] = ciag[j - 2] - '0';
                                bity[0] = ciag[j - 1] - '0';
                                indeks = (int)Math.Pow(2,3) * bity[3] + (int)Math.Pow(2, 2) * bity[2] + (2 * bity[1]) +bity[0];
                                pokerowy[indeks]++;
                            }
                        }
                        if (testy[0]) BarMonoBit.Value++;
                        if (testy[1]) BarPoker.Value++;
                        if (testy[2] || testy[3])
                        while(IndeksSerii<20000)
                        {
                            c = ciag[IndeksSerii];
                            while(IndeksSerii < 20000 && ciag[IndeksSerii].Equals(c))
                            {
                                DlugoscSerii++;
                                IndeksSerii++;
                            }
                            if (testy[2])
                            {
                                //ciągi o długości 6+ na potrzeby testu są uznawane za ciągi o długości 6
                                if (DlugoscSerii >= 6 && c.Equals('0')) Seria0[5]++;
                                else if (DlugoscSerii >= 6 && c.Equals('1')) Seria1[5]++;
                                else
                                {
                                    if(c.Equals('0')) Seria0[DlugoscSerii - 1]++;
                                    else Seria1[DlugoscSerii - 1]++;
                                }
                            }
                            if (DlugoscSerii >= 26 && testy[3])
                            {
                                sw.WriteLine("Test długiej serii - niezaliczony. Znaleziono ciąg \""+c+"\" o długości "+DlugoscSerii+", którego koniec nastąpił w miejscu o indeksie "+(i+IndeksSerii-1));
                                testy[3] = false;
                                WynikLongRun.Foreground = new SolidColorBrush(Colors.Red);
                                WynikLongRun.Text = "Test nieudany!";
                            }
                            DlugoscSerii = 0;
                        }
                        if (testy[2]) BarRun.Value++;
                        if (testy[3]) BarLongRun.Value++;
                        //sprawdzenie czy ilość jedynek w ciągu mieści się w wymaganym zakresie
                        if(testy[0])
                        {
                            if (jedynki>10275 || jedynki<9725)
                            {
                                sw.WriteLine("Test pojedynczego bitu - niezaliczony. Ilość jedynek w części zaczynającej się na indeksie "+i+" wynosi "+jedynki+", co daje nam zera w ilości "+(20000-jedynki));
                                testy[0] = false;
                                WynikMonoBit.Foreground = new SolidColorBrush(Colors.Red);
                                WynikMonoBit.Text= "Test nieudany!";
                            }
                        }
                        if(testy[1])
                        {
                            //obliczenie współczynnika pokerowego według wzoru X=(16/5000)*(SUM i=0 -> i=15 [f(i)]^2)-5000
                            double sum = 0.0;
                            for(int y=0;y<16;y++)
                            {
                                sum = sum + pokerowy[y] * pokerowy[y];
                            }
                            double X = 16.0 / 5000.0 * sum - 5000.0;
                            //sprawdzenie czy współczynnik mieści się w podanym zakresie
                            if(X<2.16 || X>46.17)
                            {
                                sw.WriteLine("Test pokerowy - niezaliczony. Współczynnik X w części zaczynającej się na indeksie " + i +" wynosi "+X);
                                testy[1] = false;
                                WynikPoker.Foreground = new SolidColorBrush(Colors.Red);
                                WynikPoker.Text = "Test nieudany!";
                            }
                        }
                        //sprawdzenie czy ciągi o różnych długościach mieszczą się w wymaganych zakresach
                        if(testy[2])
                        {
                            if((Seria0[0]<2315 || Seria0[0]>2685)
                                ||
                                (Seria0[1]<1114 || Seria0[1]>1386)
                                || 
                                (Seria0[2]<527 || Seria0[2]>723)
                                ||
                                (Seria0[3]<240 || Seria0[3]>384)
                                ||
                                (Seria0[4]<103 || Seria0[4]>209)
                                ||
                                (Seria0[5]<103 || Seria0[5]>209)
                                ||
                                (Seria1[0] < 2315 || Seria1[0] > 2685)
                                ||
                                (Seria1[1] < 1114 || Seria1[1] > 1386)
                                ||
                                (Seria1[2] < 527 || Seria1[2] > 723)
                                ||
                                (Seria1[3] < 240 || Seria1[3] > 384)
                                ||
                                (Seria1[4] < 103 || Seria1[4] > 209)
                                ||
                                (Seria1[5] < 103 || Seria1[5] > 209))
                            {
                                sw.WriteLine("Test serii - niezaliczony. Ilości ciągów prezentowały się następująco:");
                                sw.WriteLine("Dla zer:");
                                sw.WriteLine("1 - " + Seria0[0]);
                                sw.WriteLine("2 - " + Seria0[1]);
                                sw.WriteLine("3 - " + Seria0[2]);
                                sw.WriteLine("4 - " + Seria0[3]);
                                sw.WriteLine("5 - " + Seria0[4]);
                                sw.WriteLine("6+ - " + Seria0[5]);
                                sw.WriteLine("Dla jedynek:");
                                sw.WriteLine("1 - " + Seria1[0]);
                                sw.WriteLine("2 - " + Seria1[1]);
                                sw.WriteLine("3 - " + Seria1[2]);
                                sw.WriteLine("4 - " + Seria1[3]);
                                sw.WriteLine("5 - " + Seria1[4]);
                                sw.WriteLine("6 - " + Seria1[5]);
                                testy[2] = false;
                                WynikRun.Foreground = new SolidColorBrush(Colors.Red);
                                WynikRun.Text = "Test nieudany!";
                            }
                        }
                    }
                    if(testy[0])
                    {
                        sw.WriteLine("Test pojedynczego bitu - zaliczony.");
                        WynikMonoBit.Foreground = new SolidColorBrush(Colors.Green);
                        WynikMonoBit.Text = "Test udany!";
                    }
                    if (testy[1])
                    {
                        sw.WriteLine("Test pokerowy - zaliczony.");
                        WynikPoker.Foreground = new SolidColorBrush(Colors.Green);
                        WynikPoker.Text = "Test udany!";
                    }
                    if (testy[2])
                    {
                        sw.WriteLine("Test serii - zaliczony.");
                        WynikRun.Foreground = new SolidColorBrush(Colors.Green);
                        WynikRun.Text = "Test udany!";
                    }
                    if (testy[3])
                    {
                        sw.WriteLine("Test długiej serii - zaliczony.");
                        WynikLongRun.Foreground = new SolidColorBrush(Colors.Green);
                        WynikLongRun.Text = "Test udany!";
                    }
                    sw.Dispose();
                }
            }

        }

        private void WybierzTest_Click(object sender, RoutedEventArgs e)
        {
            OB.wybierz_plik(PlikTest);
        }

        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            HelpPop("Testy.txt");
        }
    }
}
