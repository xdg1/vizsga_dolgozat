using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace valutaváltó
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 



    public partial class MainWindow : Window
    {
        public MainWindow()
        {

            InitializeComponent();
            API_handling();
            FajlokLekerese();
            Adat_Feltoltes();
            AdatTorles();
        }
        List<Valuta> valutak = new List<Valuta>();
        bool dark_e = false;
        bool online = true;
        DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        string teljesdatum;

        //a from combobox listájának elemeinek rendezése beírt szövegre
        private void from_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (from.Text.Length > 0)
            {
                from.IsDropDownOpen = true;
                foreach (var item in valutak)
                {
                    if (!item.Nev.Contains(from.Text))
                    {
                        for (int i = 0; i < from.Items.Count; i++)
                        {
                            if (item.Nev == from.Items[i].ToString())
                            {
                                from.Items.RemoveAt(i);

                            }
                        }
                    }

                    if (item.Nev.Contains(from.Text) && !from.Items.Contains(item.Nev))
                    {
                        from.Items.Add(item.Nev);

                    }
                }
            }
            if (from.Text.Length == 0)
            {
                from.IsDropDownOpen = false;
                foreach (var item in valutak)
                {
                    if (!from.Items.Contains(item.Nev))
                    {
                        from.Items.Add(item.Nev);

                    }
                }
            }

        }
        //a to combobox listájának elemeinek rendezése beírt szövegre
        private void to_TextChanged(object sender, TextChangedEventArgs e)
        {


            if (to.Text.Length > 0)
            {
                to.IsDropDownOpen = true;
                foreach (var item in valutak)
                {
                    if (!item.Nev.Contains(to.Text))
                    {
                        for (int i = 0; i < to.Items.Count; i++)
                        {
                            if (item.Nev == to.Items[i].ToString())
                            {
                                to.Items.RemoveAt(i);

                            }
                        }
                    }

                    if (item.Nev.Contains(to.Text) && !to.Items.Contains(item.Nev))
                    {
                        to.Items.Add(item.Nev);

                    }
                }

            }
            if (to.Text.Length == 0)
            {
                to.IsDropDownOpen = false;
                foreach (var item in valutak)
                {
                    if (!to.Items.Contains(item.Nev))
                    {
                        to.Items.Add(item.Nev);

                    }
                }
            }

        }






        public void API_handling()
        {
            string url = "http://api.exchangeratesapi.io/v1/latest?access_key=25c75542a4271c267d556bf33d3b0b70";
            List<string> adatok;

            try
            {
                WebRequest wbget = WebRequest.Create(url);
                Stream objStream = wbget.GetResponse().GetResponseStream();
                StreamReader objReader = new StreamReader(objStream);



                while (!objReader.EndOfStream)
                {
                    string adat = objReader.ReadLine();
                    Trace.Write(adat);
                    adatok = adat.Split(',').ToList();
                    adatok[adatok.Count - 1] = adatok[adatok.Count - 1].Substring(0, adatok[adatok.Count - 1].Length - 2);
                    adatok.RemoveAt(0);
                    dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    int timestamp = int.Parse(adatok[0].Split(':')[1]);
                    dt = dt.AddSeconds(timestamp);
                    adatok.RemoveAt(0);
                    adatok.RemoveAt(0);
                    string date = adatok[0].Split(':')[1].Trim('"');
                    teljesdatum = $"{date.Split('-')[0]}{dt.ToString().Substring(4, dt.ToString().Length - 4)}";
                    datum.Content = $"Az adatok utolsó frissítési ideje: {teljesdatum}";
                    adatok.RemoveAt(0);
                    adatok[0] = adatok[0].Split('{')[1];


                    foreach (var item in adatok)
                    {
                        string[] soradat;
                        soradat = item.Split(':');
                        soradat[0] = soradat[0].Substring(1, 3);
                        foreach (string sor in File.ReadAllLines("currency_names.csv").Skip(1))
                        {
                            if (soradat[0] == sor.Split(',')[0])
                            {
                                Valuta uj = new Valuta(sor.Split(',')[1].Trim('"'), soradat[0], double.Parse(soradat[1].Replace('.', ',')));
                                valutak.Add(uj);
                            }
                        }
                    }
                }
                //Adatok logolása
                string fajlnev = teljesdatum.Substring(0, 12).Replace('.', '-');
                fajlnev = fajlnev.Remove(5, 1);
                fajlnev = fajlnev.Remove(8, 1);

                StreamWriter sw = new StreamWriter($"log/{fajlnev}.csv");
                sw.WriteLine($"{teljesdatum}");
                sw.WriteLine("Nev;Kod;Ertek");
                foreach (var item in valutak)
                {
                    sw.WriteLine($"{item.Nev};{item.Kod};{item.Ertek}");
                }
                sw.Flush();
                sw.Close();
            }
            // ha nincs internet kapcsolat akkor az utolsó csv fájlból olvassa az adatokat
            catch (Exception)
            {
                title.Text = "     Valutaváltó - Offline";
                FajlokLekerese();

                MessageBox.Show("Nincs internet kapcsolat! Az alkalmazás offline üzemmódban indul!");

                int i = 0;
                foreach (var item in File.ReadAllLines($"log/{adatokcombo.Items[0].ToString()}.csv"))
                {
                    if (i == 0)
                    {
                        datum.Content = $"Az adatok utolsó frissítési ideje: {item}";
                    }

                    if (i >= 2)
                    {
                        Valuta uj = new Valuta(item.Split(';')[0], item.Split(';')[1], double.Parse(item.Split(';')[2]));
                        valutak.Add(uj);
                    }
                    i++;
                }
                online = false;
            }





        }


        //Számolás
        private void calculate_Click(object sender, RoutedEventArgs e)
        {
            Valuta from_valuta;
            Valuta to_valuta;
            double vegeredmeny;
            double seged;
            string seged_text;

            if (from_ertek.Text != null)
            {
                if (from.SelectedIndex != -1 && to.SelectedIndex != -1)
                {

                    from_valuta = valutak.Find(x => x.Nev == from.SelectedItem.ToString());
                    to_valuta = valutak.Find(x => x.Nev == to.SelectedItem.ToString());

                    //ha vesszővel van elválasztva
                    if (from_ertek.Text.Contains(","))
                    {

                        if (from_valuta.Ertek > to_valuta.Ertek)
                        {
                            seged = to_valuta.Ertek / from_valuta.Ertek;
                            vegeredmeny = seged * double.Parse(from_ertek.Text);
                            to_ertek.Text = Math.Round(vegeredmeny, 3).ToString();
                        }
                        if (from_valuta.Ertek < to_valuta.Ertek)
                        {
                            seged = to_valuta.Ertek / from_valuta.Ertek;
                            vegeredmeny = seged * double.Parse(from_ertek.Text);
                            to_ertek.Text = Math.Round(vegeredmeny, 3).ToString();
                        }
                        if (from_valuta.Ertek == to_valuta.Ertek)
                        {
                            to_ertek.Text = from_ertek.Text;
                        }
                    }
                    //ha pont akkor azt replacelje, mert a c# a tört számokat ,-vel használja
                    if (from_ertek.Text.Contains("."))
                    {
                        seged_text = from_ertek.Text;
                        seged_text = seged_text.Replace('.', ',');

                        if (from_valuta.Ertek > to_valuta.Ertek)
                        {
                            seged = to_valuta.Ertek / from_valuta.Ertek;
                            vegeredmeny = seged * double.Parse(seged_text);
                            to_ertek.Text = Math.Round(vegeredmeny, 3).ToString();

                        }
                        if (from_valuta.Ertek < to_valuta.Ertek)
                        {
                            seged = to_valuta.Ertek / from_valuta.Ertek;
                            vegeredmeny = seged * double.Parse(seged_text);
                            to_ertek.Text = Math.Round(vegeredmeny, 3).ToString();

                        }
                        if (from_valuta.Ertek == to_valuta.Ertek)
                        {
                            to_ertek.Text = seged_text;
                        }
                    }
                    //ha egész szám
                    if (!from_ertek.Text.Contains(".") && !from_ertek.Text.Contains(","))
                    {
                        if (from_valuta.Ertek > to_valuta.Ertek)
                        {
                            seged = to_valuta.Ertek / from_valuta.Ertek;
                            vegeredmeny = seged * double.Parse(from_ertek.Text);
                            to_ertek.Text = Math.Round(vegeredmeny, 3).ToString();
                        }
                        if (from_valuta.Ertek < to_valuta.Ertek)
                        {
                            seged = to_valuta.Ertek / from_valuta.Ertek;
                            vegeredmeny = seged * double.Parse(from_ertek.Text);
                            to_ertek.Text = Math.Round(vegeredmeny, 3).ToString();
                        }
                        if (from_valuta.Ertek == to_valuta.Ertek)
                        {
                            to_ertek.Text = from_ertek.Text;
                        }
                    }
                }
            }
        }

        private void from_ertek_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Csak számok, vessző, illetve pontot fogadjon el a textbox
            char[] elfogadott_karakterek = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ',', '.' };
            bool elfogadott = false;

            if (from_ertek.Text.Length != 0)
            {
                char beutott_karakter = from_ertek.Text[from_ertek.Text.Length - 1];

                foreach (char c in elfogadott_karakterek)
                {
                    if (beutott_karakter == c)
                    {
                        elfogadott = true;
                    }
                }
                if (elfogadott == false)
                {
                    from_ertek.Text = from_ertek.Text.Substring(0, from_ertek.Text.Length - 1);
                }
            }

            //első karakter ne lehessen . vagy ,
            if (from_ertek.Text.Length == 1)
            {
                char beutott_karakter = from_ertek.Text[from_ertek.Text.Length - 1];
                if (beutott_karakter == '.' || beutott_karakter == ',')
                {
                    from_ertek.Text = from_ertek.Text.Substring(0, from_ertek.Text.Length - 1);
                }
            }

            //Ne lehessen több . vagy , 
            if (from_ertek.Text.ToCharArray().Count(x => x == '.') + from_ertek.Text.ToCharArray().Count(x => x == ',') > 1)
            {
                from_ertek.Text = from_ertek.Text.Substring(0, from_ertek.Text.Length - 1);
            }


            from_ertek.Select(from_ertek.Text.Length, 0);
        }




        //Az X gombra kattintva bezárja az alkalmazást
        private void close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        //Letálcázás
        private void hide_Click(object sender, RoutedEventArgs e)
        {
            ablak.WindowState = WindowState.Minimized;
        }

        //a title bar-nál fogva lehessen mozgatni az ablakot
        private void nav_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }




        private void DarkMode_Click(object sender, RoutedEventArgs e)
        {
            //Dark Mode
            if (dark_e == false)
            {
                dark_e = true;
                ablak.Background = (Brush)new BrushConverter().ConvertFrom("#181818");

                from_ertek.Background = (Brush)new BrushConverter().ConvertFrom("#282828");
                from_ertek.Foreground = Brushes.Wheat;
                from_ertek.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#121212");

                to_ertek.Background = (Brush)new BrushConverter().ConvertFrom("#282828");
                to_ertek.Foreground = Brushes.Wheat;
                to_ertek.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#121212");

                calculate.Foreground = Brushes.Wheat;
                calculate.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#121212");
                darkmodekep.Source = new BitmapImage(new Uri(@"/Resources/dark_mode_dark.png", UriKind.Relative));
                DarkMode.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#181818");
                DarkMode.Background = (Brush)new BrushConverter().ConvertFrom("#181818");
                hidekep.Source = new BitmapImage(new Uri(@"/Resources/minimize-button-dark.png", UriKind.Relative));
                closekep.Source = new BitmapImage(new Uri(@"/Resources/close-button-dark.png", UriKind.Relative));
                hide.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#282828");
                hide.Background = (Brush)new BrushConverter().ConvertFrom("#282828");
                close.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#282828");
                close.Background = (Brush)new BrushConverter().ConvertFrom("#282828");

                datum.Foreground = Brushes.Wheat;
                atvaltas.Foreground = Brushes.Wheat;
                nav.Background = (Brush)new BrushConverter().ConvertFrom("#282828");
                title.Foreground = Brushes.Wheat;
                calculate.Background = (Brush)new BrushConverter().ConvertFrom("#84092C");
                AdatFrissites.Background = (Brush)new BrushConverter().ConvertFrom("#84092C");

                from.Foreground = (Brush)new BrushConverter().ConvertFrom("#ba8218");
                to.Foreground = (Brush)new BrushConverter().ConvertFrom("#ba8218");
                //from combobox
                var textbox = (TextBox)from.Template.FindName("PART_EditableTextBox", from);

                if (textbox != null)
                {
                    var parent = (Border)textbox.Parent;
                    parent.Background = (Brush)new BrushConverter().ConvertFrom("#282828");
                }

                from.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#121212");


                //to combobox
                textbox = (TextBox)to.Template.FindName("PART_EditableTextBox", to);

                if (textbox != null)
                {
                    var parent = (Border)textbox.Parent;
                    parent.Background = (Brush)new BrushConverter().ConvertFrom("#282828");
                }

                to.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#121212");
                //adatokcombo 
                textbox = (TextBox)adatokcombo.Template.FindName("PART_EditableTextBox", adatokcombo);

                if (textbox != null)
                {
                    var parent = (Border)textbox.Parent;
                    parent.Background = (Brush)new BrushConverter().ConvertFrom("#282828");
                }

                adatokcombo.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#121212");

                return;
            }
            if (dark_e == true)
            {
                //vissza light mode
                dark_e = false;
                ablak.Background = (Brush)new BrushConverter().ConvertFrom("#FFFFFF");
                //Textboxok
                from_ertek.Background = (Brush)new BrushConverter().ConvertFrom("#FFFFFF");
                from_ertek.Foreground = (Brush)new BrushConverter().ConvertFrom("#80590f");
                from_ertek.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#B3B3B3");

                to_ertek.Background = (Brush)new BrushConverter().ConvertFrom("#FFFFFF");
                to_ertek.Foreground = (Brush)new BrushConverter().ConvertFrom("#80590f");
                to_ertek.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#B3B3B3");



                calculate.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#B3B3B3");
                darkmodekep.Source = new BitmapImage(new Uri(@"/Resources/dark_mode_light.png", UriKind.Relative));
                hidekep.Source = new BitmapImage(new Uri(@"/Resources/minimize-button-light.png", UriKind.Relative));
                closekep.Source = new BitmapImage(new Uri(@"/Resources/close-button-light.png", UriKind.Relative));
                hide.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#D2D2D2");
                hide.Background = (Brush)new BrushConverter().ConvertFrom("#D2D2D2");
                close.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#D2D2D2");
                close.Background = (Brush)new BrushConverter().ConvertFrom("#D2D2D2");
                DarkMode.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#FFFFFF");
                DarkMode.Background = (Brush)new BrushConverter().ConvertFrom("#FFFFFF");

                datum.Foreground = (Brush)new BrushConverter().ConvertFrom("#80590f");

                atvaltas.Foreground = Brushes.Wheat;
                nav.Background = (Brush)new BrushConverter().ConvertFrom("#D2D2D2");
                title.Foreground = (Brush)new BrushConverter().ConvertFrom("#80590f");
                calculate.Background = (Brush)new BrushConverter().ConvertFrom("#C70039");
                AdatFrissites.Background = (Brush)new BrushConverter().ConvertFrom("#C70039");

                from.Foreground = (Brush)new BrushConverter().ConvertFrom("#ba8218");
                to.Foreground = (Brush)new BrushConverter().ConvertFrom("#ba8218");

                var textbox = (TextBox)from.Template.FindName("PART_EditableTextBox", from);

                if (textbox != null)
                {
                    var parent = (Border)textbox.Parent;
                    parent.Background = (Brush)new BrushConverter().ConvertFrom("#FFFFFF");
                }

                from.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#121212");


                //to combobox
                textbox = (TextBox)to.Template.FindName("PART_EditableTextBox", to);

                if (textbox != null)
                {
                    var parent = (Border)textbox.Parent;
                    parent.Background = (Brush)new BrushConverter().ConvertFrom("#FFFFFF");
                }

                to.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#121212");
                //adatokcombo 
                textbox = (TextBox)adatokcombo.Template.FindName("PART_EditableTextBox", adatokcombo);

                if (textbox != null)
                {
                    var parent = (Border)textbox.Parent;
                    parent.Background = (Brush)new BrushConverter().ConvertFrom("#FFFFFF");
                }

                adatokcombo.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#121212");
            }


        }
        //két combobox feltöltése
        private void Adat_Feltoltes()
        {
            foreach (var item in valutak)
            {
                if (from != null)
                {
                    from.Items.Add(item.Nev);
                }
                if (to != null)
                {
                    to.Items.Add(item.Nev);
                }
            }
        }

        //Manuális adatfrissítés
        private void AdatFrissites_Click(object sender, RoutedEventArgs e)
        {
            API_handling();
            FajlokLekerese();
            Adat_Feltoltes();
        }

        //A log fájlok lekérése
        private void FajlokLekerese()
        {
            string[] fajlok = Directory.GetFiles("log/", "*.csv", SearchOption.TopDirectoryOnly);
            List<string> fajlnevek = new List<string>();
            foreach (string sor in fajlok)
            {
                fajlnevek.Add(System.IO.Path.GetFileName(sor));
            }
            foreach (var item in fajlnevek)
            {
                adatokcombo.Items.Add(item.Split('.')[0]);
            }
        }

        //Adatfrissítés, adatbázis váltás esetén
        private void adatokcombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (adatokcombo.SelectedItem != null)
            {
                string fajlnev = $"{adatokcombo.SelectedItem.ToString()}.csv";
                valutak.Clear();
                from.Items.Clear();
                to.Items.Clear();
                StreamReader sr = new StreamReader($"log/{fajlnev}");
                datum.Content = $"Az adatok utolsó frissítési ideje: {sr.ReadLine()}";
                sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    string sor = sr.ReadLine();
                    Valuta uj = new Valuta(sor.Split(';')[0], sor.Split(';')[1], double.Parse(sor.Split(';')[2]));
                    valutak.Add(uj);
                }
                sr.Close();
            }
            Adat_Feltoltes();
        }

        private void from_DropDownOpened(object sender, EventArgs e)
        {
            TextBox from_textbox = (TextBox)from.Template.FindName("PART_EditableTextBox", from);


            from_textbox.Select(from_textbox.Text.Length, 0);

        }

        private void to_DropDownOpened(object sender, EventArgs e)
        {
            TextBox to_textbox = (TextBox)to.Template.FindName("PART_EditableTextBox", to);

            to_textbox.Select(to_textbox.Text.Length, 0);
        }

        private void AdatTorles()
        {
            List<string> fajlok = Directory.GetFiles("log/", "*.csv", SearchOption.TopDirectoryOnly).ToList();
            fajlok.Sort();
            if (fajlok.Count > 15)
            {
                File.Delete($"log/{fajlok[0]}.csv");
            }
        }
    }
}


