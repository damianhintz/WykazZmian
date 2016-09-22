using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using Pragmatic.Kontrakty;
using egib.swde;

namespace WykazZmian.Domena
{
    /// <summary>
    /// Czytnik opisowego pliku SWDE.
    /// </summary>
    public class CzytnikOpisowegoSwde : IZawierającyDziałki
    {
        Dictionary<string, Działka> _indeksDziałek = new Dictionary<string, Działka>();
        Dictionary<string, JednostkaRejestrowa> _indeksJednostek = new Dictionary<string, JednostkaRejestrowa>();

        public CzytnikOpisowegoSwde(string fileName)
        {
            DokumentSwde swde = new DokumentSwde(fileName);
            Console.WriteLine("Jednostki rejestrowe...{0}", WczytajJednostkiRejestrowe(swde));
            Console.WriteLine("Właściciele...{0}", WczytajWłaścicieli(swde));
            Console.WriteLine("Władający...{0}", WczytajWładających(swde));
            Console.WriteLine("Działki...{0}", WczytajDziałki(swde));
            Console.WriteLine("Klasoużytki...{0}", WczytajKlasoużytki(swde));
        }

        public Działka SzukajIdDziałki(IdentyfikatorDziałki id)
        {
            if (!_indeksDziałek.ContainsKey(id.Id())) return null;
            return _indeksDziałek[id.Id()];
        }

        public IEnumerator<Działka> GetEnumerator() { return _indeksDziałek.Values.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return this.GetEnumerator(); }

        int WczytajDziałki(DokumentSwde swde)
        {
            var działki = swde.GetObiektyKlasy("G5DZE");
            foreach (var dze in działki)
            {
                try
                {
                    var działka = dze.WczytajJednąDziałkę();
                    var id = działka.Id.ToString();
                    _indeksDziałek.Add(id, działka);
                    //Przypisz jednostkę rejestrową do działki
                    var jdr = dze.GetRelacja("G5RJDR");
                    var g5ijr = jdr.GetAtrybut("G5IJR");
                    var jr = _indeksJednostek[JednostkaRejestrowa.ParseG5(g5ijr).ToString()];
                    działka.jr = jr;
                    //Przypisz kw z podstawy własności
                    działka.kw = dze.GetRelacjaLubNull("G5RPWŁ").WczytajSygnaturę();
                    //Jeżeli nadal brak kw to jeszcze zobaczymy w podstawie władania
                    if (string.IsNullOrEmpty(działka.kw)) działka.kw = dze.GetRelacjaLubNull("G5RPWD").WczytajSygnaturę();
                }
                catch (Exception ex)
                {
                    Logger.błąd("nie można wczytać działki " + dze.GetAtrybut("G5IDD") + ": " + ex.Message);
                }
            }
            return działki.Count();
        }

        int WczytajKlasoużytki(DokumentSwde swde)
        {
            var użytki = swde.GetObiektyKlasy("G5KLU");
            foreach (var klu in użytki)
            {
                string g5ofu = klu.GetAtrybut("G5OFU");
                string g5ozu = klu.GetAtrybut("G5OZU");
                string g5ozk = klu.GetAtrybut("G5OZK");
                string g5pew = klu.GetAtrybut("G5PEW");
                Powierzchnia powierzchnia = Powierzchnia.ParsujMetry(g5pew);
                Klasoużytek klasouzytek = new Klasoużytek(g5ofu, g5ozu, g5ozk, powierzchnia);
                var dze = klu.GetRelacja("G5RDZE");
                string g5idd = dze.GetAtrybut("G5IDD");
                IdentyfikatorDziałki identyfikator = IdentyfikatorDziałki.ParsujIdG5(g5idd);
                var id = identyfikator.ToString();
                if (_indeksDziałek.ContainsKey(id)) _indeksDziałek[id].DodajUżytek(klasouzytek);
                else Logger.ostrzeżenie("nie można dodać użytku " + klasouzytek.ToString() + " do działki " + id);
            }
            return użytki.Count();
        }

        int WczytajJednostkiRejestrowe(DokumentSwde swde)
        {
            var jednostkiRejestrowe = swde.GetObiektyKlasy("G5JDR");
            foreach (var jdr in jednostkiRejestrowe)
            {
                string g5ijr = jdr.GetAtrybut("G5IJR");
                JednostkaRejestrowa jr = JednostkaRejestrowa.ParseG5(g5ijr);
                var obr = jdr.GetRelacja("G5ROBR");
                string obrTeryt = obr.GetAtrybut("G5NRO");
                string obrNazwa = obr.GetAtrybut("G5NAZ");
                jr.obrebEwidencyjny(obrTeryt, obrNazwa);
                var jew = obr.GetRelacja("G5RJEW");
                string jewTeryt = jew.GetAtrybut("G5IDJ");
                string jewNazwa = jew.GetAtrybut("G5NAZ");
                jr.jednostkaEwidencyjna(jewTeryt, jewNazwa);
                _indeksJednostek.Add(jr.ToString(), jr);
            }
            return jednostkiRejestrowe.Count();
        }

        int WczytajWłaścicieli(DokumentSwde swde)
        {
            var wlasciciele = swde.GetObiektyKlasy("G5UDZ");
            foreach (var klu in wlasciciele)
            {
                var jdr = klu.GetRelacja("G5RWŁS"); //Jednostka rejestrowa
                string g5ijr = jdr.GetAtrybut("G5IJR");
                JednostkaRejestrowa jr = _indeksJednostek[JednostkaRejestrowa.ParseG5(g5ijr).ToString()];
                var pod = klu.GetRelacja("G5RPOD"); //Podmiot
                switch (pod.Typ)
                {
                    case "G5OSF":
                        jr.DodajWlasciciela(pod.CreateOsobaFizyczna());
                        break;
                    case "G5INS":
                        jr.DodajWlasciciela(pod.CreateInstytucja());
                        break;
                    case "G5MLZ":
                        jr.DodajWlasciciela(pod.CreateMałżeństwo());
                        break;
                    case "G5OSZ":
                        jr.DodajWlasciciela(pod.CreatePodmiotGrupowy());
                        break;
                    default: Kontrakt.fail(); break;
                }
            }
            return wlasciciele.Count();
        }

        int WczytajWładających(DokumentSwde swde)
        {
            var wladajacy = swde.GetObiektyKlasy("G5UDW");
            foreach (var klu in wladajacy)
            {
                var jdr = klu.GetRelacja("G5RWŁD"); //Jednostka rejestrowa? (opcjonalna)
                string g5ijr = jdr.GetAtrybut("G5IJR");
                JednostkaRejestrowa jr = _indeksJednostek[JednostkaRejestrowa.ParseG5(g5ijr).ToString()];
                var pod = klu.GetRelacja("G5RPOD"); //Podmiot
                switch (pod.Typ)
                {
                    case "G5OSF":
                        jr.DodajWladajacego(pod.CreateOsobaFizyczna());
                        break;
                    case "G5INS":
                        jr.DodajWladajacego(pod.CreateInstytucja());
                        break;
                    case "G5MLZ":
                        jr.DodajWladajacego(pod.CreateMałżeństwo());
                        break;
                    case "G5OSZ":
                        jr.DodajWladajacego(pod.CreatePodmiotGrupowy());
                        break;
                    default: Kontrakt.fail(); break;
                }
            }
            return wladajacy.Count();
        }
    }
}
