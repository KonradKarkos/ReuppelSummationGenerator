using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Generator_Samodecymujacy
{
    //Głównym celem utworzenia tej klasy było uporządkowanie kodu i uczynienie go bardziej czytelnym
    class ObslugaPlikow
    {
        public String rozszerzenie { get; set; } = ".txt";
        public String[] pliki { get; set; } = { "KodAuto.txt", "KodRecz.txt", "OnOffKonf.txt", "WatrtosciKonf.txt", "Jawny.txt", "Klucz.txt", "Zaszyfrowany.txt", "Zaszyfrowany.txt", "KluczDoOdszyfrowania.txt", "Odszyfrowany.txt" };
        public ObslugaPlikow()
        {
            char c = (char)10;
            Directory.CreateDirectory("Help");
            if (!File.Exists("Help\\Generator.txt"))
            {
                StreamWriter sw = new StreamWriter("Help\\Generator.txt");
                sw.Write("Generator samodecymujący polega na samotaktującym się rejestrze LFSR, który podaje na wyjście zawsze ostatni bit rejestru zamieniając jednocześnie wartości bitów na wartości bitów poprzedzających. Wartość pierwszego bitu jest zamieniana na modulo 2 z sumy wartości włączonych bitów. W przypadku podania na wyjście 0 program domyślnie czeka 5 cykli przed sprawdzeniem kolejnej wartości. W przypadku podania na wyjście 1 program zapisuje do pliku wartość z wyjścia domyślnie po 10 cyklach." + c +
                "W zakładce \"Automatyczne\" można dowolnie ustawić 12 bitów z pomocą prostego interfejsu." + c +
                "W zakładce \"Ręczne\" można ustawić dowolnej długości ciąg bitów wpisując ciągi złożone z 0 i 1 w obu wyznaczonych miejscach. Wszelkie inne symbole zostaną odrzucone, a oba ciągi 0 i 1 muszą być tej samej długości." + c +
                "Przycisk 'Stop' w zakładce \"Automatyczne\" ustawia wszystkie bity na 'On' oraz ich wartość na 1. W zakładce \"Ręczne\" wczytuje od nowa ciąg z wypełnionych pól.");
                sw.Dispose();
            }
            if (!File.Exists("Help\\Szyfrator.txt"))
            {
                StreamWriter sw = new StreamWriter("Help\\Szyfrator.txt");
                sw.Write("Zakładka \"Szyfrator\" obsługuje szyfrowanie ciągu jawnego za pomocą klucza złożonego z 0 i 1 (szyfrator strumieniowy) wykonując exclusive or na odpowiadających bitach. Ciąg wyjściowy jest podawany także w foramcie 0/1." + c +
                "Zakładka \"Deszyfrator\" obsługuje deszyfrowanie ciągu 0/1 za pomocą exclusive or z podanego ciągu 0/1 i klucza. Zarówno szyfrator jak i deszyfrator obsługują tylko znaki zgodne z ASCII");
                sw.Dispose();
            }
            if (!File.Exists("Help\\Testy.txt"))
            {
                StreamWriter sw = new StreamWriter("Help\\Testy.txt");
                sw.Write("W zakładce \"Testy\" po wyborze pliku i kliknięciu przycisku \"Start\" wybrany plik zostanie podzielony na sekwencje o długości 20000 znaków (lub o długości 2500 jeśli jest to plik binarny), na których zostaną wykonane testy zgodne ze standardem FIPS 140-2."+c+
                    "Test pojedynczego bitu, który sprawdza ilość zer i jedynek w każdym ciągu, jeśli którakolwiek z tych składowych stanowi więcej niż 51,375% długości ciągu to test zostanie niezaliczony."+c+
                    "Test pokerowy bazuje na sprawdzeniu ilości cyfr 4-bitowych w ciągu. Odbywa się poprzez podzielenie ciągu na 5000 4 bitowych ciągów, przekonwertowanie ich na liczby dziesiętne i zliczenie ilości wystąpień każdej z nich. Współczynnik testu pokerowego X wylicza się ze wzoru X = (16/5000) * (SUM i=0 -> i=15 [f(i)]^2) - 5000, gdzie f(i) to ilość wystąpień kolejnych liczb. Jeśli współczynnik X nie mieści się w przedziale (2.16, 46.17) to test pozostaje niezaliczony."+c+
                    "Test serii zlicza ilość ciągów zer i jedynek o długościach 1,2,3,4,5 oraz 6+, które muszą mieścić się w podanych przedziałach: "+c+
                    "1  2315 - 2685"+c+
                    "2  1114 - 1386" + c+
                    "3  527 - 723" + c+
                    "4  240 - 384" + c+
                    "5  103 - 209" + c+
                    "6+ 103 - 209"+c+
                    "W innym wypadku test zostanie niezaliczony."+c+
                    "Test długiej serii zostanie niezaliczony kiedy wystąpi chociaż jeden ciąg zer i jedynek o długości większej lub równej niż 23.");
                sw.Dispose();
            }
        }
        //funckja dodająca odpowiednie rozszerzenie do plików wprowadzonych w opcjach po kliknięciu "Zastosuj"
        public String dodaj_rozszerzenie(TextBox nazwa, bool wynikowy)
        {
            String linie = nazwa.Text;
            String koniec;
            if (linie.Length > 4)
            {
                koniec = linie.Substring(linie.Length - 4);
            }
            else
            {
                koniec = linie;
            }
            if (!koniec.Equals(".txt") && wynikowy == false)
            {
                linie += ".txt";
            }
            //zamiana rozszerzenia na bin jeśli została wybrana, a plik jest plikiem zapisu wygenerowanego kodu lub wczytania klucza
            else if (!koniec.Equals(rozszerzenie) && wynikowy == true)
            {
                if (koniec.Equals(".txt") || koniec.Equals(".bin")) linie = linie.Substring(0, linie.Length - 4);
                linie += rozszerzenie;
            }
            nazwa.Text = linie;
            return linie;
        }
        //Metoda odpowedzialna za otowrzenie eksploratora plików po kliknięciu któregoś z przycisków "wybierz"
        public void wybierz_plik(TextBox tab)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Pliki tekstowe (*.txt)|*.txt|Pliki binarne (*.bin)|*.bin";
            if (openFileDialog.ShowDialog() == true)
            {
                tab.Text = openFileDialog.FileName;
            }
        }
        //wczytanie tekstu jawnego/do odszyfrowania i klucza do szyfratora jeśli zostały zapisane. Plik z kodem do odszyfrowania może być zapisany binarnie
        public void wczytaj_pliki_szyfratora(int plik1, int plik2, TextBox Pierwszy, TextBox Drugi, bool deszyfrator)
        {
            if (File.Exists(pliki[plik1]) && File.Exists(pliki[plik2]))
            {
                StringBuilder sb = new StringBuilder();
                //w przypadku deszyfratora tekst do odszyfrowania może być zapisany binarnie
                if (!deszyfrator || (deszyfrator && rozszerzenie.Equals(".txt")))
                {
                    StreamReader s = new StreamReader(pliki[plik1]);
                    String linia;
                    while ((linia = s.ReadLine()) != null)
                    {
                        sb.Append(linia);
                    }
                    Pierwszy.Text = sb.ToString();
                    sb.Clear();
                    s.Dispose();
                }
                else
                {
                    BinaryReader br = new BinaryReader(File.Open(pliki[plik1], FileMode.Open));
                    int dlugosc = (int)br.BaseStream.Length;
                    while (dlugosc > 0)
                    {
                        sb.Append(Convert.ToString(br.ReadByte(), 2));
                        dlugosc--;
                    }
                    Pierwszy.Text = sb.ToString();
                    br.Dispose();
                }
                //sposób wczytywania klucza jest zależny od rozszerzenia
                if (rozszerzenie.Equals(".txt"))
                {
                    StreamReader s = new StreamReader(pliki[plik2]);
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
                    Drugi.Text = sb.ToString();
                }
                else
                {
                    BinaryReader br = new BinaryReader(File.Open(pliki[plik2], FileMode.Open));
                    int dlugosc = (int)br.BaseStream.Length;
                    while (dlugosc > 0)
                    {
                        sb.Append(Convert.ToString(br.ReadByte(), 2));
                        dlugosc--;
                    }
                    Drugi.Text = sb.ToString();
                    br.Dispose();
                }
            }
            else
            {
                MessageBox.Show("Nie istnieje przynajmniej jeden z plików konfiguracyjnych. Wczytywanie wstrzymane.");
            }
        }
        //metoda do zapisywania zawartości textbox'ów z zakładek szyfrator i desztfrator
        public void zapisz_pliki_szyfratora(int plik1, int plik2, int plik3, TextBox Pierwszy, TextBox Drugi, TextBox Trzeci)
        {
            StreamWriter s = new StreamWriter(pliki[plik1]);
            if (Pierwszy.LineCount > 0)
            {
                for (int i = 0; i < Pierwszy.LineCount; i++)
                {
                    s.Write(Pierwszy.GetLineText(i));
                }
            }
            s.Dispose();
            //wybrane rozszerzenie ma wpływ na zapis zaszyfrowanego/odszyfrowanego ciągu i klucza
            if (rozszerzenie.Equals(".txt"))
            {
                s = new StreamWriter(pliki[plik2]);
                if (Drugi.LineCount > 0)
                {
                    for (int i = 0; i < Drugi.LineCount; i++)
                    {
                        s.Write(Drugi.GetLineText(i));
                    }
                }
                s.Dispose();
                s = new StreamWriter(pliki[plik3]);
                if (Trzeci.LineCount > 0)
                {
                    for (int i = 0; i < Trzeci.LineCount; i++)
                    {
                        s.Write(Trzeci.GetLineText(i));
                    }
                }
                s.Dispose();
            }
            else
            {
                int iloscbajtow = Drugi.Text.Length / 8;
                Byte[] bajty = new Byte[iloscbajtow];
                for (int i = 0; i < iloscbajtow; i++)
                {
                    bajty[i] = Convert.ToByte(Drugi.Text.Substring(8 * i, 8), 2);
                }
                File.WriteAllBytes(pliki[plik2], bajty);
                iloscbajtow = Trzeci.Text.Length / 8;
                bajty = new Byte[iloscbajtow];
                for (int i = 0; i < iloscbajtow; i++)
                {
                    bajty[i] = Convert.ToByte(Trzeci.Text.Substring(8 * i, 8), 2);
                }
                File.WriteAllBytes(pliki[plik3], bajty);
            }
        }
        //metoda wczytująca konfigurację z plików do zakładki "Ręczne"
        public void Wczytaj_Konf_Recz(TextBox textBox, TextBox textBox1)
        {
            if (File.Exists(pliki[2]) && File.Exists(pliki[3]))
            {
                StreamReader s = new StreamReader(pliki[2]);
                String linia;
                StringBuilder sb = new StringBuilder();
                while ((linia = s.ReadLine()) != null)
                {
                    //pętla wczytująca tylko odpowiednie dane na wypadek gdyby ktoś chciał wczytać plik wydedytowany poza programem
                    foreach (char c in linia)
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
        //metoda zapisująca konfigurację do plików z zakładki "Ręczne"
        public void Zapisz_Konf_Recz(TextBox textBox, TextBox textBox1)
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
        public String Wczytaj_Pomoc(String plik)
        {
            char c = (char)10;
            StringBuilder sb = new StringBuilder();
            StreamReader sr = new StreamReader("Help\\"+plik);
            String linia;
            while ((linia = sr.ReadLine()) != null)
            {
                sb.Append(linia + c);
            }
            return sb.ToString();
        }
    }
}
