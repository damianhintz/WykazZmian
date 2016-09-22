using System;
using System.Collections.Generic;
using System.Text;
using Pragmatic.Kontrakty;

namespace WykazZmian.Domena
{
    public class JednostkaRejestrowa
    {
        private string _jewTeryt;
        private string _jewNazwa;
        private string _obrTeryt;
        private string _obrNazwa;
        private string _teryt;
        private string _obreb;
        private string _numer;

        List<AbstrakcyjnyPodmiot> _wlasciciele = new List<AbstrakcyjnyPodmiot>();
        List<AbstrakcyjnyPodmiot> _wladajacy = new List<AbstrakcyjnyPodmiot>();

        public string numer() { return _numer; }
        public string identyfikator() { return string.Format("{0}.{1}.{2}", _teryt, _obreb, _numer); }

        private JednostkaRejestrowa(string teryt, string obreb, string numer)
        {
            Kontrakt.requires(!String.IsNullOrEmpty(teryt), "Numer teryt jednostki rejestrowej jest pusty.");
            Kontrakt.requires(!String.IsNullOrEmpty(obreb), "Numer obrębu jednostki rejestrowej jest pusty.");
            Kontrakt.requires(teryt.Length.Equals(8), "Numer teryt jednostki rejestrowej jest nieprawidłowy: " + teryt);
            Kontrakt.requires(obreb.Length.Equals(4), "Numer obrębu jednostki rejestrowej jest nieprawidłowy: " + obreb);
            Kontrakt.requires(!String.IsNullOrEmpty(numer), "Numer jednostki rejestrowej jest pusty.");
            Kontrakt.requires(numer.StartsWith("G") || numer.StartsWith("B") || numer.StartsWith("L"),
                "Numer jednostki rejestrowej nie zaczyna się od G, B lub L: " + teryt + "." + obreb + "." + numer);
            Kontrakt.forAll(numer.Substring(2), char.IsDigit); //Numer jednostki rejestrowej powinien zawierać cyfry
            _teryt = teryt;
            _obreb = obreb;
            _numer = numer;
            Kontrakt.ensures(teryt.Equals(_teryt));
            Kontrakt.ensures(obreb.Equals(_obreb));
            Kontrakt.ensures(numer.Equals(_numer));
        }

        public IEnumerable<AbstrakcyjnyPodmiot> Właściciele { get { return _wlasciciele; } }
        public IEnumerable<AbstrakcyjnyPodmiot> Władający { get { return _wladajacy; } }
        public void DodajWlasciciela(AbstrakcyjnyPodmiot podmiot) { _wlasciciele.Add(podmiot); }
        public void DodajWladajacego(AbstrakcyjnyPodmiot podmiot) { _wladajacy.Add(podmiot); }

        public string terytJednostki() { return _jewTeryt; }
        public string nazwaJednostki() { return _jewNazwa; }

        public void jednostkaEwidencyjna(string teryt, string nazwa)
        {
            _jewTeryt = teryt;
            _jewNazwa = nazwa;
        }

        public string terytObrebu() { return _obrTeryt; }
        public string nazwaObrebu() { return _obrNazwa; }

        public void obrebEwidencyjny(string teryt, string nazwa)
        {
            _obrTeryt = teryt;
            _obrNazwa = nazwa;
        }

        /// <summary>
        /// D,G5IJR,D,
        /// 142307_2.0001.G00001
        /// 281407_2.0005.B1
        /// </summary>
        /// <param name="g5ijr"></param>
        /// <returns></returns>
        public static JednostkaRejestrowa ParseG5(string g5ijr)
        {
            char[] separator = new char[] { '.' };
            string[] pola = g5ijr.Split(separator);
            int maxFields = 3;
            int minFields = 3;
            Kontrakt.assert(pola.Length >= minFields && pola.Length <= maxFields, "Nieprawidłowy format identyfikatora jednostki rejestrowej: " + g5ijr);
            string numerTeryt = pola[0];
            string numerObrebu = pola[1];
            string numerJednostki = pola[2];
            //int numer = int.Parse(numerJednostki.Replace("G", String.Empty));
            return new JednostkaRejestrowa(numerTeryt, numerObrebu, numerJednostki);
        }

        public override string ToString() { return identyfikator(); }
    }
}
