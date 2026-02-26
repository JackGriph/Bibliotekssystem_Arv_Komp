# Bibliotekssystem - Arv & Komposition

Ett bibliotekshanteringssystem byggt i C# (.NET 10) som demonstrerar objektorienterade principer: **arv**, **polymorfism**, **komposition** och **interface**. Projektet innehåller en konsolapplikation (Del 1) samt en webbapplikation med Entity Framework Core och Blazor (Del 2).

## Projektstruktur

```
Bibliotekssystem_Arv_Komp/             # Kärnbibliotek (modeller, tjänster, konsol-demo)
├── Interfaces/
│   └── ISearchable.cs                 # Interface för sökfunktionalitet
├── Models/
│   ├── LibraryItem.cs                 # Abstrakt basklass för alla biblioteksobjekt
│   ├── Book.cs                        # Bok (ärver LibraryItem)
│   ├── DVD.cs                         # DVD (ärver LibraryItem)
│   ├── Magazine.cs                    # Tidskrift (ärver LibraryItem)
│   ├── Member.cs                      # Biblioteksmedlem
│   └── Loan.cs                        # Lån-hantering
├── Services/
│   ├── ItemCatalog.cs                 # Katalog med sök, sortering och statistik
│   └── LoanManager.cs                # Hanterar utlåningar och returer
└── Program.cs                         # Konsolapplikation med demo

Bibliotekssystem_Arv_Komp.Data/        # Datalager (EF Core + Repository Pattern)
├── LibraryContext.cs                  # DbContext med TPH-konfiguration
├── IBookRepository.cs                 # Repository-interface
└── BookRepository.cs                  # Repository-implementation

Bibliotekssystem_Arv_Komp.Web/         # Blazor Server-webbapplikation
├── Components/
│   ├── BookCard.razor                 # Återanvändbar bokkortskomponent (B.3)
│   ├── Layout/
│   │   ├── MainLayout.razor           # Huvudlayout
│   │   └── NavMenu.razor              # Navigation
│   └── Pages/
│       ├── Home.razor                 # Startsida med statistik
│       ├── Books.razor                # Boklista med sök och sortering
│       ├── BookDetails.razor          # Bokdetaljer och lånehistorik
│       ├── AddBook.razor              # Formulär: lägg till bok (B.4)
│       ├── Members.razor              # Medlemslista med aktiva lån
│       ├── MemberDetails.razor        # Medlemsdetaljer
│       ├── AddMember.razor            # Formulär: registrera medlem (B.4)
│       └── Loans.razor                # Utlåning med EditForm-validering (B.4)
└── Program.cs                         # Blazor-app med DI och seed data

Bibliotekssystem_Arv_Komp.Test/        # xUnit-tester (22 tester totalt)
├── BibliotekssytemTest.cs             # Del 1: 12 tester (modeller, sökning, statistik)
└── Del2Tests.cs                       # Del 2: 10 tester (repository, CRUD, integration)
```

## OOP-koncept

| Koncept | Implementation |
|---------|---------------|
| **Arv** | `Book`, `DVD`, `Magazine` ärver från `LibraryItem` |
| **Abstraktion** | `LibraryItem` är abstrakt med `GetInfo()` |
| **Polymorfism** | `ISearchable` implementeras av både `LibraryItem` och `Member` |
| **Komposition** | `ItemCatalog` innehåller `LibraryItem`, `LoanManager` hanterar `Loan` |
| **Inkapsling** | `Member.Loans` exponeras som `IReadOnlyList<Loan>` |

## Del 2: EF Core & Blazor

### Entity Framework Core
- **SQLite**-databas med `LibraryContext`
- **TPH** (Table-Per-Hierarchy) för arvshierarkin `LibraryItem` → `Book`
- **Repository Pattern** med `IBookRepository` / `BookRepository`
- Seed data skapas automatiskt vid första körning

### Blazor Server
- 7 sidor: Startsida, Boklista, Bokdetaljer, Medlemmar, Medlemsdetaljer, Utlåning, Formulär
- **Återanvändbar komponent**: `BookCard.razor` med `[Parameter]` (B.3)
- **Formulärvalidering**: `EditForm` med `DataAnnotationsValidator` och `ValidationMessage` (B.4)
- `IDbContextFactory` för korrekt DbContext-hantering i Blazor

### Tester Del 2 (10 st)
- **Repository/DbContext** (4 tester): GetAll, GetByISBN, Search, TPH-lagring
- **Databasoperationer CRUD** (3 tester): Add, Update, Delete
- **Integration EF + affärslogik** (3 tester): Skapa lån, returnera lån, max-lån-gräns

## Köra projektet

```bash
# Konsolapplikation (Del 1)
dotnet run --project Bibliotekssystem_Arv_Komp

# Webbapplikation (Del 2)
dotnet run --project Bibliotekssystem_Arv_Komp.Web
```

## Köra tester

```bash
dotnet test
```
