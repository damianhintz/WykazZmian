using System;
using System.Linq;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WykazZmian.Domena;

namespace WykazZmian.Testy
{
    [TestClass]
    public class CzytnikOznaczeniaTest
    {
        [TestMethod]
        public void test_czy_słownik_wczyta_243_oznaczenia()
        {
            var fileName = Path.Combine(@"..\..", "uzytkiG5.csv");
            var słownik = new SłownikOznaczenia();
            var czytnik = new CzytnikOznaczenia(słownik);
            czytnik.Wczytaj(fileName);
            Assert.AreEqual(243, słownik.Count());
            //"B","B",,,"Tereny mieszkaniowe",,"B",,
            var b = słownik.SzukajOznaczenia("B");
            Assert.AreEqual("B", b.ofu);
            Assert.AreEqual("", b.ozu);
            Assert.AreEqual("", b.ozk);
            //"Lz-RVIz","Lz","R","VIz","Grunty zadrzewione i zakrzewione",,"Lz-R","VIz",
            var lz = słownik.SzukajOznaczenia("Lz/RVIz");
            Assert.AreEqual("Lz", lz.ofu);
            Assert.AreEqual("R", lz.ozu);
            Assert.AreEqual("VIz", lz.ozk);
            //"E-Lz-ŁI","E-Lz","Ł","I","Użytki ekologiczne","E","Lz-Ł","I",
            var e = słownik.SzukajOznaczenia("E-Lz/ŁI");
            Assert.AreEqual("E-Lz", e.ofu);
            Assert.AreEqual("Ł", e.ozu);
            Assert.AreEqual("I", e.ozk);
        }
    }
}
