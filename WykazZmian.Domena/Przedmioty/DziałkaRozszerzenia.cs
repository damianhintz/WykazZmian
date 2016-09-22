using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pragmatic.Kontrakty;

namespace WykazZmian.Domena
{
    public static class DziałkaRozszerzenia
    {
        public static bool uległaZmianie(this Działka staraDzialka, Działka nowaDzialka)
        {
            if (!nowaDzialka.Powierzchnia.Equals(staraDzialka.Powierzchnia)) return true; //niezgodna powierzchnia działki
            if (nowaDzialka.Count() != staraDzialka.Count()) return true; //niezgodna liczba użytków
            var użytki = staraDzialka.UnionUżytki(nowaDzialka);
            foreach (var użytek in użytki)
            {
                Klasoużytek stary = staraDzialka.SzukajUżytku(użytek);
                Klasoużytek nowy = nowaDzialka.SzukajUżytku(użytek);
                if (stary == null || nowy == null) return true; //dodany/usunięty użytek
                Powierzchnia stara = stary.powierzchnia();
                Powierzchnia nowa = nowy.powierzchnia();
                if (!nowa.Equals(stara)) return true; //inna powierzchnia użytków
                //if (nowa.powyzejOdchylki(stara)) return true; //powyżej odchyłki
                Kontrakt.assert(stary.Equals(nowy), "Różne oznaczenie użytku: " + stary.ToString());
            }
            return false;
        }
    }
}
