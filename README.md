# Bibliotekssystem - Arv & Komposition

Ett bibliotekshanteringssystem byggt i C# (.NET 10) som demonstrerar objektorienterade principer: **arv**, **polymorfism**, **komposition** och **interface**.

## Projektstruktur

```
Bibliotekssystem_Arv_Komp/
├── LibraryItem.cs      # Abstrakt basklass för alla biblioteksobjekt
├── Book.cs             # Bok (ärver LibraryItem)
├── DVD.cs              # DVD (ärver LibraryItem)
├── Magazine.cs         # Tidskrift (ärver LibraryItem)
├── ISearchable.cs      # Interface för sökfunktionalitet
├── Member.cs           # Biblioteksmedlem
├── Loan.cs             # Lån-hantering
├── ItemCatalog.cs      # Katalog med sök, sortering och statistik
├── LoanManager.cs      # Hanterar utlåningar och returer
└── Program.cs          # Konsolapplikation med demo

Bibliotekssystem_Arv_Komp.Test/
└── BibliotekssytemTest.cs  # xUnit-tester (12 tester)
```

## OOP-koncept

| Koncept | Implementation |
|---------|---------------|
| **Arv** | `Book`, `DVD`, `Magazine` ärver fran `LibraryItem` |
| **Abstraktion** | `LibraryItem` är abstrakt med `GetInfo()` |
| **Polymorfism** | `ISearchable` implementeras av både `LibraryItem` och `Member` |
| **Komposition** | `ItemCatalog` innehåller `LibraryItem`, `LoanManager` hanterar `Loan` |
| **Inkapsling** | `Member.Loans` exponeras som `IReadOnlyList<Loan>` |

## Funktioner

- Hantera böcker, DVD:er och tidskrifter
- Sök på titel, författare, ISBN och allmän fritext
- Sortera efter titel, utgivningsår och författare
- Lånehantering med förfallodat och försening
- Statistik över katalog och mest aktiva lantagare

## Kora projektet

```bash
dotnet run --project Bibliotekssystem_Arv_Komp
```

## Kora tester

```bash
dotnet test
```
