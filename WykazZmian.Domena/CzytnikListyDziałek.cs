using System.Collections.Generic;
using System.Text;
using System.IO;

namespace WykazZmian.Domena
{
    /// <summary>
    /// Czytnik listy działek z pliku tekstowego.
    /// </summary>
    public class CzytnikListyDziałek : IEnumerable<IdentyfikatorDziałki>
    {
        List<IdentyfikatorDziałki> _działki = new List<IdentyfikatorDziałki>();
        private HashSet<string> _indeksId = new HashSet<string>();

        /// <summary>
        /// Dodaj identyfikator do listy.
        /// </summary>
        /// <param name="id"></param>
        public void DodajId(IdentyfikatorDziałki id)
        {
            if (!_indeksId.Add(id.Id())) return; //Pomiń jeżeli taki id został dodany do listy
            _działki.Add(id);
        }

        /// <summary>
        /// Dodaj identyfikatory z pliku.
        /// </summary>
        /// <param name="fileName"></param>
        public void DodajPlik(string fileName)
        {
            string[] lines = File.ReadAllLines(fileName, Encoding.GetEncoding(1250));
            foreach (var line in lines) if (!string.IsNullOrWhiteSpace(line)) DodajId(line);
        }

        void DodajId(string idLine)
        {
            IdentyfikatorDziałki id = IdentyfikatorDziałki.ParsujId(idLine.Trim());
            if (!_indeksId.Add(id.Id())) return;
            _działki.Add(id);
        }

        public IEnumerator<IdentyfikatorDziałki> GetEnumerator()
        {
            return _działki.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
