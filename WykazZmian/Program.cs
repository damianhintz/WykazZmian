using System;
using System.Linq;
using System.IO;
using System.Reflection;
using Pragmatic.Kontrakty;
using WykazZmian.Domena;
using WykazZmian.Raporty;
using WykazZmian.Properties;

namespace WykazZmian
{
    class Program
    {
        //Logo aplikacji
        static readonly string _opisAplikacji = "WykazZmian.exe v1.6-beta - Wykaz zmian działek i użytków z plików opisowych SWDE";
        static readonly string _dataPublikacji = "1 grudnia 2015";
        //static readonly string _licencjaDla = "OPEGIEKA Sp. z o.o. Elbląg";
        static readonly string _licencjaDla = "OPGK Sp. z o.o. w Olsztynie";
        //Parametry wiersza poleceń
        static string _exeKatalog = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        static string _roboczyKatalog;
        static string _docelowyKatalog;
        static string _starePath;
        static string _nowePath;
        static string[] _działkiPath;
        static string _szablonZmian;
        //Obiekty domeny
        static IZawierającyDziałki _staryStan;
        static IZawierającyDziałki _nowyStan;
        static RepozytoriumKerg _repozytoriumKERG;

        private Program() { }

        static void Main(string[] args)
        {
            PokażLogo(args);
            if (args.Length < 1)
            {
                Logger.błąd("Nie podano roboczego katalogu.");
                PokażKoniec();
                return;
            }
            _roboczyKatalog = args[0];
            if (Directory.Exists(_roboczyKatalog) == false)
            {
                Logger.błąd("Roboczy katalog nie istnieje: " + _roboczyKatalog);
                PokażKoniec();
                return;
            }
            Console.WriteLine("Roboczy katalog: {0}", _roboczyKatalog);
            if (args.Length < 2)
            {
                Logger.błąd("Nie podano starego pliku SWDE.");
                PokażKoniec();
                return;
            }
            _starePath = Path.Combine(_roboczyKatalog, args[1]);
            if (File.Exists(_starePath) == false)
            {
                Logger.błąd("Plik nie istnieje: " + _starePath);
                PokażKoniec();
                return;
            }
            if (args.Length < 3)
            {
                Logger.błąd("Nie podano nowego pliku SWDE.");
                PokażKoniec();
                return;
            }
            _nowePath = Path.Combine(_roboczyKatalog, args[2]);
            if (File.Exists(_nowePath) == false)
            {
                Logger.błąd("Plik nie istnieje: " + _nowePath);
                PokażKoniec();
                return;
            }
            if (args.Length < 4)
            {
                Logger.błąd("Nie określono filtru działek.");
                PokażKoniec();
                return;
            }
            _działkiPath = Directory.GetFiles(_roboczyKatalog, args[3], SearchOption.TopDirectoryOnly);
            if (args.Length < 5)
            {
                Logger.błąd("Nie podano szablonu wykazu.");
                PokażKoniec();
                return;
            }
            _szablonZmian = Path.Combine(_exeKatalog, args[4]);
            if (File.Exists(_szablonZmian) == false)
            {
                Logger.błąd("Plik nie istnieje: " + _szablonZmian);
                PokażKoniec();
                return;
            }
            //Przygotuj docelowy katalog
            _docelowyKatalog = Path.Combine(_roboczyKatalog, DateTime.Now.ToString("yyyyMMddTHHmmss"));
            if (!Directory.Exists(_docelowyKatalog)) Directory.CreateDirectory(_docelowyKatalog);
            Console.WriteLine("Docelowy katalog: {0}", _docelowyKatalog);
            //Wczytaj repozytorium KERG
            _repozytoriumKERG = RepozytoriumKerg.WczytajPlik(Path.Combine(_exeKatalog, "KERG.txt"));
            //Wczytaj stan stary i nowy z plików SWDE
            Console.WriteLine("Stan dotychczasowy z pliku: " + _starePath.name());
            var słownik = new SłownikOznaczenia();
            var czytnikOnaczenia = new CzytnikOznaczenia(słownik);
            czytnikOnaczenia.Wczytaj(Path.Combine(_exeKatalog, "uzytkiG5.csv"));
            var rozliczenie = new Rozliczenie();
            var czytnikRozliczenia = new CzytnikRozliczenia(rozliczenie, słownik);
            if (_starePath.EndsWith("*.swd"))
            {
                _staryStan = new CzytnikOpisowegoSwde(_starePath);
            }
            else
            {
                czytnikRozliczenia.Wczytaj(_starePath);
                _staryStan = rozliczenie;
            }
            Console.WriteLine("Stan nowy z pliku: " + _nowePath.name());
            _nowyStan = new CzytnikOpisowegoSwde(_nowePath);
            var działki = PrzygotujDziałkiDoWykazu();
            //Zapisz wykazy obrębami
            var obrębyPogrupowane = działki.GroupBy(id => id.NumerObrębu());
            foreach (var działkiObrębu in obrębyPogrupowane) ZapiszWykaz(działkiObrębu);
            PokażKoniec();
        }

        static void PokażKoniec()
        {
            Console.WriteLine("Koniec.");
            //Console.Read();
        }

        static void PokażLogo(string[] args)
        {
            Console.WriteLine(_opisAplikacji);
            Console.WriteLine("Data publikacji: {0}", _dataPublikacji);
            Console.WriteLine("Copyright (c) 2013-2015 OPGK Olsztyn. Wszelkie prawa zastrzeżone.");
            Console.WriteLine("Licencję na ten produkt posiada: {0}", _licencjaDla);
        }

        static CzytnikListyDziałek PrzygotujDziałkiDoWykazu()
        {
            var działki = new CzytnikListyDziałek();
            foreach (var fileName in _działkiPath) działki.DodajPlik(fileName); //Dodaj działki z pliku
            if (działki.Count() == 0)
            {
                Logger.info("wykaz dla wszystkich działek z plików SWDE, gdyż filtr działek jest pusty.");
                //Wybierz wszystkie działki z pliku SWDE - starego
                foreach (var dz in _staryStan) działki.DodajId(dz.Id);
                //Wybierz wszystkie działki z pliku SWDE - nowego
                foreach (var dz in _nowyStan) działki.DodajId(dz.Id);
            }
            else
            {
                Logger.info("wykaz dla określonych działek z pliku tekstowego.");
            }
            Console.WriteLine("Działki do wykazu: {0}", działki.Count());
            return działki;
        }

        static void ZapiszWykaz(IGrouping<string, IdentyfikatorDziałki> działkiObrębu)
        {
            string idObrebu = int.Parse(działkiObrębu.Key).ToString("0000");
            string ext = "." + idObrebu + ".docx";
            string name = _szablonZmian.asExt(ext).name();
            string fileName = Path.Combine(_docelowyKatalog, name);
            Console.WriteLine("Wykaz dla obrębu: {0} ({1} działki w obrębie)...", fileName.name(), działkiObrębu.Count());
            File.Copy(_szablonZmian, fileName);
            var dokument = new WykazZmianDocX(fileName, _repozytoriumKERG)
            {
                WykażDodane = Settings.Default.WykazDodane,
                WykażUsunięte = Settings.Default.WykazUsuniete,
                Sporządził = Settings.Default.Sporzadzil
            };
            dokument.DodajDziałki(_staryStan, _nowyStan, działkiObrębu);
            dokument.Zapisz();
        }
    }
}
