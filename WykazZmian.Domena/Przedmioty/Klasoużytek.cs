using System;

namespace WykazZmian.Domena
{
    public class Klasoużytek : ObiektPowierzchniowy, IEquatable<Klasoużytek>, IComparable<Klasoużytek>
    {
        string _ofu;
        string _ozu;
        string _ozk;

        public Klasoużytek(string ofu, string ozu, string ozk, Powierzchnia powierzchnia)
            : base(powierzchnia)
        {
            _ofu = ofu;
            _ozu = ozu;
            _ozk = ozk;
        }

        public string ofu() { return _ofu; }
        public string ozu() { return _ozu; }
        public string ozk() { return _ozk; }

        public bool Equals(Klasoużytek other) { return other.CompareTo(this) == 0; }
        public override bool Equals(object obj) { return (obj as Klasoużytek).Equals(this); }
        public override int GetHashCode() { return ToString().GetHashCode(); }
        public int CompareTo(Klasoużytek other) { return string.Compare(ToString(), other.ToString()); }
        public override string ToString() { return string.Format("({0},{1},{2})", ofu(), ozu(), ozk()); }
    }
}
