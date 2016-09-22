using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Pragmatic.Kontrakty;

namespace WykazZmian.Domena
{
    public class CzytnikOznaczenia
    {
        SłownikOznaczenia _słownik;

        public CzytnikOznaczenia(SłownikOznaczenia słownik) { _słownik = słownik; }

        public void Wczytaj(string fileName)
        {
            var lines = File.ReadAllLines(fileName, Encoding.GetEncoding(1250));
            var header = lines.First(); //OZN,OFU,OZU,OZK,OPIS,OFUst,OZUst,OZKst,
            Kontrakt.assert(header.StartsWith("OZN,OFU,OZU,OZK,OPIS"));
            var query = //"B","B",,,"Tereny mieszkaniowe",,"B",,
                from line in lines.Skip(1)
                let split = line.Replace("\"", "").Replace("\'", "").Split(',')
                select new
                {
                    ozn = split[0],
                    ofu = split[1],
                    ozu = split[2],
                    ozk = split[3]
                };
            foreach (var tuple in query) _słownik.Dodaj(tuple.ozn, tuple.ofu, tuple.ozu, tuple.ozk);
        }

    }
}
