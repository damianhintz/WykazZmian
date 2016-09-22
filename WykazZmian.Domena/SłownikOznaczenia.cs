using System;
using System.Collections.Generic;
using System.Collections;

namespace WykazZmian.Domena
{
    /// <summary>
    /// Konwersja klasoużytku na oznaczenie i odwrotnie na podstawie słownika G5 (ozn) <-> (ofu,ozu,ozk).
    /// </summary>
    public class SłownikOznaczenia : IEnumerable<string>
    {
        Dictionary<string, Oznaczenie> _oznaczenia = new Dictionary<string, Oznaczenie>();
        
        public void Dodaj(string ozn, string ofu, string ozu, string ozk)
        {
            var ooo = new Oznaczenie() { ofu = ofu, ozu = ozu, ozk = ozk };
            _oznaczenia.Add(ozn, ooo);
        }

        public Oznaczenie SzukajOznaczenia(string oznaczenie)
        {
            string ozn = oznaczenie.Replace("/", "-"); //Poprawa oznaczenia / na -
            if (_oznaczenia.ContainsKey(ozn)) return _oznaczenia[ozn];
            throw new ArgumentException(paramName: oznaczenie, message: "Brak mapowania dla oznaczenia: " + oznaczenie);
            //return new Oznaczenie { ofu = ozn, ozu = string.Empty, ozk = string.Empty }; //Nierozpoznane oznaczenie: zastosowano mapowanie {0} -> ({1},,)", oznaczenie, ozn
        }

        public IEnumerator<string> GetEnumerator() { return _oznaczenia.Keys.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
