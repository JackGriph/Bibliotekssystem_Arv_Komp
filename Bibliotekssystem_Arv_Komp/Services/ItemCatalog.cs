namespace Bibliotekssystem_Arv_Komp
{
    public class ItemCatalog
    {
        private List<LibraryItem> _items;

        public ItemCatalog()
        {
            _items = new List<LibraryItem>();
        }

        // === GRUNDLÄGGANDE OPERATIONER ===

        public void AddItem(LibraryItem item)
        {
            if (_items.Any(i => i.ItemId == item.ItemId))
            {
                Console.WriteLine($"Ett objekt med ID {item.ItemId} finns redan i katalogen.");
                return;
            }
            _items.Add(item);
            Console.WriteLine($"{item.GetItemType()} '{item.Title}' har lagts till i katalogen.");
        }

        public bool RemoveItem(string itemId)
        {
            var item = _items.FirstOrDefault(i => i.ItemId == itemId);
            if (item != null)
            {
                _items.Remove(item);
                Console.WriteLine($"{item.GetItemType()} '{item.Title}' har tagits bort från katalogen.");
                return true;
            }
            Console.WriteLine($"Inget objekt med ID {itemId} hittades.");
            return false;
        }

        public LibraryItem? FindItemById(string itemId)
        {
            return _items.FirstOrDefault(i => i.ItemId == itemId);
        }

        // === DEL 4: SÖKFUNKTIONER ===

        // 1. Allmän sökning med ISearchable (polymorfism)
        public List<LibraryItem> Search(string searchTerm)
        {
            return _items.Where(item => item.Matches(searchTerm)).ToList();
        }

        // 2. Sök specifikt på titel
        public List<LibraryItem> SearchByTitle(string title)
        {
            return _items.Where(i => i.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        // 3. Sök böcker på författare
        public List<Book> SearchByAuthor(string author)
        {
            return _items.OfType<Book>()
                .Where(book => book.Author.Contains(author, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        // 4. Sök bok på ISBN
        public Book? SearchByISBN(string isbn)
        {
            return _items.OfType<Book>().FirstOrDefault(b => b.ISBN == isbn);
        }

        // === DEL 4: SORTERINGSFUNKTIONER ===

        // 1. Sortera alla items alfabetiskt efter titel
        public List<LibraryItem> GetItemsSortedByTitle()
        {
            return _items.OrderBy(item => item.Title).ToList();
        }

        // 2. Sortera alla items efter utgivningsår
        public List<LibraryItem> GetItemsSortedByYear(bool ascending = true)
        {
            return ascending
                ? _items.OrderBy(item => item.PublishedYear).ToList()
                : _items.OrderByDescending(item => item.PublishedYear).ToList();
        }

        // 3. Sortera böcker efter författare
        public List<Book> GetBooksSortedByAuthor()
        {
            return _items.OfType<Book>()
                .OrderBy(book => book.Author)
                .ThenBy(book => book.Title)
                .ToList();
        }

        // === HJÄLPMETODER ===

        public List<LibraryItem> GetAllItems()
        {
            return _items.ToList();
        }

        public List<T> GetItemsOfType<T>() where T : LibraryItem
        {
            return _items.OfType<T>().ToList();
        }

        // === DEL 4: STATISTIK ===

        public CatalogStatistics GetStatistics()
        {
            return new CatalogStatistics
            {
                TotalItems = _items.Count,
                TotalBooks = _items.OfType<Book>().Count(),
                TotalMagazines = _items.OfType<Magazine>().Count(),
                TotalDVDs = _items.OfType<DVD>().Count(),
                AvailableItems = _items.Count(i => i.IsAvailable),
                LoanedItems = _items.Count(i => !i.IsAvailable),
                LoanedBooks = _items.OfType<Book>().Count(b => !b.IsAvailable)
            };
        }

        // === VISNINGSMETODER ===

        public void DisplaySearchResults(string searchTerm)
        {
            var results = Search(searchTerm);

            if (!results.Any())
            {
                Console.WriteLine($"Inga resultat för sökning: '{searchTerm}'");
                return;
            }

            Console.WriteLine($"\n=== Sökresultat för '{searchTerm}' ({results.Count} träffar) ===");
            foreach (var item in results)
            {
                Console.WriteLine($"\n{item.GetInfo()}");
                Console.WriteLine(new string('-', 40));
            }
        }

        public void DisplaySortedByTitle()
        {
            var sorted = GetItemsSortedByTitle();
            Console.WriteLine($"\n=== Sorterat efter titel (A-Ö) ===");
            foreach (var item in sorted)
            {
                Console.WriteLine($"- {item.Title} ({item.GetItemType()}, {item.PublishedYear})");
            }
        }

        public void DisplaySortedByYear(bool ascending = true)
        {
            var sorted = GetItemsSortedByYear(ascending);
            string order = ascending ? "äldst först" : "nyast först";
            Console.WriteLine($"\n=== Sorterat efter år ({order}) ===");
            foreach (var item in sorted)
            {
                Console.WriteLine($"- {item.PublishedYear}: {item.Title} ({item.GetItemType()})");
            }
        }

        public void DisplayStatistics()
        {
            var stats = GetStatistics();

            Console.WriteLine($"\n╔══════════════════════════════════════════════╗");
            Console.WriteLine($"║         KATALOGSTATISTIK                     ║");
            Console.WriteLine($"╚══════════════════════════════════════════════╝");

            Console.WriteLine($"\n📚 OBJEKT:");
            Console.WriteLine($"   Totalt antal: {stats.TotalItems}");
            Console.WriteLine($"   - Böcker: {stats.TotalBooks}");
            Console.WriteLine($"   - Tidskrifter: {stats.TotalMagazines}");
            Console.WriteLine($"   - DVD:er: {stats.TotalDVDs}");
            Console.WriteLine($"\n📊 STATUS:");
            Console.WriteLine($"   Tillgängliga: {stats.AvailableItems}");
            Console.WriteLine($"   Utlånade: {stats.LoanedItems} (varav {stats.LoanedBooks} böcker)");
        }
    }

    public class CatalogStatistics
    {
        public int TotalItems { get; init; }
        public int TotalBooks { get; init; }
        public int TotalMagazines { get; init; }
        public int TotalDVDs { get; init; }
        public int AvailableItems { get; init; }
        public int LoanedItems { get; init; }
        public int LoanedBooks { get; init; }
    }
}