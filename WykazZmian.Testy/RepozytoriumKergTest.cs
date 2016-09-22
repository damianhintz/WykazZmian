using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using WykazZmian.Domena;

namespace WykazZmian.Testy
{
    [TestClass]
    public class RepozytoriumKergTest
    {
        [TestMethod]
        public void test_czy_wczyta_30_numerów_kerg()
        {
            var fileName = Path.Combine(@"..\..", "RepozytoriumKergTest.txt");
            var repozytorium = RepozytoriumKerg.WczytajPlik(fileName);
            Assert.AreEqual(30, repozytorium.LiczbaObrębów);
        }
    }
}
