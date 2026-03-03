using Bibliotekssystem_Arv_Komp.Models;

namespace Bibliotekssystem_Arv_Komp.Services
{
    public class ItemCatalog
    {
        private List<LibraryItem> _items;

        public ItemCatalog()
        {
            _items = new List<LibraryItem>();
        }

        // === GRUNDLÄGGANDE OPERATIONER ===

        /// <summary>
        /// Lägger till ett objekt i katalogen. Returnerar true om lyckat, false om ID redan finns.
        /// </summary>
        public bool AddItem(LibraryItem item)
        {
            if (_items.Any(i => i.ItemId == item.ItemId))
                return false;

            _items.Add(item);
            return true;
        }

        /// <summary>
        /// Tar bort ett objekt. Returnerar true om borttaget, false om ej hittat.
        /// </summary>
        public bool RemoveItem(string itemId)
        {
            var item = _items.FirstOrDefault(i => i.ItemId == itemId);
            if (item == null)
                return false;

            _items.Remove(item);
            return true;
        }

        public LibraryItem? FindItemById(string itemId)
        {
            return _items.FirstOrDefault(i => i.ItemId == itemId);
        }

        // === DEL 4: SÖKFUNKTIONER ===

        public List<LibraryItem> Search(string searchTerm)
        {
            return _items.Where(item => item.Matches(searchTerm)).ToList();
        }

        public List<LibraryItem> SearchByTitle(string title)
        {
            return _items.Where(i => i.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public List<Book> SearchByAuthor(string author)
        {
            return _items.OfType<Book>()
                .Where(book => book.Author.Contains(author, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public Book? SearchByISBN(string isbn)
        {
            return _items.OfType<Book>().FirstOrDefault(b => b.ISBN == isbn);
        }

        // === DEL 4: SORTERINGSFUNKTIONER ===

        public List<LibraryItem> GetItemsSortedByTitle()
        {
            return _items.OrderBy(item => item.Title).ToList();
        }

        public List<LibraryItem> GetItemsSortedByYear(bool ascending = true)
        {
            return ascending
                ? _items.OrderBy(item => item.PublishedYear).ToList()
                : _items.OrderByDescending(item => item.PublishedYear).ToList();
        }

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
