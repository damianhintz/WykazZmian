using System;
using System.Linq;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WykazZmian.Domena;

namespace WykazZmian.Testy
{
    [TestClass]
    public class CzytnikRozliczeniaTest
    {
        [TestMethod]
        public void test_czy_rozliczenie_wczyta_495_działek()
        {
            var słownik = WczytajSłownik();
            var fileName = Path.Combine(@"..\..", "CzytnikRozliczeniaTest.txt");
            var rozliczenie = new Rozliczenie();
            var czytnik = new CzytnikRozliczenia(rozliczenie, słownik);
            czytnik.Wczytaj(fileName);
            Assert.AreEqual(495, rozliczenie.Count());
            //19-1/1                   23574
            var id_19_1_1 = new IdentyfikatorDziałki("19", "1/1");
            var dz_19_1_1 = rozliczenie.SzukajIdDziałki(id_19_1_1);
            Assert.AreEqual(1, dz_19_1_1.Count());
            //6-99                      9278
            var id_6_99 = new IdentyfikatorDziałki("6", "99");
            var dz_6_99 = rozliczenie.SzukajIdDziałki(id_6_99);
            Assert.AreEqual(1, dz_6_99.Count());
            //19-34                    59347
            var id_19_34 = new IdentyfikatorDziałki("19", "34");
            var dz_19_34 = rozliczenie.SzukajIdDziałki(id_19_34);
            Assert.AreEqual(59347, dz_19_34.Powierzchnia.metryKwadratowe());
            Assert.AreEqual(6, dz_19_34.Count());
            var psIV = dz_19_34.SzukajUżytku(new Klasoużytek("Ps", "Ps", "IV", new Powierzchnia(20277)));
            var lzVI = dz_19_34.SzukajUżytku(new Klasoużytek("Lz", "Ps", "VI", new Powierzchnia(3614)));
            Assert.IsNotNull(psIV);
            Assert.AreEqual(20277, psIV.powierzchnia().metryKwadratowe());
            Assert.IsNotNull(lzVI);
            Assert.AreEqual(3614, lzVI.powierzchnia().metryKwadratowe());
            //  PsIV                   20277
            //  LsVI                   17788
            //  PsV                    12329
            //  ŁV                      4232
            //  Lz/PsVI                 3614
            //  W                       1107
        }

        SłownikOznaczenia WczytajSłownik()
        {
            var fileName = Path.Combine(@"..\..", "uzytkiG5.csv");
            var słownik = new SłownikOznaczenia();
            var czytnik = new CzytnikOznaczenia(słownik);
            czytnik.Wczytaj(fileName);
            return słownik;
        }
    }
}
