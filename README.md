# Bibliotekssystem - Arv & Komposition

Ett bibliotekshanteringssystem byggt i C# (.NET 10) som demonstrerar objektorienterade principer: **arv**, **polymorfism**, **komposition** och **interface**.

## Projektstruktur

```
Bibliotekssystem_Arv_Komp/
├── LibraryItem.cs      # Abstrakt basklass for alla biblioteksobjekt
├── Book.cs             # Bok (arver LibraryItem)
├── DVD.cs              # DVD (arver LibraryItem)
├── Magazine.cs         # Tidskrift (arver LibraryItem)
├── ISearchable.cs      # Interface for sokfunktionalitet
├── Member.cs           # Biblioteksmedlem
├── Loan.cs             # Lan-hantering
├── ItemCatalog.cs      # Katalog med sok, sortering och statistik
├── LoanManager.cs      # Hanterar utlaningar och returer
└── Program.cs          # Konsolapplikation med demo

Bibliotekssystem_Arv_Komp.Test/
└── BibliotekssytemTest.cs  # xUnit-tester (12 tester)
```

## OOP-koncept

| Koncept | Implementation |
|---------|---------------|
| **Arv** | `Book`, `DVD`, `Magazine` arver fran `LibraryItem` |
| **Abstraktion** | `LibraryItem` ar abstrakt med `GetInfo()` |
| **Polymorfism** | `ISearchable` implementeras av bade `LibraryItem` och `Member` |
| **Komposition** | `ItemCatalog` innehaller `LibraryItem`, `LoanManager` hanterar `Loan` |
| **Inkapsling** | `Member.Loans` exponeras som `IReadOnlyList<Loan>` |

## Funktioner

- Hantera bocker, DVD:er och tidskrifter
- Sok pa titel, forfattare, ISBN och allman fritext
- Sortera efter titel, utgivningsar och forfattare
- Lanehantering med forfallodat och forsening
- Statistik over katalog och mest aktiva lantagare

## Kora projektet

```bash
dotnet run --project Bibliotekssystem_Arv_Komp
```

## Kora tester

```bash
dotnet test
```
