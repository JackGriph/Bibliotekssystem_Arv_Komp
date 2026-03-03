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

Bibliotekssystem_Arv_Komp.Data/        # Datalager (EF Core + Repository + Service)
├── Context/
│   ├── LibraryContext.cs              # DbContext med TPH-konfiguration
│   └── DbSeeder.cs                   # Seed data (14 böcker, 3 medlemmar)
├── Repositories/
│   ├── IBookRepository.cs            # Interface - böcker
│   ├── BookRepository.cs             # Implementation - böcker
│   ├── IMemberRepository.cs          # Interface - medlemmar
│   ├── MemberRepository.cs           # Implementation - medlemmar
│   ├── ILoanRepository.cs            # Interface - lån
│   └── LoanRepository.cs             # Implementation - lån
└── Services/
    ├── ILoanService.cs                # Interface - affärslogik utlåning
    └── LoanService.cs                 # Implementation (max 3 lån, tillgänglighetskontroll)

Bibliotekssystem_Arv_Komp.Web/         # Blazor Server-webbapplikation
├── Controllers/
│   ├── BooksController.cs             # REST API: /api/books
│   ├── MembersController.cs           # REST API: /api/members
│   ├── LoansController.cs             # REST API: /api/loans
│   └── StatsController.cs             # REST API: /api/stats
├── Dtos/
│   └── LibraryDtos.cs                 # Data Transfer Objects (delade av alla sidor)
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
└── Program.cs                         # DI-konfiguration, API-mappning och seed

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

## Del 2: EF Core, Blazor & REST API

### Arkitektur

```
Blazor-sidor  →  HttpClient  →  REST API (Controllers)  →  Services / Repositories  →  EF Core  →  SQLite
```

### Entity Framework Core
- **SQLite**-databas med `LibraryContext`
- **TPH** (Table-Per-Hierarchy) för arvshierarkin `LibraryItem` → `Book`
- **Repository Pattern** med interfaces och implementationer (Book, Member, Loan)
- **Service Layer** med `ILoanService` / `LoanService` for affärslogik (max 3 lån, tillgänglighetskontroll)
- Seed data (14 böcker + 3 medlemmar) skapas automatiskt vid första körning via `DbSeeder`

### REST API
- 4 controllers: Books, Members, Loans, Stats
- DTOs (`LibraryDtos.cs`) separerar API-kontrakt från domänmodeller
- Blazor-sidorna anropar API:t via `HttpClient` (inte direkt mot databasen)

### Blazor Server
- 8 sidor: Startsida, Boklista, Bokdetaljer, Medlemmar, Medlemsdetaljer, Utlåning, Formulär (bok + medlem)
- **Återanvändbar komponent**: `BookCard.razor` med `[Parameter]` (B.3)
- **Formulärvalidering**: `EditForm` med `DataAnnotationsValidator` och `ValidationMessage` (B.4)
- Dependency Injection med interfaces registrerade i `Program.cs`

### SOLID-principer
| Princip | Implementation |
|---------|---------------|
| **SRP** | Controllers hanterar HTTP, Services hanterar affärslogik, Repositories hanterar data |
| **OCP** | Nya repositories/services kan läggas till utan att ändra befintlig kod |
| **LSP** | `Book` kan användas överallt där `LibraryItem` förväntas (TPH) |
| **ISP** | Separata interfaces: `IBookRepository`, `IMemberRepository`, `ILoanRepository` |
| **DIP** | Controllers och Services beror på interfaces, inte konkreta klasser |

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
