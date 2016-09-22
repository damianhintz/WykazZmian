using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WykazZmian.Domena
{
    public class Oznaczenie
    {
        public string ofu;
        public string ozu;
        public string ozk;

        public override string ToString()
        {
            return string.Format("{0},{1},{2}", ofu, ozu, ozk);
        }
    }
}
