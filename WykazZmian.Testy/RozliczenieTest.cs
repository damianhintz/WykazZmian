using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WykazZmian.Domena;

namespace WykazZmian.Testy
{
    [TestClass]
    public class RozliczenieTest
    {
        IdentyfikatorDziałki id = new IdentyfikatorDziałki(numerObrębu: "1", numerDziałki: "99/1");
        Powierzchnia powierzchnia = new Powierzchnia(99);

        [TestMethod]
        public void test_czy_rozliczenie_jest_puste()
        {
            var rozliczenie = new Rozliczenie();
            Assert.AreEqual(0, rozliczenie.Count());
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void test_czy_rozliczenie_doda_null()
        {
            var rozliczenie = new Rozliczenie();
            rozliczenie.DodajDziałkę(null);
            Assert.Fail();
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void test_czy_rozliczenie_doda_działkę_bez_użytków()
        {
            var rozliczenie = new Rozliczenie();
            var działka = new Działka(id, powierzchnia);
            rozliczenie.DodajDziałkę(działka);
            Assert.Fail();
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void test_czy_rozliczenie_doda_drugi_raz_taką_samą_działkę()
        {
            var rozliczenie = new Rozliczenie();
            var działka = new Działka(id, powierzchnia);
            var użytek = new Klasoużytek(ofu: "", ozu: "", ozk: "", powierzchnia: powierzchnia);
            działka.DodajUżytek(użytek);
            rozliczenie.DodajDziałkę(działka);
            rozliczenie.DodajDziałkę(działka);
            Assert.Fail();
        }

        [TestMethod]
        public void test_czy_rozliczenie_zawiera_dodaną_działkę()
        {
            var rozliczenie = new Rozliczenie();
            var działka = new Działka(id, powierzchnia);
            var użytek = new Klasoużytek(ofu: "", ozu: "", ozk: "", powierzchnia: powierzchnia);
            działka.DodajUżytek(użytek);
            rozliczenie.DodajDziałkę(działka);
            var szukanaDziałka = rozliczenie.SzukajIdDziałki(id);
            Assert.IsNotNull(szukanaDziałka);
            Assert.AreSame(działka, szukanaDziałka);
        }
    }
}
