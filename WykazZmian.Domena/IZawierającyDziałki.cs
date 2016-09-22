using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WykazZmian.Domena
{
    public interface IZawierającyDziałki : IEnumerable<Działka>
    {
        Działka SzukajIdDziałki(IdentyfikatorDziałki id);
    }
}
