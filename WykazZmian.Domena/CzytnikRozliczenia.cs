using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using Pragmatic.Kontrakty;

namespace WykazZmian.Domena
{
    public class CzytnikRozliczenia
    {
        readonly Encoding _kodowanie = Encoding.GetEncoding(1250);
        readonly Rozliczenie _rozliczenie;
        readonly SłownikOznaczenia _słownik;

        public CzytnikRozliczenia(Rozliczenie rozliczenie, SłownikOznaczenia słownik)
        {
            _rozliczenie = rozliczenie;
            _słownik = słownik;
        }

        public void Wczytaj(string fileName)
        {
            //_klu.read(Path.Combine(Path.GetDirectoryName(fileName), "uzytkiG5.csv"));
            var lines = File.ReadAllLines(path: fileName, encoding: _kodowanie);
            var header = lines.First();
            Kontrakt.assert(
                header.Equals("Rozliczenie konturów klasyfikacyjnych na działkach [m^2]"),
                "Nieprawidłowy nagłówek pliku z rozliczeniem konturów.");
            var wczytaneDziałki = 0;
            var rekordDziałki = new List<string>();
            for (int i = 1; i < lines.Length; i++)
            {
                var trimmedLine = lines[i].Trim();
                if (KoniecRekorduDziałki(rekordDziałki, trimmedLine))
                {
                    WczytajRekordDziałki(_rozliczenie, rekordDziałki);
                    wczytaneDziałki++;
                }
                else
                {
                    //Kumulacja rekordu działki wraz z użytkami.
                    if (!String.IsNullOrEmpty(trimmedLine)) rekordDziałki.Add(trimmedLine);
                }
            }
            Kontrakt.ensures(rekordDziałki.Count == 0, "Nieprawidłowo zakończony rekord działki");
            Kontrakt.ensures(_rozliczenie.Any());
            Kontrakt.ensures(wczytaneDziałki == _rozliczenie.Count());
        }

        void WczytajRekordDziałki(Rozliczenie rozliczenie, List<string> rekordDziałki)
        {
            var liniaDziałki = rekordDziałki[0]; //1-1                      19857
            var działka = ParsujDziałkę(liniaDziałki);
            for (int i = 1; i < rekordDziałki.Count; i++) //Pomijamy pierwszy rekord reprezentujący działkę.
            {
                var liniaUżytku = rekordDziałki[i]; //  LsVI                    3987
                var użytek = ParsujKlasoużytek(liniaUżytku);
                działka.DodajUżytek(użytek);
            }
            rozliczenie.DodajDziałkę(działka);
            rekordDziałki.Clear(); //Rozpoczęcie kumulacji nowego rekordu.
        }

        bool KoniecRekorduDziałki(List<string> rekord, string linia) { return string.IsNullOrEmpty(linia) && rekord.Count > 0; }

        Działka ParsujDziałkę(string linia)
        {
            var pola = linia.Split(new [] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            var identyfikator = IdentyfikatorDziałki.ParsujId(pola[0]);
            var powierzchnia = Powierzchnia.ParsujMetry(pola[1]);
            var działka = new Działka(identyfikator, powierzchnia);
            return działka;
        }

        Klasoużytek ParsujKlasoużytek(string linia)
        {
            var pola = linia.Split(new [] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            var ozn = pola.First();
            var powierzchnia = Powierzchnia.ParsujMetry(pola[1]);
            var oznaczenie = _słownik.SzukajOznaczenia(ozn);
            var użytek = new Klasoużytek(oznaczenie.ofu, oznaczenie.ozu, oznaczenie.ozk, powierzchnia);
            //Kontrakt.ensures(oznaczenie.Equals(ozn), 
            //    "Odtworzenie oznaczenia nie jest możliwe: " + oznaczenie + " z " +
            //    użytek.ToString() + " = " + ozn);
            return użytek;
        }
    }
}
