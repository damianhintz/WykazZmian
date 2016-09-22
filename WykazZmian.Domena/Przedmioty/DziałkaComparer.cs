using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WykazZmian.Domena
{
    public class DziałkaComparer : IComparer<Działka>
    {
        NumerComparer idComparer = new NumerComparer();

        public int Compare(Działka d1, Działka d2)
        {
            var s1 = d1.jr.numer();
            var s2 = d2.jr.numer();
            int n1 = prawyNumer(s1);
            int n2 = prawyNumer(s2);
            if (n1 != n2) return n1.CompareTo(n2);
            return idComparer.Compare(d1.Id.NumerDziałki(), d2.Id.NumerDziałki());
        }

        private int lewyNumer(string s)
        {
            string digits = string.Empty;
            foreach (char c in s)
            {
                if (char.IsDigit(c)) digits += c;
                else break;
            }
            return string.IsNullOrEmpty(s) ? 0 : int.Parse(digits);
        }

        private int prawyNumer(string s)
        {
            string digits = string.Empty;
            foreach (char c in s.Reverse())
            {
                if (char.IsDigit(c)) digits = c + digits;
                else break;
            }
            if (string.IsNullOrEmpty(digits)) return 0;
            return string.IsNullOrEmpty(s) ? 0 : int.Parse(digits);
        }
    }
}
