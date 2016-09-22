using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Pragmatic.Kontrakty;

namespace WykazZmian.Domena
{
    public class RepozytoriumKerg
    {
        public int LiczbaObrębów {  get { return _obręby.Count; } }
        Dictionary<string, string> _obręby = new Dictionary<string, string>();

        public void DodajKergDlaObrębu(string terytObrębu, string kerg) { _obręby.Add(terytObrębu, kerg); }
        public string ZnajdźObręb(string terytObrębu) { return _obręby[terytObrębu]; }
        public bool ZawieraObręb(string terytObrębu) { return _obręby.ContainsKey(terytObrębu); }

        public static RepozytoriumKerg WczytajPlik(string fileName)
        {
            var repozytorium = new RepozytoriumKerg();
            if (!File.Exists(fileName))
            {
                Logger.ostrzeżenie("brak repozytorium KERG.txt");
                return repozytorium; //Zwracamy puste repozytorium
            }
            var lines = File.ReadAllLines(fileName, Encoding.GetEncoding(1250));
            foreach(string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue; //Pomiń puste linie
                var pierwszyZnak = line.TrimStart().First();
                //if (pierwszyZnak == '#') continue; //Pomiń linie z komentarzem
                if (char.IsDigit(pierwszyZnak) == false) continue; //Pomiń linie nie zaczynające się od cyfry
                var pola = line.Split('\t');
                if (pola.Length < 2) continue; //Pomiń linie bez wymaganych kolumn
                string terytObrębu = pola.First();
                string kerg = pola.Last();
                repozytorium.DodajKergDlaObrębu(terytObrębu, kerg);
            }
            return repozytorium;
        }
    }
}
