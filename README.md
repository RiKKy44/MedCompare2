
# MedCompare

**MedCompare** is a local desktop application for checking medicine information, active substances, known substance interactions, Polish medicinal product registry data, and ICD-11 codes.

The application is built as a **local medical reference and clinical decision-support prototype**. It runs on the user's computer and does not send medical data to external APIs.

> **Medical disclaimer**  
> MedCompare is an educational and technical prototype. It does not replace a physician, pharmacist, or any qualified medical professional. Missing interaction data does not mean that a combination is safe. Every result must be clinically verified.

---

# 🇵🇱 Opis projektu

MedCompare to aplikacja desktopowa napisana w technologii **WPF / .NET 8**, służąca do lokalnego wyszukiwania informacji o lekach, substancjach czynnych, znanych interakcjach, polskim rejestrze produktów leczniczych oraz kodach **ICD-11**.

Projekt działa lokalnie na komputerze użytkownika. Obecna wersja korzysta z lokalnej bazy **SQLite**, dzięki czemu aplikacja może zostać uruchomiona jako wersja portable — bez instalowania PostgreSQL i bez zewnętrznego serwera bazy danych.

Projekt ma charakter prototypu edukacyjno-technicznego. Jego celem jest pokazanie, jak można zbudować lokalną aplikację referencyjną dla danych lekowych i klasyfikacyjnych.

---

## Najważniejsze funkcje

### 1. Interaction Checker

Moduł sprawdzania interakcji między substancjami czynnymi.

Funkcje:

- wyszukiwanie leku po nazwie,
- automatyczne wykrywanie substancji czynnych leku,
- akceptowanie wykrytych substancji do aktualnego przypadku,
- ręczne dodawanie substancji czynnych,
- sprawdzanie znanych interakcji między zaakceptowanymi substancjami,
- wyświetlanie poziomu interakcji / severity,
- wyświetlanie źródła danych,
- eksport raportu tekstowego.

Przepływ danych:

```text
Drug name
  ↓
EMA drug record
  ↓
active substances
  ↓
DDInter substance mapping
  ↓
substance_interactions
  ↓
interaction result
```

W obecnej wersji checker korzysta z lokalnych danych w SQLite i sprawdza interakcje na poziomie substancji czynnych.

Ważna zasada bezpieczeństwa:

```text
No known interaction was found in the local database.
Missing interaction data does not mean that the combination is safe.
```

Brak znalezionej interakcji nie oznacza, że połączenie jest bezpieczne.

---

### 2. Drug Lookup

Moduł wyszukiwania leków.

Funkcje:

* wyszukiwanie leku po nazwie,
* pobieranie przypisanych substancji czynnych,
* mapowanie leku na substancje z lokalnej tabeli `active_substances`,
* przekazywanie substancji do Interaction Checkera.

W obecnej wersji główne wyszukiwanie leków bazuje na danych EMA zaimportowanych lokalnie do tabel:

```text
drugs
drug_active_substances
active_substances
```

Polski Rejestr Produktów Leczniczych nie jest używany jako główne źródło dla Interaction Checkera. Jest dostępny w osobnym module.

---

### 3. Manual Active Substance

Moduł ręcznego dodawania substancji czynnej.

Funkcje:

* wpisanie nazwy substancji czynnej,
* wyszukanie substancji w lokalnej bazie `active_substances`,
* dodanie jej do zaakceptowanych substancji,
* sprawdzanie interakcji także dla substancji dodanych ręcznie.

Ten tryb jest przydatny, gdy użytkownik zna dokładną nazwę substancji albo chce przetestować konkretną parę substancji.

---

### 4. Drug Explorer

Moduł eksploracji lokalnej bazy leków.

Funkcje:

* wyszukiwanie leków w lokalnej bazie,
* podgląd nazwy leku,
* podgląd producenta / podmiotu, jeżeli dane są dostępne,
* podgląd powiązanych substancji czynnych,
* szybka inspekcja danych zaimportowanych z EMA.

Ten moduł służy bardziej do przeglądania danych niż do bezpośredniego sprawdzania interakcji.

---

### 5. Polish Drug Registry

Moduł wyszukiwania w polskim rejestrze produktów leczniczych.

Funkcje:

* wyszukiwanie produktów leczniczych z polskiego rejestru,
* podgląd nazwy produktu,
* podgląd substancji czynnej zapisanej w rejestrze,
* podgląd numerów i danych rejestracyjnych, jeżeli występują w imporcie,
* obsługa linków do dokumentów, jeżeli są dostępne.

Dane pochodzą z Rejestru Produktów Leczniczych prowadzonego przez URPL.

Ten moduł jest oddzielny od Interaction Checkera, ponieważ dane z rejestru krajowego mogą zawierać opisy tekstowe, nazwy szczepów, preparaty złożone lub zapisy, które nie zawsze są bezpośrednio zgodne z identyfikatorami substancji w DDInter.

---

### 6. ICD Looker

Moduł wyszukiwania kodów **ICD-11**.

To nie jest ICD-10.

Aplikacja używa lokalnie zaimportowanych danych ICD-11 w języku polskim. Moduł umożliwia:

* wyszukiwanie po kodzie,
* wyszukiwanie po nazwie jednostki,
* wyszukiwanie po opisie,
* filtrowanie po kategoriach,
* przeglądanie wyników klasyfikacji.

ICD-11 jest jedenastą rewizją Międzynarodowej Klasyfikacji Chorób. Według WHO ICD-11 została przyjęta przez World Health Assembly w 2019 roku i globalnie weszła w życie 1 stycznia 2022 roku.

W Polsce wdrożenie ICD-11 jest procesem przejściowym. Trwają projekty i prace wspierające wdrożenie ICD-11 w systemie ochrony zdrowia. W praktyce projekt należy traktować jako przygotowany pod ICD-11 i okres wdrożeniowy w Polsce przypadający na lata 2026–2027, z roboczym założeniem przejścia od końca 2026 roku. Dokładny zakres i obowiązkowość stosowania zależą od decyzji oraz harmonogramu instytucji publicznych.

---

### 7. Database Status

Moduł statusu bazy danych.

Funkcje:

* liczba leków,
* liczba substancji czynnych,
* liczba relacji lek–substancja,
* liczba interakcji,
* szybkie potwierdzenie, czy aplikacja czyta właściwy plik SQLite.

Przykładowe tabele sprawdzane przez status:

```text
drugs
active_substances
drug_active_substances
substance_interactions
```

---

### 8. Export Report

Moduł eksportu raportu.

Funkcje:

* eksport aktualnie zaakceptowanych substancji,
* eksport wykrytych interakcji,
* zapis informacji o źródłach,
* zapis komunikatu bezpieczeństwa.

Raport ma charakter pomocniczy i nie jest dokumentem medycznym.

---

### 9. Audit Log

W obecnej wersji **Audit Log jest częściowo zaimplementowany, ale nie jest traktowany jako stabilna funkcja produkcyjna**.

Status:

```text
Audit Log: not fully working in the current version.
```

Znane ograniczenia:

* zapis zdarzeń może zależeć od zgodności schematu tabeli `audit_logs`,
* część wcześniejszych błędów dotyczyła różnic między kolumnami `event_type` i `action`,
* funkcja wymaga dalszego uporządkowania i testów.

Audit Log zostaje w projekcie jako element architektury, ale obecna wersja aplikacji nie powinna być oceniana przez pryzmat tej funkcji.

---

### 10. History

W obecnej wersji **History również nie działa jako stabilna funkcja końcowa**.

Status:

```text
History: not fully working in the current version.
```

Znane ograniczenia:

* historia wymaga dopracowania zapisu i odczytu,
* część przepływu zależy od działania serwisów audytu / historii,
* obecnie głównym działającym modułem jest Interaction Checker, Drug Lookup, Drug Explorer, Polish Drug Registry, ICD Looker i Database Status.

History zostaje w projekcie jako planowany moduł do dalszego rozwoju.

---

## Architektura aplikacji

Projekt jest podzielony warstwowo:

```text
Views
ViewModels
Services
Repositories
Database
Models
```

Główny przepływ:

```text
WPF UI
  ↓
MainViewModel
  ↓
Application services
  ↓
Repository interfaces
  ↓
SQLite repositories
  ↓
data/medcompare.db
```

Najważniejsze repozytoria SQLite:

```text
SqliteDrugRepository
SqliteSubstanceRepository
SqliteInteractionRepository
SqliteDrugExplorerRepository
SqlitePolishDrugRegistryRepository
SqliteIcdCodeRepository
SqliteAuditLogRepository
```

Najważniejsze serwisy:

```text
IDrugLookupService
ISubstanceLookupService
IInteractionCheckerService
IDrugExplorerService
IPolishDrugRegistryService
IIcdCodeService
IDatabaseStatusService
```

---

## Baza danych

Obecna wersja portable korzysta z pliku:

```text
data/medcompare.db
```

Najważniejsze tabele:

| Tabela                       | Opis                                            |
| ---------------------------- | ----------------------------------------------- |
| `drugs`                      | lokalna lista leków, głównie dane EMA           |
| `drug_active_substances`     | relacja lek → substancja czynna                 |
| `active_substances`          | słownik substancji czynnych                     |
| `substance_interactions`     | znane interakcje między substancjami            |
| `polish_drug_registry_items` | dane z polskiego rejestru produktów leczniczych |
| `icd_codes`                  | lokalna baza ICD-11                             |
| `audit_logs`                 | tabela audytu, funkcja w trakcie dopracowania   |

---

## Źródła danych

Projekt korzysta z lokalnie zaimportowanych danych pochodzących z publicznych lub otwartych źródeł.

### EMA

Źródło danych lekowych używane do listy leków i substancji:

```text
European Medicines Agency — medicines data
```

EMA udostępnia dane dotyczące leków publikowane na stronie agencji, w tym możliwość pobrania danych w formacie tabelarycznym.

### DDInter / DDInter 2.0

Źródło danych o interakcjach lek–lek / substancja–substancja:

```text
DDInter
DDInter 2.0
```

DDInter jest otwartą bazą danych interakcji lekowych, zawierającą informacje o poziomach ryzyka, mechanizmach i opisach interakcji.

W projekcie dane DDInter zostały przekształcone lokalnie do tabel:

```text
active_substances
substance_interactions
```

### Rejestr Produktów Leczniczych URPL

Źródło danych dla polskiego modułu rejestru leków:

```text
Rejestr Produktów Leczniczych Dopuszczonych do Obrotu na terytorium RP
URPL
```

Dane te są wykorzystywane w osobnej zakładce Polish Drug Registry.

### ICD-11

Źródła klasyfikacji ICD-11:

```text
WHO ICD-11 Browser
WHO ICD-11 for Mortality and Morbidity Statistics
Polskie tłumaczenie ICD-11 publikowane przez Centrum e-Zdrowia
Materiały Ministerstwa Zdrowia dotyczące wdrożenia ICD-11
```

W aplikacji ICD Looker dotyczy ICD-11, nie ICD-10.

---

## Tryb portable

Aplikacja może działać jako paczka portable.

Oczekiwany układ folderu:

```text
publish/portable/
├─ DrugCompare.exe
├─ appsettings.json
└─ data/
   └─ medcompare.db
```

Konfiguracja dla wersji portable:

```json
{
  "Database": {
    "Provider": "SQLite"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=data/medcompare.db"
  }
}
```

Plik `medcompare.db` nie powinien być commitowany do repozytorium, ponieważ jest duży i zawiera wygenerowany/importowany dataset. Powinien być dostarczany osobno jako artifact release albo kopiowany lokalnie.

---

## Uruchomienie lokalne

Wymagania:

```text
Windows
.NET 8 SDK
Visual Studio 2022 / 2026
SQLite database file: data/medcompare.db
```

Uruchomienie:

```powershell
git clone https://github.com/Faldekk/MedCompare2.git
cd MedCompare2/DrugCompare/DrugCompare
dotnet restore
dotnet build
dotnet run
```

Jeżeli aplikacja nie znajduje danych, należy sprawdzić, czy plik bazy znajduje się w:

```text
data/medcompare.db
```

lub czy `appsettings.json` wskazuje właściwą ścieżkę.

---

## Publikacja wersji portable

```powershell
dotnet clean
Remove-Item -Recurse -Force .\bin, .\obj -ErrorAction SilentlyContinue

dotnet publish .\DrugCompare.csproj `
  -c Release `
  -r win-x64 `
  --self-contained true `
  /p:PublishSingleFile=true `
  /p:IncludeNativeLibrariesForSelfExtract=true `
  /p:EnableCompressionInSingleFile=true `
  -o .\publish\portable

New-Item -ItemType Directory -Path .\publish\portable\data -Force

Copy-Item .\data\medcompare.db .\publish\portable\data\medcompare.db -Force
Copy-Item .\appsettings.json .\publish\portable\appsettings.json -Force

Compress-Archive `
  -Path .\publish\portable\* `
  -DestinationPath .\MedCompare-portable.zip `
  -Force
```

Po rozpakowaniu ZIP-a aplikacja powinna uruchamiać się bez instalowania PostgreSQL.

---

## Ograniczenia obecnej wersji

Obecna wersja działa jako lokalny prototyp, ale ma ograniczenia:

* aplikacja nie zastępuje decyzji lekarza lub farmaceuty,
* dane interakcji zależą od kompletności lokalnej bazy,
* brak interakcji w bazie nie oznacza bezpieczeństwa,
* Audit Log nie jest jeszcze stabilny,
* History nie jest jeszcze stabilne,
* dane ICD-11 są lokalnym importem i mogą wymagać aktualizacji,
* dane EMA / DDInter / RPL wymagają okresowego odświeżania,
* duplikaty substancji mogą wymagać dodatkowego czyszczenia danych.

---

## Status funkcji

| Funkcja              | Status                      |
| -------------------- | --------------------------- |
| SQLite portable mode | Działa                      |
| Drug Lookup          | Działa                      |
| Manual Substance Add | Działa                      |
| Interaction Checker  | Działa                      |
| Drug Explorer        | Działa                      |
| Polish Drug Registry | Działa                      |
| ICD-11 Looker        | Działa                      |
| Database Status      | Działa                      |
| Export Report        | Działa                      |
| Audit Log            | Częściowo / wymaga poprawek |
| History              | Częściowo / wymaga poprawek |

---

## Roadmap

Planowane dalsze prace:

* uporządkowanie Audit Log,
* naprawa i stabilizacja History,
* lepsze czyszczenie duplikatów substancji,
* wersjonowanie importów danych,
* osobny ekran informacji o źródłach danych,
* automatyczna walidacja bazy SQLite przy starcie,
* lepsze komunikaty błędów dla użytkownika,
* poprawa UI i dostępności,
* przygotowanie GitHub Release z paczką portable.

---

## Shoutout

Projekt rozwijany jako praktyczny prototyp lokalnej aplikacji medycznej i systemu wspierającego analizę danych lekowych.

Special shoutout dla taty za bycie realnym powodem, żeby doprowadzić aplikację do wersji portable, która faktycznie działa poza środowiskiem developerskim.

---

# English Summary

MedCompare is a local WPF / .NET 8 desktop application for drug lookup, active substance mapping, substance interaction checking, Polish medicinal product registry search, and ICD-11 lookup.

The application runs locally and uses SQLite in portable mode. It does not require PostgreSQL in the current portable version.

The ICD module uses ICD-11, not ICD-10. ICD-11 came into global effect on 1 January 2022 according to WHO. In Poland, ICD-11 implementation is still a transition process supported by public projects and should be treated as an ongoing 2026–2027 implementation area rather than a fully settled production standard inside this prototype.

Current working modules:

* Drug Lookup
* Manual Substance Add
* Interaction Checker
* Drug Explorer
* Polish Drug Registry
* ICD-11 Looker
* Database Status
* Export Report

Current non-stable modules:

* Audit Log
* History

MedCompare is a technical and educational clinical decision-support prototype. It must not be used as a replacement for professional medical judgment.

```
```
