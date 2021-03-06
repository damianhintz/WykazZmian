# WykazZmian v1.6-beta, 1 grudnia 2015, Copyright (c) 2013-2015 OPGK Olsztyn. Wszelkie prawa zastrzeżone.
---
Wykaz zmian działek i użytków z plików opisowych SWDE

# Nowe funkcje

* wykaz zmian dla określonych działek z pliku tekstowego
* wykaz zmian dla wszystkich działek z pliku SWDE
* wykaz zmian dla usuniętych i dodanych działek
* wykaz rozbieżności z właścicielami i władającymi
* repozytorium numerów KERG dla obrębu
* zapis raportu wykazu zmian na podstawie szablonu do formatu docx
* scalanie użytków z takim samym oznaczeniem w jeden

# Pomoc

WykazZmian.exe {roboczy katalog} {stare swde} {nowe swde} {lista plików z działkami} {szablon wykazu}
 
> WykazZmian . stare.swd nowe.swd *.dzialki WykazZmianGruntu.docx

> WykazZmian . stare.swd nowe.swd *.dzialki egib.WykazRozbieżności.docx

# Historia

Do zrobienia

- [ ] poprawka: numer księgi wieczystej tylko dla dokumentów, gdzie KDK=5

2015-12-01 v1.6-beta

* nowa funkcja: porównywanie rozliczenia klasoużytków (grafiki) z opisem

2015-08-24 v1.5+opegieka

* nowa funkcja: nowy szablon wykazu zmian zawierający jednostkę rejestrową i numer KW
* nowa funkcja: sortowanie działek po jednostce rejestrowej

2015-05-15 v1.4+opegieka

* licencja dla OPEGIEKA Sp. z o.o. Elbląg

2015-03-17 v1.3.2

* poprawka: nowy komunikat z ostrzeżeniem, nie można dodać użytku do działki, gdyż działka bez opisu nie została poprawnie wczytana

2015-03-16 v1.3.1

* poprawka: działki bez części opisowej, nie zostaną dodane do wykazu

2015-03-16 v1.3

* nowa funkcja: wykaż usunięte/stare działki
* nowa funkcja: wykaż dodane/nowe działki
* nowa funkcja: wybierz do wykazu działki z plików SWDE, jeżeli brak pliku z listą działek (*.dzialki)

2015-02-13 v1.2

* aktualizacja: zaokrąglanie powierzchni do ara

2015-01-20 v1.1.2

* poprawka: identyfikator działki może opcjonalnie zawierać numer arkusza w formacie {numer obrębu}-[numer arkusza.]{numer działki} np. 1-1.313/1

2015-01-20 v1.1.1

* poprawka: sortowanie działek zawierających w numerze na końcu znaki inne niż cyfry

2014-04-01 v1.1

* aktualizacja: pomijanie działek z wykazu zmian, które nie uległy zmianie

2013-12-20 v1.0-beta

* nowa funkcja: wykaz zmian i rozbieżności na podstawie plików SWDE
* nowa funkcja: zapis wykazów w formacie docx
* nowa funkcja: zmienna [KERG] obrębu z pliku (*.kerg)
* nowa funkcja: konfigurowalna zmienna [SPORZADZIL] z pliku (*.config)
* nowa funkcja: wykaz rozbieżności zawiera właścicieli i władających działki (zmienna [WL])
* aktualizacja: podmiot grupowy z członkami
* aktualizacja: scalanie pustych kolumn

2013-12-01 v1.0-alfa

* pierwsza wersja alfa
* nowa funkcja: wykaz rozbieżności na podstawie zapytań SQL z systemu Ewopis
* nowa funkcja: zapis wykazów w formacie *.doc
