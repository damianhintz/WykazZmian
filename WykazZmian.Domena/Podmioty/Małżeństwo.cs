using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WykazZmian.Domena
{
    public class Małżeństwo : AbstrakcyjnyPodmiot
    {
        OsobaFizyczna _zona;
        OsobaFizyczna _maz;

        public Małżeństwo(OsobaFizyczna zona, OsobaFizyczna maz)
        {
            _zona = zona;
            _maz = maz;
        }

        public override string ToString()
        {
            return string.Format("Małżeństwo:\n{0}\n{1}", _zona, _maz);
        }
    }
}
