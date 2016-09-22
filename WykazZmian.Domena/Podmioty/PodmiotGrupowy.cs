using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WykazZmian.Domena
{
    public class PodmiotGrupowy : AbstrakcyjnyPodmiot
    {
        List<AbstrakcyjnyPodmiot> _podmioty = new List<AbstrakcyjnyPodmiot>();

        string _nazwa;

        public PodmiotGrupowy(string nazwa) { _nazwa = nazwa; }

        public void dodajPodmiot(AbstrakcyjnyPodmiot podmiot) { _podmioty.Add(podmiot); }

        public override string ToString()
        {
            return "Podmiot grupowy:\n" + _nazwa +
                "\nSkłada się:\n" +
                string.Join("\n", _podmioty);
        }
    }
}
