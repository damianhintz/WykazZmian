using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WykazZmian.Domena;

namespace WykazZmian.Testy
{
    [TestClass]
    public class CzytnikListyDziałekTest
    {
        [TestMethod]
        public void test_czy_doda_id_do_listy()
        {
            var id = new IdentyfikatorDziałki("1", "100"); //1-100
            var czytnik = new CzytnikListyDziałek();
            Assert.AreEqual(0, czytnik.Count());
            czytnik.DodajId(id);
            Assert.AreEqual(1, czytnik.Count());
            var pierwszeId = czytnik.First();
            Assert.AreEqual("1-100", pierwszeId.Id());
        }

        [TestMethod]
        public void test_czy_nie_doda_dwa_razy_tego_samego_id_do_listy()
        {
            var id = new IdentyfikatorDziałki("1", "100"); //1-100
            var czytnik = new CzytnikListyDziałek();
            Assert.AreEqual(0, czytnik.Count());
            czytnik.DodajId(id);
            czytnik.DodajId(id);
            Assert.AreEqual(1, czytnik.Count());
            var pierwszeId = czytnik.First();
            Assert.AreEqual("1-100", pierwszeId.Id());
        }

        [TestMethod]
        public void test_czy_doda_do_listy_id_219_działek_z_pliku()
        {
            var fileName = Path.Combine(@"..\..", "CzytnikListyDziałekTest.dzialki");
            var czytnik = new CzytnikListyDziałek();
            Assert.AreEqual(0, czytnik.Count());
            czytnik.DodajPlik(fileName);
            Assert.AreEqual(33, czytnik.Count());
            var pierwszeId = czytnik.First();
            Assert.AreEqual("1-12/7", pierwszeId.Id());
        }
    }
}
