using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pragmatic.Kontrakty;

namespace WykazZmian.Domena
{
    public class Działka : IEnumerable<Klasoużytek>
    {
        public JednostkaRejestrowa jr { get; set; }
        public string kw { get; set; }
        public bool JestDoAra { get; set; }

        public IdentyfikatorDziałki Id { get { return _id; } }
        public Powierzchnia Powierzchnia { get { return _powierzchnia; } }

        Dictionary<string, Klasoużytek> _indeksUżytków = new Dictionary<string, Klasoużytek>();
        List<Klasoużytek> _użytki = new List<Klasoużytek>();
        IdentyfikatorDziałki _id;
        Powierzchnia _powierzchnia;

        public Działka(IdentyfikatorDziałki id, Powierzchnia powierzchnia)
        {
            _id = id;
            _powierzchnia = powierzchnia;
        }

        public string PowierzchniaPoLudzku { get { return Powierzchnia.toString(!JestDoAra); } }

        public Powierzchnia PowierzchniaUżytków
        {
            get
            {
                long suma = 0;
                foreach (Klasoużytek u in _użytki) { suma += u.powierzchnia().metryKwadratowe(); }
                return new Powierzchnia(suma);
            }
        }

        public void DodajUżytek(Klasoużytek użytek)
        {
            string oz = użytek.ToString();
            if (_indeksUżytków.ContainsKey(oz))
            {
                _indeksUżytków[oz].powierzchnia().dodaj(użytek.powierzchnia());
                Logger.uwagaSzara("sumowanie powtórzonego użytku " + Id + ": " + oz);
            }
            else
            {
                _indeksUżytków.Add(oz, użytek);
                _użytki.Add(użytek);
            }
            if (JestDoAra) //czy użytek też jest do ara
            {
                var haString = użytek.powierzchnia().hektary().ToString("F4");
                if (haString.EndsWith("00") == false)
                {
                    JestDoAra = false;
                    Logger.ostrzeżenie("dodano użytek do metra dla działki do ara: " + haString + " ha, działka " + Id);
                }
            }
        }

        public IEnumerable<Klasoużytek> UnionUżytki(Działka działka) { return działka.Union(this).OrderBy(u => u.ToString()); }
        public Klasoużytek SzukajUżytku(Klasoużytek użytek) { return this.SingleOrDefault(u => u.Equals(użytek)); }

        public IEnumerator<Klasoużytek> GetEnumerator() { return _użytki.GetEnumerator(); }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }
    }
}
