using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WykazZmian.Domena;

namespace WykazZmian.Testy
{
    [TestClass]
    public class CzytnikSwdeTest
    {
        [TestMethod, Ignore]
        public void test_czy_wczyta_tylko_dokumenty_typu_kdk_5()
        {
            var fileName = Path.Combine(@"..\..", "CzytnikSwdeTest.swd");
            var czytnik = new CzytnikOpisowegoSwde(fileName);
            Assert.Fail();
        }
    }
}
