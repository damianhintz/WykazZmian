using System;
using System.Globalization;
using Pragmatic.Kontrakty;

namespace WykazZmian.Domena
{
    public class Powierzchnia : IEquatable<Powierzchnia>
    {
        private long _m2;

        public Powierzchnia(long m2)
        {
            Kontrakt.requires(m2 > 0, "Powierzchnia nie jest dodatnia.");
            _m2 = m2;
        }

        public string toString(bool m2)
        {
            return m2 ? toHektary() : toAry();
        }

        public string toHektary()
        {
            return hektary().ToString("F4");
        }

        public string toAry()
        {
            Kontrakt.assert(hektary().ToString("F4").EndsWith("00"));
            return hektary().ToString("F2");
        }

        public Powierzchnia metryDoAra() { return new Powierzchnia(_m2 > 99 ? (long)Math.Round(_m2 / 100.0) : 1); }
        public Powierzchnia aryDoMetra() { return new Powierzchnia(_m2 * 100); }

        public void dodaj(Powierzchnia pow) { _m2 += pow.metryKwadratowe(); }

        public long roznica(Powierzchnia powierzchnia)
        {
            return _m2 - powierzchnia._m2;
        }

        public long suma(Powierzchnia powierzchnia)
        {
            return _m2 + powierzchnia._m2;
        }

        public static Powierzchnia ParsujMetry(string s)
        {
            Kontrakt.requires(!String.IsNullOrEmpty(s), "Powierzchnia jest pusta.");
            Powierzchnia pow = null;
            try
            {
                pow = new Powierzchnia(long.Parse(s));
            }
            catch (FormatException)
            {
                Kontrakt.fail("Napis " + s + " nie reprezentuje powierzchni w m^2.");
            }
            catch (OverflowException)
            {
                Kontrakt.fail("Napis " + s + " reprezentuje powierzchnię poza zakresem dopuszczalnych wartości.");
            }
            Kontrakt.ensures(pow != null);
            return pow;
        }

        public static Powierzchnia ParsujHektary(string s)
        {
            Kontrakt.requires(!String.IsNullOrEmpty(s));
            Powierzchnia pow = null;
            try
            {
                double ha = double.Parse(s, CultureInfo.InvariantCulture);
                long m2 = (long)(ha * 10000.0);
                pow = new Powierzchnia(m2);
            }
            catch (FormatException)
            {
                Kontrakt.fail("Napis " + s + " nie reprezentuje powierzchni w ha.");
            }
            catch (OverflowException)
            {
                Kontrakt.fail("Napis " + s + " reprezentuje powierzchnię poza zakresem dopuszczalnych wartości.");
            }
            Kontrakt.ensures(pow != null);
            return pow;
        }

        public double hektary()
        {
            return _m2 / 10000.0;
        }

        public double ary()
        {
            return _m2 / 100.0;
        }

        public long metryKwadratowe()
        {
            return _m2;
        }

        public bool Equals(Powierzchnia other)
        {
            return _m2.Equals(other.metryKwadratowe());
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Powierzchnia);
        }

        public override int GetHashCode()
        {
            return _m2.GetHashCode();
        }

        public override string ToString()
        {
            return hektary().ToString("F4", CultureInfo.InvariantCulture);
        }
    }
}
