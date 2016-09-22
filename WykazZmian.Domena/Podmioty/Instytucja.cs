using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WykazZmian.Domena
{
    public class Instytucja : AbstrakcyjnyPodmiot
    {
        string _nazwa;

        public Instytucja(string nazwa) { _nazwa = nazwa; }

        public override string ToString()
        {
            return _nazwa;
        }
    }
}
