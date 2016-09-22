using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using Pragmatic.Kontrakty;

namespace WykazZmian.Domena
{
    public class Rozliczenie : IZawierającyDziałki
    {
        Dictionary<string, Działka> _indeksDziałek = new Dictionary<string, Działka>();
        List<Działka> _działki = new List<Działka>();
        
        public void DodajDziałkę(Działka działka)
        {
            if (działka == null) throw new ArgumentNullException(paramName: "działka", message: "Działka jest null.");
            if (działka.Any() == false) throw new InvalidOperationException(message: "Działka nie zawiera żadnych klasoużytków.");
            if (SzukajIdDziałki(działka.Id) != null) throw new InvalidOperationException("Rozliczenie zawiera już działkę " + działka.Id);
            int dzialkiPrzed = _działki.Count;
            _działki.Add(działka);
            var id = działka.Id;
            _indeksDziałek.Add(id.ToString(), działka);
            int dzialkiPo = _działki.Count;
            Kontrakt.ensures(dzialkiPrzed + 1 == dzialkiPo);
            Kontrakt.ensures(SzukajIdDziałki(id) != null);
        }

        public IEnumerator<Działka> GetEnumerator() { return _działki.GetEnumerator(); }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public Działka SzukajIdDziałki(IdentyfikatorDziałki id) { return _indeksDziałek.ContainsKey(id.ToString()) ? _indeksDziałek[id.Id()] : null; }
    }
}
