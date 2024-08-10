using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace valutaváltó
{
    public class Valuta
    {
        public string Nev { get; set; }
        public string Kod { get; set; }
        public double Ertek { get; set; }

        public Valuta(string Nev, string Kod, double Ertek)
        {
            this.Nev = Nev;
            this.Kod = Kod;
            this.Ertek = Ertek;
        }
    }
}

