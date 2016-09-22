using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WykazZmian.Domena
{
    public class OsobaFizyczna : AbstrakcyjnyPodmiot
    {
        string _imie;
        string _drugieImie;
        string _nazwisko;

        public OsobaFizyczna(string imie, string drugieImie, string nazwisko)
        {
            _imie = imie;
            _drugieImie = drugieImie;
            _nazwisko = nazwisko;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", _imie, _drugieImie, _nazwisko);
        }
    }
}
