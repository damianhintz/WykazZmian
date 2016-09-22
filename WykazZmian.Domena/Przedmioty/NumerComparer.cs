using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WykazZmian.Domena
{
    public class NumerComparer : IComparer<string>
    {
        public int Compare(string s1, string s2)
        {
            //var s1 = d1.id.NumerDziałki();
            //var s2 = d2.id.NumerDziałki();
            int n1 = lewyNumer(s1);
            int n2 = lewyNumer(s2);
            if (n1 != n2) return n1.CompareTo(n2);
            n1 = prawyNumer(s1);
            n2 = prawyNumer(s2);
            if (n1 != n2) return n1.CompareTo(n2);
            return string.CompareOrdinal(s1, s2);
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
