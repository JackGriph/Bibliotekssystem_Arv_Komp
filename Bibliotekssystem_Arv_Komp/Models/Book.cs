using System.Collections.ObjectModel;

namespace Bibliotekssystem_Arv_Komp.Models
{
    public class Book : LibraryItem
    {
        public string ISBN { get; init; }
        public string Author { get; set; }
        public int Pages { get; set; }

        public ICollection<Loan> Loans { get; set; } = new List<Loan>();

        public Book(string isbn, string title, string author, int publishedYear, int pages = 0)
            : base(isbn, title, publishedYear)
        {
            ISBN = isbn;
            Author = author;
            Pages = pages;
        }

        public override string GetInfo()
        {
            string availability = IsAvailable ? "Tillgänglig" : "Utlånad";
            return $"[BOK]\n" +
                   $"ISBN: {ISBN}\n" +
                   $"Titel: {Title}\n" +
                   $"Författare: {Author}\n" +
                   $"Sidor: {Pages}\n" +
                   $"Utgivningsår: {PublishedYear}\n" +
                   $"Status: {availability}";
        }

        public override string GetItemType()
        {
            return "Bok";
        }

        public override bool Matches(string searchTerm)
        {
            if (base.Matches(searchTerm))
                return true;

            return Author.Contains(searchTerm, StringComparison.OrdinalIgnoreCase);
        }
    }
}