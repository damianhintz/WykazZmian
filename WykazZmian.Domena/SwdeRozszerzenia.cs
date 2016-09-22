using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using egib.swde;
using Pragmatic.Kontrakty;

namespace WykazZmian.Domena
{
    public static class SwdeRozszerzenia
    {
        public static Działka WczytajJednąDziałkę(this ObiektSwde dze)
        {
            //var jdr = dze.GetRelacja("G5RJDR");
            //string g5ijr = jdr.GetAtrybut("G5IJR");
            string g5idd = dze.GetAtrybut("G5IDD");
            string g5pew = dze.GetAtrybut("G5PEW");
            string g5dzp = dze.GetAtrybut("G5DZP");
            bool doAra = false;
            if (string.IsNullOrEmpty(g5dzp)) Logger.ostrzeżenie("pusta wartość atrybutu G5DZP: zapis powierzchni do metra: " + g5idd);
            else if (g5dzp.Equals("1")) doAra = false;
            else if (g5dzp.Equals("2")) doAra = true;
            else Logger.ostrzeżenie("niestandardowa wartość atrybutu G5DZP: " + g5dzp + " działki " + g5idd);
            if (doAra && g5pew.EndsWith("00") == false)
            {
                doAra = false;
                Logger.ostrzeżenie("powierzchnia do metra jest oznaczona w opisie jako do ara: " + g5pew + " m2, działka " + g5idd);
            }
            var powierzchnia = Powierzchnia.ParsujMetry(g5pew);
            var id = IdentyfikatorDziałki.ParsujIdG5(g5idd);
            //JednostkaRejestrowa jr = _indeksJednostek[JednostkaRejestrowa.parseG5(g5ijr).ToString()];
            var działka = new Działka(id, powierzchnia);
            działka.JestDoAra = doAra;
            return działka;
        }

        public static string WczytajSygnaturę(this ObiektSwde dok)
        {
            //var dok = dze.GetRelacjaLubNull("G5RPWŁ");
            if (dok == null) return string.Empty;
            var syg = dok.GetAtrybut("G5SYG");
            return syg;
        }

        public static OsobaFizyczna CreateOsobaFizyczna(this ObiektSwde osobaFizyczna)
        {
            string pim = osobaFizyczna.GetAtrybut("G5PIM");
            string dim = osobaFizyczna.GetAtrybut("G5DIM");
            string nzw = osobaFizyczna.GetAtrybut("G5NZW");
            return new OsobaFizyczna(pim, dim, nzw);
        }

        public static Instytucja CreateInstytucja(this ObiektSwde instytucja)
        {
            string nsk = instytucja.GetAtrybut("G5NSK");
            string npe = instytucja.GetAtrybut("G5NPE");
            return new Instytucja(string.IsNullOrEmpty(npe) ? nsk : npe);
        }

        public static Małżeństwo CreateMałżeństwo(this ObiektSwde malzenstwo)
        {
            var zona = malzenstwo.GetRelacja("G5RŻONA");
            var maz = malzenstwo.GetRelacja("G5RMĄŻ");
            return new Małżeństwo(CreateOsobaFizyczna(zona), CreateOsobaFizyczna(maz));
        }

        public static PodmiotGrupowy CreatePodmiotGrupowy(this ObiektSwde podmiotGrupowy)
        {
            string npe = podmiotGrupowy.GetAtrybut("G5NPE");
            string nsk = podmiotGrupowy.GetAtrybut("G5NSK");
            PodmiotGrupowy pg = new PodmiotGrupowy(string.IsNullOrEmpty(npe) ? nsk : npe);
            var podmioty = podmiotGrupowy.GetRelacjaWielokrotna("G5RSKD"); //Składa się
            foreach (var podmiot in podmioty)
            {
                switch (podmiot.Typ)
                {
                    case "G5OSF":
                        pg.dodajPodmiot(CreateOsobaFizyczna(podmiot));
                        break;
                    case "G5INS":
                        pg.dodajPodmiot(CreateInstytucja(podmiot));
                        break;
                    case "G5MLZ":
                        pg.dodajPodmiot(CreateMałżeństwo(podmiot));
                        break;
                    default: Kontrakt.fail(); break;
                }
            }
            return pg;
        }
    }
}
