using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Pragmatic.Kontrakty;
using Novacode;
using WykazZmian.Domena;

namespace WykazZmian.Raporty
{
    public class WykazZmianDocX
    {
        IZawierającyDziałki _staryStan;
        IZawierającyDziałki _nowyStan;
        DocX _doc;
        Table _table;
        Row _templateRow;
        int _lp = 1;
        Dictionary<string, int> _header = new Dictionary<string, int>();
        List<int> _pusteKolumny = new List<int>();
        string _fileName;
        RepozytoriumKerg _obręby;
        public bool WykażDodane { get; set; }
        public bool WykażUsunięte { get; set; }
        public string Sporządził { get; set; }

        public WykazZmianDocX(string fileName, RepozytoriumKerg obręby)
        {
            wczytajPlik(fileName);
            _obręby = obręby;
            WykażDodane = true;
            WykażUsunięte = true;
        }

        public void Zapisz()
        {
            _templateRow.Remove();
            _doc.Save();
        }

        void wczytajPlik(string fileName)
        {
            _fileName = fileName;
            _doc = DocX.Load(fileName);
            Kontrakt.assert(_doc.Tables.Any(), "Szablon wykazu zmian nie zawiera żadnych tabel.");
            _table = _doc.Tables.First();
            _templateRow = _table.Rows.Last();
            wczytajNagłówek();
        }

        void wczytajNagłówek()
        {
            int index = 0;
            foreach (var cell in _templateRow.Cells)
            {
                string key = cell.Paragraphs.First().Text;
                if (key.StartsWith("[") && key.EndsWith("]"))
                    _header.Add(key, index);
                if (string.IsNullOrEmpty(key))
                    _pusteKolumny.Add(index);
                index++;
            }
            Zmienne.header = _header;
        }

        public void DodajDziałki(IZawierającyDziałki staryStan, IZawierającyDziałki nowyStan, IEnumerable<IdentyfikatorDziałki> działki)
        {
            _staryStan = staryStan;
            _nowyStan = nowyStan;
            var zmienione = 0;
            var usunięte = 0;
            var dodane = 0;
            Działka ostatniaDziałka = null;
            //wykaż istniejące działki
            foreach (var para in tylkoIstniejące(działki).OrderBy(t => t.Item2, new DziałkaComparer()))
            {
                ostatniaDziałka = dodajIstniejącaPara(para.Item1, para.Item2);
                zmienione++;
            }
            //wykaż usunięte działki
            foreach (var działka in tylkoUsunięte(działki))
            {
                if (WykażUsunięte) addUsunięta(działka);
                Logger.info("stan nowy nie zawiera działki: " + działka.Id);
                usunięte++;
            }
            //wykaż dodane działki
            foreach (var działka in tylkoDodane(działki))
            {
                if (WykażDodane) addDodana(działka);
                Logger.info("stan dotychczasowy nie zawiera działki: " + działka.Id);
                dodane++;
            }
            Logger.info("działki zmienione, usunięte lub dodane: " + (zmienione + usunięte + dodane));
            podstawNagłówek(ostatniaDziałka);
        }

        void podstawNagłówek(Działka ostatniaDziałka)
        {
            if (ostatniaDziałka == null) return;
            JednostkaRejestrowa jr = ostatniaDziałka.jr;
            podstawKerg(jr);
            _doc.jewTeryt(jr.terytJednostki())
                .jewNazwa(jr.nazwaJednostki())
                .obrTeryt(jr.terytObrebu())
                .obrNazwa(jr.nazwaObrebu());
            _doc.podstaw("[SPORZADZIL]", Sporządził);
        }

        void podstawKerg(JednostkaRejestrowa jr)
        {
            string terytObrębu = jr.terytObrebu();
            if (_obręby.ZawieraObręb(terytObrębu))
            {
                string kergObrebu = _obręby.ZnajdźObręb(terytObrębu);
                _doc.podstaw("[KERG]", kergObrebu);
            }
            else Logger.ostrzeżenie("brak KERG obrębu: " + terytObrębu);
        }

        private IEnumerable<Tuple<Działka, Działka>> tylkoIstniejące(IEnumerable<IdentyfikatorDziałki> działki)
        {
            var posortowaneDziałki = działki.OrderBy(dz => dz.NumerDziałki(), new NumerComparer());
            foreach (var id in posortowaneDziałki)
            {
                Działka staraDziałka = _staryStan.SzukajIdDziałki(id);
                Działka nowaDziałka = _nowyStan.SzukajIdDziałki(id);
                if (staraDziałka == null || nowaDziałka == null) continue;
                //pomijanie w zestawieniu działek, które nie uległy zmianie
                if (!staraDziałka.uległaZmianie(nowaDziałka)) continue; //Działka nie uległa zmianie
                yield return new Tuple<Działka, Działka>(staraDziałka, nowaDziałka);
            }
        }

        private IEnumerable<Działka> tylkoUsunięte(IEnumerable<IdentyfikatorDziałki> działki)
        {
            var posortowaneDziałki = działki.OrderBy(id => id.NumerDziałki(), new NumerComparer());
            foreach (var id in posortowaneDziałki)
            {
                Działka staraDziałka = _staryStan.SzukajIdDziałki(id);
                Działka nowaDziałka = _nowyStan.SzukajIdDziałki(id);
                if (staraDziałka != null && nowaDziałka == null) yield return staraDziałka;
            }
        }

        private IEnumerable<Działka> tylkoDodane(IEnumerable<IdentyfikatorDziałki> działki)
        {
            var posortowaneDziałki = działki.OrderBy(id => id.NumerDziałki(), new NumerComparer());
            foreach (var id in posortowaneDziałki)
            {
                Działka staraDziałka = _staryStan.SzukajIdDziałki(id);
                Działka nowaDziałka = _nowyStan.SzukajIdDziałki(id);
                if (staraDziałka == null && nowaDziałka != null) yield return nowaDziałka;
            }
        }

        Działka dodajIstniejącaPara(Działka staraDziałka, Działka nowaDziałka)
        {
            Row row = _table.InsertRow(_templateRow);
            row.lp(_lp++);
            row.jrg(nowaDziałka.jr.numer());
            row.kw(nowaDziałka.kw);
            row.id(staraDziałka.Id)
                .nr(staraDziałka.Id.NumerDziałki())
                .pow(staraDziałka.PowierzchniaPoLudzku);
            row.idNowy(nowaDziałka.Id)
                .nrNowy(nowaDziałka.Id.NumerDziałki())
                .powNowy(nowaDziałka.PowierzchniaPoLudzku);
            row.uzDelta(roznica(staraDziałka.Powierzchnia, nowaDziałka.Powierzchnia, string.Empty));
            roznicaWarn(staraDziałka.PowierzchniaUżytków, nowaDziałka.PowierzchniaUżytków, "różnica powierzchni działki: " + staraDziałka.Id);
            row.wl(createWłaściciele(staraDziałka, nowaDziałka) + createWładający(staraDziałka, nowaDziałka));
            row.ukryjZmienne();
            var użytki = staraDziałka.UnionUżytki(nowaDziałka);
            foreach (var użytek in użytki) addParaUżytków(staraDziałka, nowaDziałka, użytek);
            addRazem(staraDziałka, nowaDziałka);
            scalajKolumny(użytki.Count());
            addPusty();
            return nowaDziałka;
        }

        void addUsunięta(Działka staraDziałka)
        {
            Row row = _table.InsertRow(_templateRow);
            row.lp(_lp++);
            if (staraDziałka.jr != null) row.jrg(staraDziałka.jr.numer());
            else row.jrg("Brak JR");
            row.kw(staraDziałka.kw);
            row.id(staraDziałka.Id)
                .nr(staraDziałka.Id.NumerDziałki())
                .pow(staraDziałka.PowierzchniaPoLudzku);
            row.idNowy(staraDziałka.Id)
                .nrNowy(staraDziałka.Id.NumerDziałki())
                .powNowy(string.Empty);
            row.wl(createWłaściciele(staraDziałka) + createWładający(staraDziałka));
            row.ukryjZmienne();
            var użytki = staraDziałka;
            foreach (var użytek in użytki) addParaUżytków(staraDziałka, null, użytek);
            addRazem(staraDziałka, null);
            scalajKolumny(użytki.Count());
            addPusty();
        }

        void addDodana(Działka nowaDziałka)
        {
            Row row = _table.InsertRow(_templateRow);
            row.lp(_lp++);
            row.jrg(nowaDziałka.jr.numer());
            row.kw(nowaDziałka.kw);
            row
                .id(nowaDziałka.Id)
                .nr(nowaDziałka.Id.NumerDziałki())
                .pow(string.Empty);
            row.idNowy(nowaDziałka.Id)
                .nrNowy(nowaDziałka.Id.NumerDziałki())
                .powNowy(nowaDziałka.PowierzchniaPoLudzku);
            row.wl(createWłaściciele(nowaDziałka) + createWładający(nowaDziałka));
            row.ukryjZmienne();
            var użytki = nowaDziałka;
            foreach (var użytek in użytki) addParaUżytków(null, nowaDziałka, użytek);
            addRazem(null, nowaDziałka);
            scalajKolumny(użytki.Count());
            addPusty();
        }

        private void addParaUżytków(Działka staraDziałka, Działka nowaDziałka, Klasoużytek użytek)
        {
            Row row = _table.InsertRow(_templateRow);
            var staryUżytek = staraDziałka != null ? staraDziałka.SzukajUżytku(użytek) : null;
            if (staryUżytek != null)
            {
                row.ofu(staryUżytek.ofu()).ozu(staryUżytek.ozu()).ozk(staryUżytek.ozk())
                    .uz(staryUżytek.powierzchnia().toString(!staraDziałka.JestDoAra));
            }
            var nowyUżytek = nowaDziałka != null ? nowaDziałka.SzukajUżytku(użytek) : null;
            if (nowyUżytek != null)
            {
                row.ofuNowy(nowyUżytek.ofu()).ozuNowy(nowyUżytek.ozu()).ozkNowy(nowyUżytek.ozk())
                    .uzNowy(nowyUżytek.powierzchnia().toString(!nowaDziałka.JestDoAra));
            }
            if (staryUżytek != null && nowyUżytek != null)
            {
                row.uzDelta(roznica(staryUżytek.powierzchnia(), nowyUżytek.powierzchnia()));
            }
            else row.uzDelta("-");
            row.ukryjZmienne();
        }

        private void addRazem(Działka staraDziałka, Działka nowaDziałka)
        {
            Row row = _table.InsertRow(_templateRow);
            if (staraDziałka != null) row.id("Razem").nr("Razem").pow(staraDziałka.PowierzchniaPoLudzku).uz(staraDziałka.PowierzchniaUżytków.toString(!staraDziałka.JestDoAra));
            if (nowaDziałka != null) row.idNowy("Razem").nrNowy("Razem").powNowy(nowaDziałka.PowierzchniaPoLudzku).uzNowy(nowaDziałka.PowierzchniaUżytków.toString(!nowaDziałka.JestDoAra));
            if (staraDziałka != null && nowaDziałka != null)
            {
                row.uzDelta(roznica(staraDziałka.PowierzchniaUżytków, nowaDziałka.PowierzchniaUżytków));
                roznicaWarn(staraDziałka.PowierzchniaUżytków, nowaDziałka.PowierzchniaUżytków, "różnica sumy powierzchni użytków: " + staraDziałka.Id);
            }
            row.pogrub().ukryjZmienne();
        }

        private void scalajKolumny(int countUzytki)
        {
            int rowsAdded = countUzytki + 1 + 1;
            scalOstatnieWierszeKolumny("[LP]", rowsAdded);
            scalOstatnieWierszeKolumny("[WL]", rowsAdded);
            scalOstatnieWierszeKolumny("[UWAGA]", rowsAdded);
            foreach (var index in _pusteKolumny) scalOstatnieWierszeKolumny(index, rowsAdded);
        }

        private void scalOstatnieWierszeKolumny(string key, int countRows)
        {
            if (!_header.ContainsKey(key)) return;
            scalOstatnieWierszeKolumny(_header[key], countRows);
        }

        private void scalOstatnieWierszeKolumny(int index, int countRows)
        {
            int startRow = _table.RowCount - countRows;
            int endRow = _table.RowCount - 1;
            _table.MergeCellsInColumn(index, startRow, endRow);
        }

        string createWłaściciele(Działka staraDzialka, Działka nowaDzialka)
        {
            HashSet<string> wlasciciele = new HashSet<string>(from podmiot in nowaDzialka.jr.Właściciele select podmiot.ToString());
            if (staraDzialka.jr != null) wlasciciele.UnionWith(from podmiot in staraDzialka.jr.Właściciele select podmiot.ToString());
            return "Właściciele:\n" + string.Join(";\n", wlasciciele);
        }
        
        string createWłaściciele(Działka działka)
        {
            if (działka.jr == null) return "Właściciele: brak danych\n";
            HashSet<string> wlasciciele = new HashSet<string>(from podmiot in działka.jr.Właściciele select podmiot.ToString());
            return "Właściciele:\n" + string.Join(";\n", wlasciciele);
        }

        string createWładający(Działka staraDziałka, Działka nowaDziałka)
        {
            HashSet<string> wladajacy = new HashSet<string>(from podmiot in nowaDziałka.jr.Władający select podmiot.ToString());
            if (staraDziałka.jr != null) wladajacy.UnionWith(from podmiot in staraDziałka.jr.Władający select podmiot.ToString());
            string joinWladajacy = string.Join(";\n", wladajacy);
            if (string.IsNullOrEmpty(joinWladajacy)) return string.Empty;
            return "\nWładający:\n" + joinWladajacy;
        }

        string createWładający(Działka działka)
        {
            if (działka.jr == null) return "\nWładający: brak danych\n";
            HashSet<string> wladajacy = new HashSet<string>(from podmiot in działka.jr.Władający select podmiot.ToString());
            string joinWladajacy = string.Join(";\n", wladajacy);
            if (string.IsNullOrEmpty(joinWladajacy)) return string.Empty;
            return "\nWładający:\n" + joinWladajacy;
        }

        private void addPusty()
        {
            Row row = _table.InsertRow(_templateRow);
            row.ukryjZmienne();
            foreach (var cell in row.Cells) cell.FillColor = Color.Orange;
        }

        private string roznica(Powierzchnia stara, Powierzchnia nowa, string equ = "0.0000")
        {
            long roznica = stara.roznica(nowa);
            if (roznica > 0) return new Powierzchnia(roznica).ToString();
            else if (roznica == 0) return equ;
            else return new Powierzchnia(-roznica).ToString();
        }

        private void roznicaWarn(Powierzchnia stara, Powierzchnia nowa, string warn)
        {
            long roznica = stara.roznica(nowa);
            if (roznica != 0) Logger.ostrzeżenie(warn + ", pow. " + roznica + " m^2");
        }

    }
}
