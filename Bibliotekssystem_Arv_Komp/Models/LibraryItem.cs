using Bibliotekssystem_Arv_Komp.Interfaces;

namespace Bibliotekssystem_Arv_Komp.Models
{
    public abstract class LibraryItem : ISearchable
    {
        public string ItemId { get; init; }
        public string Title { get; set; }
        public int PublishedYear { get; set; }
        public bool IsAvailable { get; set; }

        protected LibraryItem(string itemId, string title, int publishedYear)
        {
            ItemId = itemId;
            Title = title;
            PublishedYear = publishedYear;
            IsAvailable = true;
        }

        public abstract string GetInfo();

        public virtual string GetItemType()
        {
            return this.GetType().Name;
        }

        public virtual bool Matches(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return false;

            return ItemId.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                   Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                   PublishedYear.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase);
        }
    }
}