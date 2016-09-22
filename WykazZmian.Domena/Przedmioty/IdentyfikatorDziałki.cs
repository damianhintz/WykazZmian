using System;
using Pragmatic.Kontrakty;

namespace WykazZmian.Domena
{
    //TODO Porównywanie identyfikatorów!
    public class IdentyfikatorDziałki
    {
        private string _numerObrębu;
        private string _numerDziałki;

        public IdentyfikatorDziałki(string numerObrębu, string numerDziałki)
        {
            Kontrakt.requires(!String.IsNullOrEmpty(numerObrębu), "Numer obrębu jest pusty.");
            foreach (var c in numerObrębu) Kontrakt.requires(char.IsDigit(c), "Numer obrębu nie składa się z cyfr:" + numerObrębu);
            Kontrakt.requires(!String.IsNullOrEmpty(numerDziałki), "Numer działki jest pusty.");
            Kontrakt.requires(char.IsDigit(numerDziałki[0]),
                "Numer działki " + numerDziałki + " nie zaczyna się od cyfry.");
            _numerObrębu = numerObrębu;
            _numerDziałki = numerDziałki;
            Kontrakt.ensures(_numerObrębu.Equals(numerObrębu));
            Kontrakt.ensures(_numerDziałki.Equals(numerDziałki));
        }

        public string Id() { return string.Format("{0}-{1}", _numerObrębu, _numerDziałki); }
        public string NumerObrębu() { return _numerObrębu; }
        public string NumerDziałki() { return _numerDziałki; }
        public override string ToString() { return Id(); }

        public static IdentyfikatorDziałki ParsujId(string id)
        {
            //<idobr>-<idd>
            char[] separator = new char[] { '-' };
            int maxFields = 2;
            string[] pola = id.Split(separator, maxFields);
            string numerObrebu = String.Empty;
            string numerDzialki = pola[0];
            if (pola.Length > 1)
            {
                numerObrebu = pola[0];
                numerDzialki = pola[1];
            }
            return new IdentyfikatorDziałki(numerObrebu, numerDzialki);
        }

        public static IdentyfikatorDziałki ParsujIdG5(string g5idd)
        {
            //D,G5IDD,D,142302_2.0001.296
            //D,G5IDD,D,142302_2.0007.AR_2.656
            //<teryt>.<obr>.[arkusz].<idd>
            char[] separator = new char[] { '.' };
            string[] pola = g5idd.Split(separator);
            int maxFields = 4;
            int minFields = 3;
            Kontrakt.assert(pola.Length >= minFields && pola.Length <= maxFields);
            string numerTeryt = pola[0];
            string numerObrebu = byte.Parse(pola[1]).ToString();
            string numerDzialki = pola[pola.Length - 1];
            string numerArkusza = string.Empty;
            if (pola.Length == 4) numerArkusza = pola[2].Replace("AR_", "");
            if (!string.IsNullOrEmpty(numerArkusza)) numerDzialki = numerArkusza + "." + numerDzialki;
            return new IdentyfikatorDziałki(numerObrebu, numerDzialki);
        }

    }
}
